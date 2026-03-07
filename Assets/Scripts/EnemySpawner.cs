using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyMover enemyPrefab;
    [SerializeField] private List<EnemyData> enemyTypes = new();
    [SerializeField] private WaypointPath path;
    [SerializeField] private BaseHealth baseHealth;
    [SerializeField] private GameEconomy economy;
    [SerializeField] private int totalRounds = 10;
    [SerializeField] private int startAttackBudget = 200;
    [SerializeField] private int attackBudgetGrowthPerRound = 35;
    [SerializeField] private int maxEnemiesPerWave = 50;
    [SerializeField] private float minSpawnInterval = 0.8f;
    [SerializeField] private float maxSpawnInterval = 1.2f;
    [SerializeField] private float timeBetweenRounds = 2.0f;

    public event Action<int, int> RoundChanged;
    public event Action<int> AttackBudgetChanged;
    public int CurrentRound { get; private set; }
    public int TotalRounds => totalRounds;
    public int CurrentAttackBudget { get; private set; }
    public bool IsSpawning { get; private set; }

    private void Start()
    {
        StartCoroutine(RunRounds());
    }

    private IEnumerator RunRounds()
    {
        if (enemyPrefab == null || path == null || baseHealth == null || enemyTypes.Count == 0)
        {
            Debug.LogError("EnemySpawner: Assign enemyPrefab, enemyTypes, path and baseHealth in Inspector.");
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
            CurrentAttackBudget = startAttackBudget + (round - 1) * attackBudgetGrowthPerRound;
            AttackBudgetChanged?.Invoke(CurrentAttackBudget);

            List<EnemyData> wave = GenerateWave(CurrentAttackBudget);
            yield return SpawnWave(wave);

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

    private List<EnemyData> GenerateWave(int budget)
    {
        List<EnemyData> wave = new();
        List<EnemyData> affordable = new();

        int minCost = int.MaxValue;
        foreach (EnemyData enemy in enemyTypes)
        {
            if (enemy == null)
            {
                continue;
            }

            if (enemy.attackCost < minCost)
            {
                minCost = enemy.attackCost;
            }
        }

        if (minCost == int.MaxValue)
        {
            return wave;
        }

        int safety = 0;
        while (budget >= minCost && wave.Count < maxEnemiesPerWave && safety < 500)
        {
            safety++;
            affordable.Clear();

            for (int i = 0; i < enemyTypes.Count; i++)
            {
                EnemyData candidate = enemyTypes[i];
                if (candidate == null)
                {
                    continue;
                }

                if (candidate.attackCost <= budget)
                {
                    affordable.Add(candidate);
                }
            }

            if (affordable.Count == 0)
            {
                break;
            }

            EnemyData pick = affordable[UnityEngine.Random.Range(0, affordable.Count)];
            wave.Add(pick);
            budget -= pick.attackCost;
        }

        return wave;
    }

    private IEnumerator SpawnWave(List<EnemyData> wave)
    {
        IsSpawning = true;
        for (int i = 0; i < wave.Count; i++)
        {
            EnemyData enemyData = wave[i];
            EnemyMover enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            enemy.Setup(path, baseHealth, enemyData, economy);
            float delay = UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(delay);
        }

        IsSpawning = false;
    }
}
