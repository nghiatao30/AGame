using UnityEngine;
using DG.Tweening;
using LatteGames.EditableStateMachine;
using UnityEngine.UI;
using GachaSystem.Core;

namespace LatteGames.UnpackAnimation
{
    [CreateAssetMenu(fileName = "CardDropDownStateSO", menuName = "LatteGames/ScriptableObject/GachaSystem/StateSO/CardDropDownStateSO")]
    public class CardDropDownStateSO : StateSO
    {
        [SerializeField] protected float cardJumpOutDuration = 0.3f;
        [SerializeField] protected float startYOffset = 800;
        [SerializeField] protected ParticleSystem cardFXPrefab;

        protected Vector3 originalPos;
        protected CanvasGroup unpackCanvasGroup;
        protected ParticleSystem cardFXInstance;
        protected RectTransform cardFXRect => cardFXInstance.GetComponent<RectTransform>();
        protected OpenPackAnimationSM controller;

        public override void SetupState(object[] parameters = null)
        {
            if (parameters[0] is not OpenPackAnimationSM) return;
            var controller = (OpenPackAnimationSM)parameters[0];
            this.controller = controller;
            cardFXInstance = Instantiate(cardFXPrefab, controller.FXCanvas);
            cardFXInstance.gameObject.SetActive(false);
            originalPos = cardFXRect.anchoredPosition;
            unpackCanvasGroup = controller.UnpackUICanvasGroup;
            foreach (var transition in transitions)
            {
                transition.SetupTransition(new object[] { controller, cardFXRect });
            }
        }

        protected override void StateEnable()
        {
            if (cardFXInstance == null) return;

            GachaCard_GachaItem GachaItem = controller.CurrentGroupedCard.representativeCard as GachaCard_GachaItem;
            bool isNewCard = false;
            if (GachaItem != null)
            {
                if (GachaItem.GachaItemSO.TryGetModule<NewItemModule>(out var newItemModule))
                {
                    isNewCard = newItemModule.isNew;
                }
            }
            Image darkenImage = controller.DarkenImage;
            if (darkenImage != null && isNewCard)
                darkenImage.DOFade(1, cardJumpOutDuration);

            unpackCanvasGroup.alpha = 1;

            cardFXInstance.gameObject.SetActive(true);
            cardFXInstance.Play();

            Vector3 originalPosDefault = new Vector3(0, 375, 0);
            cardFXRect.DOKill();
            cardFXRect.anchoredPosition = isNewCard ? Vector3.zero : originalPosDefault;
            cardFXRect.anchoredPosition = originalPos + startYOffset * Vector3.up;
            cardFXRect.DOAnchorPos(originalPos, cardJumpOutDuration);
        }

        protected override void StateDisable()
        {
            cardFXRect.DOKill();
            cardFXRect.anchoredPosition = originalPos;

            cardFXInstance.Stop();
            cardFXInstance.gameObject.SetActive(false);

            Image darkenImage = controller.DarkenImage;
            if (darkenImage != null)
            {
                darkenImage.DOKill();
                darkenImage.DOFade(0, 0);
            }
        }

        protected override void StateUpdate()
        {
            //Do nothing
        }
    }
}