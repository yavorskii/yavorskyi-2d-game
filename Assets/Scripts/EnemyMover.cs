using UnityEngine;
using System.Collections.Generic;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private int damageToBase = 1;

    public static readonly List<EnemyMover> ActiveEnemies = new();

    public float PathProgress => targetWaypointIndex;
    public EnemyHealth Health { get; private set; }
    public bool ImmuneToSlow { get; private set; }

    private WaypointPath path;
    private BaseHealth baseHealth;
    private int targetWaypointIndex;
    private float baseMoveSpeed;
    private float currentMoveSpeed;
    private float slowTimer;

    private void Awake()
    {
        Health = GetComponent<EnemyHealth>();
        baseMoveSpeed = moveSpeed;
        currentMoveSpeed = baseMoveSpeed;
    }

    private void OnEnable()
    {
        ActiveEnemies.Add(this);
    }

    private void OnDisable()
    {
        ActiveEnemies.Remove(this);
    }

    public void Setup(WaypointPath waypointPath, BaseHealth targetBase, EnemyData data, GameEconomy economy)
    {
        path = waypointPath;
        baseHealth = targetBase;
        targetWaypointIndex = 0;
        baseMoveSpeed = data != null ? Mathf.Max(0.1f, data.moveSpeed) : moveSpeed;
        currentMoveSpeed = baseMoveSpeed;
        slowTimer = 0f;
        ImmuneToSlow = data != null && data.immuneToSlow;

        if (path == null || path.Waypoints.Count == 0)
        {
            Debug.LogError("EnemyMover: Path is missing or empty.");
            enabled = false;
            return;
        }

        if (Health != null)
        {
            Health.SetupFromData(data, economy);
        }

        transform.position = path.Waypoints[0].position;
    }

    public void ApplySlow(float slowMultiplier, float duration)
    {
        if (ImmuneToSlow)
        {
            return;
        }

        float clampedMultiplier = Mathf.Clamp(slowMultiplier, 0.1f, 1f);
        currentMoveSpeed = baseMoveSpeed * clampedMultiplier;
        slowTimer = Mathf.Max(slowTimer, Mathf.Max(0f, duration));
    }

    private void Update()
    {
        if (path == null || targetWaypointIndex >= path.Waypoints.Count)
        {
            return;
        }

        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                currentMoveSpeed = baseMoveSpeed;
                slowTimer = 0f;
            }
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
            currentMoveSpeed * Time.deltaTime);

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
