using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSortOrderUpdater : MonoBehaviour
{
    public int SortOrder;
    private void OnEnable()
    {
        if (gameObject.TryGetComponent<Canvas>(out Canvas canvas))
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = SortOrder;
        }
        Destroy(this);
    }
}