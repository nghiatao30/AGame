using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum TransformConstraint
{
    None = 0,
    Position = 1 << 0,
    Rotation = 1 << 1,
    Scale = 1 << 2,
    All = 7
}
