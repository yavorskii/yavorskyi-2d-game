using TMPro;
using UnityEngine;

public class HudController : MonoBehaviour
{
    [SerializeField] private GameEconomy economy;
    [SerializeField] private BaseHealth baseHealth;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text baseHpText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text attackBudgetText;

    private void OnEnable()
    {
        if (economy != null)
        {
            economy.GoldChanged += UpdateGoldText;
        }

        if (baseHealth != null)
        {
            baseHealth.HealthChanged += UpdateBaseHpText;
        }

        if (enemySpawner != null)
        {
            enemySpawner.RoundChanged += UpdateRoundText;
            enemySpawner.AttackBudgetChanged += UpdateAttackBudgetText;
        }

        RefreshAll();
    }

    private void OnDisable()
    {
        if (economy != null)
        {
            economy.GoldChanged -= UpdateGoldText;
        }

        if (baseHealth != null)
        {
            baseHealth.HealthChanged -= UpdateBaseHpText;
        }

        if (enemySpawner != null)
        {
            enemySpawner.RoundChanged -= UpdateRoundText;
            enemySpawner.AttackBudgetChanged -= UpdateAttackBudgetText;
        }
    }

    public void RefreshAll()
    {
        if (economy != null)
        {
            UpdateGoldText(economy.CurrentGold);
        }

        if (baseHealth != null)
        {
            UpdateBaseHpText(baseHealth.CurrentHealth, baseHealth.MaxHealth);
        }

        if (enemySpawner != null)
        {
            int currentRound = Mathf.Max(1, enemySpawner.CurrentRound);
            UpdateRoundText(currentRound, enemySpawner.TotalRounds);
            UpdateAttackBudgetText(enemySpawner.CurrentAttackBudget);
        }
    }

    private void UpdateGoldText(int gold)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {gold}";
        }
    }

    private void UpdateBaseHpText(int currentHp, int maxHp)
    {
        if (baseHpText != null)
        {
            baseHpText.text = $"Base HP: {currentHp}/{maxHp}";
        }
    }

    private void UpdateRoundText(int currentRound, int totalRounds)
    {
        if (roundText != null)
        {
            roundText.text = $"Round: {currentRound}/{totalRounds}";
        }
    }

    private void UpdateAttackBudgetText(int budget)
    {
        if (attackBudgetText != null)
        {
            attackBudgetText.text = $"Attack Budget: {budget}";
        }
    }
}
