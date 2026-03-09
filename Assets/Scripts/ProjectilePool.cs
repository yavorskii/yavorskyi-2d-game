using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance { get; private set; }

    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int initialSize = 128;

    private readonly Queue<Projectile> pool = new();

    private void Awake()
    {
        Instance = this;
        Warmup();
    }

    private void Warmup()
    {
        if (projectilePrefab == null)
        {
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            Projectile projectile = Instantiate(projectilePrefab, transform);
            projectile.gameObject.SetActive(false);
            pool.Enqueue(projectile);
        }
    }

    public Projectile GetProjectile()
    {
        Projectile projectile;

        if (pool.Count > 0)
        {
            projectile = pool.Dequeue();
        }
        else
        {
            projectile = Instantiate(projectilePrefab, transform);
        }

        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void ReleaseProjectile(Projectile projectile)
    {
        if (projectile == null)
        {
            return;
        }

        projectile.gameObject.SetActive(false);
        projectile.transform.SetParent(transform);
        pool.Enqueue(projectile);
    }
}
