using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GachaSystem.Core
{
    public static class GachaSystemExtension
    {
        public static List<DuplicateGachaCardsGroup> GroupDuplicate(this List<GachaCard> cards)
        {
            Dictionary<GachaCard, DuplicateGachaCardsGroup> table = new();
            foreach (var card in cards)
            {
                if (!table.ContainsKey(card))
                {
                    var newGroup = new DuplicateGachaCardsGroup()
                    {
                        cardsAmount = 1,
                        representativeCard = card
                    };
                    table.Add(card, newGroup);
                }
                else
                {
                    table[card].cardsAmount++;
                }
            }
            return new List<DuplicateGachaCardsGroup>(table.Values);
        }
    }
}
