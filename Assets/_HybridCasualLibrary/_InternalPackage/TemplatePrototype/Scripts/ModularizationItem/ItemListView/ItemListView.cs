using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemListView : LatteGames.ComposeCanvasElementVisibilityController
{
    public class BaseEventData
    {
        #region Constructor
        public BaseEventData() { }
        public BaseEventData(ItemCell itemCell, ItemListView itemListView)
        {
            this.itemCell = itemCell;
            this.itemListView = itemListView;
        }
        #endregion

        public ItemSO itemSO => itemCell?.item ?? null;
        public ItemCell itemCell;
        public ItemListView itemListView;
    }
    public class SelectedEventData : BaseEventData
    {
        #region Constructor
        public SelectedEventData(ItemCell itemCell, ItemListView itemListView) : base(itemCell, itemListView) { }
        #endregion
    }
    public class UsedEventData : BaseEventData
    {
        #region Constructor
        public UsedEventData(ItemCell itemCell, ItemListView itemListView) : base(itemCell, itemListView) { }
        #endregion
    }
    public class PreviewedEventData : BaseEventData
    {
        #region Constructor
        public PreviewedEventData(ItemCell itemCell, ItemListView itemListView) : base(itemCell, itemListView) { }
        #endregion
    }

    public UnityEvent onDatasetChanged;
    public UnityEvent<UsedEventData> onItemUsed;
    public UnityEvent<SelectedEventData> onItemSelected;
    public UnityEvent<PreviewedEventData> onItemPreviewed;

    [SerializeField]
    protected bool m_AutoUseWhenSelect = true;
    [SerializeField]
    protected bool m_SelectDefaultItemAtStart = true;
    [SerializeField]
    protected ItemCell m_ItemCellPrefab;
    [SerializeField]
    protected RectTransform m_ItemCellContainer;
    [SerializeField]
    protected ItemManagerSO m_ItemManagerSO;


    protected ItemCell m_CurrentSelectedCell;
    protected ItemCell m_LastOwnedSelectedCell;
    protected List<ItemCell> m_ItemCells;

    public ItemCell currentSelectedCell => m_CurrentSelectedCell;
    public ItemCell lastOwnedSelectedCell => m_LastOwnedSelectedCell;
    public ItemManagerSO itemManagerSO
    {
        get => m_ItemManagerSO;
        set
        {
            m_ItemManagerSO = value;
            ClearAll();
            if (value != null)
                GenerateView(m_SelectDefaultItemAtStart);
        }
    }
    public List<ItemCell> itemCells => m_ItemCells;

    protected virtual void Start()
    {
        // Generate view
        GenerateView(m_SelectDefaultItemAtStart);
    }

    protected virtual void OnDestroy()
    {
        ResetToLastOwnedSelectedItem();
    }

    protected virtual void SelectDefaultItem()
    {
        // Validate input data
        if (m_ItemCells == null || m_ItemManagerSO == null || m_ItemManagerSO.currentItemInUse == null)
            return;
        if (m_CurrentSelectedCell != null && m_CurrentSelectedCell.item == m_ItemManagerSO.currentItemInUse)
            return;
        foreach (var itemCell in m_ItemCells)
        {
            if (itemCell.item == m_ItemManagerSO.currentItemInUse)
            {
                OnItemSelected(itemCell, true);
                break;
            }
        }
    }

    protected virtual void OnItemSelected(ItemCell.ClickedEventData eventData)
    {
        OnItemSelected(eventData.itemCell, false);
    }

    protected virtual void OnItemSelected(ItemCell itemCell, bool isForceSelect)
    {
        // Check if current item == previous item or itemCell is null then ignore
        if (m_CurrentSelectedCell == itemCell || itemCell == null)
            return;
        SelectItem(itemCell, isForceSelect);

        if (itemCell.item.IsOwned())
        {
            m_LastOwnedSelectedCell = itemCell;
            if (m_AutoUseWhenSelect)
                UseItem(itemCell);
        }
        else
        {
            PreviewItem(itemCell);
        }
    }

    protected virtual void NotifyEventItemSelected(ItemCell itemCell)
    {
        onItemSelected?.Invoke(new SelectedEventData(itemCell, this));
    }

    protected virtual void NotifyEventItemUsed(ItemCell itemCell)
    {
        onItemUsed?.Invoke(new UsedEventData(itemCell, this));
    }

    protected virtual void NotifyEventItemPreviewed(ItemCell itemCell)
    {
        onItemPreviewed?.Invoke(new PreviewedEventData(itemCell, this));
    }

    protected virtual void SelectItem(ItemCell itemCell, bool isForceSelect = false)
    {
        var previousSelectedCell = m_CurrentSelectedCell;
        m_CurrentSelectedCell?.Deselect();
        m_CurrentSelectedCell = itemCell;
        m_CurrentSelectedCell.Select(isForceSelect);
        m_CurrentSelectedCell.UpdateView();
        m_ItemManagerSO.Select(itemCell.item, previousSelectedCell?.item);

        // Notify event
        NotifyEventItemSelected(itemCell);
    }

    public virtual void UseItem(ItemCell itemCell)
    {
        m_ItemManagerSO.Use(itemCell.item);

        // Notify event
        NotifyEventItemUsed(itemCell);
    }

    public virtual void PreviewItem(ItemCell itemCell)
    {
        // Notify event
        NotifyEventItemPreviewed(itemCell);
    }

    public virtual void GenerateView(bool selectDefaultItem = false)
    {
        if (m_ItemManagerSO == null)
            return;
        if (m_ItemCells != null)
        {
            if (selectDefaultItem)
                SelectDefaultItem();
            return;
        }
        m_ItemCells = new List<ItemCell>();
        foreach (var item in m_ItemManagerSO.items)
        {
            // Generate item cell UI
            var itemCellInstance = Instantiate(m_ItemCellPrefab, m_ItemCellContainer);
            itemCellInstance.transform.localScale = Vector3.one;
            itemCellInstance.onItemClicked += OnItemSelected;
            itemCellInstance.Initialize(item, m_ItemManagerSO);
            m_ItemCells.Add(itemCellInstance);
        }

        var contentSizeFitter = m_ItemCellContainer?.GetComponent<ContentSizeFitter>();
        contentSizeFitter?.SetLayoutHorizontal();
        contentSizeFitter?.SetLayoutVertical();

        // Select current selected item
        if (selectDefaultItem)
            SelectDefaultItem();

        // Notify event dataset changed
        NotifyDatasetChanged();
    }

    public virtual void UpdateView()
    {
        if (m_ItemCells == null || m_ItemCells.Count <= 0)
            return;
        foreach (var itemCell in m_ItemCells)
        {
            itemCell.UpdateView();
        }
    }

    public virtual void ResetCurrentItemCell()
    {
        m_CurrentSelectedCell?.Deselect();
        m_CurrentSelectedCell = null;
    }

    public virtual void ResetToLastOwnedSelectedItem()
    {
        if (m_ItemCells == null || m_ItemCells.Count <= 0)
            return;
        if (m_LastOwnedSelectedCell == null || m_LastOwnedSelectedCell == m_CurrentSelectedCell)
            return;
        OnItemSelected(m_LastOwnedSelectedCell, true);
    }

    public virtual void ClearAll()
    {
        if (m_ItemCells == null)
            return;
        ResetCurrentItemCell();
        foreach (var itemCell in m_ItemCells)
        {
            Destroy(itemCell.gameObject);
        }
        m_ItemCells.Clear();
        m_ItemCells = null;

        NotifyDatasetChanged();
    }

    public virtual int FindIndexOf(ItemSO item)
    {
        if (m_ItemCells == null || item == null)
            return Const.IntValue.Invalid;
        for (int i = 0; i < m_ItemCells.Count; i++)
        {
            var itemCell = m_ItemCells[i];
            if (itemCell.item == item)
                return i;
        }
        return Const.IntValue.Invalid;
    }

    public virtual int FindIndexOf(ItemCell itemCell)
    {
        if (m_ItemCells == null || itemCell == null)
            return Const.IntValue.Invalid;
        return m_ItemCells.IndexOf(itemCell);
    }

    public virtual void NotifyDatasetChanged()
    {
        onDatasetChanged?.Invoke();
    }

    public virtual ItemCell this[int i]
    {
        get => m_ItemCells[i];
        set => m_ItemCells[i] = value;
    }
}