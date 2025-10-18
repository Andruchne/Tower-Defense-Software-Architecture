using UnityEngine;

/// <summary>
/// Manages transition of cutOffFrequency for music
/// </summary>
public class WaveMusic : MonoBehaviour
{
    [SerializeField] float transitionSpeed = 5000;

    private AudioLowPassFilter _lowPassFilter;
    private float _defaultFrequency;
    private float _lowestFrequency = 1232;

    private bool _lowerFrequency;

    private void Start()
    {
        _lowPassFilter = GetComponent<AudioLowPassFilter>();
        if (_lowPassFilter == null) { _lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>(); }

        _defaultFrequency = _lowPassFilter.cutoffFrequency;

        EventBus<OnStopBreakTime>.OnEvent += DeactivateTransition;
        EventBus<OnStartedBreakTime>.OnEvent += ActivateTransition;
    }

    private void OnDestroy()
    {
        EventBus<OnStopBreakTime>.OnEvent -= DeactivateTransition;
        EventBus<OnStartedBreakTime>.OnEvent -= ActivateTransition;
    }

    private void Update()
    {
        TransitionFrequency();
    }

    private void TransitionFrequency()
    {
        if (_lowerFrequency && _lowPassFilter.cutoffFrequency > _lowestFrequency)
        {
            _lowPassFilter.cutoffFrequency -= transitionSpeed * Time.deltaTime;

            if (_lowPassFilter.cutoffFrequency < _lowestFrequency) { _lowPassFilter.cutoffFrequency = _lowestFrequency; }
        }
        else if (!_lowerFrequency && _lowPassFilter.cutoffFrequency < _defaultFrequency)
        {
            _lowPassFilter.cutoffFrequency += transitionSpeed * Time.deltaTime;

            if (_lowPassFilter.cutoffFrequency > _defaultFrequency) { _lowPassFilter.cutoffFrequency = _defaultFrequency; }
        }
    }

    private void ActivateTransition(OnStartedBreakTime onStartedBreakTime)
    {
        _lowerFrequency = true;
    }

    private void DeactivateTransition(OnStopBreakTime onStopBreakTime)
    {
        _lowerFrequency = false;
    }
}
