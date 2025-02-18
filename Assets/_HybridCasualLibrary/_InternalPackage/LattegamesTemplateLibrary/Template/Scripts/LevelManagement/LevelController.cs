using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
public class LevelController : MonoBehaviour
{
    public event Action<LevelController> LevelEnded = delegate {};
    public event Action<LevelController> LevelStarted = delegate {};
    public event Action<LevelController> LevelPaused = delegate {};
    public event Action<LevelController> LevelResumed = delegate {};
    
    public virtual void StartLevel(){
        LevelStarted?.Invoke(this);
    }
    public virtual void EndLevel(){
        LevelEnded?.Invoke(this);
    }
    public virtual void PauseLevel(){
        LevelPaused?.Invoke(this);
    }
    public virtual void ResumeLevel(){
        LevelResumed?.Invoke(this);
    }
    public virtual bool IsVictory(){
        return true;
    }
}
}