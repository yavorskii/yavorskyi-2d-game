using UnityEngine;
using System.Collections.Generic;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private int damageToBase = 1;

    public static readonly List<EnemyMover> ActiveEnemies = new();

    public float PathProgress => targetWaypointIndex;
    public EnemyHealth Health { get; private set; }

    private WaypointPath path;
    private BaseHealth baseHealth;
    private int targetWaypointIndex;

    private void Awake()
    {
        Health = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        ActiveEnemies.Add(this);
    }

    private void OnDisable()
    {
        ActiveEnemies.Remove(this);
    }

    public void Setup(WaypointPath waypointPath, BaseHealth targetBase)
    {
        path = waypointPath;
        baseHealth = targetBase;
        targetWaypointIndex = 0;

        if (path == null || path.Waypoints.Count == 0)
        {
            Debug.LogError("EnemyMover: Path is missing or empty.");
            enabled = false;
            return;
        }

        transform.position = path.Waypoints[0].position;
    }

    private void Update()
    {
        if (path == null || targetWaypointIndex >= path.Waypoints.Count)
        {
            return;
        }

        Transform target = path.Waypoints[targetWaypointIndex];
        if (target == null)
        {
            targetWaypointIndex++;
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.02f)
        {
            targetWaypointIndex++;
            if (targetWaypointIndex >= path.Waypoints.Count)
            {
                if (baseHealth != null)
                {
                    baseHealth.TakeDamage(damageToBase);
                }

                Destroy(gameObject);
            }
        }
    }
}
