using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyrphusQ.Helpers;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class AIBotInfo : PersonalInfo
{
    #region Constructors
    public AIBotInfo()
    {

    }
    public AIBotInfo(string name, Sprite avatar, KeyValuePair<string, NationalFlagManagerSO.CountryInfo> countryKeyValuePair, float totalNumOfPoints, DifficultyType difficultyType)
    {
        m_Name = name;
        m_Avatar = avatar;
        m_CountryCode = countryKeyValuePair.Key;
        m_CountryFlag = countryKeyValuePair.Value.nationalFlag;
        m_TotalNumOfPoints = totalNumOfPoints;
        m_DifficultyType = difficultyType;
    }
    #endregion

    [Serializable]
    public class SaveData
    {
        public string userId;
        public float totalNumOfPoints;
    }

    #region Fields
    [SerializeField]
    private string m_Name;
    [SerializeField]
    private Sprite m_Avatar;
    //[SerializeField]
    //private Sprite m_AvatarFrame;
    [SerializeField]
    private string m_CountryCode;
    [SerializeField]
    private Sprite m_CountryFlag;
    [SerializeField]
    private float m_TotalNumOfPoints;
    [SerializeField]
    private DifficultyType m_DifficultyType;
    [NonSerialized]
    private SaveData m_SaveData = null;
    #endregion

    #region Properties
    /// <summary>
    /// Getter Properties
    /// </summary>
    public override bool isLocal => false;
    public override string userId => m_Name;
    public override string name => m_Name;
    public override Sprite avatar => m_Avatar;
    public override Sprite avatarFrame => null;
    public override string countryCode => m_CountryCode;
    public override Sprite nationalFlag => m_CountryFlag;
    public virtual DifficultyType difficultyType => m_DifficultyType;
    #endregion

    #region Methods
    public override float GetTotalNumOfPoints()
    {
        return GetSaveData().totalNumOfPoints;
    }

    public override void SetTotalNumOfPoints(float amount)
    {
        GetSaveData().totalNumOfPoints = amount;
    }

    public virtual void InjectSaveData(SaveData saveData)
    {
        if (isLocal)
            return;
        m_SaveData = saveData;
    }

    public virtual SaveData GetSaveData()
    {
        if (isLocal)
            return null;
        if (m_SaveData == null)
        {
            m_SaveData = new SaveData()
            {
                userId = userId,
                totalNumOfPoints = m_TotalNumOfPoints,
            };
        }
        return m_SaveData;
    }
    #endregion
}
[Serializable]
public class LocalPlayerPersonalInfo : PersonalInfo
{
    #region Constructors
    public LocalPlayerPersonalInfo(PPrefStringVariable playerNameVariable, PPrefFloatVariable playerNumOfPointsVariable, ItemManagerSO avatarManagerSO, ItemManagerSO frameManagerSO, NationalFlagManagerSO countryFlagManagerSO)
    {
        m_PlayerNameVariable = playerNameVariable;
        m_PlayerNumOfPointsVariable = playerNumOfPointsVariable;
        m_AvatarManagerSO = avatarManagerSO;
        m_FrameManagerSO = frameManagerSO;
        m_NationalFlagManagerSO = countryFlagManagerSO;
    }
    #endregion

    #region Fields
    private PPrefStringVariable m_PlayerNameVariable;
    private PPrefFloatVariable m_PlayerNumOfPointsVariable;
    private ItemManagerSO m_AvatarManagerSO;
    private ItemManagerSO m_FrameManagerSO;
    private NationalFlagManagerSO m_NationalFlagManagerSO;
    #endregion

    #region Properties
    public override bool isLocal => true;
    public override string userId => m_PlayerNameVariable.value;
    public override string name
    {
        get => m_PlayerNameVariable.value;
    }
    public override Sprite avatar
    {
        get => m_AvatarManagerSO.currentItemInUse.GetThumbnailImage();
    }
    public override Sprite avatarFrame
    {
        get => m_FrameManagerSO.currentItemInUse?.GetThumbnailImage() ?? null;
    }
    public override string countryCode => m_NationalFlagManagerSO.playerCountryCode.value;
    public override Sprite nationalFlag => m_NationalFlagManagerSO.GetNationalFlag(countryCode);
    #endregion

    #region Methods
    public override float GetTotalNumOfPoints()
    {
        return m_PlayerNumOfPointsVariable.value;
    }

    public override void SetTotalNumOfPoints(float amount)
    {
        m_PlayerNumOfPointsVariable.value = amount;
    }
    #endregion
}
public class PlayerDatabaseSO : ListVariable<PersonalInfo>
{
    #region Fields
    [Header("Player's Data References")]
    [SerializeField]
    private PPrefStringVariable m_PlayerNameVariable;
    [SerializeField]
    private PPrefFloatVariable m_PlayerNumOfPointsVariable;
    [SerializeField]
    private ItemManagerSO m_AvatarManagerSO;
    [SerializeField]
    private ItemManagerSO m_FrameManagerSO;
    [SerializeField]
    private NationalFlagManagerSO m_NationalFlagManagerSO;
    [Header("Bot's Data References")]
    [SerializeField]
    private PlayerDatabaseSavedDataSO m_SavedDataSO;
    [SerializeField]
    private AnonymousBotGeneratorSO m_AnonymousBotGeneratorSO;
    [SerializeField]
    private List<AIBotInfo> m_BotInfos = new List<AIBotInfo>();

    [NonSerialized]
    private LocalPlayerPersonalInfo m_LocalPlayerPersonalInfo;
    #endregion

    #region Properties
    /// <summary>
    /// Player's Data References
    /// </summary>
    public PPrefStringVariable playerNameVariable => m_PlayerNameVariable;
    public ItemManagerSO avatarManagerSO => m_AvatarManagerSO;
    public ItemManagerSO frameManagerSO => m_FrameManagerSO;
    public NationalFlagManagerSO nationalFlagManagerSO => m_NationalFlagManagerSO;
    public LocalPlayerPersonalInfo localPlayerPersonalInfo
    {
        get
        {
            if (m_LocalPlayerPersonalInfo == null)
            {
                // Random player name in fisrt time
                if (!m_PlayerNameVariable.hasKey)
                    m_PlayerNameVariable.value = GenerateRandomName();
                m_LocalPlayerPersonalInfo = new LocalPlayerPersonalInfo(
                    m_PlayerNameVariable,
                    m_PlayerNumOfPointsVariable,
                    m_AvatarManagerSO,
                    m_FrameManagerSO,
                    m_NationalFlagManagerSO);
            }
            return m_LocalPlayerPersonalInfo;
        }
    }
    /// <summary>
    /// Bot's Data References
    /// </summary>
    public List<AIBotInfo> botInfos => m_BotInfos;

    public override List<PersonalInfo> value
    {
        get
        {
            if (m_RuntimeValue == null)
            {
                m_RuntimeValue = new List<PersonalInfo>(m_BotInfos);
                m_RuntimeValue.Add(localPlayerPersonalInfo);
            }
            return m_RuntimeValue;
        }
        set => base.value = value;
    }
    #endregion

    protected virtual void OnEnable()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
            return;
#endif
        m_SavedDataSO.onDataLoaded += OnDataLoaded;
    }

    protected virtual void OnDisable()
    {
        m_SavedDataSO.onDataLoaded -= OnDataLoaded;
    }

    protected virtual void OnDataLoaded()
    {
        foreach (var botInfo in botInfos)
        {
            var saveData = m_SavedDataSO.GetData().m_BotInfosSaveData.Find(item => item.userId == botInfo.userId);
            // ===== Update app user (added new bot) =====
            if (saveData == null)
            {
                saveData = botInfo.GetSaveData();
                m_SavedDataSO.GetData().m_BotInfosSaveData.Add(saveData);
            }
            botInfo.InjectSaveData(saveData);
        }
    }

    #region Query data
    public AIBotInfo GetRandomBotInfo(Predicate<AIBotInfo> predicate = null)
    {
        // 30% meet player in top 220
        var randomValue = Random.Range(0, 10);
        AIBotInfo botInfo = null;
        if (randomValue < 3)
        {
            botInfo = GetRandomBotInfoOnTopRank(predicate);
        }
        else
        {
            botInfo = GetRandomAnonymousBotInfo();
        }
        return botInfo;
    }

    public AIBotInfo GetRandomBotInfo(DifficultyType difficulty)
    {
        return GetRandomBotInfo(botInfo => botInfo.difficultyType == difficulty);
    }

    public AIBotInfo GetRandomBotInfoOnTopRank(Predicate<AIBotInfo> predicate = null)
    {
        var index = RandomHelper.RandomRange(0, botInfos.Count, i => predicate?.Invoke(botInfos[i]) ?? true);
        return botInfos[index];
    }

    public AIBotInfo GetRandomAnonymousBotInfo(DifficultyType difficulty = DifficultyType.Medium)
    {
        return m_AnonymousBotGeneratorSO.GetRandomBotInfo(difficulty);
    }

    public AIBotInfo GetBotByName(string name)
    {
        return botInfos.Find(bot => bot.userId == name);
    }

    public bool IsAnonymousBot(AIBotInfo botInfo)
    {
        return !botInfos.Contains(botInfo);
    }
    #endregion

    public static string GenerateRandomName()
    {
        var format = "PLAYER#xxZZZZ";
        var name = format.Select(c =>
        {
            if (c == 'x') return (char)Random.Range((int)'A', (int)'Z' + 1);
            if (c == 'Z') return (char)Random.Range((int)'0', (int)'9' + 1);
            return c;
        });
        return string.Concat(name);
    }
}