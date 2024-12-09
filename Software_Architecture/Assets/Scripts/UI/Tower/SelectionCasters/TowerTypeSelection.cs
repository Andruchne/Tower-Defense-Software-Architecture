using System;
using UnityEngine;

/// <summary>
/// Event used, for when a type is selected
/// This simplifies communication between the button and the towerSlot
/// </summary>

public class TowerTypeSelection : MonoBehaviour
{
    public event Action<TowerInfo> OnTypeSelected;

    public void InvokeTypeSelected(TowerInfo info)
    {
        OnTypeSelected?.Invoke(info);
    }
}
