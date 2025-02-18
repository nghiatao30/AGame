using UnityEngine;
using UnityEngine.UI;
using System;
using I2.Loc;
using TMPro;

namespace LatteGames.UI
{
    public class MessageManager : Singleton<MessageManager>
    {
        public static event Action<ClickedEventData> OnButtonClicked = delegate { };

        [SerializeField] protected int sortingOrder = short.MaxValue;
        [SerializeField] protected Canvas canvas;
        protected static Canvas Canvas => Instance.canvas;
        [SerializeField] protected CanvasGroup canvasGroup;
        protected static CanvasGroup CanvasGroup => Instance.canvasGroup;
        [SerializeField] protected TextMeshProUGUI titleText;
        public static string Title
        {
            get => Instance.titleText.text;
            set
            {
                Instance.titleText.text = value;
            }
        }
        [SerializeField] protected TextMeshProUGUI messageText;
        public static string Message
        {
            get => Instance.messageText.text;
            set
            {
                Instance.messageText.text = value;
            }
        }
        public static float MessageTextSize
        {
            get => Instance.messageText.fontSize;
            set
            {
                Instance.messageText.fontSize = value;
            }
        }
        [SerializeField] protected Button positiveBtn;
        [SerializeField] protected TextMeshProUGUI positiveText;
        public static string PositiveText
        {
            get => Instance.positiveText.text;
            set
            {
                Instance.positiveText.text = value;
            }
        }
        [SerializeField] protected Button negativeBtn;
        [SerializeField] protected TextMeshProUGUI negativeText;
        public static string NegativeText
        {
            get => Instance.negativeText.text;
            set
            {
                Instance.negativeText.text = value;
            }
        }

        protected static bool isAutoHide;
        protected static float defaultMessageTextSize;
        protected static RectTransform customContent;

        protected override void Awake()
        {
            base.Awake();
            defaultMessageTextSize = messageText.fontSize;
            positiveBtn.onClick.AddListener(HandlePositiveBtnClicked);
            negativeBtn.onClick.AddListener(HandleNegativeBtnClicked);
            // Turn off by default
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = false;
            CanvasGroup.FadeOut(0f, isIndependentUpdate: true);
        }

        protected virtual void HandlePositiveBtnClicked()
        {
            if (isAutoHide)
            {
                Hide(new ClickedEventData() { isPositive = true });
            }
            else
            {
                NotifyEventButtonClicked(new ClickedEventData() { isPositive = true });
            }
        }

        protected virtual void HandleNegativeBtnClicked()
        {
            if (isAutoHide)
            {
                Hide(new ClickedEventData() { isPositive = false });
            }
            else
            {
                NotifyEventButtonClicked(new ClickedEventData() { isPositive = false });
            }
        }

        protected static void NotifyEventButtonClicked(ClickedEventData clickedEventData)
        {
            if (clickedEventData == null)
                return;
            OnButtonClicked(clickedEventData);
        }

        /// <summary>
        /// Usage : Call as Singleton
        /// </summary>
        /// <param name="positiveBtnActive"></param>
        /// <param name="negativeBtnActive"></param>
        /// <param name="autoHide"></param>
        public static void Show(bool positiveBtnActive = true, bool negativeBtnActive = false, bool autoHide = true)
        {
            isAutoHide = autoHide;
            Instance.positiveBtn.gameObject.SetActive(positiveBtnActive);
            Instance.negativeBtn.gameObject.SetActive(negativeBtnActive);
            Canvas.sortingOrder = Instance.sortingOrder;
            CanvasGroup.gameObject.SetActive(true);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = true;
            CanvasGroup.FadeIn(isIndependentUpdate: true);
        }

        public static void Hide(Action onHidden = null)
        {
            Hide(null, onHidden);
        }

        public static void Hide(ClickedEventData clickedEventData, Action onHidden = null)
        {
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = false;
            CanvasGroup.FadeOut(callBack: () =>
            {
                CanvasGroup.gameObject.SetActive(false);
                Title = string.Empty;
                Message = string.Empty;
                MessageTextSize = defaultMessageTextSize;
                if (customContent != null)
                {
                    Destroy(customContent.gameObject);
                }
                onHidden?.Invoke();
                NotifyEventButtonClicked(clickedEventData);
                OnButtonClicked = delegate { }; // Clear invocation list
            },
            isIndependentUpdate: true);
        }

        public static void ResetPropertiesToDefault()
        {
            PositiveText = I2LHelper.TranslateTerm(I2LTerm.MessageManager_Okay);
            NegativeText = I2LHelper.TranslateTerm(I2LTerm.MessageManager_NoThanks);
            MessageTextSize = defaultMessageTextSize;
        }

        public static RectTransform AddCustomContent(RectTransform content)
        {
            if (customContent != null)
            {
                Destroy(customContent.gameObject);
            }
            customContent = Instantiate(content, Instance.messageText.transform.parent);
            if (!customContent.TryGetComponent<LayoutElement>(out _))
            {
                var layoutElement = customContent.gameObject.AddComponent<LayoutElement>();
                layoutElement.minHeight = customContent.sizeDelta.y;
            }
            return customContent;
        }

        public class ClickedEventData
        {
            public bool isPositive;
        }
    }
}