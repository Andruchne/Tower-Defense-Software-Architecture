using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    [SerializeField] Transform particleSystemHolder;

    private ParticleSystem _impactParticleSystem;
    private CurrentTower _currentTower;

    private void Start()
    {
        _impactParticleSystem = particleSystemHolder.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        DestroyOnFinish();
    }

    public void Initialize(CurrentTower currentTower)
    {
        this._currentTower = currentTower; 
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
            target.Hit(_currentTower.info.damage[tier - 1]);
        }
    }
}
