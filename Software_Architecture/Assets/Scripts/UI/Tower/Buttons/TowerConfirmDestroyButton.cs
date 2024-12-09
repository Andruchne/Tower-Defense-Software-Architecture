using System;
using TMPro;
using UnityEngine;

/// <summary>
/// This script is attached to the confirm tower destroy canvas
/// It also sets the amount of refund
/// </summary>

public class TowerConfirmDestroyButton : MonoBehaviour
{
    public event Action OnConfirmDestroy;
    public event Action OnCancelDestroy;

    [SerializeField] TextMeshProUGUI refundAmount;

    public void Initialize(int refundAmount)
    {
        this.refundAmount.text = refundAmount.ToString();
    }

    public void CancelDestroyClicked()
    {
        OnCancelDestroy?.Invoke();
    }

    public void ConfirmDestroyClicked()
    {
        OnConfirmDestroy?.Invoke();
    }
}
