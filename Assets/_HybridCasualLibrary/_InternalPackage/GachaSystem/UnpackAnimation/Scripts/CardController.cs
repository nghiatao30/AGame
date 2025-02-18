using System.Collections;
using System.Collections.Generic;
using GachaSystem.Core;
using HyrphusQ.GUI;
using HyrphusQ.SerializedDataStructure;
using UnityEngine;
using UnityEngine.UI;

namespace LatteGames.UnpackAnimation
{
    /// <summary>
    /// This is example only
    /// </summary>
    public class CardController :AbstractCardController
    {
        [SerializeField]
        protected Image m_BackgroundImage;
        [SerializeField]
        protected Image m_ThumbnailImage;
        [SerializeField]
        protected TextAdapter m_TitleText;
        [SerializeField]
        protected TextAdapter m_CardQuantityText;
        [SerializeField]
        protected SerializedDictionary<RarityType, Color> m_BackgroundColorDictionary;

        public override void Setup(DuplicateGachaCardsGroup cardInfo, bool isInSummary = false)
        {
            IsAnimationEnded = false;
            m_TitleText.SetText(cardInfo.representativeCard.GetDisplayName());
            if (cardInfo.representativeCard is GachaCard_Currency gachaCard_Currency)
            {
                m_CardQuantityText.SetText($"+{(cardInfo.cardsAmount * gachaCard_Currency.Amount).ToRoundedText()}");
            }
            else
            {
                m_CardQuantityText.SetText($"x{cardInfo.cardsAmount}");
            }
            m_ThumbnailImage.sprite = cardInfo.representativeCard.GetThumbnailImage();
            m_BackgroundImage.color = m_BackgroundColorDictionary.Get(cardInfo.representativeCard.GetRarityType());
            IsAnimationEnded = true;
        }

        public override void ToggleTitleVisibility(bool isVisible)
        {
            //Do Notthing
        }
    }
}