using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CsvHelper;
using CsvHelper.Configuration;
using GachaSystem.Core;
using HyrphusQ.Helpers;

[CreateAssetMenu(fileName = "GachaPackDatasheet", menuName = "LatteGames/Editor/Datasheet/GachaPackDatasheet")]
public class GachaPackDatasheet : Datasheet
{
    protected enum DataType { PackData, DropRateData }

    [Serializable]
    public class PackInfoRow
    {
        #region RowIndex
        public const int ArenaLocation = 1;
        public const int Type = 2;
        public const int Rate = 3;
        public const int TotalCards = 4;

        public const int NoBooster = 5;
        public const int AttackBoosterProb = 6;
        public const int HPBoosterProb = 7;
        public const int BoosterMinAmount = 8;
        public const int BoosterMaxAmount = 9;

        public const int MoneyMinAmount = 10;
        public const int MoneyMaxAmount = 11;

        public const int GemMinAmount = 12;
        public const int GemMaxAmount = 13;

        public const int GuaranteeEpic = 14;
        public const int GuaranteeLegendary = 15;

        public const int GearDropRate = 16;
        public const int GearDropRateCommon = 16;
        public const int GearDropRateEpic = 17;
        public const int GearDropRateLegendary = 18;

        public const int CharacterDropRate = 19;
        public const int CharacterDropRateCommon = 19;
        public const int CharacterDropRateEpic = 20;
        public const int CharacterDropRateLegendary = 21;
        #endregion

        public string define { get; set; }
        public string type { get; set; }
        public float packDropRate { get; set; }

        public int foundInArena { get; set; }
        public int totalCards { get; set; }

        public float atkBoosterProb { get; set; }
        public float hpBoosterProb { get; set; }

        public int minBoosterCardCount { get; set; }
        public int maxBoosterCardCount { get; set; }

        public int minMoneyAmount { get; set; }
        public int maxMoneyAmount { get; set; }

        public int minGemAmount { get; set; }
        public int maxGemAmount { get; set; }

        public int guaranteeEpicAmount { get; set; }
        public int guaranteeLegendaryAmount { get; set; }

        public float gearDropRateCommon { get; set; }
        public float gearDropRateEpic { get; set; }
        public float gearDropRateLegendary { get; set; }

        public float characterDropRateCommon;
        public float characterDropRateEpic;
        public float characterDropRateLegendary;

        public Dictionary<GachaItemSO, float> characterData; //<name, droprate>
        public Dictionary<BaseGachaPack.GearInfo, float> gearData; //<name, droprate>

        private void ReadData(CsvReader csv)
        {
            type = csv.GetField(Type);
            totalCards = int.Parse(csv.GetField(TotalCards));
            packDropRate = float.Parse(csv.GetField(Rate));

            atkBoosterProb = float.Parse(csv.GetField(AttackBoosterProb));
            hpBoosterProb = float.Parse(csv.GetField(HPBoosterProb));

            minBoosterCardCount = int.Parse(csv.GetField(BoosterMinAmount));
            maxBoosterCardCount = int.Parse(csv.GetField(BoosterMaxAmount));

            minMoneyAmount = int.Parse(csv.GetField(MoneyMinAmount));
            maxMoneyAmount = int.Parse(csv.GetField(MoneyMaxAmount));

            minGemAmount = int.Parse(csv.GetField(GemMinAmount));
            maxGemAmount = int.Parse(csv.GetField(GemMaxAmount));

            guaranteeEpicAmount = int.Parse(csv.GetField(GuaranteeEpic));
            guaranteeLegendaryAmount = int.Parse(csv.GetField(GuaranteeLegendary));

            gearDropRateCommon = float.Parse(csv.GetField(GearDropRateCommon));
            gearDropRateEpic = float.Parse(csv.GetField(GearDropRateEpic));
            gearDropRateLegendary = float.Parse(csv.GetField(GearDropRateLegendary));

            float.TryParse(csv.GetField(CharacterDropRateCommon), out characterDropRateCommon);
            float.TryParse(csv.GetField(CharacterDropRateEpic), out characterDropRateEpic);
            float.TryParse(csv.GetField(CharacterDropRateLegendary), out characterDropRateLegendary);
        }

        private void Display(CsvReader csv)
        {
            Debug.Log(csv.Parser.RawRecord);
            Debug.Log(csv.GetField(ArenaLocation));
        }

        internal static PackInfoRow Read(CsvReader csv)
        {
            var row = new PackInfoRow();
            try
            {
                row.ReadData(csv);
            }
            catch (Exception exc)
            {
                row.Display(csv);
                Debug.LogException(exc);
                return null;
            }
            return row;
        }
    }

    [Serializable]
    public class DropInfoRow
    {
        public const int ItemType = 0;

        public const string Arena = "ARENA {INDEX} and above";

        public static string GetArenaHeader(int arena)
        {
            return Arena.Replace("{INDEX}", arena.ToString());
        }
    }

    public class GachaPackEditorData
    {
        [field: SerializeField]
        public string sheetID { get; set; }
    }

    [SerializeField] protected string m_CharacterLabel = "CHARACTER";
    [SerializeField] protected string m_GearLabel = "GEAR";

    [SerializeField] protected Dictionary<DataType, GachaPackEditorData> gachaPackEditorDataDictionary;
    [SerializeField] protected ItemManagerSO characterManagerSO;
    [SerializeField] protected List<ItemManagerSO> gearManagerSOs;
    [SerializeField] protected List<BaseGachaPack> gachaPacks;
    [SerializeField] protected List<GachaPacksCollection> gachaPacksCollections;

    protected float noBoosterProb = 0;
    protected float gearDropRate = 0;
    protected float characterDropRate = 0;
    protected List<Dictionary<GachaItemSO, float>> characterDropRateTables = new();
    protected List<Dictionary<BaseGachaPack.GearInfo, float>> gearDropRateTables = new();

    protected void InjectData(BaseGachaPack gachaPack, PackInfoRow row)
    {
        gachaPack.SetFoundInArena(row.foundInArena);
        gachaPack.SetTotalCards(row.totalCards);
        gachaPack.SetNoBoosterProb(noBoosterProb);

        gachaPack.SetMinBoosterCardCount(row.minBoosterCardCount);
        gachaPack.SetMaxBoosterCardCount(row.maxBoosterCardCount);
        gachaPack.SetBoosterProbs(new List<ValueTuple<PvPBoosterType, float>>()
        {
            ValueTuple.Create(PvPBoosterType.Attack, row.atkBoosterProb),
            ValueTuple.Create(PvPBoosterType.Hp, row.hpBoosterProb)
        });

        gachaPack.SetGearDropRate(gearDropRate);
        gachaPack.SetCharacterDropRate(characterDropRate);

        gachaPack.SetMoneyRange(row.minMoneyAmount, row.maxMoneyAmount);
        gachaPack.SetGemRange(row.minGemAmount, row.maxGemAmount);

        gachaPack.SetGuaranteeAmount(row.guaranteeEpicAmount, row.guaranteeLegendaryAmount);

        gachaPack.SetGearRarityDropRate(row.gearDropRateCommon, row.gearDropRateEpic, row.gearDropRateLegendary);
        gachaPack.SetCharacterRarityDropRate(row.characterDropRateCommon, row.characterDropRateEpic, row.characterDropRateLegendary);

        gachaPack.SetCharacterDropRateTable(row.characterData);
        gachaPack.SetGearDropRateTable(row.gearData);

        EditorUtility.SetDirty(gachaPack);
        AssetDatabase.SaveAssetIfDirty(gachaPack);

        Debug.Log($"{gachaPack.name}");
    }

    public override void ExportData(string directoryPath)
    {
        Debug.LogError("Not support!");
    }

    public override void ImportData()
    {
        var remoteSheetUrls = new List<string>();
        var localFilePaths = new List<string>();
        foreach (var item in gachaPackEditorDataDictionary)
        {
            remoteSheetUrls.Add(remotePath.Replace("{sheetID}", item.Value.sheetID));
            localFilePaths.Add(localPath.Replace("{name}", item.Key.ToString()));
        }
        RemoteDataSync.Sync(remoteSheetUrls.ToArray(), localFilePaths.ToArray(), false, OnSyncCompleted);

        void OnSyncCompleted(bool isSucceeded)
        {
            if (!isSucceeded)
            {
                EditorUtility.DisplayDialog(RemoteDataSync.Title, RemoteDataSync.FailMessage, RemoteDataSync.OkMessage);
                return;
            }
            var packInfoRows = new List<PackInfoRow>();
            var dropInfoRows = new List<DropInfoRow>();

            gearDropRateTables.Clear();
            characterDropRateTables.Clear();
            ReadDropRateData();
            ReadPackData();
            foreach (var collection in gachaPacksCollections)
            {
                collection.PackRngInfos.Clear();
            }
            foreach (var row in packInfoRows)
            {
                BaseGachaPack pack = GetGachaPackByRow(row);
                InjectData(pack, row);
                GachaPacksCollection collection = GetGachaPackCollectionByRow(row);
                collection.PackRngInfos.Add(new GachaPacksCollection.PackRngInfo()
                {
                    pack = pack,
                    probability = row.packDropRate
                });
                EditorUtility.SetDirty(collection);
                AssetDatabase.SaveAssetIfDirty(collection);
            }
            EditorUtility.DisplayDialog(RemoteDataSync.Title, RemoteDataSync.SuccessMessage, RemoteDataSync.OkMessage);

            void ReadPackData()
            {
                var filePath = localPath.Replace("{name}", DataType.PackData.ToString());
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    IgnoreBlankLines = true,
                    MissingFieldFound = null,
                };
                string arenaDefine = null;
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Read(); //Skip first row

                    csv.Read();
                    float.TryParse(csv.GetField(PackInfoRow.GearDropRate), out gearDropRate);
                    float.TryParse(csv.GetField(PackInfoRow.CharacterDropRate), out characterDropRate);

                    csv.Read();
                    float.TryParse(csv.GetField(PackInfoRow.NoBooster), out noBoosterProb);

                    csv.Read();

                    while (csv.Read())
                    {
                        if (csv.Parser.Record.All(item => string.IsNullOrEmpty(item))) continue;
                        if (int.TryParse(csv.GetField(PackInfoRow.TotalCards), out int _) == false) continue;
                        if (string.IsNullOrEmpty(csv.GetField(PackInfoRow.ArenaLocation)) == false) arenaDefine = csv.GetField(PackInfoRow.ArenaLocation);
                        if (!int.TryParse(arenaDefine.Substring(arenaDefine.Length - 1), out int foundInArena) || foundInArena - 1 >= gachaPacksCollections.Count)
                            continue;
                        var row = PackInfoRow.Read(csv);
                        row.foundInArena = foundInArena;
                        row.define = arenaDefine;
                        row.characterData = GetCharacterDataByArena(row.foundInArena);
                        row.gearData = GetGearDataByArena(row.foundInArena);
                        if (row != null)
                        {
                            packInfoRows.Add(row);
                        }
                    }
                }
            }
            void ReadDropRateData()
            {
                var filePath = sheetLocation.localPath.Replace("{name}", DataType.DropRateData.ToString());
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    IgnoreBlankLines = true,
                    MissingFieldFound = null,
                };
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Read();
                    csv.ReadHeader();

                    string currentItemType = null;

                    while (csv.Read())
                    {
                        if(string.IsNullOrEmpty(csv.GetField(DropInfoRow.ItemType)) == false) currentItemType = csv.GetField(DropInfoRow.ItemType);
                        if (csv.Parser.Record.All(item => string.IsNullOrEmpty(item))) continue;
                        ReadDropData(csv);
                    }

                    void ReadDropData(CsvReader csv)
                    {
                        if (string.IsNullOrEmpty(currentItemType)) return;
                        if (currentItemType.Equals(m_CharacterLabel))
                        {
                            for (int i = 1; i <= gachaPacksCollections.Count; i++)
                            {
                                string characterName = csv.GetField(DropInfoRow.GetArenaHeader(i));
                                if (string.IsNullOrEmpty(characterName)) continue;
                                var characterSO = GetCharacterSOByName(characterName);
                                if (characterSO == null) continue;
                                if (characterDropRateTables.Count < i) characterDropRateTables.Add(new Dictionary<GachaItemSO, float>());
                                var prob = PercentTextToProb(csv.GetField(csv.GetFieldIndex(DropInfoRow.GetArenaHeader(i)) + 1));
                                if (characterDropRateTables[i - 1].ContainsKey(characterSO))
                                {
                                    characterDropRateTables[i - 1][characterSO] = prob;
                                }
                                else
                                {
                                    characterDropRateTables[i - 1].Add(characterSO, prob);
                                }
                            }
                        }
                        else if (currentItemType.Equals(m_GearLabel))
                        {
                            for (int i = 1; i <= gachaPacksCollections.Count; i++)
                            {
                                string gearName = csv.GetField(DropInfoRow.GetArenaHeader(i));
                                if (string.IsNullOrEmpty(gearName)) continue;
                                var gearInfo = GetGearInfoByName(gearName);
                                if (gearInfo == null) continue;
                                if (gearDropRateTables.Count < i) gearDropRateTables.Add(new Dictionary<BaseGachaPack.GearInfo, float>());
                                var prob = PercentTextToProb(csv.GetField(csv.GetFieldIndex(DropInfoRow.GetArenaHeader(i)) + 1));
                                if (gearDropRateTables[i - 1].ContainsKey(gearInfo))
                                {
                                    gearDropRateTables[i - 1][gearInfo] = prob;
                                }
                                else
                                {
                                    gearDropRateTables[i - 1].Add(gearInfo, prob);
                                }
                            }
                        }

                        float PercentTextToProb(string text)
                        {
                            text = text.Replace("%", string.Empty);
                            return float.Parse(text);
                        }
                    }
                }
            }

            GachaItemSO GetCharacterSOByName(string name)
            {
                return characterManagerSO.items.Find(character => character.GetInternalName().ToLower().Contains(name.ToLower())).Cast<GachaItemSO>();
            }

            BaseGachaPack.GearInfo GetGearInfoByName(string name)
            {
                try
                {
                    var gearInfo = new BaseGachaPack.GearInfo();
                    gearInfo.gearManagerSO = gearManagerSOs.
                        Find(manager => manager.items.
                        Any(gear => gear.GetInternalName().ToLower().Contains(name.ToLower())));
                    gearInfo.gearSO = gearInfo.gearManagerSO.items.
                        Find(gear => gear.GetInternalName().ToLower().Contains(name.ToLower())).Cast<GachaItemSO>();
                    return gearInfo;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            BaseGachaPack GetGachaPackByRow(PackInfoRow row)
            {
                var packs = gachaPacks.
                    FindAll(pack => pack.GetInternalName().ToLower().
                    Contains(row.define.Replace(" ", string.Empty).ToLower()));
                return packs.Find(pack => pack.GetInternalName().ToLower().Contains(row.type.Replace(" ", string.Empty).ToLower()));
            }

            GachaPacksCollection GetGachaPackCollectionByRow(PackInfoRow row)
            {
                var packCollection = gachaPacksCollections.Find(collection => collection.name.ToLower().Contains(row.define.Replace(" ", string.Empty).ToLower()));
                return packCollection;
            }

            Dictionary<GachaItemSO, float> GetCharacterDataByArena(int arena)
            {
                if (!characterDropRateTables.IsValidIndex(arena - 1))
                    return null;
                return characterDropRateTables[arena - 1];
            }

            Dictionary<BaseGachaPack.GearInfo, float> GetGearDataByArena(int arena)
            {
                return gearDropRateTables[arena - 1];
            }
        }
    }
}
