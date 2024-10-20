using System.Collections;
using System.Collections.Generic;
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
    public float[] damage;
    public float[] range;
    public float[] attackCooldown;
}
