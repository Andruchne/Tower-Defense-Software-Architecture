using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Description("Decides how long the shot projectile needs to reach the targetPos (In seconds)")]
    [SerializeField] float reachTargetDuration = 0.5f;

    [Description("Decides how long the tower takes to emerge")]
    [SerializeField] float riseTime = 1.0f;

    [Description("Particle which is instantiated when tower emerges from the ground")]
    [SerializeField] ParticleSystem emergeParticle;

    [Description("To instantiate slot, in case this tower gets destroyed")]
    [SerializeField] GameObject towerSlot;

    private Tweens _tween = new Tweens();

    private Projectile.ProjectileMoveType _shootType;
    private SphereCollider _detectRangeCol;

    private bool _readyToAttack;

    // This holds all the general info the tower needs to know about itself
    private CurrentTower _currentTower;

    private Transform _projectileOrigin;
    private bool _initialized;

    private Timer _timer;

    private List<ITargetable> _targets = new List<ITargetable>();

    private MenuOpener _menuOpener;
    private TowerConfigSelection _openedWindow;

    private bool _active = true;

    private void Start()
    {
        _menuOpener = GetComponentInChildren<MenuOpener>();
        _menuOpener.OnMenuOpened += MenuOpened;
        _menuOpener.OnMenuClosed += MenuClosed;
    }

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

        _menuOpener.OnMenuOpened -= MenuOpened;
        _menuOpener.OnMenuClosed -= MenuClosed;
        UnbindWindowEvents();
    }

    private void Update()
    {
        CheckForInitialization();
        CheckAttack();
    }

    public void Initialize(TowerInfo towerInfo)
    {
        // Get and set all important info
        _projectileOrigin = GetProjectileOrigin(transform);

        // First tier starts at 0
        _currentTower.currentTier = 0;
        _currentTower.info = towerInfo;

        _shootType = towerInfo.projectileMoveType;
        _detectRangeCol = GetComponent<SphereCollider>();
        _detectRangeCol.radius = towerInfo.range[0];

        _initialized = true;

        // Initialize timer
        // As tower always starts at tier 1, no need to get tier from info reference
        _timer = gameObject.AddComponent<Timer>();
        _timer.Initialize(_currentTower.info.attackCooldown[0], false, true);
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
        if (!_active) { return; }

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

    private void UpgradeTower()
    {
        _currentTower.currentTier += 1;
        int tier = _currentTower.currentTier;

        // Update tower values
        _timer.SetWaitTime(_currentTower.info.attackCooldown[tier]);
        _detectRangeCol.radius = _currentTower.info.range[tier];
        List<GameObject> towerChildren = Useful.GetAllChildren(transform);

        // Remove old towerModel
        foreach (GameObject gameObject in towerChildren)
        {
            if (gameObject.tag == "Model")
            {
                Destroy(gameObject);
            }
        }
        // Instantiate new one
        GameObject newModel = Instantiate(_currentTower.info.towerModel[_currentTower.currentTier], transform);
        _projectileOrigin = GetProjectileOrigin(newModel.transform);

        // Shake a bit
        float duration = 0.5f;

        _tween.Shake(transform, duration);
        InstantiateDirtParticle(duration);
    }

    private void SubmergeTower()
    {
        _active = false;
        Destroy(_menuOpener);

        float towerHeight = Useful.GetRenderedHeight(transform);
        float additionalOffset = 0.05f;

        float submergeDistance = towerHeight + additionalOffset;

        InstantiateDirtParticle(riseTime);
        _tween.SubmergeWithShake(transform, submergeDistance, riseTime);
        _tween.OnTweenComplete += DestroyTower;
    }

    private void DestroyTower()
    {
        _tween.OnTweenComplete -= DestroyTower;
        // Calculate position before submerging
        TowerSlot slot = Instantiate(towerSlot, 
            new Vector3(transform.position.x, transform.position.y + Useful.GetRenderedHeight(transform) + 0.05f, transform.position.z), 
            transform.rotation).GetComponent<TowerSlot>();
        slot.MakeSlotEmerge();
        Destroy(gameObject);
    }


    // Upgrade Menu
    private void MenuOpened()
    {
        _openedWindow = _menuOpener.GetCurrentMenu().GetComponent<TowerConfigSelection>();
        _openedWindow.GetComponent<TowerUpgradeDescription>().SetInfo(_currentTower);

        _openedWindow.OnUpgrade += UpgradeTower;
        _openedWindow.OnDestruct += SubmergeTower;
    }

    private void MenuClosed()
    {
        UnbindWindowEvents();
        _openedWindow = null;
    }

    private void UnbindWindowEvents()
    {
        if (_openedWindow != null)
        {
            _openedWindow.OnUpgrade -= UpgradeTower;
            _openedWindow.OnDestruct -= SubmergeTower;
        }
    }

    // Target detection
    private void OnTriggerEnter(Collider other)
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

    private Transform GetProjectileOrigin(Transform transform)
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();

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
        float towerHeight = Useful.GetRenderedHeight(transform);
        float additionalOffset = 0.05f;

        float emergeDistance = towerHeight + additionalOffset;

        InstantiateDirtParticle(riseTime);

        transform.position = new Vector3(transform.position.x,
            transform.position.y - emergeDistance,
            transform.position.z);

        _tween.EmergeWithShake(transform, emergeDistance, riseTime);
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
