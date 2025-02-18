using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class StackSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T m_Instance;
    public static T Instance
    {
        get
        {
            if (!m_Instance)
                m_Instance = GameObject.FindObjectOfType<T>();
            return m_Instance;
        }
        set
        {
            if (Instance == value)
            {
                return;
            }
            if (value.gameObject.activeInHierarchy)
            {
                m_Instance = value;
            }
            else
            {
                if (singletonStack.Count > 0)
                {
                    m_Instance = singletonStack.Pop();
                }
            }
        }
    }
    protected static Stack<T> singletonStack = new Stack<T>();
    private void OnEnable()
    {
        if (Instance != null)
        {
            singletonStack.Push(Instance);
        }
        Instance = GetComponent<T>();
    }
    private void OnDisable()
    {
        if (singletonStack.Count > 0)
        {
            Instance = singletonStack.Pop();
        }
    }
}