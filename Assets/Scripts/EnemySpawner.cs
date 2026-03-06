using System.Collections;
using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyMover enemyPrefab;
    [SerializeField] private WaypointPath path;
    [SerializeField] private BaseHealth baseHealth;
    [SerializeField] private int totalRounds = 10;
    [SerializeField] private int enemiesFirstRound = 10;
    [SerializeField] private int enemiesGrowthPerRound = 2;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private float timeBetweenRounds = 2.0f;

    public event Action<int, int> RoundChanged;
    public int CurrentRound { get; private set; }
    public int TotalRounds => totalRounds;
    public bool IsSpawning { get; private set; }

    private void Start()
    {
        StartCoroutine(RunRounds());
    }

    private IEnumerator RunRounds()
    {
        if (enemyPrefab == null || path == null || baseHealth == null)
        {
            Debug.LogError("EnemySpawner: Assign enemyPrefab, path and baseHealth in Inspector.");
            yield break;
        }

        for (int round = 1; round <= totalRounds; round++)
        {
            if (baseHealth.CurrentHealth <= 0)
            {
                yield break;
            }

            CurrentRound = round;
            RoundChanged?.Invoke(CurrentRound, totalRounds);

            int enemiesThisRound = enemiesFirstRound + (round - 1) * enemiesGrowthPerRound;
            yield return SpawnWave(enemiesThisRound);

            while (EnemyMover.ActiveEnemies.Count > 0 && baseHealth.CurrentHealth > 0)
            {
                yield return null;
            }

            if (round < totalRounds && baseHealth.CurrentHealth > 0)
            {
                yield return new WaitForSeconds(timeBetweenRounds);
            }
        }

        if (baseHealth.CurrentHealth > 0)
        {
            Debug.Log("Victory: All rounds survived.");
        }
    }

    private IEnumerator SpawnWave(int enemiesCount)
    {
        IsSpawning = true;
        for (int i = 0; i < enemiesCount; i++)
        {
            EnemyMover enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            enemy.Setup(path, baseHealth);
            yield return new WaitForSeconds(spawnInterval);
        }

        IsSpawning = false;
    }
}
