using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SpawnManager: MonoBehaviour 
{
    public List<ObjectPool<Enemy>> spawnerList; 

    void  FixedUpdate()
    {
        foreach(var spawner in spawnerList)
        {
            spawner.PullOut();
        }
    }
}
