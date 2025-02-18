using TMPro;
using UnityEngine;
using DG.Tweening;

namespace LatteGames.PvP.TrophyRoad
{
    public class TrophyRoadPointerUI : MonoBehaviour
    {
        [SerializeField] protected TrophyRoadSO trophyRoadSO;
        [SerializeField] protected TextMeshProUGUI medalsAmountText;
        [SerializeField] protected TextMeshProUGUI avatarMedalsAmountText;
        [SerializeField] protected RectTransform background;
        [SerializeField] protected float backgroundWrappingTextPadding;
        [SerializeField] protected float showingTextTime;

        protected Transform anchor;
        protected Vector2 defaultBackgroundSize;
        protected Vector2 extendedBackgroundSize; // by medalsAmountText;
        protected bool isShowing = false;
        protected float showTimer = -1f;

        public Transform Anchor { get => anchor; set => anchor = value; }

        protected virtual void Awake()
        {
            defaultBackgroundSize = background.sizeDelta;
            transform.localScale = Vector3.zero;
        }

        protected virtual void Start()
        {
            avatarMedalsAmountText.text = medalsAmountText.text = trophyRoadSO.CurrentMedals.ToString();
        }

        protected virtual void Update()
        {
            if (showTimer > 0f)
            {
                showTimer -= Time.deltaTime;
                if (showTimer <= 0f)
                {
                    HideMedalsAmount();
                }
            }
        }

        protected virtual void LateUpdate()
        {
            if (anchor != null)
            {
                transform.position = anchor.position;
            }
        }

        public virtual void Show(bool withAmount = true)
        {
            transform.ScaleUp(callback: scaleUpComplete);
            void scaleUpComplete()
            {
                if (withAmount)
                {
                    ShowMedalsAmount();
                }
            }
        }

        public virtual void Hide()
        {
            transform.ScaleDown();
        }

        /// <summary>
        /// Need a Button or an EventTrigger component to call this
        /// </summary>
        public virtual void ShowMedalsAmount()
        {
            SetMedalsAmountVisible(true);
            showTimer = showingTextTime;
        }

        protected virtual void HideMedalsAmount()
        {
            SetMedalsAmountVisible(false);
        }

        public virtual void SetMedalsAmountVisible(bool isVisible)
        {
            if (isShowing == isVisible) return;
            var targetAlpha = isVisible ? 1f : 0f;
            medalsAmountText.DOFade(targetAlpha, 0.5f).From(1f - targetAlpha).OnUpdate(tweenUpdate);
            if (isVisible)
            {
                extendedBackgroundSize = defaultBackgroundSize;
                var textTransform = medalsAmountText.rectTransform;
                extendedBackgroundSize.x = textTransform.anchoredPosition.x + textTransform.sizeDelta.x + backgroundWrappingTextPadding;
            }
            void tweenUpdate()
            {
                background.sizeDelta = Vector2.Lerp(defaultBackgroundSize, extendedBackgroundSize, medalsAmountText.alpha);
            }
            isShowing = isVisible;
        }
    }
}
