using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    public class Tabview : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> rectTransforms = null;
        [SerializeField] private float snappingTime = 0.2f;

        private RectTransform selfRect;

        internal int GetTabCount()
        {
            return rectTransforms.Count;
        }

        private RectTransform SelfRect
        {
            get
            {
                if (selfRect == null)
                    selfRect = transform as RectTransform;
                return selfRect;
            }
        }

        private int currentTab = 0;
        private bool animaitonIsRunning = false;
        public bool AnimatonIsRunning
        {
            get { return animaitonIsRunning; }
            private set { animaitonIsRunning = value; }
        }
        public void ChangeTab(int tab)
        {
            if (Mathf.Clamp(tab, 0, rectTransforms.Count - 1) == currentTab)
                return;
            animaitonIsRunning = true;
            StartCoroutine(CommonCoroutine.Wait(ChangeTabCR(currentTab, Mathf.Clamp(tab, 0, rectTransforms.Count - 1)), () => animaitonIsRunning = false));
        }

        private IEnumerator ChangeTabCR(int from, int to)
        {
            bool r2l = from < to;
            RectTransform fromTab = rectTransforms[from];
            RectTransform toTab = rectTransforms[to];

            toTab.SetAsFirstSibling();
            fromTab.SetAsLastSibling();
            Vector2 fromStartPoint = fromTab.anchoredPosition;
            Vector2 toStartPoint = toTab.anchoredPosition;
            fromStartPoint = new Vector2(0, fromTab.anchoredPosition.y);
            if (r2l)
            {
                toStartPoint = new Vector2(SelfRect.rect.width, toTab.anchoredPosition.y);
            }
            else
            {
                toStartPoint = new Vector2(-SelfRect.rect.width, toTab.anchoredPosition.y);
            }

            fromTab.anchoredPosition = fromStartPoint;
            toTab.anchoredPosition = toStartPoint;
            yield return null;

            float t = 0;
            while (t < snappingTime)
            {
                t += Time.deltaTime;
                fromTab.anchoredPosition = Vector2.MoveTowards(fromTab.anchoredPosition, fromStartPoint + Vector2.left * SelfRect.rect.width * (r2l ? 1 : -1), SelfRect.rect.width * Time.deltaTime / snappingTime);
                toTab.anchoredPosition = Vector2.MoveTowards(toTab.anchoredPosition, toStartPoint + Vector2.left * SelfRect.rect.width * (r2l ? 1 : -1), SelfRect.rect.width * Time.deltaTime / snappingTime);
                yield return null;
            }

            fromTab.anchoredPosition = fromStartPoint + Vector2.left * SelfRect.rect.width * (r2l ? 1 : -1);
            toTab.anchoredPosition = toStartPoint + Vector2.left * SelfRect.rect.width * (r2l ? 1 : -1);

            toTab.SetAsLastSibling();
            currentTab = to;
            yield return null;
        }

    }
}