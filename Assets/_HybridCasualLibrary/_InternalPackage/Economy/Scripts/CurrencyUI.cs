using System;
using HyrphusQ.Events;
using LatteGames;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    public event Action<CurrencyType> OnBuyMoreCurrencyBtnClicked = delegate { };

    [SerializeField] protected CurrencySO currencyManagerSO;
    public virtual float ActualAmount => currencyManagerSO.ShowingValue;
    [SerializeField] protected TextMeshProUGUI currencyText;
    [SerializeField] protected RectTransform currencyIcon;
    [SerializeField] protected Button m_BuyMoreBtn;
    public RectTransform CurrencyIcon => currencyIcon;

    float displayedAmount;
    public float DisplayedAmount
    {
        get => displayedAmount;
        set
        {
            displayedAmount = Mathf.Max(value, 0f);
            UpdateText(displayedAmount);
        }
    }

    bool isTextUpdateEnabled = true;
    public bool IsTextUpdateEnabled
    {
        get => isTextUpdateEnabled;
        set
        {
            isTextUpdateEnabled = value;
            if (isTextUpdateEnabled)
            {
                DisplayedAmount = ActualAmount;
            }
        }
    }
    private void Awake()
    {
        if (currencyIcon != null)
        {
            currencyIcon.GetComponent<Image>().sprite = currencyManagerSO.icon;
        }
    }
    protected virtual void OnEnable()
    {
        currencyManagerSO.onValueChanged += HandleAmountChanged;
        currencyManagerSO.OnHiddenValueChange += HandleAmountChanged;
        if (m_BuyMoreBtn != null)
            m_BuyMoreBtn.onClick.AddListener(BuyMoreCurrency);
        HandleAmountChanged();
    }

    private void BuyMoreCurrency()
    {
        OnBuyMoreCurrencyBtnClicked(currencyManagerSO.CurrencyType);
    }

    protected virtual void OnDisable()
    {
        currencyManagerSO.onValueChanged -= HandleAmountChanged;
        currencyManagerSO.OnHiddenValueChange -= HandleAmountChanged;
        if (m_BuyMoreBtn != null)
            m_BuyMoreBtn.onClick.RemoveListener(BuyMoreCurrency);
    }
    protected virtual void HandleAmountChanged()
    {
        if (!IsTextUpdateEnabled) return;
        DisplayedAmount = ActualAmount;
    }
    protected virtual void HandleAmountChanged(ValueDataChanged<float> eventData)
    {
        HandleAmountChanged();
    }

    public virtual void UpdateText(float amount)
    {
        currencyText.text = amount.RoundToInt().ToRoundedText();
    }
#if UNITY_EDITOR
    [Button("Change Icon")]
    void ChangeIcon()
    {
        if (currencyIcon != null)
        {
            currencyIcon.GetComponent<Image>().sprite = currencyManagerSO.icon;
        }
        UnityEditor.EditorUtility.SetDirty(currencyIcon);
    }
#endif
}
