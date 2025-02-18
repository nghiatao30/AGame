using System;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class SavedDataSO : SerializableScriptableObject
{
    /// <summary>
    /// Insecurity password storing. It's best practice to store password on server side instead of client side!
    /// </summary>
    protected const string k_EncryptionPassword = "c2dsaWIxMjA0MTY=";

    public event Action onDataLoaded;

    [SerializeField, BoxGroup("Save Configs")]
    protected string m_SaveFileName;
    [SerializeField, BoxGroup("Save Configs")]
    protected string m_Key = "default";
    [SerializeField, BoxGroup("Save Configs")]
    protected ES3.EncryptionType m_EncryptionType = ES3.EncryptionType.None;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (string.IsNullOrEmpty(m_SaveFileName))
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(guid))
                return;
            m_SaveFileName = $"{name}_{guid}";
        }
    }
#endif

    protected virtual void NotifyEventDataLoaded()
    {
        onDataLoaded?.Invoke();
    }

    public virtual bool IsSaveFileExist() => ES3.FileExists(m_SaveFileName);
    public abstract bool IsLoaded();
    public abstract void Save();
    public abstract void Load();
    public abstract void Delete();
}
public abstract class SavedDataSO<T> : SavedDataSO where T : SavedData, new ()
{
    [NonSerialized]
    protected T m_Data;

    public virtual T data
    {
        get
        {
            if (m_Data == null) 
                Load();
            return m_Data;
        }
        set => m_Data = value;
    }

    public virtual T defaultData
    {
        get
        {
            return new T();
        }
    }

#if UNITY_EDITOR
    [Button("Open Save File Location")]
    private void OpenSaveFileLocation()
    {
        UnityEditor.EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
#endif

    public virtual T GetData()
    {
        return data;
    }

    public override bool IsLoaded()
    {
        return m_Data != null;
    }

    public override void Load()
    {
        var settings = new ES3Settings(m_SaveFileName, m_EncryptionType, k_EncryptionPassword);
        m_Data = ES3.Load(m_Key, defaultData, settings);

        NotifyEventDataLoaded();
    }

    public override void Save()
    {
        if (m_Data == null)
            return;
        var settings = new ES3Settings(m_SaveFileName, m_EncryptionType, k_EncryptionPassword);
        ES3.Save(m_Key, m_Data, settings);
    }

    [Button("Delete Data")]
    public override void Delete()
    {
        m_Data = null;
        if (IsSaveFileExist())
            ES3.DeleteFile(m_SaveFileName);
    }
}

[Serializable]
public abstract class SavedData
{

}