using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace LatteGames.PvP.TrophyRoad
{
    public class TrophyRoadFoundInCardUI : MonoBehaviour
    {
        [SerializeField] protected GameObject checkmark;
        [SerializeField] protected GameObject expandContent;
        [SerializeField] protected TextMeshProUGUI nameText;
        [SerializeField] protected Image itemImage;
        [SerializeField] protected GameObject lockedView;

        protected virtual void Awake()
        {
            SetExpanding(false);
        }

        public virtual void SetLocked(bool isLocked)
        {
            lockedView.SetActive(isLocked);
        }

        public virtual void SetExpanding(bool shouldExpand)
        {
            expandContent.SetActive(shouldExpand);
        }

        public virtual void Setup(GachaItemSO gachaItemSO)
        {
            checkmark.SetActive(gachaItemSO.IsUnlocked());
            nameText.text = gachaItemSO.GetDisplayName();
            itemImage.sprite = gachaItemSO.GetThumbnailImage();
        }
    }
}
