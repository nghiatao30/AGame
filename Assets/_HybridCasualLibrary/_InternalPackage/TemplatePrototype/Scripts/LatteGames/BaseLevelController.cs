using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames;
using LatteGames.Template;
using HyrphusQ.Events;

public class BaseLevelController : LevelController
{
    protected bool isVictory;

    protected virtual void Awake()
    {
        GameEventHandler.AddActionEvent(LevelEventCode.OnWinLevel, OnWinLevel);
        GameEventHandler.AddActionEvent(LevelEventCode.OnLoseLevel, OnLoseLevel);
    }
    protected virtual void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(LevelEventCode.OnWinLevel, OnWinLevel);
        GameEventHandler.RemoveActionEvent(LevelEventCode.OnLoseLevel, OnLoseLevel);
    }

    protected virtual void OnWinLevel()
    {
        isVictory = true;
        EndLevel();
    }
    protected virtual void OnLoseLevel()
    {
        isVictory = false;
        EndLevel();
    }

    public override bool IsVictory()
    {
        return isVictory;
    }
}
