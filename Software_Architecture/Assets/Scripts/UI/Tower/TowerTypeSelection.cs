using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTypeSelection : MonoBehaviour
{
    public event Action<TowerInfo> onTypeSelected;

    public void InvokeTypeSelected(TowerInfo info)
    {
        onTypeSelected?.Invoke(info);
    }
}
