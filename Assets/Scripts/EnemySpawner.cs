using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum RoundPhase
{
    Menu,
    Preparation,
    Battle,
    RoundEnd,
    GameOver
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyMover enemyPrefab;
    [SerializeField] private EnemyPool enemyPool;
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
    [SerializeField] private float preparationDuration = 6.0f;
    [SerializeField] private float timeBetweenRounds = 2.0f;

    public event Action<int, int> RoundChanged;
    public event Action<int> AttackBudgetChanged;
    public event Action<RoundPhase> PhaseChanged;
    public event Action<bool> GameFinished;
    public int CurrentRound { get; private set; }
    public int TotalRounds => totalRounds;
    public int CurrentAttackBudget { get; private set; }
    public bool IsSpawning { get; private set; }
    public RoundPhase CurrentPhase { get; private set; }
    public bool IsGameFinished { get; private set; }
    public bool IsGameStarted { get; private set; }

    private void Start()
    {
        SetPhase(RoundPhase.Menu);
    }

    public void StartGame()
    {
        if (IsGameStarted)
        {
            return;
        }

        IsGameStarted = true;
        StartCoroutine(RunRounds());
    }

    [ContextMenu("Apply Stress Test Preset")]
    public void ApplyStressTestPreset()
    {
        totalRounds = 10;
        startAttackBudget = 500;
        attackBudgetGrowthPerRound = 40;
        maxEnemiesPerWave = 50;
        minSpawnInterval = 0.8f;
        maxSpawnInterval = 1.0f;
        preparationDuration = 10f;
        timeBetweenRounds = 2f;
    }

    private IEnumerator RunRounds()
    {
        if (enemyPrefab == null || enemyPool == null || path == null || baseHealth == null || enemyTypes.Count == 0)
        {
            Debug.LogError("EnemySpawner: Assign enemyPrefab, enemyPool, enemyTypes, path and baseHealth in Inspector.");
            yield break;
        }

        for (int round = 1; round <= totalRounds; round++)
        {
            if (baseHealth.CurrentHealth <= 0 || IsGameFinished)
            {
                FinishGame(false);
                yield break;
            }

            CurrentRound = round;
            RoundChanged?.Invoke(CurrentRound, totalRounds);
            CurrentAttackBudget = startAttackBudget + (round - 1) * attackBudgetGrowthPerRound;
            AttackBudgetChanged?.Invoke(CurrentAttackBudget);

            SetPhase(RoundPhase.Preparation);
            yield return WaitWithDeathCheck(preparationDuration);

            if (baseHealth.CurrentHealth <= 0 || IsGameFinished)
            {
                FinishGame(false);
                yield break;
            }

            SetPhase(RoundPhase.Battle);
            List<EnemyData> wave = GenerateWave(CurrentAttackBudget);
            yield return SpawnWave(wave);

            while (EnemyMover.ActiveEnemies.Count > 0 && baseHealth.CurrentHealth > 0 && !IsGameFinished)
            {
                yield return null;
            }

            if (baseHealth.CurrentHealth <= 0 || IsGameFinished)
            {
                FinishGame(false);
                yield break;
            }

            SetPhase(RoundPhase.RoundEnd);
            if (round < totalRounds)
            {
                yield return WaitWithDeathCheck(timeBetweenRounds);
            }
        }

        if (baseHealth.CurrentHealth > 0 && !IsGameFinished)
        {
            Debug.Log("Victory: All rounds survived.");
            FinishGame(true);
        }
    }

    private IEnumerator WaitWithDeathCheck(float duration)
    {
        float timer = Mathf.Max(0f, duration);
        while (timer > 0f)
        {
            if (baseHealth != null && baseHealth.CurrentHealth <= 0)
            {
                yield break;
            }

            timer -= Time.deltaTime;
            yield return null;
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
            if (baseHealth == null || baseHealth.CurrentHealth <= 0 || IsGameFinished)
            {
                break;
            }

            EnemyData enemyData = wave[i];
            EnemyMover enemy = enemyPool.GetEnemy(transform.position, Quaternion.identity);
            enemy.Setup(path, baseHealth, enemyData, economy, enemyPool);
            float delay = UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(delay);
        }

        IsSpawning = false;
    }

    private void SetPhase(RoundPhase phase)
    {
        CurrentPhase = phase;
        PhaseChanged?.Invoke(CurrentPhase);
    }

    private void FinishGame(bool defenderWon)
    {
        if (IsGameFinished)
        {
            return;
        }

        IsGameFinished = true;
        SetPhase(RoundPhase.GameOver);
        GameFinished?.Invoke(defenderWon);
    }
}
