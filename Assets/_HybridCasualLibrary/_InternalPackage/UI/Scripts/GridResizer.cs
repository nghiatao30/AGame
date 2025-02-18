using System;
using System.Collections;
using System.Collections.Generic;
using LatteGames.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class GridResizer : MonoBehaviour
{
    [SerializeField] GridLayoutGroup gridLayoutGroup;
    [SerializeField] List<ResizeInfo> resizeInfos;
    
    [Button("Update Size")]
    public void UpdateSize()
    {
        var cellAmount = gridLayoutGroup.transform.GetChildren().FindAll((item) => item.gameObject.activeInHierarchy).Count;
        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            foreach (var resizeInfo in resizeInfos)
            {
                resizeInfo.rectTransform.sizeDelta = new Vector2(resizeInfo.rectTransform.sizeDelta.x, resizeInfo.initSize + cellAmount * gridLayoutGroup.cellSize.y + Mathf.Max(cellAmount - 1, 0) * gridLayoutGroup.spacing.y + gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom);
            }
        }
        else if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            foreach (var resizeInfo in resizeInfos)
            {
                resizeInfo.rectTransform.sizeDelta = new Vector2(resizeInfo.initSize + cellAmount * gridLayoutGroup.cellSize.x + Mathf.Max(cellAmount - 1, 0) * gridLayoutGroup.spacing.x + gridLayoutGroup.padding.right + gridLayoutGroup.padding.left, resizeInfo.rectTransform.sizeDelta.y);
            }
        }
        else
        {
            Debug.LogError("I don't handle other cases");
        }
    }
    [Serializable]
    public class ResizeInfo
    {
        public RectTransform rectTransform;
        public float initSize;
    }
}
