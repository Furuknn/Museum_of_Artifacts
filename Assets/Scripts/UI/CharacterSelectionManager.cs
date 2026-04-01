using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterInfo
{
    public string characterName;
    public Sprite characterImage;
    public int age;
    public string weaponName;
    public string mainAbilityName;
    [TextArea] public string mainAbilityDescription;
    public List<AbilityInfo> secondaryAbilities;
    public string ultimateAbilityName;
    [TextArea] public string ultimateAbilityDescription;
}

[System.Serializable]
public class AbilityInfo
{
    public string abilityName;
    [TextArea] public string abilityDescription;
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

    private void OnCharacterSelected(int index)
    {
        if (index < 0 || index >= characters.Count) return;

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

        if (characterDetailImage != null) characterDetailImage.sprite = c.characterImage;
        if (nameText != null) nameText.text = c.characterName;
        if (ageText != null) ageText.text = c.age.ToString();
        if (weaponText != null) weaponText.text = c.weaponName;
        if (mainAbilityNameText != null) mainAbilityNameText.text = c.mainAbilityName;
        if (mainAbilityDescText != null) mainAbilityDescText.text = c.mainAbilityDescription;
        if (ultimateAbilityNameText != null) ultimateAbilityNameText.text = c.ultimateAbilityName;
        if (ultimateAbilityDescText != null) ultimateAbilityDescText.text = c.ultimateAbilityDescription;

        for (int i = 0; i < secondaryAbilityNameTexts.Count; i++)
        {
            bool hasAbility = c.secondaryAbilities != null && i < c.secondaryAbilities.Count;

            if (secondaryAbilityNameTexts[i] != null)
                secondaryAbilityNameTexts[i].text = hasAbility ? c.secondaryAbilities[i].abilityName : "";

            if (i < secondaryAbilityDescTexts.Count && secondaryAbilityDescTexts[i] != null)
                secondaryAbilityDescTexts[i].text = hasAbility ? c.secondaryAbilities[i].abilityDescription : "";
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