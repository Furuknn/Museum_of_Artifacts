// AbilitySelectionManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectionManager : MonoBehaviour
{
    public static AbilitySelectionManager Instance { get; private set; }

    [SerializeField] private List<UI_Hover_Scale> abilityButtons;
    public Button confirmButton;

    public static System.Action OnAbilityConfirmed;

    private const string AbilityKey = "SelectedAbility";

    private void Awake()
    {
        if (confirmButton == null) Debug.LogWarning("null!!");
        Instance = this;
        confirmButton.interactable = false;
    }

    public void OnAbilitySelected(UI_Hover_Scale selected)
    {
        foreach (UI_Hover_Scale btn in abilityButtons)
        {
            if (btn != selected)
                btn.Deselect();
        }

        confirmButton.interactable = true;
    }

    public void OnAbilityDeselected()
    {
        bool anyChosen = false;
        foreach (UI_Hover_Scale btn in abilityButtons)
        {
            if (btn.IsChosen) { anyChosen = true; break; }
        }

        confirmButton.interactable=anyChosen;   
    }

    public void OnConfirmPressed()
    {
        foreach(UI_Hover_Scale btn in abilityButtons)
        {
            if (btn.IsChosen && btn.abilityKeyName!=null) PlayerPrefs.SetString(AbilityKey, btn.abilityKeyName);
        }
        PlayerPrefs.Save();
        OnAbilityConfirmed?.Invoke();
    }
}