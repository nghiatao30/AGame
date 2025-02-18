using System.Collections;
using System.Collections.Generic;
using LatteGames;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelStorage", menuName = "LatteGames/ScriptableObject/LevelManagement/LoopBackLevelStorage")]
public class LoopBackLevelStorage : LevelStorage
{
    [SerializeField] int loopBackStartLevel = 3;
    [SerializeField] IntVariable playerAchievedContinuousLevel;
    public IntVariable PlayerAchievedContinuousLevel => playerAchievedContinuousLevel;

    public override LevelAsset GetLevel(int levelIndex, EndOfLevelBehaviour endOfLevelBehaviour = EndOfLevelBehaviour.Stay)
    {
        switch (endOfLevelBehaviour)
        {
            case EndOfLevelBehaviour.LoopBack:
                levelIndex = GetLevelIndex(levelIndex);
                break;
            case EndOfLevelBehaviour.Stay:
                levelIndex = Mathf.Clamp(levelIndex, 0, LevelAssets.Count - 1);
                break;
        }
        return LevelAssets[levelIndex];
    }

    public int GetLevelIndex(int levelIndex)
    {
        if (levelIndex < levelAssets.Count) return levelIndex;
        var loopFromIndex = Mathf.Clamp(loopBackStartLevel, 0, levelAssets.Count - 1);
        return (levelIndex - loopFromIndex) % (levelAssets.Count - loopFromIndex) + loopFromIndex;
    }
}
