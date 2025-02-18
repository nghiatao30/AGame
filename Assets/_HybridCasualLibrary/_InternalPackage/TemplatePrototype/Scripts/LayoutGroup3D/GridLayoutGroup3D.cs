using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridLayoutGroup3D : MonoBehaviour, ILayoutGroup3D
{
    [SerializeField]
    private Vector3 m_CellSize = Vector3.one;
    [SerializeField]
    private Vector3 m_CellSpacing = Vector3.zero;
    [SerializeField, MinValue(1)]
    private Vector3Int m_GridSize = Vector3Int.one;

    private bool m_IsInitialized;
    private Vector3 m_StartCorner;
    private Vector3[,,] m_GridPositions;

    public int lengthX => m_GridSize.x;
    public int lengthY => m_GridSize.y;
    public int lengthZ => m_GridSize.z;

    private void Awake()
    {
        Initialize();
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        CalculateGridLayout();

        Gizmos.color = Color.red;
        for (int z = 0; z < lengthZ; z++)
        {
            for (int y = 0; y < lengthY; y++)
            {
                for (int x = 0; x < lengthX; x++)
                {
                    Gizmos.DrawWireCube(m_GridPositions[x, y, z], transform.TransformDirection(new Vector3(m_CellSize.x, m_CellSize.z, m_CellSize.y)));
                }
            }
        }
    }
    private void OnValidate()
    {
        CalculateStartCorner();
    }
#endif

    private void Initialize()
    {
        if (m_IsInitialized)
            return;
        m_IsInitialized = true;
        CalculateStartCorner();
        CalculateGridLayout();
    }
    private void CalculateGridLayout()
    {
        // Calculate grid layout data
        m_GridPositions = new Vector3[lengthX, lengthY, lengthZ];

        for (int z = 0; z < lengthZ; z++)
        {
            for (int y = 0; y < lengthY; y++)
            {
                for (int x = 0; x < lengthX; x++)
                {
                    var position = m_StartCorner + new Vector3(m_CellSize.x * x + m_CellSpacing.x * x, m_CellSize.z * z + m_CellSpacing.z * z, -m_CellSize.y * y - m_CellSpacing.y * y);
                    m_GridPositions[x, y, z] = transform.TransformPoint(position);
                }
            }
        }
    }
    private void CalculateStartCorner()
    {
        // Calculate corner (Center X - Center Y - Bottom Z)
        m_StartCorner = new Vector3(
            -(m_GridSize.x * (m_CellSize.x + m_CellSpacing.x) - m_CellSpacing.x) / 2f + m_CellSize.x / 2f,
            +m_CellSize.z / 2f,
            (m_GridSize.y * (m_CellSize.y + m_CellSpacing.y) - m_CellSpacing.y) / 2f - m_CellSize.y / 2f);
    }
    private Vector3Int CalculateIndex3D(int index)
    {
        int x = index % lengthX;
        int y = index / lengthX % lengthY;
        int z = index / (lengthX * lengthY);
        return new Vector3Int(x, y, z);
    }

    public int GetCount()
    {
        return lengthX * lengthY * lengthZ;
    }
    public bool IsTheLastIndex(int index)
    {
        return index == GetCount() - 1;
    }
    public bool IsOutOfRange(int index)
    {
        return index >= GetCount();
    }
    public TransformData GetTransformDataOfIndex(int index)
    {
        var index3D = CalculateIndex3D(index);
        var position = m_GridPositions[index3D.x, index3D.y, index3D.z];
        var rotation = transform.rotation;
        var scale = Vector3.one;

        return new TransformData(position, rotation, scale);
    }
    public List<TransformData> GetLayoutDataAsList()
    {
        Initialize();
        var count = GetCount();
        var layoutList = new List<TransformData>(count);
        for (int i = 0; i < count; i++)
        {
            layoutList.Add(GetTransformDataOfIndex(i));
        }
        return layoutList;
    }
}