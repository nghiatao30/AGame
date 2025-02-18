using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInfo
{
    public virtual bool isLocal => personalInfo.isLocal;
    public virtual PersonalInfo personalInfo { get; set; }

    public virtual T Cast<T>() where T : PlayerInfo => this as T;
    public abstract void ClearIngameData();
}