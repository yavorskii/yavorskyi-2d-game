using UnityEngine;

public class TowerCombat : MonoBehaviour
{
    [SerializeField] private TowerData towerData;

    private float attackCooldown;

    public TowerData TowerData => towerData;

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
}
