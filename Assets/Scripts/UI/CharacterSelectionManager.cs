using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

[System.Serializable]
public class CharacterInfo
{
    public string characterName;
    public string characterKey; // "flashlight", "nightstick", "taser"
    public Sprite characterImage;
    public int age;
    public int secondaryAbilityCount; // how many secondaries this character has
}

public class CharacterSelectionManager : MonoBehaviour
{
    [SerializeField] private List<CharacterInfo> characters;

    [Header("Panels")]
    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private GameObject characterDetailsPanel;

    [Header("Character Select Buttons (order must match characters list)")]
    [SerializeField] private List<Button> characterButtons;

    [Header("UI Text References")]
    [SerializeField] private Image characterDetailImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text ageText;
    [SerializeField] private TMP_Text weaponText;
    [SerializeField] private TMP_Text mainAbilityNameText;
    [SerializeField] private TMP_Text mainAbilityDescText;
    [SerializeField] private List<TMP_Text> secondaryAbilityNameTexts;
    [SerializeField] private List<TMP_Text> secondaryAbilityDescTexts;
    [SerializeField] private TMP_Text ultimateAbilityNameText;
    [SerializeField] private TMP_Text ultimateAbilityDescText;

    [Header("Back Button")]
    [SerializeField] private Button backButton;

    private int currentCharacterIndex = -1;

    private void Awake()
    {
        for (int i = 0; i < characterButtons.Count; i++)
        {
            int index = i; // capture for lambda
            characterButtons[i].onClick.AddListener(() => OnCharacterSelected(index));
        }

        if (backButton != null)
            backButton.onClick.AddListener(OnBack);
    }

    private void Start()
    {
        characterSelectionPanel.SetActive(true);
        characterDetailsPanel.SetActive(false);
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        if (currentCharacterIndex >= 0)
            DisplayCharacter(currentCharacterIndex);
    }

    private void OnCharacterSelected(int index)
    {
        if (index < 0 || index >= characters.Count) return;
        currentCharacterIndex = index;
        DisplayCharacter(index);
        characterSelectionPanel.SetActive(false);
        characterDetailsPanel.SetActive(true);
    }

    private void OnBack()
    {
        characterSelectionPanel.SetActive(true);
        characterDetailsPanel.SetActive(false);
    }

    private void DisplayCharacter(int index)
    {
        CharacterInfo c = characters[index];
        string ck = c.characterKey; // e.g. "flashlight"

        if (characterDetailImage != null)
            characterDetailImage.sprite = c.characterImage;

        // Static label keys (same for all characters)
        if (nameText != null)
            nameText.text = c.characterName;

        if (ageText != null)
            ageText.text = "" + c.age;

        if (weaponText != null)
            weaponText.text = LocalizationSettings.StringDatabase
                .GetLocalizedString("MainMenu", $"UI.heroWeapon.desc.{ck}");

        if (mainAbilityNameText != null)
            mainAbilityNameText.text = LocalizationSettings.StringDatabase
                .GetLocalizedString("MainMenu", $"UI.heroMainAbility.{ck}");

        if (mainAbilityDescText != null)
            mainAbilityDescText.text = LocalizationSettings.StringDatabase
                .GetLocalizedString("MainMenu", $"UI.heroMainAbility.desc.{ck}");

        if (ultimateAbilityNameText != null)
            ultimateAbilityNameText.text = LocalizationSettings.StringDatabase
                .GetLocalizedString("MainMenu", $"UI.heroUltimateAbility.{ck}");

        if (ultimateAbilityDescText != null)
            ultimateAbilityDescText.text = LocalizationSettings.StringDatabase
                .GetLocalizedString("MainMenu", $"UI.heroUltimateAbility.desc.{ck}");

        // Secondary abilities — keys are 01, 02, 03...
        for (int i = 0; i < secondaryAbilityNameTexts.Count; i++)
        {
            string index2Digit = (i + 1).ToString("D2"); // 01, 02, 03
            bool hasAbility = i < c.secondaryAbilityCount;

            if (secondaryAbilityNameTexts[i] != null)
                secondaryAbilityNameTexts[i].text = hasAbility
                    ? LocalizationSettings.StringDatabase
                        .GetLocalizedString("MainMenu", $"UI.heroSecondaryAbility{index2Digit}.{ck}")
                    : "";

            if (i < secondaryAbilityDescTexts.Count && secondaryAbilityDescTexts[i] != null)
                secondaryAbilityDescTexts[i].text = hasAbility
                    ? LocalizationSettings.StringDatabase
                        .GetLocalizedString("MainMenu", $"UI.heroSecondaryAbility{index2Digit}.desc.{ck}")
                    : "";
        }
    }

    private void OnDestroy()
    {
        foreach (var btn in characterButtons)
            btn.onClick.RemoveAllListeners();

        if (backButton != null)
            backButton.onClick.RemoveListener(OnBack);
    }
}