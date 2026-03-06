using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints = new();

    public IReadOnlyList<Transform> Waypoints => waypoints;

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count; i++)
        {
            Transform point = waypoints[i];
            if (point == null)
            {
                continue;
            }

            Gizmos.DrawSphere(point.position, 0.12f);
            if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(point.position, waypoints[i + 1].position);
            }
        }
    }
}
