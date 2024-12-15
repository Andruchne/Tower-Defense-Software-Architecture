using NUnit.Framework;
using UnityEngine;

public class TestWaveSpawner
{
    // Placeholder wave info
    // As a wave scriptable object consists of three numbers (two arrays and one int),
    // we replace this object with a Vector3 instead

    // Translating this to a wave object, would mean that there are
    // 2 waves in total - 3 different enemy types to spawn - Of which 4 are supposed to be spawned each
    // Of course, using the Scriptable Object instead, will lead to more variety
    Vector3 _waveInfo = new Vector3(2, 3, 4);

    // To progress through the wave info provided
    private Vector3 _currentProgress = new Vector3();

    // State to check, that indicates if Unit Test was successful
    private bool _allEnemiesSpawned;

    [Test]
    public void TestSpawnerSimplePasses()
    {
        // Spawn first wave
        SimulateSpawnWave();

        // Check if wave is finished
        Assert.True(_allEnemiesSpawned);

        // Start moving onto next wave
        _allEnemiesSpawned = false;
        SimulateSpawnWave();

        // Check if wave is finished
        Assert.True(_allEnemiesSpawned);
        _allEnemiesSpawned = false;

        // All waves should have been spawned now
        // Only checking X, as this indicates the amount of waves finished spawning
        Assert.AreEqual(_waveInfo.x, _currentProgress.x);

    }

    public void SimulateSpawnWave()
    {
        // Simulate spawning in all enemy batches
        for (int i = 0; i < _waveInfo.y * _waveInfo.z; i++)
        {
            NextStage();
        }
    }

    public void NextStage()
    {
        int currentX = (int)_currentProgress.x;
        int currentY = (int)_currentProgress.y;
        int currentZ = (int)_currentProgress.z;
        currentZ++;

        // If all enemies of array are spawned, move onto next batch
        int enemyCount = (int)_waveInfo.z;

        if (currentZ >= enemyCount)
        {
            currentZ = 0;
            currentY++;
        }

        // Check if wave is finished
        int countOfEnemyGroups = (int)_waveInfo.y;
        if (currentY >= countOfEnemyGroups)
        {
            currentY = 0;
            currentX++;

            _allEnemiesSpawned = true;
        }

        // Update current progress
        _currentProgress = new Vector3(currentX, currentY, currentZ);
    }
}
