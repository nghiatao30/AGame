using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHapticNotifier
{
    event Action onBeginHaptic;
    event Action onStopHaptic;
}
