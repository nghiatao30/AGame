using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace LatteGames.PvP.TrophyRoad
{
    public abstract class TrophyRoadFoundInCardsTabUI : MonoBehaviour
    {
        [SerializeField] protected Image arenaBackgroundImage;
        [SerializeField] protected TextMeshProUGUI arenaIndexText;
        [SerializeField] protected TrophyRoadFoundInCardUI foundInCardUIPrefab;
        [SerializeField] protected RectTransform cardsContainer;
        [SerializeField] protected GridLayoutGroup gridLayoutGroup;
        [SerializeField] protected float expandCellWidth;
        [Header("Progress view")]
        [SerializeField] protected GameObject progressView;
        [SerializeField] protected Image currentFill;
        [SerializeField] protected Image highestAchievedFill;
        [SerializeField] protected TextMeshProUGUI currentRequiredMedalsText;
        [SerializeField] protected TextMeshProUGUI nextRequiredMedalsText;
        [Header("Locked views")]
        [SerializeField] protected GrayscaleUI grayscaleUI;

        protected TrophyRoadSO trophyRoadSO;
        protected TrophyRoadSO.ArenaSection currentSection;
        protected TrophyRoadSO.ArenaSection nextSection;
        protected Vector2 defaultCellSize;
        protected Vector2 expandCellSize;
        protected List<TrophyRoadFoundInCardUI> cardUIs = new();

        protected virtual void Awake()
        {
            defaultCellSize = gridLayoutGroup.cellSize;
            expandCellSize = defaultCellSize;
            expandCellSize.x = expandCellWidth;
        }

        public virtual void SetLocked(bool isLocked)
        {
            grayscaleUI.SetGrayscale(isLocked);
            foreach (var cardUI in cardUIs)
            {
                cardUI.SetLocked(isLocked);
            }
        }

        public virtual void SetExpanding(bool shouldExpand)
        {
            if (shouldExpand && nextSection != null)
            {
                UpdateProgressView();
                progressView.SetActive(true);
            }
            else
            {
                progressView.SetActive(false);
            }

            gridLayoutGroup.cellSize = shouldExpand ? expandCellSize : defaultCellSize;
            foreach (var cardUI in cardUIs)
            {
                cardUI.SetExpanding(shouldExpand);
            }
        }

        protected virtual void UpdateProgressView()
        {
            var curRequiredMedals = currentSection.GetRequiredMedals();
            var nextRequiredMedals = nextSection.GetRequiredMedals();
            currentFill.fillAmount = Mathf.InverseLerp(curRequiredMedals, nextRequiredMedals, trophyRoadSO.CurrentMedals);
            highestAchievedFill.fillAmount = Mathf.InverseLerp(curRequiredMedals, nextRequiredMedals, trophyRoadSO.HighestAchievedMedals);
            currentRequiredMedalsText.text = curRequiredMedals.ToString();
            nextRequiredMedalsText.text = nextRequiredMedals.ToString();
        }

        public virtual void Setup(TrophyRoadSO trophyRoadSO, TrophyRoadSO.ArenaSection currentSection, TrophyRoadSO.ArenaSection nextSection)
        {
            this.trophyRoadSO = trophyRoadSO;
            this.currentSection = currentSection;
            this.nextSection = nextSection;

            var arenaSO = currentSection.arenaSO;
            arenaBackgroundImage.sprite = arenaSO.GetThumbnailImage();
            arenaIndexText.text = (arenaSO.index + 1).ToString();
        }

        protected virtual void GenerateFoundInCardUIs<T>(List<GachaItemManagerSO<T>> gachaItemManagerSOs) where T : GachaItemSO
        {
            foreach (var gachaItemManagerSO in gachaItemManagerSOs)
            {
                foreach (var gachaItem in gachaItemManagerSO.genericItems)
                {
                    if (gachaItem.foundInArena != currentSection.arenaSO.index + 1) continue;
                    var cardUI = Instantiate(foundInCardUIPrefab, cardsContainer);
                    cardUI.Setup(gachaItem);
                    cardUIs.Add(cardUI);
                }
            }
        }
    }

    public abstract class TrophyRoadFoundInCardsTabUI<T> : TrophyRoadFoundInCardsTabUI where T : GachaItemSO
    {
        [SerializeField] protected List<GachaItemManagerSO<T>> gachaItemManagerSOs = new();

        public override void Setup(TrophyRoadSO trophyRoadSO, TrophyRoadSO.ArenaSection currentSection, TrophyRoadSO.ArenaSection nextSection)
        {
            base.Setup(trophyRoadSO, currentSection, nextSection);
            GenerateFoundInCardUIs(gachaItemManagerSOs);
        }
    }

    public abstract class TrophyRoadFoundInCardsTabUI<T1, T2> : TrophyRoadFoundInCardsTabUI where T1 : GachaItemSO where T2 : GachaItemSO
    {
        [SerializeField] protected List<GachaItemManagerSO<T1>> type1GachaItemManagerSOs = new();
        [SerializeField] protected List<GachaItemManagerSO<T2>> type2GachaItemManagerSOs = new();

        public override void Setup(TrophyRoadSO trophyRoadSO, TrophyRoadSO.ArenaSection currentSection, TrophyRoadSO.ArenaSection nextSection)
        {
            base.Setup(trophyRoadSO, currentSection, nextSection);
            GenerateFoundInCardUIs(type1GachaItemManagerSOs);
            GenerateFoundInCardUIs(type2GachaItemManagerSOs);
        }
    }

    public abstract class TrophyRoadFoundInCardsTabUI<T1, T2, T3> : TrophyRoadFoundInCardsTabUI where T1 : GachaItemSO where T2 : GachaItemSO where T3 : GachaItemSO
    {
        [SerializeField] protected List<GachaItemManagerSO<T1>> type1GachaItemManagerSOs = new();
        [SerializeField] protected List<GachaItemManagerSO<T2>> type2GachaItemManagerSOs = new();
        [SerializeField] protected List<GachaItemManagerSO<T3>> type3GachaItemManagerSOs = new();

        public override void Setup(TrophyRoadSO trophyRoadSO, TrophyRoadSO.ArenaSection currentSection, TrophyRoadSO.ArenaSection nextSection)
        {
            base.Setup(trophyRoadSO, currentSection, nextSection);
            GenerateFoundInCardUIs(type1GachaItemManagerSOs);
            GenerateFoundInCardUIs(type2GachaItemManagerSOs);
            GenerateFoundInCardUIs(type3GachaItemManagerSOs);
        }
    }
}
