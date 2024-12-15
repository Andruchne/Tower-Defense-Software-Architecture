using UnityEngine;

/// <summary>
/// Used to create new enemies.
/// This is used to instantiate the monsters, inside the spawner.
/// </summary>

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/Enemy")]
public class EnemyType : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;
}
