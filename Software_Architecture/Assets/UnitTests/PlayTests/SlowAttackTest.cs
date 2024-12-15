using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SlowAttackTest
{
    [UnityTest]
    public IEnumerator SingleTargetAttackTestWithEnumeratorPasses()
    {
        SceneManager.LoadScene("SlowAttackTest");

        // Wait for the scene to load
        yield return null;

        // Check for gameObjects
        List<Enemy> enemies = GameObject.FindObjectsOfType<Enemy>().ToList();
        Assert.IsTrue(enemies.Count >= 1, "Not enough enemies in scene to properly test");

        int startCount = enemies.Count;
        // Save initial speed
        float defaultSpeed = enemies[0].GetCurrentSpeed();

        // Wait until an enemy has been defeated
        float timeLimit = 20.0f;
        float currentTime = 0.0f;
        bool enemySlowed = false;

        float slowedSpeed = 1.0f;
        int slowedEnemyIndex = 0;

        while (!enemySlowed)
        {
            currentTime += Time.deltaTime;

            // Check if an enemy was slowed
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].GetCurrentSpeed() < defaultSpeed)
                {
                    enemySlowed = true;
                    slowedSpeed = enemies[i].GetCurrentSpeed();
                    slowedEnemyIndex = i;
                }
            }

            if (currentTime >= timeLimit)
            {
                Assert.Fail("No enemy was slowed within time");
            }
            yield return null;
        }

        // Wait for tower to throw slow attack one more time
        yield return new WaitForSeconds(5.5f);

        // Assuming he gets hit by another slow attack, check if effect doesn't stack
        Assert.AreEqual(slowedSpeed, enemies[slowedEnemyIndex].GetCurrentSpeed(),
            "Slow effect stacks on target");
    }
}
