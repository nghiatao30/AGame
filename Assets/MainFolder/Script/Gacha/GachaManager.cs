using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class GachaManager : MonoBehaviour
{   

    public static GachaManager Instance;

    [SerializeField] GachaRate[] gachaRates;

    [SerializeField] RewardItemSO currentReward;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void Gacha(Vector3 posSpawn)
    {
        int randomNum = UnityEngine.Random.Range(1,100);

        for(int i = 0; i < gachaRates.Length; i++)
        {
            if(gachaRates[i].rate >= randomNum)
            {
                GetReward(i,posSpawn);
                return;
            }
        }
    }

    public void GetReward(int index, Vector3 posSpawn)
    {

        RewardItemSO[] rewardItems = gachaRates[index].rewardList;

        int randomIndex = UnityEngine.Random.Range(0, rewardItems.Length);

        currentReward = rewardItems[randomIndex];

        GameObject rewardPrefab = rewardItems[randomIndex].Prefab;

        float randomPositionXSpawn = UnityEngine.Random.Range(-1, 1);

        float randomPositionZSpawn = UnityEngine.Random.Range(-1, 1);

        Vector3 randomPosSpawnOffset = new Vector3(randomPositionXSpawn, 0 , randomPositionZSpawn);

        Vector3 randomPosSpawn = posSpawn + randomPosSpawnOffset;

        Instantiate(rewardPrefab, randomPosSpawn, Quaternion.identity);

    }

}
