using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int rewardGold = 1;
    [Header("Hit Flash")]
    [SerializeField] private Color hitFlashColor = new(1f, 0.72f, 0.72f, 1f);
    [SerializeField] private float hitFlashDuration = 0.09f;

    private int currentHealth;
    private GameEconomy economy;
    private EnemyMover owner;
    private SpriteRenderer spriteRenderer;
    private Color baseColor = Color.white;
    private Coroutine hitFlashRoutine;

    private void Awake()
    {
        currentHealth = maxHealth;
        owner = GetComponent<EnemyMover>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            baseColor = spriteRenderer.color;
        }
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
        PlayHitFlash();

        if (currentHealth == 0)
        {
            if (economy != null && rewardGold > 0)
            {
                economy.AddGold(rewardGold);
            }

            if (owner != null)
            {
                owner.Despawn();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void PlayHitFlash()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (hitFlashRoutine != null)
        {
            StopCoroutine(hitFlashRoutine);
        }

        hitFlashRoutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        float duration = Mathf.Max(0.02f, hitFlashDuration);
        float half = duration * 0.5f;
        float t = 0f;

        while (t < half)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / half);
            spriteRenderer.color = Color.Lerp(baseColor, hitFlashColor, k);
            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / half);
            spriteRenderer.color = Color.Lerp(hitFlashColor, baseColor, k);
            yield return null;
        }

        spriteRenderer.color = baseColor;
        hitFlashRoutine = null;
    }
}
