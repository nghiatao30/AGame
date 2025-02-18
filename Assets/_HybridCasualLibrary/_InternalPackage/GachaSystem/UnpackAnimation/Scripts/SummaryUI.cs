using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GachaSystem.Core;

namespace LatteGames.UnpackAnimation
{
    public class SummaryUI : ComposeCanvasElementVisibilityController
    {
        [SerializeField] protected ScaleGridLayoutGroup scaleGridLayoutGroup;
        [SerializeField] protected AnimationCurve cellScaleAnimationCurve;
        [SerializeField] protected List<AbstractCardController> cardControllers;

        protected float originalScale;
        protected Vector2 originalSpacing;
        public RectTransform rect => GetComponent<RectTransform>();

        protected virtual void Awake()
        {
            originalScale = scaleGridLayoutGroup.cellScale;
            originalSpacing = scaleGridLayoutGroup.spacing;
        }

        public virtual void SetupCards(List<DuplicateGachaCardsGroup> cards, int fixedColumn)
        {
            foreach (var card in cardControllers)
            {
                card.gameObject.SetActive(false);
            }
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cardControllers[i];
                card.gameObject.SetActive(true);
                card.Setup(cards[i], true);
            }

            scaleGridLayoutGroup.fixedColumn = fixedColumn;
            scaleGridLayoutGroup.cellScale = originalScale * cellScaleAnimationCurve.Evaluate(scaleGridLayoutGroup.fixedColumn);
            scaleGridLayoutGroup.spacing = originalSpacing * cellScaleAnimationCurve.Evaluate(scaleGridLayoutGroup.fixedColumn);
            scaleGridLayoutGroup.UpdateView();
        }
    }
}