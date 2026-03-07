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

        if (towerData.towerType == TowerType.Mage)
        {
            ApplyMageAttack(target.transform.position);
        }
        else if (towerData.towerType == TowerType.Freezer)
        {
            ApplyFreezerAttack(target);
        }
        else
        {
            target.Health.TakeDamage(towerData.damage);
        }

        float attacksPerSecond = Mathf.Max(0.01f, towerData.attacksPerSecond);
        attackCooldown = 1f / attacksPerSecond;
    }

    private EnemyMover GetBestTarget()
    {
        EnemyMover bestTarget = null;
        float bestProgress = float.MinValue;
        Vector3 towerPosition = transform.position;
        float range = towerData.range;
        EnemyMover[] enemies = FindObjectsByType<EnemyMover>(FindObjectsSortMode.None);

        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyMover enemy = enemies[i];
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

    private void ApplyMageAttack(Vector3 impactPosition)
    {
        float splashRadius = Mathf.Max(0f, towerData.splashRadius);
        EnemyMover[] enemies = FindObjectsByType<EnemyMover>(FindObjectsSortMode.None);

        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyMover enemy = enemies[i];
            if (enemy == null || enemy.Health == null)
            {
                continue;
            }

            float distanceToImpact = Vector3.Distance(impactPosition, enemy.transform.position);
            if (distanceToImpact <= splashRadius)
            {
                enemy.Health.TakeDamage(towerData.damage);
            }
        }
    }

    private void ApplyFreezerAttack(EnemyMover target)
    {
        target.Health.TakeDamage(towerData.damage);
        target.ApplySlow(towerData.slowMultiplier, towerData.slowDuration);
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
