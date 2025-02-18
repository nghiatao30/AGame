using System;
using System.Collections.Generic;
using DG.Tweening;
using LatteGames;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HyrphusQ.GUI
{
    public class SnappableScrollView : MonoBehaviour, IPointerDownHandler
    {
        public event Action<int> OnSnapped = delegate { };

        [SerializeField] List<RectTransform> elements = new();
        [SerializeField] Scrollbar scrollbar;
        [SerializeField] float snapDuration;
        [SerializeField] float unfocusScale = 1f;
        [SerializeField] float focusScale;
        [SerializeField] float snapDelay;
        [SerializeField] bool highlightFocusedItem = true;
        List<float> snapValues = new();
        List<CanvasGroup> elementCanvasGroups = new();

        float t = 0f;
        float getT() => t;
        void setT(float value) => t = value;

        Coroutine snapDelayCoroutine;

        public List<RectTransform> Elements
        {
            get => elements;
            set
            {
                elements = value;
                Awake();
            }
        }

        public int CurrentSnapIndex
        {
            get
            {
                snapValues.FindClosest(scrollbar.value, out var snapIndex);
                return snapIndex;
            }
        }

        private void Awake()
        {
            snapValues.Clear();
            elementCanvasGroups.Clear();
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements.Count - 1 != 0)
                    snapValues.Add(i / (elements.Count - 1f));
                else snapValues.Add(0);
                elementCanvasGroups.Add(elements[i].gameObject.GetOrAddComponent<CanvasGroup>());
            }
            Snap(0);
        }

        void SmoothSnap()
        {
            Snap(snapDuration);
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                DelayThenSnap();
            }
        }

        public void SetSnapControlEnabled(bool isEnabled)
        {
            enabled = isEnabled;
            if (!enabled)
            {
                StopSnapDelayCoroutine();
            }
            else
            {
                SmoothSnap();
            }
        }

        private void DelayThenSnap()
        {
            StopSnapDelayCoroutine();
            snapDelayCoroutine = StartCoroutine(CommonCoroutine.Delay(snapDelay, false, SmoothSnap));
        }

        private void StopSnapDelayCoroutine()
        {
            if (snapDelayCoroutine != null)
            {
                StopCoroutine(snapDelayCoroutine);
            }
        }

        public void Snap(float snapDuration)
        {
            var startValue = scrollbar.value;
            var snapValue = snapValues.FindClosest(startValue, out var snapIndex);
            t = 0f;
            if(snapDuration <= 0)
            {
                scrollbar.value = snapValue;
                OnSnapped(snapIndex);
            }
            else
                DOTween.To(getT, setT, 1f, snapDuration).OnUpdate(snapUpdate).OnComplete(() => OnSnapped(snapIndex));

            void snapUpdate()
            {
                scrollbar.value = Mathf.Lerp(startValue, snapValue, t);
            }

            if (highlightFocusedItem == false) return;
            for (int i = 0; i < elementCanvasGroups.Count; i++)
            {
                if (i != snapIndex)
                {
                    elementCanvasGroups[i].interactable = false;
                    elementCanvasGroups[i].DOFade(0.1f, snapDuration);
                    elementCanvasGroups[i].transform.DOScale(unfocusScale * Vector3.one, snapDuration);
                }
                else
                {
                    elementCanvasGroups[i].interactable = true;
                    elementCanvasGroups[i].DOFade(1f, snapDuration);
                    elementCanvasGroups[i].transform.DOScale(focusScale * Vector3.one, snapDuration);
                }
            }
        }

        public void SnapAt(int snapIndex, float duration)
        {
            snapIndex = Mathf.Clamp(snapIndex, 0, elements.Count - 1);
            var snapValue = snapIndex / (snapValues.Count - 1f);

            var startValue = scrollbar.value;

            t = 0f;
            DOTween.To(getT, setT, 1f, duration).OnUpdate(snapUpdate);
            void snapUpdate()
            {
                scrollbar.value = Mathf.Lerp(startValue, snapValue, t);
            }
            if (highlightFocusedItem == false) return;
            for (int i = 0; i < elementCanvasGroups.Count; i++)
            {
                if (i != snapIndex)
                {
                    elementCanvasGroups[i].interactable = false;
                    elementCanvasGroups[i].DOFade(0.1f, duration);
                    elementCanvasGroups[i].transform.DOScale(unfocusScale * Vector3.one, duration);
                }
                else
                {
                    elementCanvasGroups[i].interactable = true;
                    elementCanvasGroups[i].DOFade(1f, duration);
                    elementCanvasGroups[i].transform.DOScale(focusScale * Vector3.one, duration);
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StopSnapDelayCoroutine();
        }
    }
}