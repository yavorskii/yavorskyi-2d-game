using UnityEngine;
using System.Collections;

public class TowerCombat : MonoBehaviour
{
    [SerializeField] private TowerData towerData;
    [Header("Shoot Animation")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private SpriteRenderer visualSprite;
    [SerializeField] private float animDuration = 0.12f;
    [SerializeField] private float animScaleMultiplier = 1.12f;
    [SerializeField] private Color shootFlashColor = new(1f, 0.92f, 0.72f, 1f);

    private float attackCooldown;
    private Vector3 initialVisualScale;
    private Color initialVisualColor = Color.white;
    private Coroutine shootAnimRoutine;

    public TowerData TowerData => towerData;

    private void Awake()
    {
        if (visualRoot == null)
        {
            visualRoot = transform;
        }

        initialVisualScale = visualRoot.localScale;

        if (visualSprite == null)
        {
            visualSprite = GetComponentInChildren<SpriteRenderer>();
        }

        if (visualSprite != null)
        {
            initialVisualColor = visualSprite.color;
        }
    }

    private void Update()
    {
        if (towerData == null)
        {
            return;
        }

        attackCooldown -= Time.deltaTime;
        if (attackCooldown > 0f)
        {
            return;
        }

        EnemyMover target = GetBestTarget();
        if (target == null || target.Health == null)
        {
            return;
        }

        PlayShootAnimation();

        ProjectilePool pool = ProjectilePool.Instance;
        if (pool == null)
        {
            ApplyInstantFallback(target);
            float fallbackRate = Mathf.Max(0.01f, towerData.attacksPerSecond);
            attackCooldown = 1f / fallbackRate;
            return;
        }

        Projectile projectile = pool.GetProjectile();
        projectile.Launch(transform.position, target, towerData);

        float attacksPerSecond = Mathf.Max(0.01f, towerData.attacksPerSecond);
        attackCooldown = 1f / attacksPerSecond;
    }

    private EnemyMover GetBestTarget()
    {
        EnemyMover bestTarget = null;
        float bestProgress = float.MinValue;
        Vector3 towerPosition = transform.position;
        float range = towerData.range;

        foreach (EnemyMover enemy in EnemyMover.ActiveEnemies)
        {
            if (enemy == null || enemy.Health == null)
            {
                continue;
            }

            float distance = Vector3.Distance(towerPosition, enemy.transform.position);
            if (distance > range)
            {
                continue;
            }

            if (enemy.PathProgress > bestProgress)
            {
                bestProgress = enemy.PathProgress;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    private void ApplyInstantFallback(EnemyMover target)
    {
        if (towerData.towerType == TowerType.Mage)
        {
            float splashRadius = Mathf.Max(0f, towerData.splashRadius);
            foreach (EnemyMover enemy in EnemyMover.ActiveEnemies)
            {
                if (enemy == null || enemy.Health == null)
                {
                    continue;
                }

                float distanceToImpact = Vector3.Distance(target.transform.position, enemy.transform.position);
                if (distanceToImpact <= splashRadius)
                {
                    enemy.Health.TakeDamage(towerData.damage);
                }
            }

            return;
        }

        if (towerData.towerType == TowerType.Freezer)
        {
            target.Health.TakeDamage(towerData.damage);
            target.ApplySlow(towerData.slowMultiplier, towerData.slowDuration);
            return;
        }

        target.Health.TakeDamage(towerData.damage);
    }

    private void OnDrawGizmosSelected()
    {
        if (towerData == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, towerData.range);
    }

    private void PlayShootAnimation()
    {
        if (towerData == null || towerData.towerType != TowerType.Archer)
        {
            return;
        }

        if (shootAnimRoutine != null)
        {
            StopCoroutine(shootAnimRoutine);
            shootAnimRoutine = null;
        }

        shootAnimRoutine = StartCoroutine(ShootAnimRoutine());
    }

    private IEnumerator ShootAnimRoutine()
    {
        if (visualRoot == null)
        {
            yield break;
        }

        float duration = Mathf.Max(0.03f, animDuration);
        float half = duration * 0.5f;
        Vector3 punchScale = initialVisualScale * Mathf.Max(1f, animScaleMultiplier);

        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / half);
            visualRoot.localScale = Vector3.Lerp(initialVisualScale, punchScale, k);

            if (visualSprite != null)
            {
                visualSprite.color = Color.Lerp(initialVisualColor, shootFlashColor, k);
            }

            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / half);
            visualRoot.localScale = Vector3.Lerp(punchScale, initialVisualScale, k);

            if (visualSprite != null)
            {
                visualSprite.color = Color.Lerp(shootFlashColor, initialVisualColor, k);
            }

            yield return null;
        }

        visualRoot.localScale = initialVisualScale;
        if (visualSprite != null)
        {
            visualSprite.color = initialVisualColor;
        }

        shootAnimRoutine = null;
    }
}
