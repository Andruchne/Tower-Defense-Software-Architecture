using UnityEngine;

/// <summary>
/// Logic for buttons in the Main Menu scene
/// OnLevelUnloadedEvent is published to toggle Cursor to the right state
/// </summary>

public class MainMenu : MonoBehaviour
{
    public void SwitchToNextScene()
    {
        EventBus<OnLevelUnloadedEvent>.Publish(new OnLevelUnloadedEvent());
        GameManager.Instance.LoadSceneNext();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
