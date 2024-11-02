using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Description("Decides how long the shot projectile needs to reach the targetPos (In seconds)")]
    [SerializeField] float reachTargetDuration = 0.5f;

    [Description("Decides how the projectile is going to move to it's target position")]
    [SerializeField] Projectile.ProjectileMoveType shootType;

    // This holds all the general info the tower needs to know about itself
    [SerializeField] private TowerInfo info;

    protected void Attack(Vector3 targetPos)
    {
        // Make sure scriptable object is valid
        if (info.attackType.projectile == null)
        {
            Debug.LogError("Scriptable Object " + info.attackType.name + ": No valid projectile given");
            return;
        }

        Projectile proj = Instantiate(info.attackType.projectile).GetComponent<Projectile>();

        // In case the prefab holds no projectile script
        if (proj == null)
        {
            Debug.LogError("Projectile Prefab " + info.attackType.projectile.name + ": No projectile script attached");
            return;
        }

        proj.Shoot(targetPos, shootType, reachTargetDuration);
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
}
