using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RewardItem : MonoBehaviour
{
    public RewardItemSO rewardItemInfo;
    [SerializeField] string name;

    [SerializeField] GameObject prefab;



    void Start()
    {
        if(rewardItemInfo != null)
        {
            name = rewardItemInfo.Name;

            prefab = rewardItemInfo.Prefab;
        }
    }

}
