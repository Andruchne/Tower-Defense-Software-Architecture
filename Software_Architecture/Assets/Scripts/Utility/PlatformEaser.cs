using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purely for effects.
/// This script eases in platforms from the center out, when the scene is loaded.
/// </summary>

public class PlatformEaser : MonoBehaviour
{
    [Tooltip("Distance from which the platform will travel from")]
    [SerializeField] float setbackDistance = -5.0f;

    [Tooltip("How much time needs to pass, in order to reach the destination")]
    [SerializeField] float timeToMove = 1.5f;

    [Tooltip("How much time needs to pass, to move next platforms")]
    [SerializeField] float timeBetweenStages = 0.15f;

    // To be able to tell, when all platforms are in place
    private static List<PlatformEaser> _platformEasers = new List<PlatformEaser>();
    private static int _activePlatforms;

    private List<GameObject> _children = new List<GameObject>();
    private Dictionary<int, Vector3> _initialPositions = new Dictionary<int, Vector3>();

    // For sorted tweening
    private List<float> _numbersOrdered = new List<float>();
    private Dictionary<float, List<PlatformInfo>> _objectDict = new Dictionary<float, List<PlatformInfo>>();
    private int _currentIndex;

    // This is used to synchronize tweened platforms, even when this script is attached to multiple objects
    private int _currentStage;

    private Timer _timer;

    private bool _active;

    void Start()
    {
        Setup();
        SortPlatforms();
    }

    private void OnDestroy()
    {
        if (_timer != null)
        {
            _timer.OnTimerFinished -= TweenPlatformsIn;
            _timer.OnTimerFinished -= TweenPlatformsAway;
        }

        EventBus<OnLevelFinishedEvent>.OnEvent -= RemovePlatforms;
    }

    private void Setup()
    {
        // As these fields are static and values will stay even after restarting scene,
        // reset them completely on Setup()
        _platformEasers.Clear();
        _activePlatforms = 0;

        _platformEasers.Add(this);

        for (int i = 0; i < transform.childCount; i++)
        {
            // Get child
            _children.Add(transform.GetChild(i).gameObject);
            // Save their initial position
            Vector3 initPos = _children[i].transform.position;
            _initialPositions.Add(i, initPos);
            // Set their new temporary starting position
            _children[i].transform.position = new Vector3(initPos.x, initPos.y + setbackDistance, initPos.z);
        }

        // Setup Timer
        _timer = gameObject.AddComponent<Timer>();
        _timer.Initialize(timeBetweenStages, true);

        _active = true;
        _timer.OnTimerFinished += TweenPlatformsIn;

        // For when level is finished
        EventBus<OnLevelFinishedEvent>.OnEvent += RemovePlatforms;
    }

    private void SortPlatforms()
    {
        if (_initialPositions.Count != _children.Count) { return; }

        for (int i = 0; i < _children.Count; i++)
        {
            // The biggest number decides when it's supposed to appear
            float number = GetBiggestAxisNumber(i);

            // If this number didn't exist in the dictionary before, add it and its number to the list
            if (!_objectDict.ContainsKey(number))
            {
                _objectDict.Add(number, new List<PlatformInfo>());
                _numbersOrdered.Add(number);
            }

            // Add info, in order apply the effect later on
            PlatformInfo info = new PlatformInfo(_children[i], _initialPositions[i]);
            _objectDict[number].Add(info);
        }

        // Sort the numbers from lowest, to highest value
        _numbersOrdered.Sort();

        // Start the first wave of platform tweening
        _timer.StartTimer();
    }

    private void CheckLevelLoaded()
    {
        if (_activePlatforms > 0) { return; }

        // Invoke OnLevelLoaded, after all platforms are moved
        EventBus<OnLevelLoadedEvent>.Publish(new OnLevelLoadedEvent());
    }

    private void CheckLevelUnloaded()
    {
        foreach (PlatformEaser easer in _platformEasers)
        {
            if (!easer.GetActive())
            {
                return;
            }
        }
        // Invoke OnLevelLoaded, after all platforms are moved
        EventBus<OnLevelUnloadedEvent>.Publish(new OnLevelUnloadedEvent());
    }

    private void RemovePlatforms(OnLevelFinishedEvent onLevelFinishedEvent)
    {
        // Reset values
        _currentIndex = 0;
        _currentStage = 0;
        _active = true;

        _timer.OnTimerFinished -= TweenPlatformsIn;
        _timer.OnTimerFinished += TweenPlatformsAway;
        _timer.ResetTimer(true);

        CheckLevelUnloaded();
    }

    private void TweenPlatformsIn()
    {
        // Check if it's time to tween the current platform
        // This unifies tweening between all platforms, no matter the parent
        if (Mathf.Round(_numbersOrdered[_currentIndex]) == _currentStage)
        {
            // Tween all platforms contained inside the dicitonary, with the fitting number
            List<PlatformInfo> platforms = _objectDict[_numbersOrdered[_currentIndex]];
            for (int i = 0; i < platforms.Count; i++)
            {
                _activePlatforms++;
                LeanTween.move(platforms[i].gameObject, platforms[i].initialPosition, timeToMove)
                    .setEase(LeanTweenType.easeOutBack)
                    .setOnComplete(() => {
                        // Decrease platform count
                        _activePlatforms--;
                        CheckLevelLoaded();
                    });
            }
            _currentIndex++;
        }

        // Increase stage and only continue tweening, if there are still platforms to tween
        _currentStage++;
        if (_currentIndex >= _numbersOrdered.Count)
        {
            _active = false;
            _timer.StopTimer();
        }
    }

    private void TweenPlatformsAway()
    {
        // Check if it's time to tween the current platform
        // This unifies tweening between all platforms, no matter the parent
        if (Mathf.Round(_numbersOrdered[_currentIndex]) == _currentStage)
        {
            // Tween all platforms contained inside the dicitonary, with the fitting number
            List<PlatformInfo> platforms = _objectDict[_numbersOrdered[_currentIndex]];

            for (int i = 0; i < platforms.Count; i++)
            {
                _activePlatforms++;
                LeanTween.move(platforms[i].gameObject,
                    platforms[i].initialPosition + new Vector3(0, setbackDistance, 0),
                    timeToMove).setEase(LeanTweenType.easeInBack)
                    .setOnComplete(() => {
                        // Decrease platform count
                        _activePlatforms--;
                    });
            }
            _currentIndex++;
        }

        // Increase stage and only continue tweening, if there are still platforms to tween
        _currentStage++;
        if (_currentIndex >= _numbersOrdered.Count)
        {
            _active = false;
            _timer.StopTimer();
        }
    }

    private float GetBiggestAxisNumber(int index)
    {
        Vector3 pos = _children[index].transform.position;
        float biggestNumber = 0;

        if (Mathf.Abs(pos.x) > biggestNumber) { biggestNumber = Mathf.Abs(pos.x); }
        if (Mathf.Abs(pos.z) > biggestNumber) { biggestNumber = Mathf.Abs(pos.z); }

        return biggestNumber;
    }

    public bool GetActive()
    {
        return _active;
    }
}