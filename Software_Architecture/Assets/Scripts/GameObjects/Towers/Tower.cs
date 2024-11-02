using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{


    // This holds all the general info the tower needs to know about itself
    private TowerInfo info;

    protected void Attack(Transform target)
    {
        //Instantiate(info.attackType.projectile);

    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ITargetable>() != null)
        {
            Attack(other.transform);
        }
    }
}
