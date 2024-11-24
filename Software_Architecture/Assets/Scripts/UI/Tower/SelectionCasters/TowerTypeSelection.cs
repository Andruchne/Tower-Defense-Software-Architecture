using System;
using UnityEngine;

public class TowerTypeSelection : MonoBehaviour
{
    public event Action<TowerInfo> OnTypeSelected;

    public void InvokeTypeSelected(TowerInfo info)
    {
        OnTypeSelected?.Invoke(info);
    }
}
