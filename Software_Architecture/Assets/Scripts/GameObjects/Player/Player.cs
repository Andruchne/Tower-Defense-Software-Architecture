using System.ComponentModel;
using UnityEngine;

/// <summary>
/// This is the player 
/// It instantiates the HUD, manages health and gold, and does some tweening, when hit/defeated
/// </summary>

public class Player : MonoBehaviour
{
    [SerializeField] GameObject playerHUD;

    [Description("Particle which is instantiated when tower submerges")]
    [SerializeField] ParticleSystem emergeParticle;

    [Description("Particle which is instantiated when game is won")]
    [SerializeField] ParticleSystem fireworkParticle;

    [SerializeField] float maxHealth = 5;

    [SerializeField] int startGold = 200;

    // Health
    private float Health
    {
        get
        {
            return _health;
        }
        set
        {
            // Player defeated
            if (value <= 0) { Defeated(); }

            _health = Mathf.Clamp(value, 0, maxHealth);

            // Inform UI
            EventBus<OnUpdateCurrentHealth>.Publish(new OnUpdateCurrentHealth(value / maxHealth));
        }
    }
    private float _health;

    // Gold
    private int Gold
    {
        get
        {
            return _gold;
        }
        set
        {
            _gold = value;
            if (_gold < 0) { _gold = 0; }

            // Inform UI
            EventBus<OnUpdateCurrentGold>.Publish(new OnUpdateCurrentGold(value));
        }
    }
    private int _gold;

    private Tweens _tween = new Tweens();
    private bool _defeated;

    // Used to transition, after having won
    Timer _timer;

    private void Start()
    {
        _timer = gameObject.AddComponent<Timer>();
        _timer.Initialize(5, false);
        _timer.OnTimerFinished += FinishGame;

        Instantiate(playerHUD);
        playerHUD.GetComponent<PlayerHUD>().Initialize(startGold);

        Gold = startGold;
        Health = maxHealth;

        EventBus<OnDamagePlayerEvent>.OnEvent += DamagePlayer;
        EventBus<OnGetGoldEvent>.OnEvent += GainGold;
        EventBus<OnWithdrawGoldEvent>.OnEvent += WithdrawGold;
        EventBus<OnPlayerHUDLoaded>.OnEvent += PlayerHUDLoaded;
        EventBus<OnLevelWon>.OnEvent += QueueFanfare;
    }

    private void OnDestroy()
    {
        EventBus<OnDamagePlayerEvent>.OnEvent -= DamagePlayer;
        EventBus<OnGetGoldEvent>.OnEvent -= GainGold;
        EventBus<OnWithdrawGoldEvent>.OnEvent -= WithdrawGold;
        EventBus<OnPlayerHUDLoaded>.OnEvent -= PlayerHUDLoaded;
        EventBus<OnLevelWon>.OnEvent -= QueueFanfare;

        _tween.OnTweenComplete -= PlayerSubmerged;
        _timer.OnTimerFinished -= FinishGame;
    }

    private void FinishGame()
    {
        EventBus<OnLevelFinishedEvent>.Publish(new OnLevelFinishedEvent());
    }

    private void PlayerHUDLoaded(OnPlayerHUDLoaded onPlayerHUDLoaded)
    {
        // Keep UI up to date
        EventBus<OnUpdateCurrentGold>.Publish(new OnUpdateCurrentGold(Gold));
        EventBus<OnUpdateCurrentHealth>.Publish(new OnUpdateCurrentHealth(Health / maxHealth));
    }

    private void Defeated()
    {
        if (_defeated) { return; }

        _defeated = true;

        EventBus<OnPlayerDefeatedEvent>.Publish(new OnPlayerDefeatedEvent());

        float effectDuration = 2.0f;
        float additionalOffset = 0.05f;
        _tween.SubmergeWithShake(transform, Useful.GetRenderedHeight(transform) + additionalOffset, effectDuration);
        InstantiateDirtParticle(effectDuration);

        _tween.OnTweenComplete += PlayerSubmerged;
    }

    private void PlayerSubmerged()
    {
        _tween.OnTweenComplete -= PlayerSubmerged;

        EventBus<OnLevelFinishedEvent>.Publish(new OnLevelFinishedEvent());
    }

    private void DamagePlayer(OnDamagePlayerEvent onDamagePlayerEvent)
    {
        Health -= onDamagePlayerEvent.damage;

        float shakeTime = 0.5f;
        _tween.Shake(transform, shakeTime);
    }

    private void GainGold(OnGetGoldEvent onGetGoldEvent)
    {
        Gold += onGetGoldEvent.goldAmount;
    }

    private void WithdrawGold(OnWithdrawGoldEvent onWithdrawGoldEvent)
    {
        Gold -= onWithdrawGoldEvent.goldAmount;
    }

    private void InstantiateDirtParticle(float duration)
    {
        if (emergeParticle != null)
        {
            ParticleSystem[] particleSystems = emergeParticle.GetComponentsInChildren<ParticleSystem>();

            // Set the duration for all particle systems
            foreach (ParticleSystem particleSys in particleSystems)
            {
                ParticleSystem.MainModule mainModule = particleSys.main;
                mainModule.duration = duration;
            }

            Instantiate(emergeParticle, transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
        }
        else { Debug.LogError("Player: No emergeParticle to instantiate"); }
    }

    private void QueueFanfare(OnLevelWon onLevelWon)
    {
        if (emergeParticle != null)
        {
            Instantiate(fireworkParticle, transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
            _timer.StartTimer();
        }
        else { Debug.LogError("Player: No fireworkParticle to instantiate"); }
    }

    public int GetCurrentGold()
    {
        return Gold;
    }
}
