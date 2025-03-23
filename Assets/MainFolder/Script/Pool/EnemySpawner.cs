using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : ObjectPool<Enemy> 
{   
    [SerializeField] private Enemy enemyPrefab;

    private void Awake()
    {
        prefabObject = enemyPrefab;
    }

    void Start()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        poolingQueue = new Queue<Enemy>();
        for (int i = 0; i < initialNum; i++)
        {
            Enemy enemy = InstantiateMethod();
            //enemy.spawner = this;
            enemy.transform.SetParent(this.transform);
            PullIn(enemy);
        }
    }

    public override Enemy PullOut()
    {   
        if (poolingQueue.Count <= 0)
            RefillPool();

        Enemy enemy = poolingQueue.Dequeue();

        float randomPositionXSpawn = Random.Range(-1, 1);

        float randomPositionZSpawn = Random.Range(-1, 1);

        Vector3 randomPosSpawnOffset = new Vector3(randomPositionXSpawn, 0 , randomPositionZSpawn);

        Vector3 randomPosSpawn = transform.position + randomPosSpawnOffset;

        enemy.gameObject.transform.position = randomPosSpawn;

        enemy.OnSpawned();

        enemy.onReturnedToPool = (e) =>
        {
            PullIn(e);
        };

        return enemy;
    }

    public override void PullIn(Enemy item)
    {
        base.PullIn(item);

        item.gameObject.SetActive(false);
    }

    protected override Enemy InstantiateMethod()
    {
        Enemy instance = Instantiate(prefabObject, anchoredTransform);
        //instance.spawner = this;
        instance.transform.SetParent(this.transform);
        return instance;
    }

    
}
