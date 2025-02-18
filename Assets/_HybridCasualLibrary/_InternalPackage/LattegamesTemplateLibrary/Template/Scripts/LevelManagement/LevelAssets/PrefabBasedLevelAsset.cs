using UnityEngine;

namespace LatteGames
{
    [CreateAssetMenu(fileName = "Level_1", menuName = "LatteGames/ScriptableObject/LevelManagement/PrefabBasedLevelAsset", order = 0)]
    public class PrefabBasedLevelAsset : LevelAsset
    {
        [SerializeField]
        private LevelController levelPrefab = null;

        public override bool CanBeLoadedImmediately()
        {
            return true;
        }

        public override bool CanBeUnloadedImmediately()
        {
            return true;
        }

        public override IAsyncLevelLoad LoadLevelAsync()
        {
            var newLevel = Instantiate(levelPrefab);
            return new AsyncLevelLoad(newLevel);
        }

        public override LevelController LoadLevelImmediately()
        {
            return Instantiate(levelPrefab);
        }

        public override IAsyncLevelUnload UnLoadLevelAsync(LevelController level)
        {
            if (level == null) goto Return;

            if (!Application.isPlaying)
                DestroyImmediate(level.gameObject);
            else
                Destroy(level.gameObject);

            Return:
            return new AsyncLevelUnload();
        }

        public override void UnLoadLevelImmediately(LevelController level)
        {
            if (!Application.isPlaying)
                DestroyImmediate(level.gameObject);
            else
                Destroy(level.gameObject);
        }

        public class AsyncLevelLoad : IAsyncLevelLoad
        {
            private LevelController prefab;
            public AsyncLevelLoad(LevelController prefab)
            {
                this.prefab = prefab;
            }

            public bool Finished()
            {
                return true;
            }

            public LevelController GetLevelController()
            {
                return this.prefab;
            }

            public float GetProgress()
            {
                return 1;
            }
        }

        public class AsyncLevelUnload : IAsyncLevelUnload
        {
            public bool Finished()
            {
                return true;
            }

            public float GetProgress()
            {
                return 1;
            }
        }
    }
}