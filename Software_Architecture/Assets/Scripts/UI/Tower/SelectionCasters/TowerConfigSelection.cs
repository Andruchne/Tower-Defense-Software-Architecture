using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerConfigSelection : MonoBehaviour
{
    public event Action OnUpgrade;
    public event Action OnDestruct;

    public void InvokeUpgrade()
    {
        OnUpgrade?.Invoke();
    }

    public void InvokeDestruct()
    {
        OnDestruct?.Invoke();
    }
}
