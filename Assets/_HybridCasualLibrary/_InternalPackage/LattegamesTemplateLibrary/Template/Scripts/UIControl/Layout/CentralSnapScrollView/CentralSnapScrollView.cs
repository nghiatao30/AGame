using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using LatteGames.Utils;

namespace LatteGames
{
    public class CentralSnapScrollView : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        private Coroutine snapCR;
        private Canvas _canvas;
        private Canvas Canvas
        {
            get
            {
                if (_canvas == null)
                    _canvas = GetComponentInParent<Canvas>();
                return _canvas;
            }
        }
        public UnityEvent OnIndexChanged;
        private List<RectTransform> childElements;
        public Action IndexPositionChanged;
        public int CurrentCentralIndex { get; private set; }
        private int currentCentralViewIndex;
        public float position;
        public float spacing = 300;
        public float zeroSpacing = 300;
        public int zeroPaddingIndex = 0;
        public float snapTime = 0.2f;
        public float inertia = 100;
        public bool autoInit = true;
        private float velocity;

        public bool IsBlock { get; set; }

        private float GetDragDelta(Vector2 delta)
        {
            return delta.x / Canvas.scaleFactor;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (snapCR != null)
                StopCoroutine(snapCR);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (IsBlock)
                return;
            var delta = GetDragDelta(eventData.delta);
            position -= delta;
            velocity = -delta;
            UpdateViewPosition();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            snapCR = StartCoroutine(EndDragSnapCR());
        }

        public void ScrollToItem(RectTransform viewItem)
        {
            if (IsBlock)
                return;

            var clickIndex = viewList.IndexOf(viewItem);
            if (clickIndex == -1)
                return;
            var crrIndex = viewList.IndexOf(displayList[CurrentCentralIndex]);
            var indexDelta = clickIndex - crrIndex;
            var targetIndex = currentCentralViewIndex + indexDelta;
            if (snapCR != null)
                StopCoroutine(snapCR);
            snapCR = StartCoroutine(SnapToIndexCR(targetIndex));
        }

        public void SetPosition(RectTransform viewItem)
        {
            var clickIndex = viewList.IndexOf(viewItem);
            if (clickIndex == -1)
                return;
            var crrIndex = viewList.IndexOf(displayList[CurrentCentralIndex]);
            var indexDelta = clickIndex - crrIndex;
            var targetIndex = currentCentralViewIndex + indexDelta;
            position = targetIndex * spacing;
            UpdateViewPosition();
        }

        private IEnumerator EndDragSnapCR()
        {
            var vMag = Mathf.Abs(velocity);
            var vDir = Mathf.Sign(velocity);
            while (vMag > 0)
            {
                yield return null;
                vMag -= Time.deltaTime * inertia;
                float lastPosition = position;
                position += vDir * vMag;
                UpdateViewPosition();
                if (lastPosition == position)
                    vMag = 0;
            }
            snapCR = StartCoroutine(SnapToIndexCR(currentCentralViewIndex));
        }

        private IEnumerator SnapToIndexCR(int index)
        {
            float targetPosition = index * spacing;
            float dis = Mathf.Abs(targetPosition - position);
            float dir = Mathf.Sign(targetPosition - position);
            float speed = dis / snapTime;
            float moved = 0;
            while (moved < dis)
            {
                yield return null;
                float deltaChange = Mathf.Min(Time.deltaTime * speed, dis - moved);
                position += dir * deltaChange;
                moved += deltaChange;
                UpdateViewPosition();
            }
        }

        private List<RectTransform> displayList;
        private List<RectTransform> viewList;

        private void UpdateViewPosition()
        {
            if (displayList == null)
                return;
            viewList.Clear();

            float fullLength = (displayList.Count - 1) * spacing;
            if (position < -fullLength)
                position += (fullLength + spacing) * Mathf.Abs(position / fullLength);
            if (position > fullLength)
                position -= (fullLength + spacing) * Mathf.Abs(position / fullLength);
            //Calculate display window
            float displayFromPosition = position - fullLength / 2;
            int displayFromIndex = Mathf.RoundToInt((displayFromPosition - Mathf.RoundToInt(displayFromPosition / fullLength)) / spacing);
            int displayToIndex = displayFromIndex + displayList.Count;

            int negativeOffset = Mathf.Max(0, 1 - (displayFromIndex / displayList.Count)) * displayList.Count;
            float xDis = -1;
            for (int i = displayFromIndex; i < displayToIndex; i++)
            {
                float xPos = i * spacing - position;
                if (xDis == -1 || xPos < xDis)
                {
                    xDis = Mathf.Abs(xPos);
                    zeroPaddingIndex = i;
                }
                int realIndex = (i + negativeOffset) % displayList.Count;
                viewList.Add(displayList[realIndex]);
                displayList[realIndex].anchoredPosition = new Vector2(xPos, displayList[realIndex].anchoredPosition.y);
            }

            for (int i = 0; i < displayList.Count; i++)
            {
                float xPos = displayList[i].anchoredPosition.x;
                float scaleFactor = Mathf.Clamp01(Mathf.Abs(xPos) / spacing);
                displayList[i].anchorMax = new Vector2(0.5f,displayList[i].anchorMax.y);
                displayList[i].anchorMin = new Vector2(0.5f,displayList[i].anchorMin.y);
                displayList[i].anchoredPosition = displayList[i].anchoredPosition + Vector2.right * Mathf.Lerp(0, zeroSpacing, scaleFactor) * Mathf.Sign(xPos);
                displayList[i].sizeDelta = new Vector2(Mathf.Lerp(zeroSpacing * 2 + spacing, spacing, scaleFactor), displayList[i].sizeDelta.y);
            }

            currentCentralViewIndex = zeroPaddingIndex;
            var realZeroIndex = (zeroPaddingIndex + negativeOffset) % displayList.Count;
            var indexChanged = realZeroIndex != CurrentCentralIndex;
            CurrentCentralIndex = realZeroIndex;
            if (indexChanged)
            {
                OnIndexChanged?.Invoke();
                IndexPositionChanged?.Invoke();
            }
        }

        public void Init(List<RectTransform> childElements)
        {
            this.childElements = childElements;
            this.displayList = new List<RectTransform>(childElements);
            this.viewList = new List<RectTransform>(childElements);
            position = 0;
            UpdateViewPosition();
        }

        private void Start() {
            if(autoInit)
                Init(new List<Transform>(transform.GetChildren()).ConvertAll(tf => tf as RectTransform));
        }
    }
}