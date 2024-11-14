using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    [SerializeField] Transform particleSystemHolder;

    private ParticleSystem _impactParticleSystem;
    private CurrentTower _currentTower;

    private void Update()
    {
        DestroyOnFinish();
    }

    public void Initialize(CurrentTower currentTower)
    {
        _impactParticleSystem = particleSystemHolder.GetComponent<ParticleSystem>();

        this._currentTower = currentTower;
        SetImpactRadius();
    }

    private void SetImpactRadius()
    {
        float radius = _currentTower.info.effectRadius[_currentTower.currentTier];

        // Set collider radius
        GetComponent<SphereCollider>().radius = radius;

        // Update particle effect radius
        ParticleSystem.ShapeModule shape = _impactParticleSystem.shape;
        shape.radius = radius;
    }

    private void DestroyOnFinish()
    {
        if (!_impactParticleSystem.isPlaying && Time.time >= _impactParticleSystem.main.duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ITargetable target = other.GetComponent<ITargetable>();

        if (target != null)
        {
            int tier = _currentTower.currentTier;
            target.Hit(_currentTower.info.damage[tier]);
        }
    }
}
