using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomCardUI : MonoBehaviour
{
    [SerializeField] protected List<RarityBackgroundMapping> rarityBackgroundMappings = new();
    [SerializeField] protected TextMeshProUGUI amountText;
    [SerializeField] protected Image currentBackgroundImage;

    public virtual void Setup(RarityType rarity, int amount)
    {
        foreach (var mapping in rarityBackgroundMappings)
        {
            if (mapping.rarity != rarity) continue;
            currentBackgroundImage.enabled = false;
            currentBackgroundImage = mapping.backgroundImage;
            currentBackgroundImage.enabled = true;
        }
        amountText.text = $"x{amount}";
    }

    [System.Serializable]
    protected class RarityBackgroundMapping
    {
        public RarityType rarity;
        public Image backgroundImage;
    }
}
