using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteration : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            if(Input.GetKeyDown(KeyCode.E))
            {                
               GachaManager.Instance.Gacha(transform.position);     
            }
        } 
    }

}
