using UnityEngine;

/// <summary>
/// Script attached to sphere collider, which damages any ITargetable's entering
/// It's set to AOE on default, turning into single target when _singleTarget is true
/// </summary>

public class ImpactDamage : MonoBehaviour
{
    protected ParticleSystem[] _impactParticleSystems;
    protected CurrentTower _currentTower;

    // Timer to destroy collider earlier
    private Timer _timer;

    // For single target
    private bool _singleTarget;
    private bool _targetHit;

    private void Update()
    {
        DestroyOnFinish();
    }

    private void OnDestroy()
    {
        _timer.OnTimerFinished -= DestroyCollider;
    }

    public void Initialize(CurrentTower currentTower, bool singleTarget = false)
    {
        _timer = gameObject.AddComponent<Timer>();
        _timer.Initialize(0.1f, false, true);
        _timer.OnTimerFinished += DestroyCollider;

        _impactParticleSystems = GetComponentsInChildren<ParticleSystem>();

        _currentTower = currentTower;
        _singleTarget = singleTarget;
        SetImpactRadius();
    }

    private void SetImpactRadius()
    {
        float radius = _currentTower.info.effectRadius[_currentTower.currentTier];

        // Set collider radius
        GetComponent<SphereCollider>().radius = radius;

        // Update particle effect radius
        foreach (ParticleSystem pSystem in _impactParticleSystems)
        {
            ParticleSystem.ShapeModule shape = pSystem.shape;
            shape.radius = radius;
        }
    }

    private void DestroyCollider()
    {
        Destroy(GetComponent<SphereCollider>());
    }

    private void DestroyOnFinish()
    {
        bool destroy = true;

        foreach (ParticleSystem pSystem in _impactParticleSystems)
        {
            if (destroy && (pSystem.isPlaying || Time.time < pSystem.main.duration))
            {
                destroy = false;
            }
        }

        if (destroy) { Destroy(gameObject); }
    }

    protected virtual void EnemyEntered(ITargetable target)
    {
        int tier = _currentTower.currentTier;
        target.Hit(_currentTower.info.power[tier]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_targetHit) { return; }

        ITargetable target = other.GetComponent<ITargetable>();

        if (target != null)
        {
            EnemyEntered(target);

            if (_singleTarget) { _targetHit = true; }
        }
    }
}
