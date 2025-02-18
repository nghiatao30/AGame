using System;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;
using UnityEngine.UI;

public class ItemCell : MonoBehaviour
{
    #region Events
    public class ClickedEventData
    {
        public ClickedEventData(ItemCell itemCell)
        {
            this.itemCell = itemCell;
        }

        public ItemCell itemCell { get; set; }
    }
    public event Action<ClickedEventData> onItemClicked;
    #endregion

    #region Fields
    // Protected & Private Modifier
    [SerializeField]
    protected Button m_Button;
    [SerializeField]
    protected Image m_ThumbnailImage;
    [SerializeField]
    protected Image m_ThumbnailLock;

    protected ItemSO m_ItemSO;
    protected ItemManagerSO m_ItemManagerSO;
    protected ISelectionResponse[] m_SelectionResponses;
    #endregion

    #region Properties
    // Public modifier
    public ItemSO item => m_ItemSO;
    public ItemManagerSO itemManagerSO => m_ItemManagerSO;
    // Protected & Private modifier
    protected ISelectionResponse[] selectionResponses
    {
        get
        {
            if (m_SelectionResponses == null)
                m_SelectionResponses = GetComponents<ISelectionResponse>();
            return m_SelectionResponses;
        }
    }
    #endregion

    // Protected & Private methods
    protected virtual void NotifyEventItemClicked(ClickedEventData eventData)
    {
        onItemClicked?.Invoke(eventData);
    }

    protected virtual void OnItemClicked()
    {
        NotifyEventItemClicked(new ClickedEventData(this));
    }

    // Public methods
    public virtual void Select(bool isForceSelect = false)
    {
        foreach (var selectionResponse in selectionResponses)
        {
            selectionResponse.Select(isForceSelect);
        }
    }

    public virtual void Deselect()
    {
        foreach (var selectionResponse in selectionResponses)
        {
            selectionResponse.Deselect();
        }
    }

    public virtual void Initialize(ItemSO item, ItemManagerSO itemManagerSO)
    {
        if (item == null)
            return;
        name = item.GetDisplayName();
        m_ItemSO = item;
        m_ItemManagerSO = itemManagerSO;
        m_Button.onClick.RemoveListener(OnItemClicked);
        m_Button.onClick.AddListener(OnItemClicked);
        UpdateView();
    }

    public virtual void UpdateView()
    {
        if (item == null)
            return;
        m_ThumbnailImage = m_ItemSO.CreateIconImage(m_ThumbnailImage);
        m_ThumbnailLock?.gameObject.SetActive(!item.IsOwned());
    }
}