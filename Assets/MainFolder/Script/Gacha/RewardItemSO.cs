using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RewardItemSO", menuName = "RewardItemSO/RewardItem", order = 0)]

public class RewardItemSO : ScriptableObject
{
   public String Name;
   public GameObject Prefab;
   
}
