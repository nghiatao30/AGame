using System.Collections;
using System.Collections.Generic;
using GachaSystem.Core;
using HyrphusQ.Events;
using LatteGames;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PackDockReplaceUI : MonoBehaviour
{
    public UnityEvent<GachaPack> OnInit;
    public UnityEvent OnSelectUnlockedNode;
    public UnityEvent OnSelectLockedNode;

    [SerializeField] protected CanvasGroupVisibility canvasGroupVisibility;
    [SerializeField] protected Button closeBtn, replaceBtn, collectAndReplaceBtn;
    [SerializeField] protected Image thumbnailImg;
    [SerializeField] protected List<PackDockReplaceNodeUI> nodes;

    protected GachaPack gachaPack;
    protected PackDockReplaceNodeUI currentNodeUI;

    protected virtual void Awake()
    {
        GameEventHandler.AddActionEvent(GachaPackDockEventCode.OnShowReplaceUI, OnShowReplaceUI);
        closeBtn.onClick.AddListener(OnCloseBtnClicked);
        replaceBtn.onClick.AddListener(OnReplaceBtnClicked);
        collectAndReplaceBtn.onClick.AddListener(OnCollectAndReplaceBtnClicked);
    }

    protected virtual void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(GachaPackDockEventCode.OnShowReplaceUI, OnShowReplaceUI);
        closeBtn.onClick.RemoveListener(OnCloseBtnClicked);
        replaceBtn.onClick.RemoveListener(OnReplaceBtnClicked);
        collectAndReplaceBtn.onClick.RemoveListener(OnCollectAndReplaceBtnClicked);

        if (PackDockManager.Instance)
        {
            var slots = PackDockManager.Instance.gachaPackDockData.gachaPackDockSlots;

            for (var i = 0; i < slots.Count; i++)
            {
                var node = nodes[i];
                var slot = slots[i];
                node.OnClicked -= OnNodeClicked;
                slot.OnStateChanged -= OnStateChanged;
            }
        }
    }

    protected virtual void OnShowReplaceUI(object[] _params)
    {
        gachaPack = (GachaPack)_params[0];

        Init();
        UpdateView();

        canvasGroupVisibility.Show();
    }

    protected virtual void Init()
    {
        thumbnailImg.sprite = gachaPack.GetThumbnailImage();

        var slots = PackDockManager.Instance.gachaPackDockData.gachaPackDockSlots;
        currentNodeUI = nodes[0];
        for (var i = 0; i < slots.Count; i++)
        {
            var node = nodes[i];
            var slot = slots[i];
            if (slot.State == GachaPackDockSlotState.Unlocked)
            {
                currentNodeUI = node;
            }
            node.Setup(slot);
            node.OnClicked += OnNodeClicked;
            slot.OnStateChanged += OnStateChanged;
        }
        UpdateNodeSelectedState();
        OnInit?.Invoke(gachaPack);
    }

    protected virtual void UpdateView()
    {
        if (currentNodeUI.GachaPackDockSlot.State == GachaPackDockSlotState.Unlocked)
        {
            OnSelectUnlockedNode?.Invoke();
            replaceBtn.gameObject.SetActive(false);
            collectAndReplaceBtn.gameObject.SetActive(true);
        }
        else
        {
            OnSelectLockedNode?.Invoke();
            replaceBtn.gameObject.SetActive(true);
            collectAndReplaceBtn.gameObject.SetActive(false);
        }
    }

    protected virtual void UpdateNodeSelectedState()
    {
        foreach (var node in nodes)
        {
            if (node == currentNodeUI)
            {
                node.Select();
            }
            else
            {
                node.Unselect();
            }
        }
    }

    protected virtual void OnStateChanged(GachaPackDockSlotState oldState, GachaPackDockSlotState newState)
    {
        if (newState == GachaPackDockSlotState.Unlocked)
        {
            UpdateView();
        }
    }

    protected virtual void OnCloseBtnClicked()
    {
        canvasGroupVisibility.Hide();
    }

    protected virtual void OnNodeClicked(PackDockReplaceNodeUI nodeUI)
    {
        currentNodeUI = nodeUI;
        UpdateNodeSelectedState();
        UpdateView();
    }

    protected virtual void OnReplaceBtnClicked()
    {
        PackDockManager.Instance.ReplacePack(currentNodeUI.GachaPackDockSlot, gachaPack);
        canvasGroupVisibility.Hide();
    }

    protected virtual void OnCollectAndReplaceBtnClicked()
    {
        PackDockManager.Instance.CollectAndReplacePack(currentNodeUI.GachaPackDockSlot, gachaPack);
        canvasGroupVisibility.Hide();
    }
}
