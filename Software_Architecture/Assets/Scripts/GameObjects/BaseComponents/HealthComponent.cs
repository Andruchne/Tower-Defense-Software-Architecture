using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public event Action OnDeath;

    [SerializeField] float maxHealth = 3;
    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;

            if (_health <= 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
    private float _health;

    private void Start()
    {
        _health = maxHealth;
    }
}
