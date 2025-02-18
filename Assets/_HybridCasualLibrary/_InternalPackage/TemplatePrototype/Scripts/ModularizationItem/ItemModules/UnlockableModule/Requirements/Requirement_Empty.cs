using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Requirement_Empty : Requirement
{
    public override bool IsMeetRequirement()
    {
        return true;
    }

    public override void ExecuteRequirement()
    {
        // Do nothing
    }
}