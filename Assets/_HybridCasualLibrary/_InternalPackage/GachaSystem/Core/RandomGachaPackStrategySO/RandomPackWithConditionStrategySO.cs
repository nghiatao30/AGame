using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GachaSystem.Core
{
    [CreateAssetMenu(fileName = "RandomPackWithConditionStrategySO", menuName = "LatteGames/Gacha/RandomPackStrategySO/RandomPackWithConditionStrategySO")]
    public class RandomPackWithConditionStrategySO : RandomPackStrategySO
    {
        protected enum GachaPackType
        {
            Classic,
            Great,
            Ultra
        }

        [SerializeField]
        protected PvPArenaVariable m_CurrentChosenArena;
        [SerializeField]
        protected IntVariable m_EncodingNumberVariable;
        [Tooltip(
            "1. First element is first [n] matches\n" +
            "2. Second element is after received a great box\n" +
            "3. Third element is after received a ultra box")]
        [SerializeField, TableMatrix]
        protected GachaPackType[][] m_GachaPackTypeArr2D = new GachaPackType[0][];

        protected GachaPackType GetPackType(GachaPack gachaPack)
        {
            if (gachaPack.name.Contains("Classic"))
                return GachaPackType.Classic;
            if (gachaPack.name.Contains("Great"))
                return GachaPackType.Great;
            return GachaPackType.Ultra;
        }

        public override GachaPack GetRandom(List<GachaPacksCollection.PackRngInfo> packRngInfo)
        {
            // Init gacha pack arr 2D
            var gachaPackArr2D = new GachaPack[][]
            {
                    // First [n] matches
                    GachaPackTypesToGachaPacks(GetPackTypeArrOfIndex(0)),
                    // After received a great box
                    GachaPackTypesToGachaPacks(GetPackTypeArrOfIndex(1)),
                    // After received a ultra box
                    GachaPackTypesToGachaPacks(GetPackTypeArrOfIndex(2)),
            };

            if (m_CurrentChosenArena.value.totalNumOfWonMatches < gachaPackArr2D[0].Length)
            {
                return gachaPackArr2D[0][m_CurrentChosenArena.value.totalNumOfWonMatches];
            }

            int encodingNumber = m_EncodingNumberVariable.value++;
            var x = DecodeColumnIndex(encodingNumber);
            var y = DecodeRowIndex(encodingNumber);
            if (x >= gachaPackArr2D[y].Length)
            {
                var pack = base.GetRandom(packRngInfo);
                if (GetPackType(pack) == GachaPackType.Great)
                {
                    m_EncodingNumberVariable.value = 100;
                }
                else if (GetPackType(pack) == GachaPackType.Ultra)
                {
                    m_EncodingNumberVariable.value = 200;
                }
                return pack;
            }
            return gachaPackArr2D[y][x];

            int DecodeColumnIndex(int decodingNumber)
            {
                return decodingNumber % 100;
            }
            int DecodeRowIndex(int decodingNumber)
            {
                return decodingNumber / 100;
            }
            GachaPack FindGachaPackOfType(GachaPackType packType)
            {
                return packRngInfo.Find(item => GetPackType(item.pack) == packType).pack;
            }
            GachaPack[] GachaPackTypesToGachaPacks(GachaPackType[] packTypes)
            {
                var gachaPacks = new GachaPack[packTypes.Length];
                for (int i = 0; i < packTypes.Length; i++)
                {
                    gachaPacks[i] = FindGachaPackOfType(packTypes[i]);
                }
                return gachaPacks;
            }
            GachaPackType[] GetPackTypeArrOfIndex(int index)
            {
                if (index >= m_GachaPackTypeArr2D.GetLength(0))
                    return new GachaPackType[] { };
                return m_GachaPackTypeArr2D[index];
            }
        }
    }
}