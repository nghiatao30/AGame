using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectionResponse
{
    void Select(bool isForceSelect = false);
    void Deselect();
}