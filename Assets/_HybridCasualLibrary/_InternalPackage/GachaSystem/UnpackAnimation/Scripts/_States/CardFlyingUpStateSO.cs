using System.Collections.Generic;
using DG.Tweening;
using GachaSystem.Core;
using HyrphusQ.Events;
using LatteGames.EditableStateMachine;
using LatteGames.Template;
using UnityEngine;
using UnityEngine.UI;

namespace LatteGames.UnpackAnimation
{
    [CreateAssetMenu(fileName = "CardFlyingUpStateSO", menuName = "LatteGames/ScriptableObject/GachaSystem/StateSO/CardFlyingUpStateSO")]
    public class CardFlyingUpStateSO : StateSO
    {
        [SerializeField] protected float cardJumpOutDuration = 0.3f;
        [SerializeField] protected float startYOffset = 800;
        [SerializeField] protected ParticleSystem cardFXPrefab;
        [SerializeField] protected float delayToFirstOpenCard = 0.4f;
        [SerializeField] protected float delayToOpenCard = 0.7f;

        protected Vector3 originalPos;
        protected CanvasGroup unpackCanvasGroup;
        protected ParticleSystem cardFXInstance;
        protected RectTransform cardFXRect => cardFXInstance.GetComponent<RectTransform>();
        protected OpenPackAnimationSM controller;

        protected Dictionary<string, ItemSO> newItemSOUnlocking;

        public override void SetupState(object[] parameters = null)
        {
            GameEventHandler.AddActionEvent(ItemManagementEventCode.OnItemUnlocked, OnGetNewGearUnlock);
            GameEventHandler.AddActionEvent(UnpackEventCode.OnUnpackDone, OnUnpackDone);
            newItemSOUnlocking = new Dictionary<string, ItemSO>();

            if (parameters[0] is not OpenPackAnimationSM) return;
            controller = (OpenPackAnimationSM)parameters[0];
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

            DuplicateGachaCardsGroup currentCard = controller.CurrentGroupedCard;
            GachaCard_GachaItem GachaItem = controller.CurrentGroupedCard.representativeCard as GachaCard_GachaItem;
            bool isNewCard = false;
            if (GachaItem != null)
            {
                string name = "";
                if (GachaItem.GachaItemSO.TryGetModule<NameItemModule>(out var nameItemModule))
                    name = nameItemModule.displayName;

                if (newItemSOUnlocking.ContainsKey(name) && !currentCard.isBonusCard)
                {
                    isNewCard = true;
                    newItemSOUnlocking.Remove(name);
                }
            }

            var bagInstance = (Bag)controller.PackInstance;
            bagInstance.packShadow.SetActive(!isNewCard);
            SkinnedMeshRenderer boxSkinMesh = bagInstance.packGameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (boxSkinMesh != null)
                boxSkinMesh.enabled = !isNewCard;

            bagInstance.lightFX.gameObject.SetActive(!isNewCard);
            bagInstance.packAnimator.SetFloat("OpenSpeed", 1);
            if (controller.CurrentShowingCardIndex == 0)
            {
                bagInstance.packAnimator.SetTrigger("FirstOpen");
                bagInstance.lightFX.Play();
                GameEventHandler.Invoke(UnpackEventCode.OnOpenSubPackStart, controller.CurrentSubPackInfo);
            }
            else
            {
                bagInstance.packAnimator.SetTrigger("Open");
            }

            Image darkenImage = controller.DarkenImage;
            if (darkenImage != null && isNewCard)
            {
                darkenImage
                    .DOFade(1, cardJumpOutDuration)
                    .SetDelay(controller.CurrentShowingCardIndex == 0 ? delayToFirstOpenCard : delayToOpenCard);
            }

            SoundManager.Instance.PlaySFX(GeneralSFX.UIBoxShaking);

            Vector3 originalPosDefault = new Vector3(0, 375, 0);
            cardFXRect.DOKill();
            cardFXRect.anchoredPosition = isNewCard ? Vector3.zero : originalPosDefault;
            originalPos = cardFXRect.anchoredPosition;
            foreach (var transition in transitions)
            {
                transition.SetupTransition(new object[] { controller, cardFXRect });
            }

            unpackCanvasGroup.alpha = 1;
            cardFXInstance.gameObject.SetActive(true);
            cardFXRect.anchoredPosition = originalPos + startYOffset * Vector3.down;
            cardFXRect.DOAnchorPos(originalPos, cardJumpOutDuration).SetDelay(controller.CurrentShowingCardIndex == 0 ? delayToFirstOpenCard : delayToOpenCard).OnStart(() =>
            {
                cardFXInstance.Play();
                SoundManager.Instance.PlaySFX(GeneralSFX.UIOpenBox);
            });
        }

        protected override void StateDisable()
        {
            var bagInstance = (Bag)controller.PackInstance;
            bagInstance.packAnimator.SetFloat("OpenSpeed", 3);
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

        private void OnGetNewGearUnlock(params object[] parrams)
        {
            if (parrams == null || parrams.Length <= 0) return;

            ItemSO itemSO = parrams[1] as ItemSO;
            if (itemSO != null)
            {
                string name = itemSO.GetModule<NameItemModule>().displayName;
                if (!newItemSOUnlocking.ContainsKey(name))
                    newItemSOUnlocking.Add(name, itemSO);
            }
        }

        private void OnUnpackDone()
        {
            newItemSOUnlocking.Clear();
        }
    }
}