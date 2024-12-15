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
        float timeLimit = 15.0f;
        float currentTime = 0.0f;
        bool enemySlowed = false;
        while (!enemySlowed)
        {
            currentTime += Time.deltaTime;

            // Check if an enemy was slowed
            foreach (Enemy enemy in enemies)
            {
                if (enemy.GetCurrentSpeed() < defaultSpeed) { enemySlowed = true; }
            }

            if (currentTime >= timeLimit)
            {
                Assert.Fail("No enemy was slowed within time");
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
    }
}
