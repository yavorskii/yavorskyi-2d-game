using UnityEngine;
using System;

public class BaseHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;

    public event Action<int, int> HealthChanged;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"Base HP: {currentHealth}/{maxHealth}");
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth == 0)
        {
            Debug.Log("Game Over: Base destroyed.");
        }
    }
}
