using System;
using UnityEngine;

public class TowerConfirmDestroyButton : MonoBehaviour
{
    public event Action OnConfirmDestroy;
    public event Action OnCancelDestroy;

    public void CancelDestroyClicked()
    {
        OnCancelDestroy?.Invoke();
    }

    public void ConfirmDestroyClicked()
    {
        OnConfirmDestroy?.Invoke();
    }
}
