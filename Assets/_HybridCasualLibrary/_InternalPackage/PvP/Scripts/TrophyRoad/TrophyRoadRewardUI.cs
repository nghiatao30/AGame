using System.Collections.Generic;
using GachaSystem.Core;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using LatteGames.UnpackAnimation;
using System;
using HyrphusQ.Helpers;
using System.Collections;

namespace LatteGames.PvP.TrophyRoad
{
    public class TrophyRoadRewardUI : MonoBehaviour
    {
        [SerializeField] protected Image backgroundImage;
        [SerializeField] protected Image leftBackgroundImage;
        [SerializeField] protected Image lockImage;
        [SerializeField] protected TextMeshProUGUI claimText;
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected Image checkImage;
        [SerializeField] protected ColorsSetup lockedColorsSetup;
        [SerializeField] protected ColorsSetup unlockedColorsSetup;
        [SerializeField] protected ColorsSetup claimedColorsSetup;
        [SerializeField] protected AbstractCardController cardControllerPrefab;
        [SerializeField] protected RandomCardUI randomCardPrefab;
        [SerializeField] protected float cardScale;
        [SerializeField] protected RectTransform leftCardAnchorBase;
        [SerializeField] protected RectTransform rightCardAnchorBase;
        [SerializeField] protected Color lockedCardColor;
        [SerializeField] protected float updateUIAnimDuration;
        [Header("FTUE")]
        [SerializeField] protected PPrefBoolVariable PPrefFTUEClaimReward;
        [SerializeField] protected Animator tutorialHandPrefab;
        [SerializeField] protected RuntimeAnimatorController animatorController;
        [SerializeField] protected RectTransform handContainer;

        protected TrophyRoadSO.Milestone milestone;
        protected List<GachaCard> cards;

        protected List<RectTransform> cardControllers = new();
        protected List<RectTransform> leftCardAnchors = new();
        protected List<RectTransform> rightCardAnchors = new();
        protected Animator handInstance;
        protected List<Tween> tweens = new();

        protected bool Unlocked => milestone != null && (milestone.Unlocked || milestone.Claimed);
        protected bool Claimed => milestone != null && milestone.Claimed;

        protected virtual void Awake()
        {
            UpdateUIImmediately();
        }

        protected virtual void OnDestroy()
        {
            UnsubEvents();
        }

        protected void UnsubEvents()
        {
            if (milestone == null) return;
            milestone.OnClaimed -= HandleClaimed;
        }

        protected virtual void HandleClaimed()
        {
            UpdateUIImmediately();
        }

        public virtual void Setup(TrophyRoadSO.Milestone milestone)
        {
            UnsubEvents();
            this.milestone = milestone;
            cards = this.milestone.GetCards();
            this.milestone.OnClaimed += HandleClaimed;
            GenerateCardsUI();
            UpdateUIImmediately();
        }

        /// <summary>
        /// Refresh the UI immediately by the milestone state
        /// </summary>
        public virtual void UpdateUIImmediately()
        {
            StopAllCoroutines();
            StartCoroutine(CRPlayUpdatingUIAnimation(0f));
        }

        /// <summary>
        /// Need a Button or an EventTrigger component to call this
        /// </summary>
        public void Claim()
        {
            if (milestone == null) return;
            milestone.TryClaimCards(cards);
        }

        public Coroutine CRUpdateUIAnimation() => StartCoroutine(CRPlayUpdatingUIAnimation(updateUIAnimDuration));

        protected virtual IEnumerator CRPlayUpdatingUIAnimation(float duration)
        {
            ClearTweens();

            var claimable = Unlocked && !Claimed;

            // Lerp tween
            var oldBackgroundImageColor = backgroundImage.color;
            var oldLeftBackgroundImageColor = leftBackgroundImage.color;
            var t = 0f;
            var tween = DOTween.To(() => t, value => t = value, 1f, duration).OnUpdate(tweenUpdate);
            void tweenUpdate()
            {
                var colorSetup = Claimed ? claimedColorsSetup : (Unlocked ? unlockedColorsSetup : lockedColorsSetup);
                backgroundImage.color = Color.Lerp(oldBackgroundImageColor, colorSetup.backgroundColor, t);
                leftBackgroundImage.color = Color.Lerp(oldLeftBackgroundImageColor, colorSetup.leftColor, t);
                foreach (var cardController in cardControllers)
                {
                    foreach (var img in cardController.GetComponentsInChildren<Image>())
                    {
                        if (img.sprite == null) continue;
                        if (img.TryGetComponentInParent<TextMeshProUGUI>(out _)) continue;
                        img.color = Color.Lerp(lockedCardColor, Color.white, Unlocked ? t : (1f - t));
                    }
                }
                lockImage.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, !Unlocked ? t : (1f - t));
                claimText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, claimable ? t : (1f - t));
            }
            tweens.Add(tween);
            yield return new WaitWhile(tween.IsActive);

            // Update other views immediately
            canvasGroup.interactable = claimable;
            checkImage.enabled = Claimed;
            var cardAnchors = checkImage.enabled ? rightCardAnchors : leftCardAnchors;
            for (int i = 0; i < cardControllers.Count; i++)
            {
                var cardTransform = cardControllers[i].transform;
                cardTransform.SetParent(cardAnchors[i]);
                cardTransform.localPosition = Vector3.zero;
                cardTransform.localRotation = Quaternion.identity;
                cardTransform.localScale = cardScale * Vector3.one;
            }

            // FTUE
            if (claimable && PPrefFTUEClaimReward.value == false)
            {
                handInstance = Instantiate(tutorialHandPrefab, handContainer);
                handInstance.runtimeAnimatorController = animatorController;
                PPrefFTUEClaimReward.value = true;
            }
            else if (!claimable && handInstance != null)
            {
                Destroy(handInstance.gameObject);
            }
        }

        public void ClearTweens()
        {
            foreach (var tween in tweens)
            {
                if (tween == null) continue;
                DOTween.Kill(tween);
            }
            tweens.Clear();
        }

        protected virtual void GenerateCardsUI()
        {
            GenerateExplicitCards();
            GenerateRandomCards();
        }

        protected virtual void GenerateExplicitCards()
        {
            if (cards == null) return;
            var groups = cards.GroupDuplicate();
            for (int i = 0; i < groups.Count; i++)
            {
                CreateAnchorsBothSides();
                var cardControllerInstance = Instantiate(cardControllerPrefab, leftCardAnchors[^1]);
                cardControllerInstance.Setup(groups[i], true);
                cardControllerInstance.transform.localPosition = Vector3.zero;
                cardControllerInstance.transform.localScale = cardScale * Vector3.one;
                cardControllers.Add(cardControllerInstance.GetComponent<RectTransform>());
            }
        }

        protected virtual void GenerateRandomCards()
        {
            if (milestone.reward == null || milestone.reward.generalItems == null) return;
            var cardGenerator = GachaCardGenerator.Instance;
            foreach (var item in milestone.reward.generalItems)
            {
                var willGenerateRandomCards = TryGenerateRandomCardsInfo(item, out var rarity, out var count);
                if (willGenerateRandomCards)
                {
                    CreateAnchorsBothSides();
                    var randomCardInstance = Instantiate(randomCardPrefab, leftCardAnchors[^1]);
                    randomCardInstance.Setup(rarity, count);
                    randomCardInstance.transform.localPosition = Vector3.zero;
                    randomCardInstance.transform.localScale = cardScale * Vector3.one;
                    cardControllers.Add(randomCardInstance.GetComponent<RectTransform>());
                }
            }
        }

        /// <summary>
        /// Determine whether gacha cards can be generated from randomItemSO.
        /// <para>Output: The rarity and count of the cards</para>
        /// <para>Overriding functions must add new generated cards to the list</para>
        /// </summary>
        /// <param name="randomItemSO">The key should have RarityItemModule or a Rarity property, the value is the cards count</param>
        /// <returns></returns>
        protected virtual bool TryGenerateRandomCardsInfo(KeyValuePair<ItemSO, ShopProductSO.DiscountableValue> randomItemSO, out RarityType rarity, out int count)
        {
            rarity = randomItemSO.Key.GetRarityType();
            count = Mathf.RoundToInt(randomItemSO.Value.value);
            return true;
        }

        protected void CreateAnchorsBothSides()
        {
            var newLeftCardAnchor = Instantiate(leftCardAnchorBase, leftCardAnchorBase.parent);
            newLeftCardAnchor.gameObject.SetActive(true);
            leftCardAnchors.Add(newLeftCardAnchor);
            var newRightCardAnchor = Instantiate(rightCardAnchorBase, rightCardAnchorBase.parent);
            newRightCardAnchor.gameObject.SetActive(true);
            rightCardAnchors.Add(newRightCardAnchor);
        }

        [Serializable]
        protected class ColorsSetup
        {
            public Color backgroundColor;
            public Color leftColor;
        }
    }
}
