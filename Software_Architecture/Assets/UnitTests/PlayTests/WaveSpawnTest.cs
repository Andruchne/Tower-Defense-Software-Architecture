using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class WaveSpawnTest
{
    private int _spawnedEnemiesCount;

    [UnityTest]
    public IEnumerator WaveSpawnTestWithEnumeratorPasses()
    {
        SceneManager.LoadScene("WaveSpawnTest");

        // Wait for the scene to load
        yield return null;

        // Check for gameObjects
        WaveManager waveManager = GameObject.FindObjectOfType<WaveManager>();
        Assert.IsNotNull(waveManager, "WaveManager not found in the scene");

        waveManager.OnEnemySpawned += IncreaseSpawnedCount;

        // To be able to unsubscribe from event properly
        try
        {
            // Get total enemy count to spawn
            Level level = waveManager.GetLevel();
            int enemiesToSpawn = 0;
            for (int i = 0; i < level.waves.Length; i++)
            {
                for (int a = 0; a < level.waves[i].enemyGroups.Length; a++)
                {
                    enemiesToSpawn += level.waves[i].enemyGroups[a].count;
                }
            }

            // Wait until all enemies have been spawned
            float timeLimit = 15.0f;
            float currentTime = 0.0f;
            while (_spawnedEnemiesCount != enemiesToSpawn)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= timeLimit)
                {
                    Assert.Fail("Enemies were not spawned or time wasn't enough to spawn them all");
                }
                yield return null;
            }

            Assert.AreEqual(enemiesToSpawn, _spawnedEnemiesCount, 
                "Not all given enemies were spawned");
        }
        finally
        {
            waveManager.OnEnemySpawned -= IncreaseSpawnedCount;
        }

        yield return new WaitForSeconds(1);
    }

    private void IncreaseSpawnedCount(ITargetable target)
    {
        _spawnedEnemiesCount++;
    }
}
