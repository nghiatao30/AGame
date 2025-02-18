using UnityEngine;
using HyrphusQ.Events;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

namespace LatteGames.PvP.TrophyRoad
{
    public class TrophyRoadUI : MonoBehaviour
    {
        protected static bool firstTimeSetup = true;
        protected static float lastOpenHighestAchievedMedals; // highestAchievedMedals stored from the last time this opened
        protected static float lastOpenCurrentMedals; // currentMedals stored from the last time this opened

        [SerializeField] EventCode trophyRoadOpenedEventCode;
        [SerializeField] EventCode trophyRoadClosedEventCode;
        [SerializeField] EventCode unpackStartEventCode;
        [SerializeField] EventCode unpackDoneEventCode;
        [SerializeField] protected TrophyRoadSO trophyRoadSO;
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected TrophyRoadArenaSectionUI arenaSectionUIPrefab;
        [SerializeField] protected ScrollRect scrollRect;
        [SerializeField] protected RectTransform scrollViewContent;
        [SerializeField] protected TrophyRoadPointerUI pointerUI;

        protected bool isVisible = false;
        protected List<TrophyRoadArenaSectionUI> sectionUIs = new();
        protected TrophyRoadArenaSectionUI currentSectionUI; // determined by current medals not highest medals
        protected RectTransform ContentParent => scrollViewContent.parent as RectTransform;

        protected virtual void Awake()
        {
            GameEventHandler.AddActionEvent(trophyRoadOpenedEventCode, HandleOpened);
            GameEventHandler.AddActionEvent(trophyRoadClosedEventCode, HandleClosed);
        }

        protected virtual void OnDestroy()
        {
            GameEventHandler.RemoveActionEvent(trophyRoadOpenedEventCode, HandleOpened);
            GameEventHandler.RemoveActionEvent(trophyRoadClosedEventCode, HandleClosed);
            GameEventHandler.RemoveActionEvent(unpackStartEventCode, HandleUnpackStart);
            GameEventHandler.RemoveActionEvent(unpackDoneEventCode, HandleUnpackDone);
        }

        protected virtual IEnumerator Start()
        {
            yield return null;
            Setup();
        }

        protected void UpdateLastOpenMedals()
        {
            lastOpenHighestAchievedMedals = trophyRoadSO.HighestAchievedMedals;
            lastOpenCurrentMedals = trophyRoadSO.CurrentMedals;
        }

        /// <summary>
        /// Only called once at Start
        /// </summary>
        protected virtual void Setup()
        {
            if (firstTimeSetup)
            {
                UpdateLastOpenMedals();
            }
            firstTimeSetup = false;

            // Generate sectionUIs
            TrophyRoadArenaSectionUI prevSectionUI = null;
            foreach (var section in trophyRoadSO.ArenaSections)
            {
                var newSectionUI = Instantiate(arenaSectionUIPrefab, scrollViewContent);
                newSectionUI.PosY = prevSectionUI != null ? (prevSectionUI.PosY + prevSectionUI.Height) : 0f;
                newSectionUI.Setup(trophyRoadSO, section);
                prevSectionUI = newSectionUI;
                sectionUIs.Add(newSectionUI);

                newSectionUI.UpdateFillsImmediately(lastOpenHighestAchievedMedals, lastOpenCurrentMedals);
                if (currentSectionUI == null && !newSectionUI.IsCurrentFillFull)
                {
                    currentSectionUI = newSectionUI;
                }
                newSectionUI.OnExpand += HandleSectionExpand;
                newSectionUI.OnShrink += HandleSectionShrink;
            }
            if (currentSectionUI == null)
                currentSectionUI = sectionUIs[^1];

            // Setup scollView
            var newSize = scrollViewContent.sizeDelta;
            newSize.y = prevSectionUI.PosY + prevSectionUI.Height;
            scrollViewContent.sizeDelta = newSize;
            pointerUI.transform.SetParent(scrollViewContent);
        }

        protected virtual void HandleSectionExpand(TrophyRoadArenaSectionUI.ExpandEventData data)
        {
            scrollRect.inertia = false;
            SnapScrollViewAt(data.sectionUI.PosY + data.sectionUI.Height);
        }

        protected virtual void HandleSectionShrink(TrophyRoadArenaSectionUI.ShrinkEventData data)
        {
            scrollRect.inertia = true;
        }

        private void SnapScrollViewAt(float yPos)
        {
            var newPos = scrollViewContent.anchoredPosition;
            newPos.y = ContentParent.rect.height - yPos;
            scrollViewContent.anchoredPosition = newPos;
        }

        protected virtual IEnumerator CRPlayUpdatingUIAnimation()
        {
            canvasGroup.interactable = false;
            // List out sections that need updating
            List<TrophyRoadArenaSectionUI> sectionUIsNeedUpdate = new();
            var newHighestAchievedMedals = trophyRoadSO.HighestAchievedMedals;
            var newCurrentMedals = trophyRoadSO.CurrentMedals;
            foreach (var sectionUI in sectionUIs)
            {
                if (sectionUI.NeedUpdate(newHighestAchievedMedals, newCurrentMedals))
                {
                    sectionUIsNeedUpdate.Add(sectionUI);
                }
            }
            if (sectionUIsNeedUpdate.Count > 0)
            {
                // Reverse the updating list if player has been demoted to lower arena
                if (newCurrentMedals < lastOpenCurrentMedals)
                {
                    sectionUIsNeedUpdate.Reverse();
                }
                currentSectionUI = sectionUIsNeedUpdate[^1];
                // Update highest fills
                foreach (var sectionUI in sectionUIsNeedUpdate)
                {
                    yield return StartCoroutine(sectionUI.CRPlayUpdatingHighestAchievedFillAnimation(newHighestAchievedMedals));
                }
                // Update current fills
                foreach (var sectionUI in sectionUIsNeedUpdate)
                {
                    yield return StartCoroutine(sectionUI.CRPlayUpdatingCurrentFillAnimation(newCurrentMedals));
                }
            }

            if (currentSectionUI != null)
            {
                pointerUI.Anchor = currentSectionUI.PointerAnchor;
                pointerUI.Show();
            }
            canvasGroup.interactable = true;
        }

        protected virtual void HandleOpened()
        {
            if (isVisible) return;
            SetVisible(true);
            GameEventHandler.AddActionEvent(unpackStartEventCode, HandleUnpackStart); // This occurs when opening rewards

            StartCoroutine(CRPlayUpdatingUIAnimation());
            UpdateLastOpenMedals();
            SnapScrollViewAt(currentSectionUI.PosY + currentSectionUI.GetFillHeightFromMedals(trophyRoadSO.CurrentMedals) + ContentParent.rect.height * 0.5f);
            scrollRect.inertia = true;
        }

        protected virtual void HandleClosed()
        {
            if (!isVisible) return;
            foreach (var sectionUI in sectionUIs)
            {
                sectionUI.UpdateUIImmediately(); // Refresh in case the player backs during animation
            }
            SetVisible(false);
            GameEventHandler.RemoveActionEvent(unpackStartEventCode, HandleUnpackStart);

            StopAllCoroutines();
            pointerUI.Hide();
            foreach (var sectionUI in sectionUIs)
            {
                sectionUI.Shrink();
            }
            scrollRect.inertia = false;
        }

        private void HandleUnpackStart()
        {
            HandleClosed(); // Don't invoke the OnTrophyRoadClose as it will reopen the main game canvas
            GameEventHandler.AddActionEvent(unpackDoneEventCode, HandleUnpackDone);
        }

        private void HandleUnpackDone()
        {
            GameEventHandler.Invoke(trophyRoadOpenedEventCode); // Invoke the OnTrophyRoadOpened to close the main game canvas
            GameEventHandler.RemoveActionEvent(unpackDoneEventCode, HandleUnpackDone);
        }

        protected virtual void SetVisible(bool isVisible)
        {
            this.isVisible = isVisible;
            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
    }
}
