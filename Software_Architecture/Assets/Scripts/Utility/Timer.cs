using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public event Action OnTimerFinished;

    private float _waitTime;
    private float _currentPassedTime;

    private bool _active;
    private bool _loop;

    // As MonoBehaviour is in use, using the class constructor is not an option
    // To use this timer, instantiate a timer object and call Initialize() for setup
    public void Initialize(float waitTime, bool loop = false, bool startTimer = false)
    {
        _waitTime = waitTime;
        _loop = loop;
        if (startTimer) { StartTimer(); }
    }

    private void Update()
    {
        RunTimer();
    }

    private void RunTimer()
    {
        if (!_active) { return; }

        _currentPassedTime += Time.deltaTime;

        // Timer finished
        if (_currentPassedTime >= _waitTime)
        {
            // Continue timer if set to loop
            if (_loop) { ResetTimer(true); }
            // Deactivate and reset timer otherwise
            else { ResetTimer(); }

            OnTimerFinished?.Invoke();
        }
    }

    // Methods to adjust timer
    public void SetWaitTime(float waitTime)
    {
        _waitTime = waitTime;
    }

    public void SetLoop(bool loop)
    {
        _loop = loop;
    }

    // Methods for managing timer
    public void StartTimer()
    {
        _active = true;
    }

    public void StopTimer(bool resetTimer = false)
    {
        _active = false;

        if (resetTimer) { ResetTimer(); }
    }

    public void ResetTimer(bool startTimer = false)
    {
        _currentPassedTime = 0;

        if (startTimer) { StartTimer(); }
        else { StopTimer(); }
    }
}
