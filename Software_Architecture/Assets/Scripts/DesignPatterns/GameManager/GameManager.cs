using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] float timePerBreak = 60;

    private Player _player;
    private Timer _breakTimer;

    #region Singleton Pattern
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        ToggleCursorState(new OnLevelLoadedEvent());

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
        _player = FindObjectOfType<Player>();

        _breakTimer = gameObject.AddComponent<Timer>();
        _breakTimer.Initialize(timePerBreak);

        // Show/Hide cursor while loading/unloading
        EventBus<OnLevelLoadedEvent>.OnEvent += ToggleCursorState;
        EventBus<OnLevelLoadedEvent>.OnEvent += StartBreak;
        EventBus<OnLevelFinishedEvent>.OnEvent += ToggleCursorState;

        EventBus<OnStopBreakTimeEarly>.OnEvent += StopBreakEarly;
        _breakTimer.OnTimerUpdated += UpdateTimeHUD;
        _breakTimer.OnTimerFinished += StopBreak;
    }

    private void OnDestroy()
    {
        EventBus<OnLevelLoadedEvent>.OnEvent -= ToggleCursorState;
        EventBus<OnLevelLoadedEvent>.OnEvent -= StartBreak;
        EventBus<OnLevelFinishedEvent>.OnEvent -= ToggleCursorState;

        _breakTimer.OnTimerUpdated -= UpdateTimeHUD;
        _breakTimer.OnTimerFinished -= StopBreak;
    }

    #region Level Management

    private void UpdateTimeHUD()
    {
        EventBus<OnUpdateCurrentTime>.Publish(new OnUpdateCurrentTime((int)_breakTimer.GetTimeLeft()));
    }

    private void StartBreak<T>(T onEvent)
    {
        _breakTimer.ResetTimer(true);
        EventBus<OnStartBreakTime>.Publish(new OnStartBreakTime());
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
        if (nextSceneIndex >= SceneManager.sceneCount) { nextSceneIndex = 0; }
        SceneManager.LoadScene(nextSceneIndex);
    }
    #endregion

    #region Getters
    public int GetPlayerGold()
    {
        if (_player == null) { Debug.LogError("GameManager: No player available"); }

        return _player.GetCurrentGold();
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
}
