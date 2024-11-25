using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            EventBus<OnLevelFinishedEvent>.Publish(new OnLevelFinishedEvent(true));
        }
    }

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

        // Show/Hide cursor while loading/unloading
        EventBus<OnLevelLoadedEvent>.OnEvent += ToggleCursorState;
        EventBus<OnLevelUnloadedEvent>.OnEvent += ToggleCursorState;

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        EventBus<OnLevelLoadedEvent>.OnEvent -= ToggleCursorState;
        EventBus<OnLevelUnloadedEvent>.OnEvent -= ToggleCursorState;
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

    #region Utility
    private void ToggleCursorState<T>(T onLevelEvent)
    {
        Cursor.visible = !Cursor.visible;
        Debug.Log(Cursor.lockState);
        if (Cursor.lockState == CursorLockMode.None) { Cursor.lockState = CursorLockMode.Locked; }
        else { Cursor.lockState = CursorLockMode.None; }
    }
    #endregion
}
