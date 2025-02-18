using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LatteGames.UI
{
    [RequireComponent(typeof(Canvas), typeof(Image))]
    public class ForceClickUI : MonoBehaviour
    {
        [SerializeField] RectTransform forceClickHand;
        private Canvas canvas;
        private List<Data> currentForce;
        private Image image;
        private Animator player;
        public bool IsActive => image.enabled;

        bool wasPaused;

        [System.Serializable]
        public struct Data
        {
            public GameObject gameObject;
            public bool clickable;
            public bool defaultHasCanvas;
            public bool defaultHasGraphicRayCaster;
            public int defaultSortOrderValue;
            public bool defaultOverrideSorting;
            public AdditionalCanvasShaderChannels defaultAdditionalCanvasShaderChannels;
            public Data(GameObject gameObject) : this() => this.gameObject = gameObject;
            public Data(GameObject gameObject, bool clickable) : this(gameObject) => this.clickable = clickable;
        }

        public class ForceClickHandData
        {
            public Vector3 position;
            public bool isFlip;
        }

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 998;
            image = GetComponent<Image>();
        }

        /* Usage for other script
        ForceClickUI forceClickUI = FindObjectOfType<ForceClickUI>();

        var forceClickData = new List<ForceClickUI.Data>();
        forceClickData.Add(new ForceClickUI.Data(gameObject, true));

        var forceClickHand = new ForceClickUI.ForceClickHandData();
        forceClickHand.position = rectTransform1.anchoredPosition;
        forceClickHand.isFlip = false;

        forceClickUI?.ForceClick(forceClickData, forceClickHand);
         */

        public void ForceClick(List<Data> data, ForceClickHandData forceClickHandData = null, bool isPauseGame = true)
        {
            if (IsActive) return;
            image.enabled = true;
            if (player != null) player.enabled = false;
            for (int i = 0; i < data.Count; i++)
            {
                Data item = data[i];
                if (item.gameObject.TryGetComponent(out Canvas itemCanvas))
                {
                    item.defaultHasCanvas = true;
                    item.defaultOverrideSorting = itemCanvas.overrideSorting;
                    item.defaultSortOrderValue = itemCanvas.sortingOrder;
                    item.defaultAdditionalCanvasShaderChannels = itemCanvas.additionalShaderChannels;
                }
                else
                {
                    item.defaultHasCanvas = false;
                    itemCanvas = item.gameObject.AddComponent<Canvas>();
                }
                itemCanvas.additionalShaderChannels = AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent | AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2 | AdditionalCanvasShaderChannels.TexCoord3;
                itemCanvas.overrideSorting = true;
                itemCanvas.sortingOrder = canvas.sortingOrder + i + 1;
                if (item.clickable)
                {
                    if (item.gameObject.TryGetComponent(out GraphicRaycaster _))
                    {
                        item.defaultHasGraphicRayCaster = true;
                    }
                    else
                    {
                        item.defaultHasGraphicRayCaster = false;
                        item.gameObject.AddComponent<GraphicRaycaster>();
                    }
                }

                if (!item.gameObject.activeInHierarchy)
                {
                    CanvasSortOrderUpdater updater = item.gameObject.AddComponent<CanvasSortOrderUpdater>();
                    updater.SortOrder = canvas.sortingOrder + i + 1;
                }
            }
            if (forceClickHandData != null)
            {
                forceClickHand.anchoredPosition = forceClickHandData.position;
                if (forceClickHandData.isFlip)
                    forceClickHand.rotation = Quaternion.Euler(Vector3.up * 180f);
            }
            if (isPauseGame)
            {
                wasPaused = true;
            }
            currentForce = data;
        }

        /* Usage for other script
         ForceClickUI forceClickUI = FindObjectOfType<ForceClickUI>();
         forceClickUI.RemoveEnforcement();
         */

        public void DisableForceClick()
        {
            if (currentForce == null) return;
            image.enabled = false;
            if (player != null) player.enabled = true;
            for (int i = 0; i < currentForce.Count; i++)
            {
                Data item = currentForce[i];
                if (item.gameObject.GetComponent<CanvasSortOrderUpdater>()) Destroy(item.gameObject.GetComponent<CanvasSortOrderUpdater>());
                if (item.clickable)
                {
                    if (!item.defaultHasGraphicRayCaster)
                        Destroy(item.gameObject.GetComponent<GraphicRaycaster>());
                }
                if (!item.defaultHasCanvas)
                {
                    Destroy(item.gameObject.GetComponent<Canvas>());
                }
                else
                {
                    var canvas = item.gameObject.GetComponent<Canvas>();
                    canvas.overrideSorting = item.defaultOverrideSorting;
                    canvas.sortingOrder = item.defaultSortOrderValue;
                    canvas.additionalShaderChannels = item.defaultAdditionalCanvasShaderChannels;
                }
            }
            if (wasPaused)
            {
                wasPaused = false;
            }
            forceClickHand.gameObject.SetActive(false);
            currentForce = null;
        }
    }
}