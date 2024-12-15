using UnityEngine;

/// <summary>
/// Used to create new waves of enemies.
/// These values are used in the WaveManager.
/// </summary>

[CreateAssetMenu(fileName = "New Wave", menuName = "ScriptableObjects/Wave")]
public class Wave : ScriptableObject
{
    public EnemyGroup[] enemyGroups;
}
