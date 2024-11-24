using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBar;

    public void SetFillAmount(float percent)
    {
        healthBar.fillAmount = percent;
    }
}
