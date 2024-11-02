using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] GameObject impactPrefab;

    public abstract void Shoot(Transform target);

    protected void Impact()
    {
        // Ground height is at 0.2, we're assuming it stays this way
        Instantiate(
            impactPrefab, 
            new Vector3(transform.position.x, 0.2f, transform.position.z), 
            Quaternion.identity, 
            null);
    }
}
