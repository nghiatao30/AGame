using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TransformData
{
    [SerializeField]
    private Vector3 m_Position;
    [SerializeField]
    private Vector3 m_EulerAngles;
    [SerializeField]
    private Vector3 m_LocalScale;

    public Vector3 position
    {
        get => m_Position;
        set => m_Position = value;
    }
    public Quaternion rotation
    {
        get => Quaternion.Euler(m_EulerAngles);
        set => m_EulerAngles = value.eulerAngles;
    }
    public Vector3 scale
    {
        get => m_LocalScale;
        set => m_LocalScale = value;
    }

    public TransformData(Transform transform, Space space = Space.World)
    {
        m_Position = space == Space.World ? transform.position : transform.localPosition;
        m_EulerAngles = space == Space.World ? transform.eulerAngles : transform.localEulerAngles;
        m_LocalScale = space == Space.World ? transform.lossyScale : transform.localScale;
    }

    public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        m_Position = position;
        m_EulerAngles = rotation.eulerAngles;
        m_LocalScale = scale;
    }
    public TransformData(Vector3 position, Vector3 eulerAngles, Vector3 scale)
    {
        m_Position = position;
        m_EulerAngles = eulerAngles;
        m_LocalScale = scale;
    }

    public static TransformData Default => new TransformData()
    {
        m_Position = Vector3.zero,
        m_EulerAngles = Vector3.zero,
        m_LocalScale = Vector3.one,
    };
}