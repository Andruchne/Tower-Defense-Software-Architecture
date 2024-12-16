using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Enables toggling debug states, using the inspector
/// It publishes EventBus events, to notify about these debug changes
/// </summary>

public class DebugSettings : MonoBehaviour
{
    [Range(0, 5)]
    [SerializeField] float gameSpeed = 1;

    [SerializeField] bool oneHitEnemies;
    [SerializeField] bool infiniteRiches;
    [SerializeField] bool immediateLooser;
    [SerializeField] bool invincibleBase;

    #region Getters & Setters

    private bool _oneHitEnemies;
    private bool OneHitEnemies
    {
        get { return _oneHitEnemies; }
        set
        {
            if (value != _oneHitEnemies)
            {
                _oneHitEnemies = value;
                EventBus<OnOneHitEnemies>.Publish(new OnOneHitEnemies(value));
            }
        }
    }

    private bool _infiniteRiches;
    private bool InfiniteRiches
    {
        get { return _infiniteRiches; }
        set
        {
            if (value != _infiniteRiches)
            {
                _infiniteRiches = value;
                EventBus<OnInfiniteRiches>.Publish(new OnInfiniteRiches(value));
            }
        }
    }

    private bool _immediateLooser;
    private bool ImmediateLooser
    {
        get { return _immediateLooser; }
        set
        {
            if (value != _immediateLooser)
            {
                _immediateLooser = value;
                EventBus<OnImmediateLooser>.Publish(new OnImmediateLooser(value));
            }
        }
    }

    private bool _invincibleBase;
    private bool InvincibleBase
    {
        get { return _invincibleBase; }

        set
        {
            if (value != _invincibleBase)
            {
                _invincibleBase = value;
                EventBus<OnInvincibleBase>.Publish(new OnInvincibleBase(value));
            }
        }
    }
    #endregion

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnValidate()
    {
        // Called when [SerializeField] is updated in runtime
        UpdateAllValues();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedInitialization());
    }

    private IEnumerator DelayedInitialization()
    {
        yield return new WaitForEndOfFrame();

        // Keep the values, also when restarting
        EventBus<OnInfiniteRiches>.Publish(new OnInfiniteRiches(_infiniteRiches));
        EventBus<OnImmediateLooser>.Publish(new OnImmediateLooser(_immediateLooser));
        EventBus<OnInvincibleBase>.Publish(new OnInvincibleBase(_invincibleBase));
    }

    private void UpdateAllValues()
    {
        OneHitEnemies = oneHitEnemies;
        InfiniteRiches = infiniteRiches;
        ImmediateLooser = immediateLooser;
        InvincibleBase = invincibleBase;

        // Set game speed
        Time.timeScale = gameSpeed;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
