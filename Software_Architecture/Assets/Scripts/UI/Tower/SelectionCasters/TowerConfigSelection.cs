using System;
using UnityEngine;

/// <summary>
/// Holds the events, executed by the Tower Config Canvas buttons
/// It is attached to the most upper transform, of the menu prefab
/// This makes it easier for the script, which instantiates the menu, to listen to selections
/// </summary>

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
