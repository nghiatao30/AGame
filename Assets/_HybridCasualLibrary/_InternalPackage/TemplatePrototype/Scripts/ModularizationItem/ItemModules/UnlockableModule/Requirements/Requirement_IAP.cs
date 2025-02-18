using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames.Monetization;

[System.Serializable]
public class Requirement_IAP : Requirement
{
    public string bundleID => string.Empty;

    public override bool IsMeetRequirement()
    {
        return false;
    }

    public override void ExecuteRequirement()
    {
        // Do nothing
    }
}