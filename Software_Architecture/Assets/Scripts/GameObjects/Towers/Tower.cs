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

    // This holds all the general info the tower needs to know about itself
    private CurrentTower _currentTower;

    private Transform _projectileOrigin;
    private bool _initialized;

    private void Start()
    {
        _currentTower.currentTier = 1;
        _projectileOrigin = GetProjectileOrigin();
    }

    public void Initialize(TowerInfo towerInfo)
    {
        // Get and set all important info
        _currentTower.currentTier = 1;
        _currentTower.info = towerInfo;

        _shootType = towerInfo.projectileMoveType;
        _detectRangeCol = GetComponent<SphereCollider>();
        _detectRangeCol.radius = towerInfo.range[0];

        _initialized = true;
    }

    protected void Attack(Vector3 targetPos)
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

    protected void OnTriggerEnter(Collider other)
    {
        ITargetable target = other.GetComponent<ITargetable>();

        if (target != null)
        {
            Attack(target.GetNextPosition(reachTargetDuration));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ITargetable target = other.GetComponent<ITargetable>();

        if (target != null)
        {
            
        }
    }

    public Transform GetProjectileOrigin()
    {
        // Get all child transforms
        Transform[] children = GetComponentsInChildren<Transform>(includeInactive: true);

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
