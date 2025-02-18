using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILayoutGroup3D
{
    int GetCount();
    bool IsTheLastIndex(int index);
    bool IsOutOfRange(int index);
    TransformData GetTransformDataOfIndex(int index);
    List<TransformData> GetLayoutDataAsList();
}