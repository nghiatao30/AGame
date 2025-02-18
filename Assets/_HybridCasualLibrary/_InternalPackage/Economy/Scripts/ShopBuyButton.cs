using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HyrphusQ.Events;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using HyrphusQ.SerializedDataStructure;
using System;
using UnityEngine.Events;
using I2.Loc;

public class ShopBuyButton : MonoBehaviour
{
    readonly string bindingKey = "{value}";
    [TitleGroup("Mandatory"), PropertyOrder(0)]
    public ShopProductSO shopProductSO;
    [TitleGroup("Mandatory"), PropertyOrder(0)]
    [SerializeField] protected Button button;
    [TitleGroup("Optional"), PropertyOrder(100)]
    [SerializeField] protected SerializedDictionary<CurrencyType, RectTransform> currencyEmitPoints;
    [TitleGroup("Optional"), PropertyOrder(100)]
    [SerializeField] protected Image productIcon;
    [TitleGroup("Optional"), PropertyOrder(100)]
    [SerializeField] protected DiscountableValueText priceTxt;
    [TitleGroup("Optional"), PropertyOrder(100)]
    [SerializeField] protected SerializedDictionary<CurrencyType, CurrencyDiscountableValueText> currencyItemValueTxtDic;
    [TitleGroup("Optional"), PropertyOrder(100)]
    [SerializeField] protected SerializedDictionary<ItemSO, DiscountableValueText> generalItemValueTxtDic;
    [TitleGroup("Optional"), PropertyOrder(100)]
    [SerializeField] protected SerializedDictionary<PPrefIntVariable, DiscountableValueText> consumableItemValueTxtDic;
    [TitleGroup("Optional"), PropertyOrder(100)]
    public UnityEvent<bool> OnInteractiveChanged;
    [TitleGroup("Optional"), PropertyOrder(100)]
    public UnityEvent<ShopProductSO> OnSetupView;
    [TitleGroup("Optional"), PropertyOrder(100)]
    public ResourceLocationProvider sourceLocationProvider;

    Dictionary<TextMeshProUGUI, string> customizedTextDic = new Dictionary<TextMeshProUGUI, string>();
    protected bool hasSetup;

    public SerializedDictionary<CurrencyType, RectTransform> CurrencyEmitPoints { get => currencyEmitPoints; }
    public virtual bool IsInteractiable
    {
        get
        {
            return button.interactable;
        }
        set
        {
            if (isLockBtn)
            {
                return;
            }
            if (button.interactable != value)
            {
                button.interactable = value;
                OnInteractiveChanged?.Invoke(button.interactable);
            }
        }
    }
    bool isLockBtn = false;
    public bool IsLockBtn
    {
        get
        {
            return isLockBtn;
        }
        set
        {
            if (isLockBtn != value)
            {
                isLockBtn = value;
                if (IsInteractiable)
                {
                    IsInteractiable = true;
                }
            }
        }
    }
    protected virtual void SetupView()
    {
        foreach (var pair in currencyItemValueTxtDic)
        {
            var currencyDiscountableValueText = pair.Value;
            var currencyItems = shopProductSO.currencyItems;
            if (currencyItems == null || !currencyItems.ContainsKey(pair.Key))
            {
                continue;
            }
            if (currencyDiscountableValueText.valueText != null)
            {
                if (pair.Value.isShowCurrencyIcon)
                {
                    var spriteAsset = CurrencyManager.Instance?.GetCurrencySO(pair.Key).TMPSprite;
                    SetText(currencyDiscountableValueText.valueText, $"{currencyItems[pair.Key].value.ToRoundedText()} {spriteAsset}");

                }
                else
                {
                    SetText(currencyDiscountableValueText.valueText, $"{currencyItems[pair.Key].value.ToRoundedText()}");
                }
            }
            if (currencyDiscountableValueText.originalValueText != null)
            {
                SetText(currencyDiscountableValueText.originalValueText, currencyItems[pair.Key].originalValue.ToRoundedText());
            }
            if (currencyDiscountableValueText.discountRateText != null)
            {
                var localizationParamsManager = currencyDiscountableValueText.discountRateText.GetComponent<LocalizationParamsManager>();
                if (localizationParamsManager != null)
                {
                    localizationParamsManager.SetParameterValue("PERCENT", currencyItems[pair.Key].discountRate.ToString());
                }
                else
                {
                    SetText(currencyDiscountableValueText.discountRateText, $"{currencyItems[pair.Key].discountRate}%");
                }
            }
        }
        foreach (var pair in generalItemValueTxtDic)
        {
            var discountableValueText = pair.Value;
            var items = shopProductSO.generalItems;
            if (items == null || !items.ContainsKey(pair.Key))
            {
                continue;
            }
            if (discountableValueText.valueText != null)
            {
                SetText(discountableValueText.valueText, items[pair.Key].value.ToRoundedText());
            }
            if (discountableValueText.originalValueText != null)
            {
                SetText(discountableValueText.originalValueText, items[pair.Key].originalValue.ToRoundedText());
            }
            if (discountableValueText.discountRateText != null)
            {
                var localizationParamsManager = discountableValueText.discountRateText.GetComponent<LocalizationParamsManager>();
                if (localizationParamsManager != null)
                {
                    localizationParamsManager.SetParameterValue("PERCENT", items[pair.Key].discountRate.ToString());
                }
                else
                {
                    SetText(discountableValueText.discountRateText, $"{items[pair.Key].discountRate}%");
                }
            }
        }
        foreach (var pair in consumableItemValueTxtDic)
        {
            var discountableValueText = pair.Value;
            var items = shopProductSO.consumableItems;
            if (items == null || !items.ContainsKey(pair.Key))
            {
                continue;
            }
            if (discountableValueText.valueText != null)
            {
                SetText(discountableValueText.valueText, items[pair.Key].value.ToRoundedText());
            }
            if (discountableValueText.originalValueText != null)
            {
                SetText(discountableValueText.originalValueText, items[pair.Key].originalValue.ToRoundedText());
            }
            if (discountableValueText.discountRateText != null)
            {
                var localizationParamsManager = discountableValueText.discountRateText.GetComponent<LocalizationParamsManager>();
                if (localizationParamsManager != null)
                {
                    localizationParamsManager.SetParameterValue("PERCENT", items[pair.Key].discountRate.ToString());
                }
                else
                {
                    SetText(discountableValueText.discountRateText, $"{items[pair.Key].discountRate}%");
                }
            }
        }
        if (productIcon != null)
        {
            productIcon.sprite = shopProductSO.icon;
        }
        CheckButtonInteractive();
        OnSetupView?.Invoke(shopProductSO);
    }
    protected virtual void OnButtonClicked()
    {

    }
    protected virtual void Awake()
    {
        FindCustomizedTexts();
    }
    protected void SetText(TextMeshProUGUI _TMP, string content)
    {
        if (customizedTextDic.ContainsKey(_TMP))
        {
            _TMP.text = customizedTextDic[_TMP].Replace(bindingKey, content);
        }
        else
        {
            _TMP.text = content;
        }
    }
    protected void FindCustomizedTexts()
    {
        FindCustomizedTextsInTextContainer(priceTxt);

        foreach (var pair in currencyItemValueTxtDic)
        {
            var textContainer = pair.Value;
            FindCustomizedTextsInTextContainer(textContainer);
        }
        foreach (var pair in generalItemValueTxtDic)
        {
            var textContainer = pair.Value;
            FindCustomizedTextsInTextContainer(textContainer);
        }
        foreach (var pair in consumableItemValueTxtDic)
        {
            var textContainer = pair.Value;
            FindCustomizedTextsInTextContainer(textContainer);
        }
        void FindCustomizedTextsInTextContainer(DiscountableValueText discountableValueText)
        {
            if (discountableValueText != null)
            {
                if (discountableValueText.valueText != null)
                {
                    if (discountableValueText.valueText.text.Contains(bindingKey) && !customizedTextDic.ContainsKey(discountableValueText.valueText))
                    {
                        customizedTextDic.Add(discountableValueText.valueText, discountableValueText.valueText.text);
                    }
                }
                if (discountableValueText.originalValueText != null)
                {
                    if (discountableValueText.originalValueText.text.Contains(bindingKey) && !customizedTextDic.ContainsKey(discountableValueText.originalValueText))
                    {
                        customizedTextDic.Add(discountableValueText.originalValueText, discountableValueText.originalValueText.text);
                    }
                }
                if (discountableValueText.discountRateText != null)
                {
                    if (discountableValueText.discountRateText.text.Contains(bindingKey) && !customizedTextDic.ContainsKey(discountableValueText.discountRateText))
                    {
                        customizedTextDic.Add(discountableValueText.discountRateText, discountableValueText.discountRateText.text);
                    }
                }
            }
        }
    }
    public virtual void OverrideSetup(ShopProductSO shopProductSO)
    {
        if (shopProductSO != null)
        {
            UnsubscribeEvents();
        }

        this.shopProductSO = shopProductSO;
        SubscribeEvents();
        SetupView();
        hasSetup = true;
    }
    public virtual void UpdateView()
    {
        SetupView();
    }
    protected virtual void OnEnable()
    {
        if (!hasSetup && shopProductSO != null)
        {
            SubscribeEvents();
            SetupView();
        }
    }
    protected virtual void OnDisable()
    {
        UnsubscribeEvents();
        hasSetup = false;
    }
    protected virtual void SubscribeEvents()
    {
        button.onClick.AddListener(OnButtonClicked);
    }
    protected virtual void UnsubscribeEvents()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }
    protected virtual void CheckButtonInteractive()
    {

    }
    [Serializable]
    public class DiscountableValueText
    {
        public TextMeshProUGUI valueText;
        public TextMeshProUGUI originalValueText;
        public TextMeshProUGUI discountRateText;
    }
    [Serializable]
    public class CurrencyDiscountableValueText : DiscountableValueText
    {
        public bool isShowCurrencyIcon;
    }
}
