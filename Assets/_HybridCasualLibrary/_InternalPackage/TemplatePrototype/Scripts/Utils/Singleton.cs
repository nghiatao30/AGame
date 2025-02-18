using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ensure the script run early during excution
[DefaultExecutionOrder(-100)]
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Toggle verbose mode for log messages
    /// </summary>
    public bool m_Verbose = true;
    /// <summary>
    /// Keep the GameObject alive when changing scenes
    /// </summary>
    public bool m_KeepAlive = false;
    /// <summary>
    /// Allow or prevent duplicates of the same singleton
    /// </summary>
    public bool m_DuplicateAllow = false;

    /// <summary>
    /// The single instance of singleton
    /// </summary>
    protected static T s_Instance;
    public static T Instance
    {
        get
        {
            // If the instance doesn't exist, try to find it
            if (!s_Instance)
                s_Instance = GameObject.FindObjectOfType<T>();
            return s_Instance;
        }
    }

    protected virtual void Awake()
    {
        // If a duplicate is found and duplicates aren't allowed
        if (s_Instance && !m_DuplicateAllow)
        {
            // If verbose mode is true, log about destroying the duplicate
            if (m_Verbose)
                Debug.Log($"Destroy duplicate singleton {typeof(T)}");
            // Destroy the extra instance
            DestroyImmediate(gameObject);
            return;
        }
        // If duplicates aren't allowed and we want to keep it alive between scenes
        if (!m_DuplicateAllow && m_KeepAlive)
            DontDestroyOnLoad(gameObject);
        // Set this GameObject as the instance
        s_Instance = GetComponent<T>();
    }
}