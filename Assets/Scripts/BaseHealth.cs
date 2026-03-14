using UnityEngine;
using System;
using System.Collections;

public class BaseHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    [Header("Hit Animation")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private SpriteRenderer visualSprite;
    [SerializeField] private float hitAnimDuration = 0.16f;
    [SerializeField] private float hitScaleMultiplier = 0.9f;
    [SerializeField] private Color hitFlashColor = new(1f, 0.55f, 0.55f, 1f);

    public event Action<int, int> HealthChanged;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    private int currentHealth;
    private Vector3 initialScale = Vector3.one;
    private Color initialColor = Color.white;
    private Coroutine hitAnimRoutine;

    private void Awake()
    {
        if (visualRoot == null)
        {
            visualRoot = transform;
        }

        if (visualSprite == null)
        {
            visualSprite = GetComponentInChildren<SpriteRenderer>();
        }

        initialScale = visualRoot.localScale;
        if (visualSprite != null)
        {
            initialColor = visualSprite.color;
        }

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
        PlayHitFeedback();
        Debug.Log($"Base HP: {currentHealth}/{maxHealth}");
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth == 0)
        {
            Debug.Log("Game Over: Base destroyed.");
        }
    }

    private void PlayHitFeedback()
    {
        if (GameAudio.Instance != null)
        {
            GameAudio.Instance.PlayBaseHit();
        }

        if (hitAnimRoutine != null)
        {
            StopCoroutine(hitAnimRoutine);
        }

        hitAnimRoutine = StartCoroutine(HitAnimRoutine());
    }

    private IEnumerator HitAnimRoutine()
    {
        if (visualRoot == null)
        {
            yield break;
        }

        float duration = Mathf.Max(0.03f, hitAnimDuration);
        float half = duration * 0.5f;
        Vector3 squashed = initialScale * Mathf.Clamp(hitScaleMultiplier, 0.6f, 1f);

        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / half);
            visualRoot.localScale = Vector3.Lerp(initialScale, squashed, k);
            if (visualSprite != null)
            {
                visualSprite.color = Color.Lerp(initialColor, hitFlashColor, k);
            }

            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / half);
            visualRoot.localScale = Vector3.Lerp(squashed, initialScale, k);
            if (visualSprite != null)
            {
                visualSprite.color = Color.Lerp(hitFlashColor, initialColor, k);
            }

            yield return null;
        }

        visualRoot.localScale = initialScale;
        if (visualSprite != null)
        {
            visualSprite.color = initialColor;
        }

        hitAnimRoutine = null;
    }
}
