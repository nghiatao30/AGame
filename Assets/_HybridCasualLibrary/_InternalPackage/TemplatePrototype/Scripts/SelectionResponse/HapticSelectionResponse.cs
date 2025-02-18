using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticSelectionResponse : MonoBehaviour, ISelectionResponse
{
    [SerializeField]
    protected HapticTypes hapticType = HapticTypes.Selection;

    public void Select(bool isForceSelect = false)
    {
        if (isForceSelect)
            return;
        // Play haptics here
        HapticManager.Instance.PlayFlashHaptic(hapticType);
    }

    public void Deselect()
    {
        // Do nothing
    }
}