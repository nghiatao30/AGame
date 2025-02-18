using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPunishment
{
    void TakePunishment();
}
public abstract class PunishmentModule : ItemModule, IPunishment
{
    public abstract void TakePunishment();
}