using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace GachaSystem.Core
{
    [CreateAssetMenu(fileName = "GachaPacksList", menuName = "LatteGames/Gacha/GachaPacksList")]
    public class GachaPacksList : ScriptableObject
    {
        [SerializeField]
        private List<GachaPack> packs;
        public List<GachaPack> Packs { get => packs; private set => packs = value; }
#if UNITY_EDITOR
        [BoxGroup("Editor Only")]
        [FolderPath]
        public string productsFolderPath;

        [BoxGroup("Editor Only")]
        [Button("GetProducts")]
        void GetProducts()
        {
            if (Packs != null)
            {
                Packs.Clear();
            }
            Packs = EditorUtils.FindAssetsOfType<GachaPack>(productsFolderPath);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
