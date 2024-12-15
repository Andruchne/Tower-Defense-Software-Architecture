
/// <summary>
/// This struct is used in Spawner
/// Used to store the enemies, which are supposed to be spawned in the wave
/// </summary>

[System.Serializable]
public struct EnemyGroup
{
    public EnemyType enemy;
    public int count;
}
