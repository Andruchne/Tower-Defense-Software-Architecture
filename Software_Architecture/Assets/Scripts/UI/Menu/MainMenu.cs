using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        //EventBus<OnLevelLoadedEvent>.Publish(new OnLevelLoadedEvent());
    }

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
