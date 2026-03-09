using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private EnemyMover enemyPrefab;
    [SerializeField] private int initialSize = 64;

    private readonly Queue<EnemyMover> pool = new();

    private void Awake()
    {
        Warmup();
    }

    private void Warmup()
    {
        if (enemyPrefab == null)
        {
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            EnemyMover enemy = Instantiate(enemyPrefab, transform);
            enemy.gameObject.SetActive(false);
            pool.Enqueue(enemy);
        }
    }

    public EnemyMover GetEnemy(Vector3 position, Quaternion rotation)
    {
        EnemyMover enemy;

        if (pool.Count > 0)
        {
            enemy = pool.Dequeue();
        }
        else
        {
            enemy = Instantiate(enemyPrefab, transform);
        }

        enemy.transform.SetPositionAndRotation(position, rotation);
        enemy.gameObject.SetActive(true);
        return enemy;
    }

    public void ReleaseEnemy(EnemyMover enemy)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.gameObject.SetActive(false);
        enemy.transform.SetParent(transform);
        pool.Enqueue(enemy);
    }
}
