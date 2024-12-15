using UnityEngine;

/// <summary>
/// Used to create new towers.
/// Values are parsed when hovering over tower slot and when creating a tower.
/// </summary>

[CreateAssetMenu(fileName = "New Tower", menuName = "ScriptableObjects/Tower")]
public class TowerInfo : ScriptableObject
{
    public string towerTypeName;
    public AttackType attackType;
    public GameObject[] towerModel;
    public Projectile.ProjectileMoveType projectileMoveType;
    public float[] power;
    public float[] range;
    public float[] attackCooldown;
    public float[] effectRadius;
    public int[] cost;
}
