using System.Collections;
using System.Collections.Generic;
using GachaSystem.Core;
using HyrphusQ.Events;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestPackDockSystem : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] GachaPackDockSO gachaPackDockSO;
    [SerializeField] GachaPack addedGachaPack;
    [Button]
    void AddGachaPack()
    {
        PackDockManager.Instance.TryToAddPack(addedGachaPack);
    }

    [Button]
    void ShowReplaceUI()
    {
        if (PackDockManager.Instance.IsFull)
        {
            GameEventHandler.Invoke(GachaPackDockEventCode.OnShowReplaceUI, addedGachaPack);
        }
    }
#endif
}
