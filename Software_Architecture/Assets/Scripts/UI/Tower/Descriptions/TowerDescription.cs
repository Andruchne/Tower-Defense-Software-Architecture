using TMPro;
using UnityEngine;

/// <summary>
/// Updates all current tower info, on the tower UI component for selecting tower
/// The window appears when hovering over tower type, after clicking on TowerSlot
/// </summary>

public class TowerDescription : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI towerTypeText;
    [Space]
    [SerializeField] TextMeshProUGUI attackTypeText;
    [SerializeField] TextMeshProUGUI attackDescriptionText;
    [Space]
    [SerializeField] TextMeshProUGUI powerNumberText;
    [Space]
    [SerializeField] TextMeshProUGUI rangeNumberText;
    [Space]
    [SerializeField] TextMeshProUGUI attackCooldownText;
    [Space]
    [SerializeField] TextMeshProUGUI effectRadiusText;

    public void SetInfo(TowerInfo tInfo)
    {
        towerTypeText.text = tInfo.towerTypeName;
        attackTypeText.text = tInfo.attackType.attackType;
        attackDescriptionText.text = tInfo.attackType.attackTypeDescription;

        // Array length of all values are bound together, so check only one
        if (tInfo.power.Length > 0)
        {
            // As tower will always start from tier 1, always take first index
            powerNumberText.text = tInfo.power[0].ToString();
            rangeNumberText.text = tInfo.range[0].ToString();
            attackCooldownText.text = tInfo.attackCooldown[0].ToString();
            effectRadiusText.text = tInfo.effectRadius[0].ToString();
        }
    }
}
