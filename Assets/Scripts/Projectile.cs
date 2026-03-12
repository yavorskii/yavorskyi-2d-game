using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Flight Visuals")]
    [SerializeField] private float launchOffset = 0.45f;
    [SerializeField] private bool rotateAlongVelocity = true;
    [SerializeField] private Sprite archerProjectileSprite;
    [SerializeField] private Sprite mageProjectileSprite;
    [SerializeField] private Sprite freezerProjectileSprite;
    [SerializeField] private Sprite cannonProjectileSprite;
    [SerializeField] private Vector3 archerScale = new(0.28f, 0.28f, 1f);
    [SerializeField] private Vector3 mageScale = new(0.22f, 0.22f, 1f);
    [SerializeField] private Vector3 freezerScale = new(0.24f, 0.24f, 1f);
    [SerializeField] private Vector3 cannonScale = new(0.25f, 0.25f, 1f);

    private EnemyMover target;
    private TowerData towerData;
    private Vector3 fallbackTargetPosition;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Launch(Vector3 startPosition, EnemyMover targetEnemy, TowerData data)
    {
        Vector3 destination = targetEnemy != null ? targetEnemy.transform.position : startPosition;
        Vector3 direction = (destination - startPosition).normalized;
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = Vector3.right;
        }

        transform.position = startPosition + direction * launchOffset;
        ApplyRotation(direction);

        target = targetEnemy;
        towerData = data;
        fallbackTargetPosition = targetEnemy != null ? targetEnemy.transform.position : startPosition;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 25;
        }

        ApplyVisualByTowerType();
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
        Vector3 direction = destination - transform.position;
        if (direction.sqrMagnitude > 0.0001f)
        {
            ApplyRotation(direction.normalized);
        }

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

    private void ApplyRotation(Vector3 direction)
    {
        if (!rotateAlongVelocity)
        {
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ApplyVisualByTowerType()
    {
        if (spriteRenderer == null || towerData == null)
        {
            return;
        }

        switch (towerData.towerType)
        {
            case TowerType.Archer:
                if (archerProjectileSprite != null)
                {
                    spriteRenderer.sprite = archerProjectileSprite;
                }

                transform.localScale = archerScale;
                break;

            case TowerType.Mage:
                if (mageProjectileSprite != null)
                {
                    spriteRenderer.sprite = mageProjectileSprite;
                }

                transform.localScale = mageScale;
                break;

            case TowerType.Freezer:
                if (freezerProjectileSprite != null)
                {
                    spriteRenderer.sprite = freezerProjectileSprite;
                }

                transform.localScale = freezerScale;
                break;

            case TowerType.Cannon:
                if (cannonProjectileSprite != null)
                {
                    spriteRenderer.sprite = cannonProjectileSprite;
                }

                transform.localScale = cannonScale;
                break;
        }
    }
}
