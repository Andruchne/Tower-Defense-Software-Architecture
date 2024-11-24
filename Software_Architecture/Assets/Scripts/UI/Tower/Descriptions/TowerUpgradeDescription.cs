using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUpgradeDescription : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI towerTierText;
    [SerializeField] TextMeshProUGUI towerTypeText;

    [SerializeField] TextMeshProUGUI[] oldTexts;
    [SerializeField] TextMeshProUGUI[] newTexts;
    [SerializeField] TextMeshProUGUI[] finalNumberTexts;
    [SerializeField] TextMeshProUGUI[] arrowTexts;

    [SerializeField] ExtendedButton upgradeButton;

    public void SetInfo(CurrentTower currentTower)
    {
        int tier = currentTower.currentTier;

        towerTierText.text = "Tier " + (tier + 1);
        towerTypeText.text = currentTower.info.towerTypeName;

        // If current tier is max, hide everything unnecessary and set final text
        if ((tier + 1) == currentTower.info.power.Length)
        {
            // Hide all upgrade texts
            for (int i = 0; i < arrowTexts.Length; i++)
            {
                oldTexts[i].gameObject.SetActive(false);
                newTexts[i].gameObject.SetActive(false);

                arrowTexts[i].gameObject.SetActive(false);
                finalNumberTexts[i].gameObject.SetActive(true);
            }

            // Set all texts
            finalNumberTexts[0].text = currentTower.info.power[tier].ToString();
            finalNumberTexts[1].text = currentTower.info.range[tier].ToString();
            finalNumberTexts[2].text = currentTower.info.attackCooldown[tier].ToString();
            finalNumberTexts[3].text = currentTower.info.effectRadius[tier].ToString();

            upgradeButton.interactable = false;

            return;
        }

        // Set upgrade values
        // Array length of all values are bound together, so check only one
        if (currentTower.info.power.Length > 1)
        {
            oldTexts[0].text = currentTower.info.power[tier].ToString();
            newTexts[0].text = currentTower.info.power[tier + 1].ToString();

            oldTexts[1].text = currentTower.info.range[tier].ToString();
            newTexts[1].text = currentTower.info.range[tier + 1].ToString();

            oldTexts[2].text = currentTower.info.attackCooldown[tier].ToString();
            newTexts[2].text = currentTower.info.attackCooldown[tier + 1].ToString();

            oldTexts[3].text = currentTower.info.effectRadius[tier].ToString();
            newTexts[3].text = currentTower.info.effectRadius[tier + 1].ToString();
        }
    }
}
