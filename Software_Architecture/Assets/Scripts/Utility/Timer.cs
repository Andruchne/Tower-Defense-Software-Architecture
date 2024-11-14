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

    public void Initialize(float waitTime, bool startTimer = false)
    {
        _waitTime = waitTime;
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
            OnTimerFinished?.Invoke();
            _currentPassedTime = 0;
        }
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
