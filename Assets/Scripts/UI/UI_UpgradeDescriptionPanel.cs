using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_UpgradeDescriptionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField]private TextMeshProUGUI skillCostText;

    public void SetDescription(string description, string skillCost)
    {
        descriptionText.text = description;
        skillCostText.text = $"Skill Cost: {skillCost}";
    }
}
