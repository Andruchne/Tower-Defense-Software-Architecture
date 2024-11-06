using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Description("Decides how long the shot projectile needs to reach the targetPos (In seconds)")]
    [SerializeField] float reachTargetDuration = 0.5f;

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
            _timer.onTimerFinished -= ActivateReadyToAttack;
        }

        // Just in case target list isn't empty
        foreach (ITargetable target in _targets)
        {
            target.onTargetDestroyed -= RemoveDestroyedTarget;
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

        _currentTower.currentTier = 1;
        _currentTower.info = towerInfo;

        _shootType = towerInfo.projectileMoveType;
        _detectRangeCol = GetComponent<SphereCollider>();
        _detectRangeCol.radius = towerInfo.range[0];

        _initialized = true;

        // Initialize timer
        // As tower always starts at tier 1, no need to get tier from info reference
        _timer = GetComponent<Timer>();
        _timer.Initialize(_currentTower.info.attackCooldown[0], true);

        _timer.onTimerFinished += ActivateReadyToAttack;
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

        Projectile proj = Instantiate(
            _currentTower.info.attackType.projectile, 
            _projectileOrigin.position, 
            Quaternion.identity, 
            null).GetComponent<Projectile>();

        // In case the prefab holds no projectile script
        if (proj == null)
        {
            Debug.LogError("Projectile Prefab " + _currentTower.info.attackType.projectile.name + ": No projectile script attached");
            return;
        }

        proj.Initialize(_currentTower);
        proj.Shoot(targetPos, _shootType, reachTargetDuration);
    }



    // Target detection
    protected void OnTriggerEnter(Collider other)
    {
        ITargetable target = other.GetComponent<ITargetable>();

        if (target != null)
        {
            target.onTargetDestroyed += RemoveDestroyedTarget;
            _targets.Add(target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ITargetable target = other.GetComponent<ITargetable>();

        if (target != null)
        {
            target.onTargetDestroyed -= RemoveDestroyedTarget;
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
        target.onTargetDestroyed -= RemoveDestroyedTarget;
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
}
