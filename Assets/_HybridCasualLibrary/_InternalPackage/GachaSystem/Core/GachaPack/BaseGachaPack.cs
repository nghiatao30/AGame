using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using GachaSystem.Core;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "BaseGachaPack", menuName = "LatteGames/Gacha/BaseGachaPack")]
public class BaseGachaPack : GachaPack
{
    //Arena
    [BoxGroup("Pack Info"), SerializeField]
    protected int foundInArena = 1;
    [BoxGroup("Pack Info"), SerializeField]
    protected float DownRatio = 0.2f;

    //Booster Cards
    [FoldoutGroup("Booster Card"), SerializeField, Range(0f, 1f)]
    protected float noBoosterProb;
    [FoldoutGroup("Booster Card"), SerializeField]
    protected int minBoosterCardsCount, maxBoosterCardsCount;
    [FoldoutGroup("Booster Card"), SerializeField]
    protected List<BoosterCardRngInfo> boosterCardRngInfos = new();

    //Money Cards
    [TabGroup("Money Card"), SerializeField]
    protected List<CurrencyRngInfo> moneyCardRngInfos;

    //Gem Cards
    [TabGroup("Gem Card"), SerializeField]
    protected List<CurrencyRngInfo> gemCardRngInfos;

    //Gear Cards
    [TabGroup("NewGroup", "Gear Card"), SerializeField, Range(0f, 1f)]
    protected float gearCardProbability;
    [TabGroup("NewGroup", "Gear Card"), SerializeField]
    protected List<ItemCardRngInfo> gearCardRngInfos = new();
    [TabGroup("NewGroup", "Gear Card"), SerializeField]
    protected List<GearDropRateTable> gearTypeDropRateTables = new();

    //Character Cards
    [TabGroup("NewGroup", "Character Card"), SerializeField, Range(0f, 1f)]
    protected float characterCardProbability;
    [TabGroup("NewGroup", "Character Card"), SerializeField]
    protected List<ItemCardRngInfo> characterCardRngInfos = new();
    [TabGroup("NewGroup", "Character Card"), SerializeField]
    protected List<CharacterDropRateTable> characterDropRateTables = new();

    protected List<GearDropRateTable> adjustedGearTypeDropRateTables = new();
    protected List<CharacterDropRateTable> adjustedCharacterDropRateTables = new();

    protected class ItemCardCount
    {
        public int commonCount = 0;
        public int epicCount = 0;
        public int legendaryCount = 0;

        public void AddOne(RarityType rarityType)
        {
            switch (rarityType)
            {
                case RarityType.Common:
                    commonCount++;
                    break;
                case RarityType.Epic:
                    epicCount++;
                    break;
                case RarityType.Legendary:
                    legendaryCount++;
                    break;
            }
        }
    }

    protected enum ItemType { Gear, Character }

    #region Methods
    public override List<GachaCard> GenerateCards()
    {
        var result = new List<GachaCard>();
        var itemCardAmountByRarity = new Dictionary<ItemType, ItemCardCount>
        {
            { ItemType.Character, new ItemCardCount() },
            { ItemType.Gear, new ItemCardCount() }
        };
        int remainAmount = TotalCardsCount;

        // Money cards
        RandomMoneyCards();

        // Gem cards
        RandomGemCards();

        // Booster cards
        RandomBoosterCards();

        // Get gear/character cards count
        CalculateItemCardAmount();

        // Gear cards
        RandomGearCards();

        // Character cards
        RandomCharacterCards();

        return result;

        void RandomMoneyCards()
        {
            if (remainAmount <= 0) return;
            var moneyCardRngInfo = moneyCardRngInfos.GetRandomRedistribute();
            if (moneyCardRngInfo != null)
            {
                result.Add(cardTemplates.Generate<GachaCard_Currency>(moneyCardRngInfo.amount, CurrencyType.Standard, ResourceLocationProvider));
                remainAmount--;
            }
        }

        void RandomGemCards()
        {
            if (remainAmount <= 0) return;
            var gemCardRngInfo = gemCardRngInfos.GetRandomRedistribute();
            if (gemCardRngInfo != null)
            {
                result.Add(cardTemplates.Generate<GachaCard_Currency>(gemCardRngInfo.amount, CurrencyType.Premium, ResourceLocationProvider));
                remainAmount--;
            }
        }

        void RandomBoosterCards()
        {
            if (remainAmount <= 0) return;
            var willGenerateBoosterCards = Random.value > noBoosterProb;
            if (willGenerateBoosterCards)
            {
                var boosterCardCount = Random.Range(minBoosterCardsCount, maxBoosterCardsCount + 1);
                for (int i = 0; i < boosterCardCount; i++)
                {
                    var boosterCardRngInfo = boosterCardRngInfos.GetRandomRedistribute();
                    if (boosterCardRngInfo != null)
                    {
                        result.Add(cardTemplates.Generate<GachaCard_Booster>(boosterCardRngInfo.boosterType));
                        remainAmount--;
                    }
                }
            }
        }

        void CalculateItemCardAmount()
        {
            var cardRemain = remainAmount;
            var guaranteeEpicCount = GetGuaranteedCardsCount(RarityType.Epic);
            var guaranteeLegendaryCount = GetGuaranteedCardsCount(RarityType.Legendary);
            List<GachaItemSO> availableCharacterSOList = new();
            List<GachaItemSO> availableGearList = new();
            List<ItemTypeRandom> typeRngList = new()
            {
                new ItemTypeRandom() // Gear
                {
                    ItemType = ItemType.Gear,
                    Probability = gearCardProbability,
                    ItemCardRngInfos = gearCardRngInfos
                },
                new ItemTypeRandom() // Character
                {
                    ItemType = ItemType.Character,
                    Probability = characterCardProbability,
                    ItemCardRngInfos = characterCardRngInfos
                },
            };
            availableCharacterSOList = GetAllAvailableCharacterSO();

            availableGearList = GetAllAvailableGearSO();

            while (cardRemain > 0)
            {
                var rngInfos = typeRngList.GetRandomRedistribute();
                if (rngInfos != null)
                {
                    RarityType rarityType = RarityType.Common;
                    if (guaranteeLegendaryCount > 0)
                    {
                        rarityType = RarityType.Legendary;
                        guaranteeLegendaryCount--;
                    }
                    else if (guaranteeEpicCount > 0)
                    {
                        rarityType = RarityType.Epic;
                        guaranteeEpicCount--;
                    }
                    else
                    {
                        rarityType = rngInfos.ItemCardRngInfos.GetRandomRedistribute().rarity;
                    }
                    switch (rngInfos.ItemType)
                    {
                        case ItemType.Character:
                            AddCharacterCard(rarityType);
                            break;
                        case ItemType.Gear:
                            AddGearCard(rarityType);
                            break;
                    }
                    cardRemain--;

                    void AddCharacterCard(RarityType rarity)
                    {
                        while (true)
                        {
                            if (IsRarityAvailableInCharacter(rarity))
                            {
                                itemCardAmountByRarity[ItemType.Character].AddOne(rarity);
                                return;
                            }
                            else if (IsRarityAvailableInGearType(rarity))
                            {
                                itemCardAmountByRarity[ItemType.Gear].AddOne(rarity);
                                return;
                            }
                            else
                            {
                                if (rarity == RarityType.Legendary) rarity = RarityType.Epic;
                                else if (rarity == RarityType.Epic) rarity = RarityType.Common;
                                else return;
                            }
                        }
                    }

                    void AddGearCard(RarityType rarity)
                    {
                        while (true)
                        {
                            if (IsRarityAvailableInGearType(rarity))
                            {
                                itemCardAmountByRarity[ItemType.Gear].AddOne(rarity);
                                return;
                            }
                            else if (IsRarityAvailableInCharacter(rarity))
                            {
                                itemCardAmountByRarity[ItemType.Character].AddOne(rarity);
                                return;
                            }
                            else
                            {
                                if (rarity == RarityType.Legendary) rarity = RarityType.Epic;
                                else if (rarity == RarityType.Epic) rarity = RarityType.Common;
                                else return;
                            }
                        }
                    }

                    bool IsRarityAvailableInCharacter(RarityType rarityType)
                    {
                        return availableCharacterSOList.Any(character => character.GetRarityType() == rarityType);
                    }

                    bool IsRarityAvailableInGearType(RarityType rarityType)
                    {
                        return availableGearList.Any(gear => gear.GetRarityType() == rarityType);
                    }
                }
                else
                {
                    Debug.LogError("RngInfos is null");
                }
            }
        }

        void RandomGearCards()
        {
            int commonCount = itemCardAmountByRarity[ItemType.Gear].commonCount;
            int epicCount = itemCardAmountByRarity[ItemType.Gear].epicCount;
            int legendaryCount = itemCardAmountByRarity[ItemType.Gear].legendaryCount;

            // Determine which gear for each gear card
            AdjustGearDropRateTables();
            result.AddRange(GenerateGearCardByRarity(RarityType.Common, commonCount));
            result.AddRange(GenerateGearCardByRarity(RarityType.Epic, epicCount));
            result.AddRange(GenerateGearCardByRarity(RarityType.Legendary, legendaryCount));
        }

        void RandomCharacterCards()
        {
            int commonCount = itemCardAmountByRarity[ItemType.Character].commonCount;
            int epicCount = itemCardAmountByRarity[ItemType.Character].epicCount;
            int legendaryCount = itemCardAmountByRarity[ItemType.Character].legendaryCount;

            // Determine which character for each character card
            AdjustCharacterDropRateTables();
            result.AddRange(GenerateCharacterCardByRarity(RarityType.Common, commonCount));
            result.AddRange(GenerateCharacterCardByRarity(RarityType.Epic, epicCount));
            result.AddRange(GenerateCharacterCardByRarity(RarityType.Legendary, legendaryCount));
        }
    }

    /// <summary>
    /// Generate character cards in the pack only
    /// </summary>
    /// <param name="cardCount">Number of cards to generate</param>
    /// <returns></returns>
    public virtual List<GachaCard_GachaItem> GenerateCharacterCards(int cardCount)
    {
        var result = new List<GachaCard_GachaItem>();
        int remainAmount = cardCount;
        var itemCardCount = new ItemCardCount();

        var availableCharacterList = GetAllAvailableCharacterSO();

        while (remainAmount > 0)
        {
            var randomRarity = characterCardRngInfos.GetRandomRedistribute().rarity;
            randomRarity = ValidateRarity(randomRarity);

            itemCardCount.AddOne(randomRarity);
            remainAmount--;
        }

        RandomCharacterCards();

        return result;

        void RandomCharacterCards()
        {
            int commonCount = itemCardCount.commonCount;
            int epicCount = itemCardCount.epicCount;
            int legendaryCount = itemCardCount.legendaryCount;

            // Determine which character for each character card
            AdjustCharacterDropRateTables();
            result.AddRange(GenerateCharacterCardByRarity(RarityType.Common, commonCount));
            result.AddRange(GenerateCharacterCardByRarity(RarityType.Epic, epicCount));
            result.AddRange(GenerateCharacterCardByRarity(RarityType.Legendary, legendaryCount));
        }

        RarityType ValidateRarity(RarityType rarity)
        {
            if (!availableCharacterList.Any(character => character.GetRarityType().Equals(rarity)))
            {
                switch (rarity)
                {
                    case RarityType.Legendary:
                        rarity = ValidateRarity(RarityType.Epic);
                        break;
                    case RarityType.Epic:
                        rarity = ValidateRarity(RarityType.Common);
                        break;
                    case RarityType.Common:
                        throw new Exception("There is no characters in list");
                    default:
                        throw new Exception($"This rarity: {rarity} is not contain in the game");
                }
            }
            return rarity;
        }
    }

    public virtual void AdjustCharacterDropRateTables()
    {
        adjustedCharacterDropRateTables.Clear();
        foreach (var table in characterDropRateTables)
        {
            adjustedCharacterDropRateTables.Add(GetAdjustedCharacterDropRates(table));
        }
    }

    /// <summary>
    /// Generate gear cards in the pack only
    /// </summary>
    /// <param name="cardCount">Number of cards to generate</param>
    /// <returns></returns>
    public virtual List<GachaCard_GachaItem> GenerateGearCards(int cardCount)
    {
        var result = new List<GachaCard_GachaItem>();
        int remainAmount = cardCount;
        var itemCardCount = new ItemCardCount();

        while (remainAmount > 0)
        {
            var randomRarity = gearCardRngInfos.GetRandomRedistribute().rarity;

            itemCardCount.AddOne(randomRarity);

            remainAmount--;
        }

        RandomGearCards();

        return result;

        void RandomGearCards()
        {
            int commonCount = itemCardCount.commonCount;
            int epicCount = itemCardCount.epicCount;
            int legendaryCount = itemCardCount.legendaryCount;

            // Determine which character for each character card
            AdjustGearDropRateTables();
            result.AddRange(GenerateGearCardByRarity(RarityType.Common, commonCount));
            result.AddRange(GenerateGearCardByRarity(RarityType.Epic, epicCount));
            result.AddRange(GenerateGearCardByRarity(RarityType.Legendary, legendaryCount));
        }
    }

    #region Character Card Generating Process
    protected virtual List<GachaCard_GachaItem> GenerateCharacterCardByRarity(RarityType rarity, int cardCount)
    {
        var result = new List<GachaCard_GachaItem>();
        if (cardCount <= 0) return result;
        // Split into groups
        int groupCount = cardCount > GachSystemConfigs.MAX_CARD_TO_SLIPT_GROUP ? Random.Range(GachSystemConfigs.MIN_GROUP_TO_RANDOM, GachSystemConfigs.MAX_GROUP_TO_RANDOM) : 1;
        int[] groups = new int[groupCount];
        while (groupCount > 0)
        {
            var offset = groupCount > 1 ? Random.Range(0.5f, 1.5f) : 1f;
            var curGroup = Mathf.FloorToInt((float)cardCount / groupCount * offset);
            groups[groupCount - 1] = curGroup;
            cardCount -= curGroup;
            groupCount--;
        }
        // Determine which character for each group, which means the group will only have cards of that character
        foreach (var group in groups)
        {
            var droppedCharacter = GetCharacterDropRateTableByRarity(rarity).dropRates.GetRandomRedistribute();
            var charCard = cardTemplates.Generate<GachaCard_GachaItem>(droppedCharacter.CharacterSO);
            for (int i = 0; i < group; i++)
            {
                result.Add(charCard);
            }
        }
        return result;
    }

    protected virtual CharacterDropRateTable GetCharacterDropRateTableByRarity(RarityType rarity)
    {
        return adjustedCharacterDropRateTables.Find(table => table.rarity == rarity);
    }

    protected virtual CharacterDropRateTable GetAdjustedCharacterDropRates(CharacterDropRateTable src)
    {
        var result = new CharacterDropRateTable();
        var maxUpgraded = new List<CharacterDropRate>();
        var notMaxUpgraded = new List<CharacterDropRate>();

        result.rarity = src.rarity;
        // Setup result & classify maxUpgraded/notMaxUpgraded
        foreach (var dropRate in src.dropRates)
        {
            var clonedDropRate = new CharacterDropRate()
            {
                CharacterSO = dropRate.CharacterSO,
                probability = dropRate.Probability
            };

            if (clonedDropRate.CharacterSO.foundInArena > foundInArena) continue;
            result.dropRates.Add(clonedDropRate);
            if (clonedDropRate.CharacterSO.IsMaxUpgradeLevel()) maxUpgraded.Add(clonedDropRate);
            else notMaxUpgraded.Add(clonedDropRate);
        }

        // Check maxUpgraded count
        if (maxUpgraded.Count <= 0 || maxUpgraded.Count >= result.dropRates.Count) goto Return;

        // Update drop rates within maxUpgraded
        float oddRate = 0f;
        foreach (var dropRate in maxUpgraded)
        {
            oddRate += dropRate.Probability * (1f - DownRatio);
            dropRate.probability *= DownRatio;
        }

        // Update drop rates within notMaxUpgraded
        float remainRate = 0f;
        foreach (var dropRate in notMaxUpgraded)
        {
            remainRate += dropRate.Probability;
        }
        foreach (var dropRate in notMaxUpgraded)
        {
            dropRate.probability *= 1f + oddRate / remainRate;
        }

    Return:
        return result;
    }
    #endregion

    #region Gear Card Generating Process
    protected virtual List<GachaCard_GachaItem> GenerateGearCardByRarity(RarityType rarity, int cardCount)
    {
        var result = new List<GachaCard_GachaItem>();
        if (cardCount <= 0) return result;
        // Split into groups
        int groupCount = cardCount > GachSystemConfigs.MAX_CARD_TO_SLIPT_GROUP ? Random.Range(GachSystemConfigs.MIN_GROUP_TO_RANDOM, GachSystemConfigs.MAX_GROUP_TO_RANDOM) : 1;
        int[] groups = new int[groupCount];
        while (groupCount > 0)
        {
            var offset = groupCount > 1 ? Random.Range(0.5f, 1.5f) : 1f;
            var curGroup = Mathf.FloorToInt((float)cardCount / groupCount * offset);
            groups[groupCount - 1] = curGroup;
            cardCount -= curGroup;
            groupCount--;
        }
        // Determine which gear for each group, which means the group will only have cards of that gear
        foreach (var group in groups)
        {
            var droppedGear = GetGearDropRateTableByRarity(rarity).GetRandomRedistribute();
            var gearCard = cardTemplates.Generate<GachaCard_GachaItem>(droppedGear.ItemSO);
            for (int i = 0; i < group; i++)
            {
                result.Add(gearCard);
            }
        }
        return result;
    }

    public virtual void AdjustGearDropRateTables()
    {
        adjustedGearTypeDropRateTables.Clear();
        foreach (var table in gearTypeDropRateTables)
        {
            adjustedGearTypeDropRateTables.Add(GetAdjustedGearTypeDropRates(table));
        }
    }

    protected virtual List<GearTypeDropRate.GearDropRate> GetGearDropRateTableByRarity(RarityType rarity)
    {
        return adjustedGearTypeDropRateTables.
            SelectMany(table => table.dropRates).
            SelectMany(table => table.gearDropRates).
            Where(table => table.ItemSO.GetRarityType() == rarity).
            ToList();
    }

    protected virtual List<GachaItemSO> GetAllAvailableCharacterSO()
    {
        List<GachaItemSO> result = new();
        foreach (var table in characterDropRateTables)
        {
            foreach (var drop in table.dropRates)
            {
                if (IsCharacterAvailable(drop.CharacterSO))
                {
                    result.Add(drop.CharacterSO);
                }
            }
        }
        return result;

        bool IsCharacterAvailable(GachaItemSO characterSO)
        {
            if (characterSO.foundInArena > foundInArena) return false;
            return true;
        }
    }

    protected virtual List<GachaItemSO> GetAllAvailableGearSO()
    {
        List<GachaItemSO> result = new();
        foreach (var table in gearTypeDropRateTables)
        {
            foreach (var drop in table.dropRates)
            {
                foreach (var gear in drop.gearDropRates)
                {
                    if (gear.ItemSO.foundInArena <= foundInArena)
                    {
                        result.Add(gear.ItemSO);
                    }
                }
            }
        }
        return result;
    }

    protected virtual GearDropRateTable GetAdjustedGearTypeDropRates(GearDropRateTable src)
    {
        var result = new GearDropRateTable();
        var notMaxUpgraded = new List<GearTypeDropRate>();
        float oddRate = 0;

        // Setup result & classify maxUpgraded/notMaxUpgraded
        foreach (var dropRate in src.dropRates)
        {
            var clonedDropRate = GetAdjustedGearDropRates(dropRate, out float gearOddRate);
            result.dropRates.Add(clonedDropRate);

            if (gearOddRate > 0)
            {
                oddRate += gearOddRate;
            }
            else
            {
                notMaxUpgraded.Add(clonedDropRate);
            }
        }

        float remainRate = 0;
        foreach (var table in notMaxUpgraded)
        {
            remainRate += table.Probability;
        }

        foreach (var table in notMaxUpgraded)
        {
            foreach (var gear in table.gearDropRates)
            {
                gear.probability *= 1f + oddRate / remainRate;
            }
        }

        return result;
    }

    protected virtual GearTypeDropRate GetAdjustedGearDropRates(GearTypeDropRate src, out float gearOddRate)
    {
        var result = new GearTypeDropRate()
        {
            GearTypeManagerSO = src.GearTypeManagerSO,
        };
        var maxUpgraded = new List<GearTypeDropRate.GearDropRate>();
        var notMaxUpgraded = new List<GearTypeDropRate.GearDropRate>();
        // Setup result & classify maxUpgraded/notMaxUpgraded
        var dropRateResult = new List<GearTypeDropRate.GearDropRate>();
        foreach (var dropRate in src.gearDropRates)
        {
            var clonedDropRate = new GearTypeDropRate.GearDropRate()
            {
                ItemSO = dropRate.ItemSO,
                probability = dropRate.probability
            };

            if (dropRate.ItemSO.foundInArena <= foundInArena)
            {
                dropRateResult.Add(clonedDropRate);
                if (dropRate.ItemSO.IsMaxUpgradeLevel())// Check if gear max upgraded
                    maxUpgraded.Add(clonedDropRate);
                else
                    notMaxUpgraded.Add(clonedDropRate);
            }
        }
        gearOddRate = 0;
        // Check maxUpgraded count
        if (maxUpgraded.Count <= 0)
        {
            goto Return;
        }

        if (maxUpgraded.Count >= dropRateResult.Count)
        {
            gearOddRate = src.Probability * (1 - DownRatio);
        }

        // Update drop rates within maxUpgraded
        float oddRate = 0f;
        foreach (var dropRate in maxUpgraded)
        {
            oddRate += dropRate.probability * (1f - DownRatio);
            dropRate.probability *= DownRatio;
        }

        // Update drop rates within notMaxUpgraded
        float remainRate = 0f;
        foreach (var dropRate in notMaxUpgraded)
        {
            remainRate += dropRate.probability;
        }
        foreach (var dropRate in notMaxUpgraded)
        {
            dropRate.probability *= 1f + oddRate / remainRate;
        }

    Return:
        result.gearDropRates = dropRateResult;
        return result;
    }
    #endregion
    #endregion

    #region DataImport
    public virtual void SetFoundInArena(int index) => foundInArena = index;
    public virtual void SetTotalCards(int amount) => TotalCardsCount = amount;

    public virtual void SetNoBoosterProb(float prob) => noBoosterProb = prob;
    public virtual void SetMinBoosterCardCount(int amount) => minBoosterCardsCount = amount;
    public virtual void SetMaxBoosterCardCount(int amount) => maxBoosterCardsCount = amount;

    public virtual void SetGearDropRate(float prob) => gearCardProbability = prob;
    public virtual void SetCharacterDropRate(float prob) => characterCardProbability = prob;

    public virtual void SetMoneyRange(int minAmount, int maxAmount)
    {
        SetCurrencyAmount(CurrencyType.Standard, new CurrencyRngInfo()
        {
            min = minAmount,
            max = maxAmount,
            probability = 1
        });
    }

    public virtual void SetGemRange(int minAmount, int maxAmount)
    {
        SetCurrencyAmount(CurrencyType.Premium, new CurrencyRngInfo()
        {
            min = minAmount,
            max = maxAmount,
            probability = 1
        });
    }

    public virtual void SetBoosterProbs(List<ValueTuple<PvPBoosterType, float>> boosterProbs)
    {
        boosterCardRngInfos.Clear();
        foreach (var item in boosterProbs)
        {
            boosterCardRngInfos.Add(new BoosterCardRngInfo()
            {
                boosterType = item.Item1,
                probability = item.Item2
            });
        }
    }

    public virtual void SetGuaranteeAmount(int epicAmount, int legendaryAmount)
    {
        SetGuaranteedCardsCount(RarityType.Epic, epicAmount);
        SetGuaranteedCardsCount(RarityType.Legendary, legendaryAmount);
    }

    public virtual void SetGearRarityDropRate(float commonRate, float epicRate, float legendaryRate)
    {
        SetItemRarityDropRate(ItemType.Gear, new List<ItemCardRngInfo>()
        {
            new ItemCardRngInfo(RarityType.Common, commonRate),
            new ItemCardRngInfo(RarityType.Epic, epicRate),
            new ItemCardRngInfo(RarityType.Legendary, legendaryRate),
        });
    }

    public virtual void SetCharacterRarityDropRate(float commonRate, float epicRate, float legendaryRate)
    {
        SetItemRarityDropRate(ItemType.Character, new List<ItemCardRngInfo>()
        {
            new ItemCardRngInfo(RarityType.Common, commonRate),
            new ItemCardRngInfo(RarityType.Epic, epicRate),
            new ItemCardRngInfo(RarityType.Legendary, legendaryRate),
        });
    }

    public virtual void SetCharacterDropRateTable(Dictionary<GachaItemSO, float> characterDropInfos)
    {
        characterDropRateTables.Clear();
        if (characterDropInfos == null)
            return;
        var rarities = characterDropInfos.Select(drop => drop.Key.GetRarityType()).Distinct().ToList();
        foreach (var rarity in rarities)
        {
            characterDropRateTables.Add(DropInfoToDropRateTable(rarity));
        }

        CharacterDropRateTable DropInfoToDropRateTable(RarityType rarityType)
        {
            return new CharacterDropRateTable()
            {
                rarity = rarityType,
                dropRates = characterDropInfos.
            Where(dropInfo => dropInfo.Key.GetRarityType() == rarityType).
            Select(dropInfo => new CharacterDropRate() { CharacterSO = dropInfo.Key, probability = dropInfo.Value / 100 })
            .ToList()
            };
        }
    }

    public virtual void SetGearDropRateTable(Dictionary<GearInfo, float> gearDropInfos)
    {
        gearTypeDropRateTables.Clear();
        if (gearDropInfos == null)
            return;
        var gearTypes = gearDropInfos.Select(gearInfo => gearInfo.Key.gearManagerSO).Distinct().ToList();
        GearDropRateTable table = new();
        foreach (var gearType in gearTypes)
        {
            var dropRateTable = DropInfoToDropRateTable(gearType);
            table.dropRates.Add(dropRateTable);
        }
        gearTypeDropRateTables.Add(table);

        GearTypeDropRate DropInfoToDropRateTable(ItemManagerSO gearManagerSO)
        {
            return new GearTypeDropRate()
            {
                GearTypeManagerSO = gearManagerSO,
                gearDropRates = gearDropInfos.
                Where(info => info.Key.gearManagerSO == gearManagerSO).
                Select(info => new GearTypeDropRate.GearDropRate()
                {
                    ItemSO = info.Key.gearSO,
                    Probability = info.Value / 100
                }).
                ToList()
            };
        }
    }

    [Serializable]
    public class GearInfo
    {
        public ItemManagerSO gearManagerSO;
        public GachaItemSO gearSO;
    }

    //Internal
    protected virtual void SetCurrencyAmount(CurrencyType type, CurrencyRngInfo currencyRngInfo)
    {
        if (type == CurrencyType.Standard)
        {
            moneyCardRngInfos.Clear();
            moneyCardRngInfos.Add(currencyRngInfo);
        }
        else if (type == CurrencyType.Premium)
        {
            gemCardRngInfos.Clear();
            gemCardRngInfos.Add(currencyRngInfo);
        }
    }

    protected virtual void SetItemRarityDropRate(ItemType type, List<ItemCardRngInfo> itemCardRngInfos)
    {
        if (type == ItemType.Character)
        {
            characterCardRngInfos.Clear();
            characterCardRngInfos.AddRange(itemCardRngInfos);
        }
        else
        {
            gearCardRngInfos.Clear();
            gearCardRngInfos.AddRange(itemCardRngInfos);
        }
    }

    #endregion

    [ContextMenu("Print generated cards 50 Times/Normal")]
    protected virtual void PrintGeneratedCards()
    {
        List<GachaCard> gachaCards = new();
        for (int i = 0; i < 50; i++)
        {
            gachaCards.AddRange(GenerateCards());
        }
        var cardGroups = gachaCards.GroupDuplicate();
        cardGroups = cardGroups.OrderByDescending(group => group.cardsAmount).ToList();
        int totalItemCards = 0;
        foreach (var card in cardGroups)
        {
            if (card.representativeCard.TryGetModule<RarityItemModule>(out var _) == false) continue;
            totalItemCards += card.cardsAmount;
        }
        var sb = new System.Text.StringBuilder();
        foreach (var card in cardGroups)
        {
            if (card.representativeCard.TryGetModule<RarityItemModule>(out var _) == false) continue;
            sb.Append($"{card.representativeCard} = {card.cardsAmount / (float)totalItemCards * 100:0.00}% ");
        }
        Debug.Log(sb.ToString());
    }

    [ContextMenu("Print generated cards 50 Times/Character Only")]
    protected virtual void PrintGeneratedCharacterCards()
    {
        List<GachaCard> gachaCards = new();
        for (int i = 0; i < 50; i++)
        {
            gachaCards.AddRange(GenerateCharacterCards(TotalCardsCount));
        }
        var cardGroups = gachaCards.GroupDuplicate();
        cardGroups = cardGroups.OrderByDescending(group => group.cardsAmount).ToList();
        int totalItemCards = 0;
        foreach (var card in cardGroups)
        {
            //if (card.representativeCard.TryGetModule<RarityItemModule>(out var _) == false) continue;
            totalItemCards += card.cardsAmount;
        }
        var sb = new System.Text.StringBuilder();
        foreach (var card in cardGroups)
        {
            //if (card.representativeCard.TryGetModule<RarityItemModule>(out var _) == false) continue;
            sb.Append($"{card.representativeCard} = {card.cardsAmount / (float)totalItemCards * 100:0.00}% ");
        }
        Debug.Log(sb.ToString());
    }

    [ContextMenu("Print generated cards 50 Times/Gear Only")]
    protected virtual void PrintGeneratedGearCards()
    {
        List<GachaCard> gachaCards = new();
        for (int i = 0; i < 50; i++)
        {
            gachaCards.AddRange(GenerateGearCards(TotalCardsCount));
        }
        var cardGroups = gachaCards.GroupDuplicate();
        cardGroups = cardGroups.OrderByDescending(group => group.cardsAmount).ToList();
        int totalItemCards = 0;
        foreach (var card in cardGroups)
        {
            //if (card.representativeCard.TryGetModule<RarityItemModule>(out var _) == false) continue;
            totalItemCards += card.cardsAmount;
        }
        var sb = new System.Text.StringBuilder();
        foreach (var card in cardGroups)
        {
            //if (card.representativeCard.TryGetModule<RarityItemModule>(out var _) == false) continue;
            sb.Append($"{card.representativeCard} = {card.cardsAmount / (float)totalItemCards * 100:0.00}% ");
        }
        Debug.Log(sb.ToString());
    }

    [ContextMenu("Print adjusted drop Rate table/Gear")]
    protected virtual void PrintAdjustedGearDropRateTable()
    {
        AdjustGearDropRateTables();
        var sb = new System.Text.StringBuilder();
        foreach (var table in adjustedGearTypeDropRateTables)
        {
            foreach (var dropRateTable in table.dropRates)
            {
                foreach (var gear in dropRateTable.gearDropRates)
                {
                    sb.Append($"{gear.ItemSO.GetDisplayName()} {gear.Probability * 100:0.00} /");
                }
            }
        }
        Debug.Log(sb);
    }

    [ContextMenu("Print adjusted drop Rate table/Character")]
    protected virtual void PrintAdjustedCharacterDropRateTable()
    {
        AdjustCharacterDropRateTables();
        var sb = new System.Text.StringBuilder();
        foreach (var table in adjustedCharacterDropRateTables)
        {
            foreach (var character in table.dropRates)
            {
                sb.Append($"{character.CharacterSO.GetDisplayName()} {character.Probability * 100:0.00} /");
            }
        }
        Debug.Log(sb);
    }

    #region Random Classes
    [Serializable]
    protected class BoosterCardRngInfo : IRandomizable
    {
        public PvPBoosterType boosterType;
        [Range(0f, 1f)] public float probability;
        public float Probability { get => probability; set => probability = value; }
    }

    [Serializable]
    protected class CurrencyRngInfo : IRandomizable
    {
        [HorizontalGroup("Group1"), LabelWidth(50)] public float min, max;
        public int amount => GetRandomAmount();
        public int GetRandomAmount()
        {
            return (int)Random.Range(min, max);
        }

        [Range(0f, 1f)] public float probability;
        public float Probability { get => probability; set => probability = value; }
    }

    [Serializable]
    protected class ItemCardRngInfo : IRandomizable
    {
        public RarityType rarity;
        [Range(0f, 1f)] public float probability;
        public float Probability { get => probability; set => probability = value; }

        public ItemCardRngInfo(RarityType rarity, float prob)
        {
            this.rarity = rarity;
            this.probability = prob;
        }
    }

    protected class ItemTypeRandom : IRandomizable
    {
        public ItemType ItemType;
        public List<ItemCardRngInfo> ItemCardRngInfos;
        private float probability;
        public float Probability
        {
            get => probability;
            set => probability = value;
        }
    }
    #endregion

    #region Character/Gear Drop Rate Classes
    [Serializable]
    protected class DropRateTable<T> where T : ItemDropRate
    {
        public List<T> dropRates = new();
    }

    [Serializable]
    protected class ItemDropRate
    {

    }

    [Serializable]
    protected class CharacterDropRate : ItemDropRate, IRandomizable
    {
        public GachaItemSO CharacterSO;
        [Range(0f, 1f)] public float probability;
        public float Probability { get => probability; set => probability = value; }
        public override string ToString()
        {
            return $"{CharacterSO.GetInternalName()}{Probability * 100f}%";
        }
    }

    [Serializable]
    protected class GearTypeDropRate : ItemDropRate, IRandomizable
    {
        public ItemManagerSO GearTypeManagerSO;
        public List<GearDropRate> gearDropRates = new();

        public float Probability { get => gearDropRates.Sum(gear => gear.Probability); set => throw new NotImplementedException(); }
        private List<GachaItemSO> allItemSO;
        public virtual List<GachaItemSO> AllItemSO
        {
            get
            {
                if (allItemSO == null || allItemSO.Count <= 0)
                {
                    allItemSO = gearDropRates.Select(table => table.ItemSO).ToList();
                }
                return allItemSO;
            }
        }

        public override string ToString()
        {
            return $"{GearTypeManagerSO.analyticsName}{Probability * 100f}%";
        }
        [Serializable]
        public class GearDropRate : IRandomizable
        {
            public GachaItemSO ItemSO;
            [Range(0f, 1f)] public float probability;
            public float Probability { get => probability; set => probability = value; }
        }
    }

    [Serializable]
    protected class CharacterDropRateTable : DropRateTable<CharacterDropRate>
    {
        public RarityType rarity;
        private List<GachaItemSO> allItemSO;
        public virtual List<GachaItemSO> AllItemSO
        {
            get
            {
                if (allItemSO == null || allItemSO.Count <= 0)
                {
                    allItemSO = dropRates.Select(table => table.CharacterSO).ToList();
                }
                return allItemSO;
            }
        }
    }

    [Serializable]
    protected class GearDropRateTable : DropRateTable<GearTypeDropRate>
    {
        private List<ItemManagerSO> allItemSO;
        public virtual List<ItemManagerSO> AllItemSO
        {
            get
            {
                if (allItemSO == null || allItemSO.Count <= 0)
                {
                    allItemSO = dropRates.Select(table => table.GearTypeManagerSO).ToList();
                }
                return allItemSO;
            }
        }
    }
    #endregion
}