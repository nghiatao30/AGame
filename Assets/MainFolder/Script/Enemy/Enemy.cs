using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{   
    //public EnemySpawner spawner;

    public System.Action<Enemy> onReturnedToPool;

    public void Update()
    {
        transform.Translate(Vector3.forward * 5f * Time.deltaTime);
    }

    public void OnSpawned()
    {
        // Called when pulled from pool
        gameObject.SetActive(true);
        StartCoroutine(ReturnToPoolAfterDelay(5f));
    }

    private IEnumerator ReturnToPoolAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);     
        onReturnedToPool?.Invoke(this);         
        //spawner.PullIn(this);                     
    }
}
