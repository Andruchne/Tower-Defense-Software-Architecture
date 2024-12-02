using System;
using System.ComponentModel;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public event Action OnDeath;

    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] float maxHealth = 3;

    [Description("How high up the healthbar is supposed to be placed from the object")]
    [SerializeField] float _height;

    private Camera _camera;
    private Transform _healthBarWindow;
    private HealthBar _healthBar;

    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;

            if (_healthBar != null) { _healthBar.SetFillAmount(value / maxHealth); }
            if (_health <= 0) { OnDeath?.Invoke(); }
        }
    }
    private float _health;

    private void Start()
    {
        _camera = Camera.main;
        _health = maxHealth;

        if (healthBarPrefab != null) 
        { 
            _healthBarWindow = Instantiate(healthBarPrefab).transform;
            _healthBar = _healthBarWindow.GetComponent<HealthBar>();
        }
    }

    private void OnDestroy()
    {
        if (_healthBarWindow != null) { Destroy(_healthBarWindow.gameObject); }
    }

    private void Update()
    {
        UpdateHealthBarPos();
    }

    private void UpdateHealthBarPos()
    {
        if (_healthBarWindow != null)
        {
            Vector3 pos = _camera.WorldToScreenPoint(transform.position) + new Vector3(0, _height, 0);
            _healthBarWindow.GetChild(0).position = pos;
        }
    }
}
