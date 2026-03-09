using UnityEngine;

public class Projectile : MonoBehaviour
{
    private EnemyMover target;
    private TowerData towerData;
    private Vector3 fallbackTargetPosition;

    public void Launch(Vector3 startPosition, EnemyMover targetEnemy, TowerData data)
    {
        transform.position = startPosition;
        target = targetEnemy;
        towerData = data;
        fallbackTargetPosition = targetEnemy != null ? targetEnemy.transform.position : startPosition;
    }

    private void Update()
    {
        if (towerData == null)
        {
            Release();
            return;
        }

        Vector3 destination = target != null ? target.transform.position : fallbackTargetPosition;
        float speed = Mathf.Max(0.1f, towerData.projectileSpeed);
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) <= 0.03f)
        {
            ApplyImpact(destination);
            Release();
        }
    }

    private void ApplyImpact(Vector3 impactPosition)
    {
        if (towerData == null)
        {
            return;
        }

        if (towerData.towerType == TowerType.Mage)
        {
            ApplyMageImpact(impactPosition);
            return;
        }

        if (towerData.towerType == TowerType.Freezer)
        {
            if (target != null && target.Health != null)
            {
                target.Health.TakeDamage(towerData.damage);
                target.ApplySlow(towerData.slowMultiplier, towerData.slowDuration);
            }

            return;
        }

        if (target != null && target.Health != null)
        {
            target.Health.TakeDamage(towerData.damage);
        }
    }

    private void ApplyMageImpact(Vector3 impactPosition)
    {
        float radius = Mathf.Max(0f, towerData.splashRadius);

        foreach (EnemyMover enemy in EnemyMover.ActiveEnemies)
        {
            if (enemy == null || enemy.Health == null)
            {
                continue;
            }

            float distance = Vector3.Distance(impactPosition, enemy.transform.position);
            if (distance <= radius)
            {
                enemy.Health.TakeDamage(towerData.damage);
            }
        }
    }

    private void Release()
    {
        target = null;
        towerData = null;
        if (ProjectilePool.Instance != null)
        {
            ProjectilePool.Instance.ReleaseProjectile(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
