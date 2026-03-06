using UnityEngine;

public class TowerArcher : MonoBehaviour
{
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float attacksPerSecond = 1.0f;
    [SerializeField] private int damagePerHit = 1;

    private float attackCooldown;

    private void Update()
    {
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

        target.Health.TakeDamage(damagePerHit);
        attackCooldown = 1f / Mathf.Max(0.01f, attacksPerSecond);
    }

    private EnemyMover GetBestTarget()
    {
        EnemyMover bestTarget = null;
        float bestProgress = float.MinValue;
        Vector3 towerPosition = transform.position;

        foreach (EnemyMover enemy in EnemyMover.ActiveEnemies)
        {
            if (enemy == null)
            {
                continue;
            }

            float distance = Vector3.Distance(towerPosition, enemy.transform.position);
            if (distance > attackRange)
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
