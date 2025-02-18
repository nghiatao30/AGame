using System.Collections;
using System.Collections.Generic;
using LatteGames.Utils;
using UnityEngine;

public class ScaleGridLayoutGroup : MonoBehaviour
{
    [SerializeField] List<Transform> cells = new List<Transform>();
    [SerializeField] public float cellScale = 1;
    [SerializeField] public Vector2 spacing;
    [SerializeField] public Vector2 startingPointOffset;
    [SerializeField] public int fixedColumn = 1;
    RectTransform _rectTransform;
    RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    public void UpdateView()
    {
        cells.Clear();
        cells.AddRange(transform.GetChildren().FindAll((item) => item.gameObject.activeInHierarchy));
        int rowAmount = Mathf.CeilToInt((float)cells.Count / fixedColumn);
        for (int i = 0; i < rowAmount; i++)
        {
            int columnAmount = ((float)cells.Count - fixedColumn * i) >= fixedColumn ? fixedColumn : cells.Count % fixedColumn;
            var startingPoint = (Vector2)rectTransform.anchoredPosition - new Vector2(0, rectTransform.offsetMin.y) + new Vector2(startingPointOffset.x - (((float)columnAmount / 2 - 0.5f) * spacing.x), -startingPointOffset.y);
            for (int j = 0; j < columnAmount; j++)
            {
                if(i * fixedColumn + j >= cells.Count)
                {
                    break;
                }
                var cell = cells[i * fixedColumn + j];
                int rowIndex = i;
                int columnIndex = j;
                cell.localScale = Vector3.one * cellScale;
                cell.GetComponent<RectTransform>().anchoredPosition = startingPoint + new Vector2(columnIndex * spacing.x, rowIndex * -spacing.y);
            }
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateView();
    }
#endif
}
