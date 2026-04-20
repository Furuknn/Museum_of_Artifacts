using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class UI_UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Upgrade ID (must match SkillTreeManager)")]
    [SerializeField] private string upgradeCode;

    [Header("Localization")]
    [SerializeField] private string localizationKey;

    [Header("Description Data")]
    [TextArea]
    [SerializeField] public string descriptionText;
    [SerializeField] private int skillCost;

    [Header("Upgrades unlocked after purchasing this one")]
    [SerializeField] private List<UI_UpgradeButton> unlocksOnPurchase;

    private string numberColor = "#4AFF4A";

    public string UpgradeCode => upgradeCode;
    public string Description => descriptionText;

    public string SkillCost
    {
        get
        {
            // Read the real cost from the source of truth
            var upgrade = SkillTreeManager.Instance?.GetUpgrade(upgradeCode);
            return upgrade != null ? upgrade.cost.ToString() : skillCost.ToString();
        }
    }

    private Button button;
    private Image image;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        button.onClick.AddListener(() => SkillTreeManager.Instance.UpgradeStat(upgradeCode));

        RefreshFromSave();
    }
    private void OnEnable()
    {
        SkillTreeManager.OnUpgradeUnlocked += OnUpgradeUnlocked;

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        ApplyDescription();
    }
    private void OnDisable()
    {
        SkillTreeManager.OnUpgradeUnlocked -= OnUpgradeUnlocked;

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void Start()
    {
        // If this upgrade itself is already purchased, skip locking its children
        if (PlayerPrefs.GetInt(upgradeCode, 0) == 1) return;

        foreach (var next in unlocksOnPurchase)
        {
            if (next == null) continue;
            if (PlayerPrefs.GetInt(next.UpgradeCode, 0) == 0)
                next.Lock();
        }
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        ApplyDescription();
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
        {
            RefreshFromSave();
            foreach (var next in unlocksOnPurchase)
                if (next != null) next.Unlock();
        }
    }


    public void Lock() => button.interactable = false;
    public void Unlock() => button.interactable = true;

    public void ApplyDescription()
    {
        string raw;

        if (!string.IsNullOrEmpty(localizationKey))
            raw = LocalizationSettings.StringDatabase.GetLocalizedString("SkillTree", localizationKey);
        else
            raw = descriptionText; // fallback to Inspector text if no key assigned

        descriptionText = TMPNumberColorizer.ColorizeNumbers(raw, numberColor);
    }

    public void SetDescription(string descText)
    {
        descriptionText = descText;
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
