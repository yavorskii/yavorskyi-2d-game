using UnityEngine;

public enum TowerType
{
    Archer,
    Mage,
    Freezer,
    Cannon
}

[CreateAssetMenu(menuName = "TD/Tower Data", fileName = "TowerData")]
public class TowerData : ScriptableObject
{
    public TowerType towerType;
    public int cost = 100;
    public float range = 2.5f;
    public float attacksPerSecond = 1f;
    public int damage = 1;
    public float splashRadius = 0f;
    public float slowMultiplier = 0.7f;
    public float slowDuration = 1.5f;
}
