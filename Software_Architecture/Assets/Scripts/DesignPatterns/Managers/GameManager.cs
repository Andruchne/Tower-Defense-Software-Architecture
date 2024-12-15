using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] float timePerBreak = 60;
    [SerializeField] bool showCursorAtStart;

    private Player _player;
    private WaveManager _waveManager;

    private Timer _breakTimer;

    private bool _oneShotTargets;

    #region Singleton Pattern
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private void Start()
    {
        if (showCursorAtStart) { ToggleCursorState(new OnLevelLoadedEvent()); }

        _player = FindObjectOfType<Player>();
        _waveManager = FindObjectOfType<WaveManager>();

        _breakTimer = gameObject.AddComponent<Timer>();
        _breakTimer.Initialize(timePerBreak);

        // Show/Hide cursor while loading/unloading
        EventBus<OnLevelLoadedEvent>.OnEvent += ToggleCursorState;
        EventBus<OnLevelLoadedEvent>.OnEvent += StartBreak;
        EventBus<OnLevelUnloadedEvent>.OnEvent += ToggleCursorState;
        EventBus<OnLevelFinishedEvent>.OnEvent += ToggleCursorState;
        EventBus<OnQueueUpBreakTime>.OnEvent += StartBreak;
        EventBus<OnStopBreakTimeEarly>.OnEvent += StopBreakEarly;

        EventBus<OnOneHitEnemies>.OnEvent += SetOneShot;

        _breakTimer.OnTimerRunning += UpdateTimeHUD;
        _breakTimer.OnTimerFinished += StopBreak;

        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _player = FindObjectOfType<Player>();
        _waveManager = FindObjectOfType<WaveManager>();
    }

    private void OnDestroy()
    {
        EventBus<OnLevelLoadedEvent>.OnEvent -= ToggleCursorState;
        EventBus<OnLevelLoadedEvent>.OnEvent -= StartBreak;
        EventBus<OnLevelUnloadedEvent>.OnEvent -= ToggleCursorState;
        EventBus<OnLevelFinishedEvent>.OnEvent -= ToggleCursorState;
        EventBus<OnQueueUpBreakTime>.OnEvent -= StartBreak;
        EventBus<OnStopBreakTimeEarly>.OnEvent -= StopBreakEarly;

        EventBus<OnOneHitEnemies>.OnEvent -= SetOneShot;

        if (_breakTimer != null)
        {
            _breakTimer.OnTimerRunning -= UpdateTimeHUD;
            _breakTimer.OnTimerFinished -= StopBreak;
        }

        SceneManager.sceneLoaded -= SceneLoaded;
    }

    #region Level Management

    private void UpdateTimeHUD()
    {
        EventBus<OnUpdateCurrentTime>.Publish(new OnUpdateCurrentTime((int)_breakTimer.GetTimeLeft()));
    }

    private void StartBreak<T>(T onEvent)
    {
        _breakTimer.ResetTimer(true);
        EventBus<OnStartedBreakTime>.Publish(new OnStartedBreakTime());
    }

    // Two seperate methods, to listen to Action event, as well as EventBus
    private void StopBreakEarly(OnStopBreakTimeEarly onStopBreakTimeEarly)
    {
        StopBreak();
    }

    private void StopBreak()
    {
        _breakTimer.ResetTimer(false);
        EventBus<OnStopBreakTime>.Publish(new OnStopBreakTime());
    }

    #endregion

    #region Load Scene Logic
    public void LoadSceneSpecific(int sceneIndex)
    {
        if (sceneIndex < 0 && sceneIndex > SceneManager.sceneCountInBuildSettings)
        {
            // If given index is invalid, load default level
            sceneIndex = 0;
        }

        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadSceneNext()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Check if next index is valid to load, else reset it to zero
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings) { nextSceneIndex = 0; }
        SceneManager.LoadScene(nextSceneIndex);
    }
    #endregion

    #region Getters
    public int GetPlayerGold()
    {
        if (_player == null) 
        { 
            Debug.LogError("GameManager: No player available");
            return 0;
        }

        return _player.GetCurrentGold();
    }

    public bool GetWaveActiveState()
    {
        if (_waveManager == null) 
        { 
            Debug.LogError("GameManager: No waveManager available");
            return false;
        }

        return _waveManager.IsActive();
    }

    public bool GetOneShotTargets()
    {
        return _oneShotTargets;
    }
    #endregion

    #region Utility
    private void ToggleCursorState<T>(T onLevelEvent)
    {
        Cursor.visible = !Cursor.visible;
        if (Cursor.lockState == CursorLockMode.None) { Cursor.lockState = CursorLockMode.Locked; }
        else { Cursor.lockState = CursorLockMode.None; }
    }
    #endregion

    #region Debugging

    private void SetOneShot(OnOneHitEnemies onOneHitEnemies)
    {
        _oneShotTargets = onOneHitEnemies.state;
    }

    #endregion
}
