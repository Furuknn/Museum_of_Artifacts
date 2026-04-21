// AbilitySelectionManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectionManager : MonoBehaviour
{
    public static AbilitySelectionManager Instance { get; private set; }

    public static System.Action OnAbilityConfirmed;

    private const string AbilityKey = "SelectedAbility";

    [System.Serializable]
    public class CharacterAbilityGroup
    {
        public string characterKey; // e.g. "NightStick", "Flashlight"
        public List<UI_Hover_Scale> abilityButtons;
        public Button confirmButton;
    }

    [SerializeField] private List<CharacterAbilityGroup> characterGroups;

    private CharacterAbilityGroup activeGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (PlayerPrefs.HasKey(AbilityKey))
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);

        foreach (var group in characterGroups)
            if (group.confirmButton != null)
            {
                group.confirmButton.interactable = false;
                group.confirmButton.onClick.AddListener(() => OnConfirmPressed());
            }
    }

    // Call this when the active character/tree changes
    public void SetActiveCharacter(int heroIndex)
    {
        if (heroIndex < 0 || heroIndex >= characterGroups.Count) return;
        activeGroup = characterGroups[heroIndex];
    }

    public void OnAbilitySelected(UI_Hover_Scale selected)
    {
        if (activeGroup == null) return;

        foreach (var btn in activeGroup.abilityButtons)
            if (btn != selected) btn.Deselect();

        activeGroup.confirmButton.interactable = true;
    }

    public void OnAbilityDeselected()
    {
        if (activeGroup == null) return;

        bool anyChosen = false;
        foreach (var btn in activeGroup.abilityButtons)
            if (btn.IsChosen) { anyChosen = true; break; }

        activeGroup.confirmButton.interactable = anyChosen;
    }

    public void OnConfirmPressed()
    {
        if (activeGroup == null) return;

        foreach (var btn in activeGroup.abilityButtons)
            if (btn.IsChosen && btn.abilityKeyName != null)
                PlayerPrefs.SetString(AbilityKey, btn.abilityKeyName);

        PlayerPrefs.Save();
        OnAbilityConfirmed?.Invoke();
    }

    public void ResetSkillTree()
    {
        SkillTreeManager.Instance.DeleteUpgradePrefs();
        SkillTreeManager.Instance.UpdateSkillTreeUI();
    }

    
}