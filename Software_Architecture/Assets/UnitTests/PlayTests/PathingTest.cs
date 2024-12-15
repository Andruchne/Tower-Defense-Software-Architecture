using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PathingTest
{
    [UnityTest]
    public IEnumerator PathingTestWithEnumeratorPasses()
    {
        SceneManager.LoadScene("PathingTest");

        // Wait for the scene to load
        yield return null;

        // Check for gameObjects
        Enemy enemy = GameObject.FindObjectOfType<Enemy>();
        Assert.IsNotNull(enemy, "Enemy not found in the scene");

        GameObject destinationObj = GameObject.FindGameObjectWithTag("Destination");
        Assert.IsNotNull(destinationObj, "Destination not found in the scene");

        Vector3 destination = destinationObj.transform.position;

        // Deactivate enemy's attack logic
        enemy.DeactivateAttack();

        // Wait until enemy reaches destination or a timeLimit reached
        float timeLimit = 15.0f;
        float currentTime = 0.0f;
        while (Vector3.Distance(enemy.transform.position, destination) > 0.1f)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= timeLimit)
            {
                Assert.Fail("Enemy did not reach the destination within the timeout period");
            }
            yield return null;
        }

        // Final position check with a small tolerance
        float maxDistance = 0.1f;
        Assert.IsTrue(Vector3.Distance(destination, enemy.transform.position) < maxDistance,
                      "Enemy did not reach the exact destination position");
    }
}
