using System.ComponentModel;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject playerHUD;

    [Description("Particle which is instantiated when tower submerges")]
    [SerializeField] ParticleSystem emergeParticle;

    [SerializeField] float maxHealth = 5;

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

    private void Start()
    {
        Instantiate(playerHUD);

        Health = maxHealth;
        Gold = 0;

        EventBus<OnDamagePlayerEvent>.OnEvent += DamagePlayer;
        EventBus<OnGetGoldEvent>.OnEvent += GainGold;

        // Keep UI up to date
        EventBus<OnUpdateCurrentGold>.Publish(new OnUpdateCurrentGold(Gold));
        EventBus<OnUpdateCurrentHealth>.Publish(new OnUpdateCurrentHealth(Health / maxHealth));
    }

    private void OnDestroy()
    {
        EventBus<OnDamagePlayerEvent>.OnEvent -= DamagePlayer;
        EventBus<OnGetGoldEvent>.OnEvent -= GainGold;

        _tween.OnTweenComplete -= PlayerSubmerged;
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
        else { Debug.LogError("Tower Prefab: No emergeParticle to instantiate"); }
    }
}
