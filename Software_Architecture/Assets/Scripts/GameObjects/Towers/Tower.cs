using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Description("Decides how long the shot projectile needs to reach the targetPos (In seconds)")]
    [SerializeField] float reachTargetDuration = 0.5f;

    [Description("Particle which is instantiated when tower emerges from the ground")]
    [SerializeField] ParticleSystem emergeParticle;

    private Projectile.ProjectileMoveType _shootType;
    private SphereCollider _detectRangeCol;

    private bool _readyToAttack;

    // This holds all the general info the tower needs to know about itself
    private CurrentTower _currentTower;

    private Transform _projectileOrigin;
    private bool _initialized;

    private Timer _timer;

    private List<ITargetable> _targets = new List<ITargetable>();

    private void OnDestroy()
    {
        if (_initialized)
        {
            _timer.OnTimerFinished -= ActivateReadyToAttack;
        }

        // Just in case target list isn't empty
        foreach (ITargetable target in _targets)
        {
            target.OnTargetDestroyed -= RemoveDestroyedTarget;
        }
    }

    private void Update()
    {
        CheckForInitialization();
        CheckAttack();
    }

    public void Initialize(TowerInfo towerInfo)
    {
        // Get and set all important info
        _projectileOrigin = GetProjectileOrigin();

        // First tier starts at 0
        _currentTower.currentTier = 0;
        _currentTower.info = towerInfo;

        _shootType = towerInfo.projectileMoveType;
        _detectRangeCol = GetComponent<SphereCollider>();
        _detectRangeCol.radius = towerInfo.range[0];

        _initialized = true;

        // Initialize timer
        // As tower always starts at tier 1, no need to get tier from info reference
        _timer = GetComponent<Timer>();
        _timer.Initialize(_currentTower.info.attackCooldown[0], true);

        _timer.OnTimerFinished += ActivateReadyToAttack;

        MakeTowerEmerge();
    }

    private void CheckForInitialization()
    {
        if (!_initialized)
        {
            Debug.LogError("Tower: Tried updating without prior initialization. Destroying object.");
            Destroy(gameObject);
        }
    }

    private void CheckAttack()
    {
        if (_targets.Count > 0 && _readyToAttack)
        {
            ITargetable target = GetCurrentTarget();

            Attack(target.GetNextPosition(reachTargetDuration));
            _timer.ResetTimer(true);
            _readyToAttack = false;
        }
    }

    private void Attack(Vector3 targetPos)
    {
        // Make sure scriptable object is valid
        if (_currentTower.info.attackType.projectile == null)
        {
            Debug.LogError("Scriptable Object " + _currentTower.info.attackType.name + ": No valid projectile given");
            return;
        }

        Projectile projectile = Instantiate(
            _currentTower.info.attackType.projectile, 
            _projectileOrigin.position, 
            Quaternion.identity).GetComponent<Projectile>();

        // In case the prefab holds no projectile script
        if (projectile == null)
        {
            Debug.LogError("Projectile Prefab " + _currentTower.info.attackType.projectile.name + ": No projectile script attached");
            return;
        }

        projectile.Initialize(_currentTower);
        projectile.Shoot(targetPos, _shootType, reachTargetDuration);
    }



    // Target detection
    protected void OnTriggerEnter(Collider other)
    {
        ITargetable target = other.GetComponent<ITargetable>();

        if (target != null)
        {
            target.OnTargetDestroyed += RemoveDestroyedTarget;
            _targets.Add(target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ITargetable target = other.GetComponent<ITargetable>();

        if (target != null)
        {
            target.OnTargetDestroyed -= RemoveDestroyedTarget;
            _targets.Remove(target);
        }
    }



    // For projectile movement and target handling
    private ITargetable GetCurrentTarget()
    {
        if (_targets.Count <= 0) { return null; }

        return _targets[0];
    }

    private void RemoveDestroyedTarget(ITargetable target)
    {
        target.OnTargetDestroyed -= RemoveDestroyedTarget;
        _targets.Remove(target);
    }

    private void ActivateReadyToAttack()
    {
        _readyToAttack = true;
    }

    private Transform GetProjectileOrigin()
    {
        Transform[] children = GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.CompareTag("ProjectileOrigin"))
            {
                return child;
            }
        }

        return transform;
    }

    // For the spawn Effect
    private void MakeTowerEmerge()
    {
        // Do the shake effect
        float towerHeight = GetTowerHeight();
        float additionalOffset = 0.05f;

        float emergeDistance = towerHeight + additionalOffset;
        float riseTime = 2;

        InstantiateParticle(riseTime);

        transform.position = new Vector3(transform.position.x,
            transform.position.y - emergeDistance,
            transform.position.z);

        EmergeWithShake(emergeDistance, riseTime);
    }

    private float GetTowerHeight()
    {
        Bounds combinedBounds = new Bounds(transform.position, Vector3.zero);

        // Go through each render component and add them to the bounds
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        // Return total height
        return combinedBounds.size.y;
    }

    private void EmergeWithShake(float riseHeight, float riseTime)
    {
        Vector3 originalPosition = transform.position;

        float shakeIntensity = 0.025f;
        float shakeFrequency = 0.05f;
        float shakeTimer = 0;

        // Move the object up a few shakes
        LeanTween.moveY(gameObject, originalPosition.y + riseHeight, riseTime)
        .setEase(LeanTweenType.linear)
        .setOnUpdate((float value) => {

            shakeTimer += Time.deltaTime;

            if (shakeTimer >= shakeFrequency)
            {
            // Generate random offsets for X and Z axes to shake
             float shakeOffsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float shakeOffsetZ = Random.Range(-shakeIntensity, shakeIntensity);

            transform.position = new Vector3(
                originalPosition.x + shakeOffsetX,
                transform.position.y,
                originalPosition.z + shakeOffsetZ);

            // Reset timer for next shake
            shakeTimer = 0f;
            }
        })
        .setOnComplete(() => {
            // Reset X and Z positions to original pos
            transform.position = new Vector3(originalPosition.x, transform.position.y, originalPosition.z);
        });
    }

    private void InstantiateParticle(float riseTime)
    {
        if (emergeParticle != null)
        {
            ParticleSystem[] particleSystems = emergeParticle.GetComponentsInChildren<ParticleSystem>();

            // Set the duration for all particle systems
            foreach (ParticleSystem particleSys in particleSystems)
            {
                ParticleSystem.MainModule mainModule = particleSys.main;
                mainModule.duration = riseTime;
            }

            Instantiate(emergeParticle, transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
        }
        else { Debug.LogError("Tower Prefab: No emergeParticle to instantiate"); }
    }
}
