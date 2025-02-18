using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PackDockReplaceNodeUI : MonoBehaviour
{
    public UnityEvent OnSlotIsUnlocked;
    public UnityEvent OnSlotIsLocked;
    public UnityEvent OnSelected;
    public UnityEvent OnUnselected;
    public event Action<PackDockReplaceNodeUI> OnClicked;

    [SerializeField] protected Image thumbnailImg;
    [SerializeField] protected Button button;

    protected GachaPackDockSlot gachaPackDockSlot;

    public GachaPackDockSlot GachaPackDockSlot => gachaPackDockSlot;

    public virtual void Setup(GachaPackDockSlot slot)
    {
        gachaPackDockSlot = slot;
        thumbnailImg.sprite = slot.GachaPack.GetThumbnailImage();
        gachaPackDockSlot.OnStateChanged += OnStateChanged;
        button.onClick.AddListener(OnButtonClicked);
        if (gachaPackDockSlot.State == GachaPackDockSlotState.Unlocked)
        {
            OnSlotIsUnlocked?.Invoke();
        }
        else
        {
            OnSlotIsLocked?.Invoke();
        }
    }

    protected virtual void OnDestroy()
    {
        if (gachaPackDockSlot != null)
        {
            gachaPackDockSlot.OnStateChanged -= OnStateChanged;
        }
        button.onClick.RemoveListener(OnButtonClicked);
    }

    protected virtual void OnStateChanged(GachaPackDockSlotState oldState, GachaPackDockSlotState newState)
    {
        if (gachaPackDockSlot.State == GachaPackDockSlotState.Unlocked)
        {
            OnSlotIsUnlocked?.Invoke();
        }
        else
        {
            OnSlotIsLocked?.Invoke();
        }
    }

    protected virtual void OnButtonClicked()
    {
        OnClicked?.Invoke(this);
    }

    public virtual void Select()
    {
        OnSelected?.Invoke();
    }

    public virtual void Unselect()
    {
        OnUnselected?.Invoke();
    }
}
