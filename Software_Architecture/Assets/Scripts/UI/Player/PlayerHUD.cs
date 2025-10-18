using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Manages the player HUD
/// In here, it only listens to events, changing the components when invoked
/// </summary>

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] Image healthIcon;

    [SerializeField] GameObject timeHolder;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI speedText;

    private string _lastTime;
    private SoundPlayer _soundPlayer;

    private void Start()
    {
        _soundPlayer = GetComponent<SoundPlayer>();

        EventBus<OnUpdateCurrentHealth>.OnEvent += UpdateHealth;
        EventBus<OnUpdateCurrentTime>.OnEvent += UpdateTime;
        EventBus<OnUpdateCurrentGold>.OnEvent += UpdateCurrentGold;
        EventBus<OnUpdateCurrentWave>.OnEvent += UpdateWave;
        EventBus<OnStopBreakTime>.OnEvent += HideTimer;
        timeHolder.SetActive(false);

        EventBus<OnPlayerHUDLoaded>.Publish(new OnPlayerHUDLoaded());
    }

    private void OnDestroy()
    {
        EventBus<OnUpdateCurrentHealth>.OnEvent -= UpdateHealth;
        EventBus<OnUpdateCurrentTime>.OnEvent -= UpdateTime;
        EventBus<OnUpdateCurrentGold>.OnEvent -= UpdateCurrentGold;
        EventBus<OnUpdateCurrentWave>.OnEvent -= UpdateWave;
        EventBus<OnStopBreakTime>.OnEvent -= HideTimer;
    }

    public void Initialize(int startGold)
    {
        UpdateCurrentGold(new OnUpdateCurrentGold(startGold));
    }

    public void SetSpeedMultiplier(float value)
    {
        GameManager.Instance.SetGameSpeed(value);
        speedText.text = "Speed: " + value.ToString("F2") + "x";
    }

    public void SetMasterSoundVolume(float value)
    {
        GameManager.Instance.SetMasterVolume(value);
    }

    private void UpdateHealth(OnUpdateCurrentHealth onUpdateCurrentHealth)
    {
        healthIcon.fillAmount = onUpdateCurrentHealth.currentPercent;
    }

    private void UpdateTime(OnUpdateCurrentTime onUpdateCurrentTime)
    {
        // Make visible when time updated
        if (!timeHolder.activeSelf) { timeHolder.SetActive(true); }

        float currentTime = onUpdateCurrentTime.currentAmount;

        int minutes = (int)MathF.Floor(currentTime / 60);
        int seconds = (int)MathF.Floor(currentTime % 60);

        string time = string.Format("{0:0}:{1:00}", minutes, seconds);
        timeText.text = time;

        if (time != _lastTime) { _soundPlayer.Play(); }
        _lastTime = time;
    }

    private void UpdateWave(OnUpdateCurrentWave onUpdateCurrentWave)
    {
        waveText.text = string.Format("{0}/{1}", onUpdateCurrentWave.currentWave, onUpdateCurrentWave.maxWave);
    }

    private void UpdateCurrentGold(OnUpdateCurrentGold onUpdateCurrentGold)
    {
        goldText.text = onUpdateCurrentGold.currentAmount.ToString();
    }

    public void StopBreakTime()
    {
        EventBus<OnStopBreakTimeEarly>.Publish(new OnStopBreakTimeEarly());
    }

    // To synch hiding timer, without additional checks
    private void HideTimer(OnStopBreakTime onStopBreakTime)
    {
        timeHolder.SetActive(false);
    }
}
