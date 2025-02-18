using System.Collections.Generic;
using UnityEngine;
using LatteGames.EditableStateMachine;
using GachaSystem.Core;
using TMPro;
using DG.Tweening;
using I2.Loc;
using LatteGames.Template;
using System.Linq;
using HyrphusQ.Events;

namespace LatteGames.UnpackAnimation
{
    [CreateAssetMenu(fileName = "ShowingCardStateSO", menuName = "LatteGames/ScriptableObject/GachaSystem/StateSO/ShowingCardStateSO")]
    public class ShowingCardStateSO : StateSO
    {
        [SerializeField] protected float minShowingCardTime = 1f;
        [SerializeField] protected AbstractCardController cardControllerPrefab;
        [SerializeField] protected List<ParticleSystem> cardFXPrefabs;

        protected bool isCTAShown = false;
        protected float elapsedTime = 0;

        protected OpenPackAnimationSM controller;
        protected DuplicateGachaCardsGroup currentCard;
        protected AbstractCardController cardControllerInstance;
        protected ParticleSystem cardExplosionFX;
        protected TMP_Text tapCTAText;

        protected List<ParticleSystem> cardFXList;
        protected RectTransform cardControllerInstanceRect;

        protected bool isNewGachaCard = false;
        protected object titleGachaCardID;
        protected object unlockedGachaCardID;
        protected GameObject titleGlowingObject;
        protected GameObject unlockedGlowingObject;

        protected Dictionary<string, ItemSO> newItemSOUnlocking;

        public override void SetupState(object[] parameters = null)
        {
            GameEventHandler.AddActionEvent(ItemManagementEventCode.OnItemUnlocked, OnGetNewGearUnlock);
            GameEventHandler.AddActionEvent(UnpackEventCode.OnUnpackDone, OnUnpackDone);
            newItemSOUnlocking = new Dictionary<string, ItemSO>();

            if (parameters[0] is not OpenPackAnimationSM) return;
            controller = (OpenPackAnimationSM)parameters[0];
            tapCTAText = controller.TapCTA;
            cardControllerInstance = Instantiate(cardControllerPrefab, controller.UnpackUICanvasGroup.transform);
            cardControllerInstance.gameObject.SetActive(false);

            cardFXList = new List<ParticleSystem>();
            for (int i = 0; i < cardFXPrefabs.Count; i++)
            {
                var fx = Instantiate(cardFXPrefabs[i], controller.FXCanvas);
                fx.gameObject.SetActive(false);
                cardFXList.Add(fx);
            }

            foreach (var transition in transitions)
            {
                transition.SetupTransition(new object[] { controller, cardControllerInstance, minShowingCardTime });
            }
        }

        protected override void StateEnable()
        {
            if (controller == null) return;
            currentCard = controller.CurrentGroupedCard;
            if (currentCard == null) return;

            bool isNewCard = false;

            GachaCard_GachaItem GachaItem = controller.CurrentGroupedCard.representativeCard as GachaCard_GachaItem;
            if (GachaItem != null)
            {
                string name = "";
                if (GachaItem.GachaItemSO.TryGetModule<NameItemModule>(out var nameItemModule))
                    name = nameItemModule.displayName;

                controller.TitleNewCardTxt.SetText($"{name}");

                if (newItemSOUnlocking.ContainsKey(name) && !currentCard.isBonusCard)
                {
                    isNewCard = true;
                    newItemSOUnlocking.Remove(name);
                }  
            }

            Vector3 originalPosDefault = new Vector3(0, 375, 0);
            isNewGachaCard = isNewCard;
            cardControllerInstance.GetComponent<RectTransform>().anchoredPosition = isNewGachaCard ? Vector3.zero : originalPosDefault;

            for (int i = 0; i < cardFXList.Count; i++)
            {
                RectTransform cardFX = cardFXList[i].GetComponent<RectTransform>();
                if (cardFX != null)
                {
                    cardFX.anchoredPosition = isNewGachaCard ? Vector3.zero : originalPosDefault;
                }
            }

            HideGrowCardFX();
            HideTitleNewGachaCard();

            cardControllerInstanceRect = cardControllerInstance.GetComponent<RectTransform>();
            controller.RemainingItemAmount--;
            cardExplosionFX = GetCorrespondingShowingCardFX(currentCard.representativeCard);

            cardControllerInstance.IsAnimationEnded = false;
            cardControllerInstance.Setup(currentCard);
            cardControllerInstance.ToggleTitleVisibility(!isNewCard);

            if (isNewGachaCard)
            {
                if (controller.LightImage != null)
                {
                    controller.LightImage.GetComponent<RectTransform>().localScale = Vector3.one * 5;
                    controller.LightImage.DOFade(1, 0);
                    controller.LightImage.DOFade(0, 2f);
                    controller.LightImage.gameObject.SetActive(true);
                    ShowGrowCardFX();
                }
                tapCTAText.gameObject.SetActive(false);
                tapCTAText.DOKill();
            }
            else
            {
                cardExplosionFX.gameObject.SetActive(true);
                cardExplosionFX.Play();
                cardControllerInstance.gameObject.SetActive(true);
                tapCTAText.gameObject.SetActive(false);
                tapCTAText.DOKill();
                tapCTAText.color = Color.white.ToTransparent();
            }

            elapsedTime = 0;
            isCTAShown = false;
            SoundManager.Instance.PlaySFX(GeneralSFX.UICardFlip);
        }

        protected override void StateDisable()
        {
            if (controller == null) return;

            cardExplosionFX.Stop();
            cardExplosionFX.gameObject.SetActive(false);
            cardControllerInstance.IsAnimationEnded = true;
            cardControllerInstance.gameObject.SetActive(false);

            tapCTAText.gameObject.SetActive(false);
            tapCTAText.DOKill();
            tapCTAText.color = Color.white.ToTransparent();

            HideGrowCardFX();
            HideTitleNewGachaCard();

            controller.LightImage.DOKill();
            controller.LightImage.gameObject.SetActive(false);

            isCTAShown = true;
        }

        protected override void StateUpdate()
        {
            if (elapsedTime >= minShowingCardTime)
            {
                if (cardControllerInstance.IsAnimationEnded == true)
                {
                    ShowCTA();
                }
            }
            elapsedTime += Time.deltaTime;
        }

        protected virtual void ShowCTA()
        {
            if (isCTAShown == true) return;
            isCTAShown = true;
            tapCTAText.gameObject.SetActive(true);
            tapCTAText.DOFade(1, 0.3f);

            HideTitleNewGachaCard();
            HideGrowCardFX();

            cardControllerInstance.gameObject.SetActive(true);
            if (isNewGachaCard)
            {
                tapCTAText.gameObject.SetActive(false);
                cardExplosionFX.gameObject.SetActive(true);
                cardExplosionFX.Play();
                ShowTitleNewGachaCard();
            }
        }

        protected virtual ParticleSystem GetCorrespondingShowingCardFX(GachaCard gachaCard)
        {
            if (gachaCard == null)
            {
                return cardFXList[0];
            }
            if (gachaCard.TryGetModule<RarityItemModule>(out var cardRarity))
            {
                return cardRarity.rarityType switch
                {
                    RarityType.Epic => cardFXList[1],
                    RarityType.Legendary => cardFXList[2],
                    _ => cardFXList[0],
                };
            }
            else return cardFXList[0];
        }

        protected virtual void ShowGrowCardFX()
        {
            if (cardFXList[3] == null) return;
            cardFXList[3].Stop();
            cardFXList[3].gameObject.SetActive(true);
            cardFXList[3].Play();
        }

        protected virtual void HideGrowCardFX()
        {
            if (cardFXList[3] == null) return;
            cardFXList[3].Stop();
            cardFXList[3].gameObject.SetActive(false);
        }

        protected virtual void ShowTitleNewGachaCard()
        {
            if (controller == null) return;
            if (controller.TitleNewCardTxt == null || controller.NewCardUnlockedTxt == null) return;

            controller.TitleNewCardTxt.DOKill();
            controller.NewCardUnlockedTxt.DOKill();

            var titleSequence = DOTween.Sequence()
                .SetDelay(0.3f)
                .Append(
                    controller.TitleNewCardTxt
                    .DOScale(1, 0.5f)
                    .SetEase(Ease.InBack))
                    .AppendInterval(1f).Play();

            titleSequence.id = System.Guid.NewGuid();
            titleGachaCardID = titleSequence.id;

            var unlockedSequence = DOTween.Sequence()
                .SetDelay(0.3f)
                .Append(
                    controller.TitleNewCardTxt
                    .DOFade(1, 0.5f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        titleGlowingObject = Instantiate(controller.TitleNewCardTxt.gameObject, controller.TitleNewCardTxt.transform);
                        titleGlowingObject.transform.localPosition = Vector3.zero;
                        TMP_Text titleGlowingTxt = titleGlowingObject.GetComponent<TMP_Text>();
                        if (titleGlowingTxt != null)
                        {
                            titleGlowingTxt.DOFade(0.4f, 0);
                            titleGlowingTxt.DOScale(3f, 1f);
                            titleGlowingTxt.DOFade(0, 0.5f).OnComplete(() => { Destroy(titleGlowingObject); });
                        }

                        controller.NewCardUnlockedTxt.DOScale(1, 0.5f).SetEase(Ease.InBack);
                        controller.NewCardUnlockedTxt
                        .DOFade(1, 0.5f)
                        .SetEase(Ease.InBack)
                        .OnComplete(() =>
                        {
                            unlockedGlowingObject = Instantiate(controller.NewCardUnlockedTxt.gameObject, controller.NewCardUnlockedTxt.transform);
                            unlockedGlowingObject.transform.localPosition = Vector3.zero;
                            TMP_Text unlockedGlowingTxt = unlockedGlowingObject.GetComponent<TMP_Text>();
                            if (unlockedGlowingTxt != null)
                            {
                                unlockedGlowingTxt.DOFade(0.4f, 0);
                                unlockedGlowingTxt.DOScale(3f, 1f);
                                unlockedGlowingTxt.DOFade(0, 0.5f).OnComplete(() => { Destroy(unlockedGlowingObject); });
                            }
                        });
                    }))
                .AppendInterval(1f).Play();

            unlockedSequence.id = System.Guid.NewGuid();
            unlockedGachaCardID = unlockedSequence.id;

            controller.TitleNewCardTxt.gameObject.SetActive(true);
            controller.NewCardUnlockedTxt.gameObject.SetActive(true);
        }


        protected virtual void HideTitleNewGachaCard()
        {
            DOTween.Kill(titleGachaCardID);
            DOTween.Kill(unlockedGachaCardID);

            controller.TitleNewCardTxt.DOKill();
            controller.NewCardUnlockedTxt.DOKill();

            controller.TitleNewCardTxt.DOFade(0, 0);
            controller.TitleNewCardTxt.DOScale(3, 0);

            controller.NewCardUnlockedTxt.DOFade(0, 0);
            controller.NewCardUnlockedTxt.DOScale(3, 0);

            controller.TitleNewCardTxt.gameObject.SetActive(false);
            controller.NewCardUnlockedTxt.gameObject.SetActive(false);

            Destroy(titleGlowingObject);
            Destroy(unlockedGlowingObject);
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