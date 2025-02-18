using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LatteGames
{
    public class RecycleListView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float topRecycleDelayOffset = 0;
        [SerializeField] private float botRecycleDelayOffset = 0;
        private bool autoScroll = false;
        private ViewItem targetScroll;

        private bool isDragging = true;
        private float endDragVelocity;
        public bool emitDragEvents;
        private float _position = 0;
        public float Position { get => _position; set => _position = value; }

        public IBeginDragHandler beginDragHandler;
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (this.enabled == false)
                return;
            isDragging = Mathf.Abs(Vector2.Dot(eventData.delta.normalized, Vector2.up)) > 0.5f && autoScroll == false;
            if (emitDragEvents && beginDragHandler != null && isDragging == false)
                beginDragHandler.OnBeginDrag(eventData);
            if (isDragging == false)
                return;
            endDragVelocity = 0;

        }
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
        private RectTransform _selfRectTf;
        private RectTransform SelfRectTf
        {
            get
            {
                if (_selfRectTf == null)
                    _selfRectTf = transform as RectTransform;
                return _selfRectTf;
            }
        }

        private float GetViewportLength()
        {
            return SelfRectTf.rect.height;
        }
        public IDragHandler dragHandler;
        public void OnDrag(PointerEventData eventData)
        {
            if (emitDragEvents && dragHandler != null && isDragging == false)
                dragHandler.OnDrag(eventData);
            if (isDragging == false)
                return;
            _position += GetDragDelta(eventData.delta);
            endDragVelocity = GetDragDelta(eventData.delta);
        }

        public IEndDragHandler endDragHandler;

        public void OnEndDrag(PointerEventData eventData)
        {
            if (emitDragEvents && endDragHandler != null && isDragging == false)
                endDragHandler.OnEndDrag(eventData);
            if (isDragging == false)
                return;
            isDragging = false;
        }
        private float GetDragDelta(Vector2 delta)
        {
            return delta.y / Canvas.scaleFactor;
        }

        public void ScrollTo(object data)
        {
            int index = this.items.FindIndex(item => item.data == data);
            if (index == -1)
                return;
            this.autoScroll = true;
            this.targetScroll = this.items[index];
        }

        public List<GameObject> GetAllView()
        {
            var views = new List<GameObject>();
            foreach (var item in items)
            {
                if (item.rectTf != null && !views.Contains(item.rectTf.gameObject))
                    views.Add(item.rectTf.gameObject);
            }
            return views;
        }

        public void SetPositionTO(object data)
        {
            int index = this.items.FindIndex(item => item.data == data);
            if (index == -1)
                return;
            this.LastFirstItemInVP = index;
            float p = 0;
            for (int i = 0; i < index; i++)
            {
                p += this.items[i].Height;
            }
            this._position = p;
        }

        //Position transform
        private int LastFirstItemInVP = 0;
        private void Update()
        {
            if (this.items == null || this.items.Count == 0)
                return;
            if (isDragging == false && autoScroll == false)
            {
                float vMag = Mathf.Abs(endDragVelocity);
                float vSign = Mathf.Sign(endDragVelocity);
                vMag -= Time.deltaTime * 60.0f;
                if (vMag > 0)
                {
                    endDragVelocity = vMag * vSign;
                    _position += endDragVelocity;
                }
                else
                {
                    endDragVelocity = 0;
                }
            }

            //* Lock top
            if (this._position < 0)
            {
                endDragVelocity = 0;
                this._position = 0;
            }

            float localPos = 0;
            float viewPortL = GetViewportLength();
            ViewItem lastItemInViewPort = null;
            for (int i = 0; i < this.items.Count; i++)
            {
                var item = this.items[i];
                item.UpdateHeight();

                if (IsInViewPort(localPos, item.Height, viewPortL))
                {
                    //* item is in viewport*/
                    if (item.rectTf == null)
                    {
                        item.rectTf = GetHolder(item.data).transform as RectTransform;
                        this.recursiveListViewHandler.UpdateView(item.data, item.rectTf.gameObject);
                        float h1 = item.Height;
                        LayoutRebuilder.ForceRebuildLayoutImmediate(item.rectTf);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(item.rectTf);
                        item.UpdateHeight();
                        float h2 = item.Height;
                        if (i < LastFirstItemInVP)
                            _position += h2 - h1;
                        if (i > 0 && i <= this.items.Count - 1)
                        {
                            var prevItem = this.items[i - 1];
                            if (IsInViewPort(prevItem.LocalPos, prevItem.Height, viewPortL) == false)
                            {
                                //* By update layout previous item has been pushed out of view */
                                //* Recycle */
                                CacheHolder(prevItem);
                            }
                        }
                    }
                    lastItemInViewPort = item;
                }
                item.UpdateLocalPos(localPos);
                if (IsInViewPort(item.LocalPos, item.Height, viewPortL) == false)
                {
                    //* item is out side */
                    //* recycle */
                    CacheHolder(item);
                }
                localPos += item.Height;
            }

            //* Lock bot */
            //* if last item is in view port and its bottom also in view port => reach the end */
            if (lastItemInViewPort == this.items[this.items.Count - 1])
            {
                float lastItemBottom = Position - localPos;
                if (lastItemBottom > -viewPortL)
                {
                    _position = Mathf.Max(0, -viewPortL + localPos);
                    endDragVelocity = 0;
                    autoScroll = false;
                }
                //* reach the end */
            }

            foreach (var item in this.items)
            {
                item.UpdatePosition(Position);
            }

            if (autoScroll)
            {
                if (IsInViewPort(targetScroll.LocalPos, targetScroll.Height, viewPortL) == false)
                {
                    this._position += Mathf.Sign(targetScroll.LocalPos - this._position) * viewPortL * 5 * Time.deltaTime;
                }
                else
                {
                    autoScroll = false;
                }
            }

            for (int i = 0; i < this.items.Count; i++)
            {
                if (IsInViewPort(this.items[i].LocalPos, this.items[i].Height, viewPortL))
                {
                    LastFirstItemInVP = i;
                    break;
                }
            }
        }

        private bool IsInViewPort(float lPos, float h, float viewPortL)
        {
            float beginPos = Position - lPos;
            float endPos = beginPos - h;

            float topPos = this.topRecycleDelayOffset;
            float endVPPos = -viewPortL - this.botRecycleDelayOffset;
            return (endVPPos <= beginPos && beginPos <= topPos) ||
                (endVPPos <= endPos && endPos <= topPos) ||
                (endPos <= topPos && topPos <= beginPos) ||
                (endPos <= endVPPos && endVPPos <= beginPos);
        }

        private List<ViewItem> items = new List<ViewItem>();
        public int ItemCount { get => items.Count; }
        public void AppendData(List<object> data)
        {
            foreach (var info in data)
            {
                items.Add(new ViewItem(info));
            }
        }
        public void InsertAt(int index, object data)
        {
            items.Insert(index, new ViewItem(data));
        }

        public void ClearData()
        {
            foreach (var item in this.items)
            {
                GameObject prefab = this.recursiveListViewHandler.GetPrefab(item.data);
                if (cache.ContainsKey(prefab) == false)
                    cache.Add(prefab, new List<GameObject>());

                GameObject recycleHolder = item.GetObjectHolderForRecycle();
                if (recycleHolder != null)
                {
                    recycleHolder.SetActive(false);
                    cache[prefab].Add(recycleHolder);
                }
            }
            this.items.Clear();
            _position = 0;
        }

        private Dictionary<GameObject, List<GameObject>> cache = new Dictionary<GameObject, List<GameObject>>();
        private GameObject GetHolder(object data)
        {
            GameObject prefab = this.recursiveListViewHandler.GetPrefab(data);
            if (cache.ContainsKey(prefab) && cache[prefab].Count > 0)
            {
                GameObject holder = cache[prefab][cache[prefab].Count - 1];
                cache[prefab].Remove(holder);
                holder.SetActive(true);
                return holder;
            }

            return Instantiate(prefab, transform);
        }

        private void CacheHolder(ViewItem item)
        {
            GameObject prefab = this.recursiveListViewHandler.GetPrefab(item.data);
            if (cache.ContainsKey(prefab) == false)
                cache.Add(prefab, new List<GameObject>());

            GameObject recycleHolder = item.GetObjectHolderForRecycle();
            if (recycleHolder != null)
            {
                //Debug.Log($"Recycle {item.data}/{item.Height}");
                recycleHolder.SetActive(false);
                cache[prefab].Add(recycleHolder);
            }
        }



        private IHandler recursiveListViewHandler;
        public void Init(IHandler recursiveListViewHandler)
        {
            this.recursiveListViewHandler = recursiveListViewHandler;
        }


        private class ViewItem
        {
            public object data;
            private float height;
            public float Height { get => height + 1; }
            public RectTransform rectTf;

            public ViewItem(object data)
            {
                this.data = data;
            }

            public void UpdateHeight()
            {
                if (this.rectTf == null)
                    return;
                this.height = this.rectTf.rect.height;
            }

            public GameObject GetObjectHolderForRecycle()
            {
                if (this.rectTf == null)
                    return null;
                RectTransform rTf = this.rectTf;
                this.rectTf = null;
                return rTf.gameObject;
            }

            public void UpdatePosition(float viewPos)
            {
                if (this.rectTf == null)
                    return;
                this.rectTf.anchoredPosition = new Vector2(this.rectTf.anchoredPosition.x, viewPos - localPos);
            }

            private float localPos = 0;
            public float LocalPos { get => this.localPos; }
            public void UpdateLocalPos(float localPos)
            {
                this.localPos = localPos;
            }
        }

        public interface IHandler
        {
            GameObject GetPrefab(object data);
            void UpdateView(object data, GameObject holder);
        }
    }
}