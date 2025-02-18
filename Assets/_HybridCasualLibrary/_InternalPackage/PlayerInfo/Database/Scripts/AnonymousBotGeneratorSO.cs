using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnonymousBotGeneratorSO : ScriptableObject
{
    [SerializeField]
    private PlayerDatabaseSO m_PlayerDatabaseSO;
    [SerializeField]
    private List<Sprite> m_Avatars;
    [SerializeField]
    private List<Sprite> m_AvatarFrames;

    private KeyValuePair<string, NationalFlagManagerSO.CountryInfo> RandomCountryInfo()
    {
        return m_PlayerDatabaseSO.nationalFlagManagerSO.GetRandomCountryInfo();
    }

    private Sprite RandomAvatarOrFrame(List<Sprite> randomSprites, ItemManagerSO itemManagerSO)
    {
        var randomIndex = Random.Range(0, randomSprites.Count + itemManagerSO.itemCount);
        if (randomIndex < randomSprites.Count)
            return randomSprites[randomIndex];
        return itemManagerSO.items[randomIndex - randomSprites.Count].GetThumbnailImage();
    }

    private Sprite RandomAvatar()
    {
        return RandomAvatarOrFrame(m_Avatars, m_PlayerDatabaseSO.avatarManagerSO);
    }

    private Sprite RandomAvatarFrame()
    {
        return RandomAvatarOrFrame(m_AvatarFrames, m_PlayerDatabaseSO.frameManagerSO);
    }

    private float RandomNumOfMedals()
    {
        return Random.Range(0, 101);
    }

    private DifficultyType CreateBossTypes(DifficultyType difficulty)
    {
        return difficulty;
    }

    public AIBotInfo GetRandomBotInfo(DifficultyType difficulty)
    {
        return new AIBotInfo(
            PlayerDatabaseSO.GenerateRandomName(),
            RandomAvatar(),
            RandomCountryInfo(),
            RandomNumOfMedals(),
            CreateBossTypes(difficulty)
            );
    }
}