using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

namespace LatteGames.PvP.TrophyRoad
{
    public class TrophyRoadArenaSectionUI : MonoBehaviour
    {
        public event Action<ExpandEventData> OnExpand = delegate { };
        public event Action<ShrinkEventData> OnShrink = delegate { };

        [Header("General")]
        [SerializeField] protected float spacing; // Between each milestone
        [SerializeField] protected TrophyRoadRewardUI rewardUIPrefab;
        [SerializeField] protected RectTransform upcomingContentLabelPrefab;
        [SerializeField] protected TrophyRoadArenaBannerUI bannerUI;
        [SerializeField] protected RectTransform milestoneBase;
        [SerializeField] protected RectTransform pointerAnchor;
        [Header("Fill bars")]
        [SerializeField] protected Fill fillBackground;
        [SerializeField] protected Fill currentfill;
        [SerializeField] protected Fill highestAchievedFill;
        [Header("Expandable tab")]
        [SerializeField] protected TrophyRoadFoundInCardsTabUI cardsTabUI;
        [SerializeField] protected RectTransform expandableTab;
        [SerializeField] protected float expandTabWidth;
        [SerializeField] protected Image shrinkTrigger;
        [Header("Locked views")]
        [SerializeField] protected Image darkLayer;

        protected TrophyRoadSO trophyRoadSO;
        protected TrophyRoadSO.ArenaSection lastSection;
        protected TrophyRoadSO.ArenaSection section;
        protected TrophyRoadSO.ArenaSection nextSection;
        protected List<TrophyRoadSO.Milestone> SectionMilestones => section.milestones;
        protected List<MilestoneUI> milestoneUIs = new();
        protected float defaultTabWidth;
        protected bool isExpanding = false;
        protected List<Tween> tweens = new();

        public RectTransform PointerAnchor => pointerAnchor;
        public Fill CurrentFill => currentfill;
        public bool IsCurrentFillFull => currentfill.Height >= Height;

        /// <summary>
        /// The section RectTransform height
        /// </summary>
        public virtual float Height { get; protected set; }
        /// <summary>
        /// The anchored position Y inside the scollview
        /// </summary>
        public virtual float PosY
        {
            get => GetComponent<RectTransform>().anchoredPosition.y;
            set
            {
                var rectTransform = GetComponent<RectTransform>();
                var newPos = rectTransform.anchoredPosition;
                newPos.y = value;
                rectTransform.anchoredPosition = newPos;
            }
        }

        protected virtual void Awake()
        {
            defaultTabWidth = expandableTab.sizeDelta.x;
            shrinkTrigger.enabled = false;
        }

        protected void ClearTweens()
        {
            foreach (var tween in tweens)
            {
                if (tween == null) continue;
                DOTween.Kill(tween);
            }
            tweens.Clear();
        }

        public void Expand()
        {
            SetExpanding(true);
        }

        public void Shrink()
        {
            SetExpanding(false);
        }

        public virtual void SetExpanding(bool shouldExpand)
        {
            if (isExpanding == shouldExpand) return;
            var tabSize = expandableTab.sizeDelta;
            tabSize.x = shouldExpand ? expandTabWidth : defaultTabWidth;
            expandableTab.sizeDelta = tabSize;
            shrinkTrigger.enabled = shouldExpand;
            cardsTabUI.SetExpanding(shouldExpand);

            Delegate toBeInvoked = shouldExpand ? OnExpand : OnShrink;
            object data = shouldExpand ?
                new ExpandEventData() { sectionUI = this } :
                new ShrinkEventData() { sectionUI = this };
            toBeInvoked.DynamicInvoke(data);

            isExpanding = shouldExpand;
        }

        public virtual void Setup(TrophyRoadSO trophyRoadSO, TrophyRoadSO.ArenaSection section)
        {
            this.trophyRoadSO = trophyRoadSO;
            this.section = section;
            var nextSectionIndex = trophyRoadSO.ArenaSections.IndexOf(section) + 1;
            var lastSectionIndex = trophyRoadSO.ArenaSections.IndexOf(section) - 1;

            try
            {
                lastSection = trophyRoadSO.ArenaSections[lastSectionIndex];
            }
            catch
            {
                lastSection = null;
            }

            try
            {
                
                nextSection = trophyRoadSO.ArenaSections[nextSectionIndex];
            }
            catch
            {
                nextSection = null;
            }

            var curHeight = spacing;
            bannerUI.Setup(curHeight, section.arenaSO);

            var newMilestoneBase = new MilestoneUI(Instantiate(milestoneBase, milestoneBase.parent));
            for (int i = 0; i < newMilestoneBase.RectTransform.transform.childCount; i++)
                newMilestoneBase.RectTransform.transform.GetChild(i).gameObject.SetActive(false);
            newMilestoneBase.PosY = curHeight;
            if (lastSection != null)
            {
                newMilestoneBase.RequiredAmount = section.GetRequiredMedals();
            }
            else
            {
                newMilestoneBase.RequiredAmount = 0;
            }


            milestoneUIs.Add(newMilestoneBase);
            for (int i = 0; i < SectionMilestones.Count; i++)
            {
                var milestone = SectionMilestones[i];
                curHeight += spacing;
                var newMilestoneUI = new MilestoneUI(Instantiate(milestoneBase, milestoneBase.parent));
                newMilestoneUI.PosY = curHeight;
                newMilestoneUI.RequiredAmount = milestone.requiredAmount;
                if (milestone.reward != null)
                {
                    var rewardUI = Instantiate(rewardUIPrefab, newMilestoneUI.RectTransform);
                    rewardUI.Setup(milestone);
                    newMilestoneUI.RewardUI = rewardUI;
                }
                else if (i == SectionMilestones.Count - 1) // Last milestone of the entire road
                {
                    Instantiate(upcomingContentLabelPrefab, newMilestoneUI.RectTransform);
                }
                milestoneUIs.Add(newMilestoneUI);
            }
            curHeight += spacing;
            if (nextSection != null)
            {
                var nextArenaMilestoneUI = new MilestoneUI(Instantiate(milestoneBase, milestoneBase.parent));
                nextArenaMilestoneUI.PosY = curHeight;
                nextArenaMilestoneUI.RequiredAmount = nextSection.GetRequiredMedals();
                milestoneUIs.Add(nextArenaMilestoneUI);
            }

            fillBackground.Height = curHeight;
            Height = curHeight;

            SetupCardsTab();
            UpdateLockedViewsImmediately();
        }

        /// <summary>
        /// Update locked views immediately by arena state
        /// </summary>
        public virtual void UpdateLockedViewsImmediately()
        {
            var sectionLocked = !section.IsUnlocked;
            darkLayer.enabled = sectionLocked;
            bannerUI.SetLocked(sectionLocked);
            cardsTabUI.SetLocked(sectionLocked);
        }

        /// <summary>
        /// Refresh the UI immediately by the arena state
        /// </summary>
        public virtual void UpdateUIImmediately()
        {
            StopAllCoroutines();
            ClearTweens();
            UpdateFillsImmediately(trophyRoadSO.HighestAchievedMedals, trophyRoadSO.CurrentMedals);
            UpdateLockedViewsImmediately();
            foreach (var milestoneUI in milestoneUIs)
            {
                if (milestoneUI.RewardUI == null) continue;
                milestoneUI.RewardUI.UpdateUIImmediately();
            }
        }

        public virtual IEnumerator CRPlayUpdatingHighestAchievedFillAnimation(float highestAchievedMedals)
        {
            var oldFillHeight = highestAchievedFill.Height;
            var newFillHeight = GetFillHeightFromMedals(highestAchievedMedals);
            MilestoneUI passedMilestoneUI = null;
            if (newFillHeight > oldFillHeight) // This means player has more medals from the last time
            {
                if (darkLayer.enabled && section.IsUnlocked) // Player has just unlocked this arena -> update locked views & play confetti
                {
                    UpdateLockedViewsImmediately();
                    ConfettiParticleUI.Instance.PlayFX();
                    yield return Yielders.Get(1f);
                }
                passedMilestoneUI = milestoneUIs.Find(milestoneUI => milestoneUI.PosY <= newFillHeight && milestoneUI.PosY > oldFillHeight);
            }
            if (passedMilestoneUI != null && passedMilestoneUI.RewardUI != null)
            {
                yield return WaitForFillTween(highestAchievedFill, passedMilestoneUI.PosY);
                yield return passedMilestoneUI.RewardUI.CRUpdateUIAnimation();
            }
            yield return WaitForFillTween(highestAchievedFill, newFillHeight);
        }

        public virtual IEnumerator CRPlayUpdatingCurrentFillAnimation(float currentMedals)
        {
            var newFillHeight = GetFillHeightFromMedals(currentMedals);
            yield return WaitForFillTween(currentfill, newFillHeight);
        }

        protected WaitWhile WaitForFillTween(Fill fill, float targetHeight)
        {
            if (Mathf.Approximately(fill.Height, targetHeight)) return null;
            var fillTween = FillTween(fill, targetHeight);
            tweens.Add(fillTween);
            return new WaitWhile(fillTween.IsActive);
        }

        protected Tween FillTween(Fill fill, float targetHeight)
        {
            return DOTween.To(() => fill.Height, value => fill.Height = value, targetHeight, 1f);
        }

        public virtual bool NeedUpdate(float highestAchievedMedals, float currentMedals)
        {
            return !Mathf.Approximately(highestAchievedFill.Height, GetFillHeightFromMedals(highestAchievedMedals)) ||
                !Mathf.Approximately(currentfill.Height, GetFillHeightFromMedals(currentMedals));
        }

        /// <summary>
        /// Update fill bars immediately
        /// </summary>
        /// <param name="highestAchievedMedals"></param>
        /// <param name="currentMedals"></param>
        public virtual void UpdateFillsImmediately(float highestAchievedMedals, float currentMedals)
        {
            highestAchievedFill.Height = GetFillHeightFromMedals(highestAchievedMedals);
            currentfill.Height = GetFillHeightFromMedals(currentMedals);
        }

        public virtual float GetFillHeightFromMedals(float medals)
        {
            var index = milestoneUIs.FindIndex(milestoneUI => milestoneUI.RequiredAmount > medals);
            if (index > 0)
            {
                var up = milestoneUIs[index];
                var down = milestoneUIs[index - 1];
                var t = Mathf.InverseLerp(down.RequiredAmount, up.RequiredAmount, medals);
                return Mathf.Lerp(down.PosY, up.PosY, t);
            }
            else if (index < 0)
            {
                return Height;
            }

            return spacing;
        }

        /// <summary>
        /// Setup the cards tab on the left
        /// </summary>
        protected virtual void SetupCardsTab()
        {
            var rectTransform = GetComponent<RectTransform>();
            var size = rectTransform.sizeDelta;
            size.y = Height;
            rectTransform.sizeDelta = size;
            cardsTabUI.Setup(trophyRoadSO, section, nextSection);
        }

        public class MilestoneUI
        {
            RectTransform rectTransform;
            TextMeshProUGUI requiredAmountText;
            float requiredAmount;
            TrophyRoadRewardUI rewardUI;
            public MilestoneUI(RectTransform rectTransform)
            {
                this.rectTransform = rectTransform;
                rectTransform.gameObject.SetActive(true);
                requiredAmountText = rectTransform.GetComponentInChildren<TextMeshProUGUI>();
            }
            public RectTransform RectTransform => rectTransform;
            public float PosY
            {
                get => rectTransform.anchoredPosition.y;
                set
                {
                    rectTransform.anchoredPosition = new Vector2(0f, value);
                }
            }
            public float RequiredAmount
            {
                get => requiredAmount;
                set
                {
                    requiredAmount = value;
                    requiredAmountText.text = value.ToString();
                }
            }
            public TrophyRoadRewardUI RewardUI { get => rewardUI; set => rewardUI = value; }
        }

        [Serializable]
        public class Fill
        {
            public Image fillImage;
            public RectTransform RectTransform => fillImage.rectTransform;
            public float Height
            {
                get => RectTransform.sizeDelta.y;
                set
                {
                    RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, value);
                }
            }
        }

        public class ExpandEventData
        {
            public TrophyRoadArenaSectionUI sectionUI;
        }

        public class ShrinkEventData
        {
            public TrophyRoadArenaSectionUI sectionUI;
        }
    }
}
