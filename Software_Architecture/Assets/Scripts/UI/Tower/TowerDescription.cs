using TMPro;
using UnityEngine;

public class TowerDescription : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI towerTypeText;
    [Space]
    [SerializeField] TextMeshProUGUI attackTypeText;
    [SerializeField] TextMeshProUGUI attackDescriptionText;
    [Space]
    [SerializeField] TextMeshProUGUI damageNumberText;
    [Space]
    [SerializeField] TextMeshProUGUI rangeNumberText;
    [Space]
    [SerializeField] TextMeshProUGUI attackCooldownText;

    public void SetInfo(TowerInfo tInfo)
    {
        towerTypeText.text = tInfo.towerTypeName;
        attackTypeText.text = tInfo.attackType.attackType;
        attackDescriptionText.text = tInfo.attackType.attackTypeDescription;

        // Array length of all values are bound together, so check only one
        if (tInfo.damage.Length > 0)
        {
            damageNumberText.text = tInfo.damage[0].ToString();
            rangeNumberText.text = tInfo.range[0].ToString();
            attackCooldownText.text = tInfo.attackCooldown[0].ToString();
        }
    }
}
