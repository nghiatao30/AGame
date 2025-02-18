using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HyrphusQ.Events
{
    public class OnTouchInteractionCallback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        private UnityEvent<PointerEventData> onBeginDragEvent;
        [SerializeField]
        private UnityEvent<PointerEventData> onEndDragEvent;
        [SerializeField]
        private UnityEvent<PointerEventData> onDragEvent;
        [SerializeField]
        private UnityEvent<PointerEventData> onPointerDownEvent;
        [SerializeField]
        private UnityEvent<PointerEventData> onPointerUpEvent;
        [SerializeField]
        private UnityEvent<PointerEventData> onPointerClickEvent;

        public event Action<PointerEventData> onBeginDrag;
        public event Action<PointerEventData> onEndDrag;
        public event Action<PointerEventData> onDrag;
        public event Action<PointerEventData> onPointerDown;
        public event Action<PointerEventData> onPointerUp;
        public event Action<PointerEventData> onPointerClick;

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDragEvent?.Invoke(eventData);
            onBeginDrag?.Invoke(eventData);
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDragEvent?.Invoke(eventData);
            onEndDrag?.Invoke(eventData);
        }
        public void OnDrag(PointerEventData eventData)
        {
            onDragEvent?.Invoke(eventData);
            onDrag?.Invoke(eventData);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDownEvent?.Invoke(eventData);
            onPointerDown?.Invoke(eventData);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUpEvent?.Invoke(eventData);
            onPointerUp?.Invoke(eventData);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClickEvent?.Invoke(eventData);
            onPointerClick?.Invoke(eventData);
        }
    }
}