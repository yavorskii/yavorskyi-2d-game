using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int rewardGold = 1;

    private int currentHealth;
    private GameEconomy economy;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetupFromData(EnemyData data, GameEconomy gameEconomy)
    {
        if (data != null)
        {
            maxHealth = Mathf.Max(1, data.maxHealth);
            rewardGold = Mathf.Max(0, data.rewardGold);
        }

        currentHealth = maxHealth;
        economy = gameEconomy;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (currentHealth == 0)
        {
            if (economy != null && rewardGold > 0)
            {
                economy.AddGold(rewardGold);
            }

            Destroy(gameObject);
        }
    }
}
