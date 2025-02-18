using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LatteGames.PvP.TrophyRoad
{
    public class TrophyRoadArenaBannerUI : MonoBehaviour
    {
        [SerializeField] protected Image arenaBackgroundImage;
        [SerializeField] protected Image infoBackgroundImage;
        [SerializeField] protected TextMeshProUGUI arenaIndexText;
        [SerializeField] protected TextMeshProUGUI requiredAmountText;
        [SerializeField] protected GrayscaleUI grayscaleUI;

        public virtual void SetLocked(bool isLocked)
        {
            grayscaleUI.SetGrayscale(isLocked);
        }

        public virtual void Setup(float height, PvPArenaSO arenaSO)
        {
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
            arenaBackgroundImage.sprite = arenaSO.GetThumbnailImage();
            if (arenaSO.TryGetModule<MonoImageItemModule>(out var imageItemModule))
            {
                infoBackgroundImage.color = imageItemModule.thumbnailTintColor;
            }
            arenaIndexText.text = (arenaSO.index + 1).ToString();
            var requirements = arenaSO.GetUnlockRequirements();
            if (requirements == null)
            {
                requiredAmountText.transform.parent.gameObject.SetActive(false);
                return;
            }
            foreach (var req in requirements)
            {
                if (req is Requirement_Currency requirement_Currency && requirement_Currency.currencyType == CurrencyType.Medal)
                {
                    requiredAmountText.text = requirement_Currency.requiredAmountOfCurrency.ToString();
                }
            }
        }
    }
}
