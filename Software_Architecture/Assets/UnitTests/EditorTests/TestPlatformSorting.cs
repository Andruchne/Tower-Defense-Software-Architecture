using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestPlatformSorting
{
    private int _testObjectCount = 20;

    // As we're only looking at X and Y to sort positions (when actually applying it, X and Z), we'll use Vector2 for testing
    private List<Vector2> _objectPositions = new List<Vector2>();
    // Dictionary, that would hold all the sorted platforms 
    private Dictionary<float, List<Vector2>> _objectDict = new Dictionary<float, List<Vector2>>();

    private List<float> _numbersOrdered = new List<float>();

    private int _currentIndex;
    private int _currentStage;

    private int _activatedObjectsCount;

    [Test]
    public void TestPlatformSortingSimplePasses()
    {
        GenerateRandomPositions();
        SortPlatforms();

    }

    public void TestObjects()
    {
        for (int i = 0; i < _testObjectCount; i++)
        {
            TweenPlatformsIn();
        }
    }

    private void SortPlatforms()
    {
        for (int i = 0; i < _objectPositions.Count; i++)
        {
            // The biggest number decides when it's supposed to appear
            float number = GetBiggestAxisNumber(i);

            // If this number didn't exist in the dictionary before, add it and its number to the list
            if (!_objectDict.ContainsKey(number))
            {
                List<Vector2> t = new List<Vector2>();
                _objectDict.Add(number, new List<Vector2>());
                _numbersOrdered.Add(number);
            }

            // Add info, in order apply the effect later on
            _objectDict[number].Add(_objectPositions[i]);
        }

        // Sort the numbers from lowest, to highest value
        _numbersOrdered.Sort();
    }

    private void TweenPlatformsIn()
    {
        // Check if it's time to tween the current platform
        // This unifies tweening between all platforms, no matter the parent
        if (Mathf.Round(_numbersOrdered[_currentIndex]) == _currentStage)
        {
            // Tween all platforms contained inside the dicitonary, with the fitting number
            List<Vector2> platforms = _objectDict[_numbersOrdered[_currentIndex]];
            for (int i = 0; i < platforms.Count; i++)
            {
                // In here the tween would happen
                _activatedObjectsCount++;
            }
            _currentIndex++;
        }

        // Increase stage and only continue tweening, if there are still platforms to tween
        _currentStage++;
        if (_currentIndex >= _numbersOrdered.Count)
        {
            Assert.AreEqual(_testObjectCount, _activatedObjectsCount);
        }
    }

    private void GenerateRandomPositions()
    {
        for (int i = 0; i < _testObjectCount; i++)
        {
            int randomX = Random.Range(0, 20);
            int randomY = Random.Range(0, 20);

            _objectPositions.Add(new Vector2(randomX, randomY));
        }
    }

    private float GetBiggestAxisNumber(int index)
    {
        Vector2 pos = _objectPositions[index];
        float biggestNumber = 0;

        if (Mathf.Abs(pos.x) > biggestNumber) { biggestNumber = Mathf.Abs(pos.x); }
        if (Mathf.Abs(pos.y) > biggestNumber) { biggestNumber = Mathf.Abs(pos.y); }

        return biggestNumber;
    }
}
