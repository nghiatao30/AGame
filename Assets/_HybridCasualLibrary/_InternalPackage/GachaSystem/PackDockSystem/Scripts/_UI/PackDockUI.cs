using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackDockUI : MonoBehaviour
{
    [SerializeField] protected PackDockSlotUI slotPrefab;
    [SerializeField] protected Transform slotContainer;

    protected virtual void Start()
    {
        for (var i = 0; i < PackDockManager.Instance.GachaPackDockSO.data.gachaPackDockSlots.Count; i++)
        {
            var slotUI = Instantiate(slotPrefab, slotContainer);
            slotUI.Initialize(i);
        }
    }
}
