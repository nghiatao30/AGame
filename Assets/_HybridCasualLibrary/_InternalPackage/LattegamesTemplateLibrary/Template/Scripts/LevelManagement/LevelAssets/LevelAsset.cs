using UnityEngine;

namespace LatteGames{
public abstract class LevelAsset : ScriptableObject {
    public abstract bool CanBeLoadedImmediately();
    public abstract bool CanBeUnloadedImmediately();
    public abstract LevelController LoadLevelImmediately();
    public abstract IAsyncLevelLoad LoadLevelAsync();
    public abstract void UnLoadLevelImmediately(LevelController level);
    public abstract IAsyncLevelUnload UnLoadLevelAsync(LevelController level);

    public interface IAsyncLevelLoad
    {
        bool Finished();
        float GetProgress();
        LevelController GetLevelController();
    }

    public interface IAsyncLevelUnload
    {
        bool Finished();
        float GetProgress();
    }
}
}