using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Upgrade ID (must match SkillTreeManager)")]
    [SerializeField] private string upgradeCode;

    [Header("Description Data")]
    [TextArea]
    [SerializeField] private string descriptionText;
    [SerializeField] private int skillCost;

    private string numberColor = "#4AFF4A";

    public string UpgradeCode => upgradeCode;
    public string Description => descriptionText;
    public string SkillCost => skillCost.ToString();

    private Button button;
    private Image image;

    private void Awake()
    {
        button= GetComponent<Button>();
        image= GetComponent<Image>();

        ApplyDescription();
        RefreshFromSave();

        SkillTreeManager.OnUpgradeUnlocked += OnUpgradeUnlocked;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        UpgradeDescriptionManager.Instance.Show(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpgradeDescriptionManager.Instance.Hide();
    }

    private void OnUpgradeUnlocked(string unlockedUpgrade)
    {
        if (unlockedUpgrade == upgradeCode)
            RefreshFromSave();
    }

    public void ApplyDescription()
    {
        string rawText = descriptionText;

        descriptionText =
            TMPNumberColorizer.ColorizeNumbers(rawText, numberColor);
    }

    private void RefreshFromSave()
    {
        if (PlayerPrefs.GetInt(upgradeCode, 0) == 1)
        {
            button.interactable = false;

            if (image != null)
                image.color = Color.green;
        }
    }

    public void Lock()
    {
        button.interactable = false;
    }

    private void OnDestroy()
    {
        SkillTreeManager.OnUpgradeUnlocked -= OnUpgradeUnlocked;
    }

}

public static class TMPNumberColorizer
{
    // Matches +20%, -5%, +1, 30%, 100 etc.
    private static readonly Regex numberRegex =
        new Regex(@"[+-]?\d+%?", RegexOptions.Compiled);

    public static string ColorizeNumbers(string input, string hexColor)
    {
        return numberRegex.Replace(input, match =>
        {
            return $"<color={hexColor}>{match.Value}</color>";
        });
    }
}
