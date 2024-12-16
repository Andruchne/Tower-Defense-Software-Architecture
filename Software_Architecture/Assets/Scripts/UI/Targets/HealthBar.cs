using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script to update the healthbar component
/// </summary>

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBar;

    public void SetFillAmount(float percent)
    {
        healthBar.fillAmount = percent;
    }
}
