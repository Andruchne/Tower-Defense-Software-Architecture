using UnityEngine;

/// <summary>
/// Object to store waves, used in a level.
/// An object of this type, needs to be parsed into the WaveManager.
/// </summary>

[CreateAssetMenu(fileName = "New Level", menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    public Wave[] waves;
    public float[] intervalPerSpawn;
}
