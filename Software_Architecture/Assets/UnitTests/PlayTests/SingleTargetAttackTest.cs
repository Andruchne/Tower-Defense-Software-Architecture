using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SingleTargetAttackTest
{
    private List<Enemy> _enemies;
    private int _startCount;

    [UnityTest]
    public IEnumerator SingleTargetAttackTestWithEnumeratorPasses()
    {
        SceneManager.LoadScene("SingleTargetAttackTest");

        // Wait for the scene to load
        yield return null;

        // Check for gameObjects
        _enemies = GameObject.FindObjectsOfType<Enemy>().ToList();
        Assert.IsTrue(_enemies.Count > 0, "Not enough enemies in scene to properly test");

        _startCount = _enemies.Count;

        // Listen to them being defeated
        for (int i = 0; i < _startCount; i++)
        {
            _enemies[i].OnTargetDestroyed += RemoveTargetFromList;
        }

        // To unsubscribe from event
        try
        {
            // Wait until an enemy has been defeated
            float timeLimit = 15.0f;
            float currentTime = 0.0f;
            while (_enemies.Count == _startCount)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= timeLimit)
                {
                    Assert.Fail("No enemy was defeated within time");
                }
                yield return null;
            }

            // Check if more than one enemies have been defeated
            int defeatedCount = _startCount - _enemies.Count;
            Assert.AreEqual(1, defeatedCount, "More than one enemy defeated");
        }
        finally
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].OnTargetDestroyed -= RemoveTargetFromList;
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    private void RemoveTargetFromList(ITargetable target)
    {
        target.OnTargetDestroyed -= RemoveTargetFromList;
        _enemies.Remove((Enemy)target);
    }
}
