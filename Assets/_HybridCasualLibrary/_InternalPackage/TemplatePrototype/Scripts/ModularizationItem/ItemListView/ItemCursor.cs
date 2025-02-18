using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCursor : MonoBehaviour
{
    private RectTransform m_RectTransform;
    private ItemListView m_ItemListView;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_ItemListView = GetComponentInParent<ItemListView>();
        m_ItemListView.onItemSelected.AddListener(OnItemSelected);
    }

    private void OnDestroy()
    {
        m_ItemListView.onItemSelected.RemoveListener(OnItemSelected);
    }

    private void OnItemSelected(ItemListView.SelectedEventData arg0)
    {
        m_RectTransform.SetParent(arg0.itemCell.GetComponent<RectTransform>());
        m_RectTransform.localPosition = Vector3.zero;
    }
}