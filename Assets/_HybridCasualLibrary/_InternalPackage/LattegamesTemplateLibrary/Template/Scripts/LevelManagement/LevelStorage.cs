using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    [CreateAssetMenu(fileName = "LevelStorage", menuName = "LatteGames/ScriptableObject/LevelManagement/LevelStorage", order = 0)]
    public class LevelStorage : ScriptableObject
    {
        [SerializeField] protected List<LevelAsset> levelAssets = new List<LevelAsset>();
        public List<LevelAsset> LevelAssets { get => levelAssets; set => levelAssets = value; }

        public enum EndOfLevelBehaviour
        {
            Stay,
            LoopBack
        }

        public virtual LevelAsset GetLevel(int levelIndex, EndOfLevelBehaviour endOfLevelBehaviour = EndOfLevelBehaviour.Stay)
        {
            switch (endOfLevelBehaviour)
            {
                case EndOfLevelBehaviour.LoopBack:
                    levelIndex = levelIndex % LevelAssets.Count;
                    break;
                case EndOfLevelBehaviour.Stay:
                    levelIndex = Mathf.Clamp(levelIndex, 0, LevelAssets.Count - 1);
                    break;
            }
            return LevelAssets[levelIndex];
        }

        public virtual int GetLevelIndex(LevelAsset levelAsset)
        {
            return LevelAssets.IndexOf(levelAsset);
        }
    }
}