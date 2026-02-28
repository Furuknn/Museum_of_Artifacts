using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_AbilityButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Upgrade Buttons")]
    [SerializeField] private List<Button> relatedUpgradeButtons = new List<Button>();

    [SerializeField] private string selectedAbility;

    [Header("Scale Settings")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float scaleSpeed = 10f;

    [Header("Colors")]
    [SerializeField] private Color selectedColor = Color.green;

    private Vector3 originalScale;
    private Button abilityButton;
    private Image abilityImage;
    private bool isSelected;

    public static System.Action OnAbilitySelected;


    void Awake()
    {
        abilityButton = GetComponent<Button>();
        abilityImage = GetComponent<Image>();
        originalScale = transform.localScale;

        SetUpgradeButtonsInteractable(false);

        RefreshFromSave();
        OnAbilitySelected += RefreshFromSave;
    }

    void Update()
    {
        // Smooth scaling
        Vector3 targetScale = isHovered ? originalScale * hoverScale : originalScale;
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed
        );
    }

    private bool isHovered;

    // -------------------- EVENTS --------------------

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected || !abilityButton.interactable) return;

        isHovered = true;
        SetUpgradeButtonsInteractable(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected || !abilityButton.interactable) return;

        isHovered = false;
        SetUpgradeButtonsInteractable(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectAbility();
    }

    // -------------------- LOGIC --------------------

    private void SetUpgradeButtonsInteractable(bool value)
    {
        foreach (Button btn in relatedUpgradeButtons)
        {
            btn.interactable = value;

            // Optional visual feedback
            CanvasGroup cg = btn.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = value ? 1f : 0.5f;
            }
        }
    }

    private void SelectAbility()
    {
        if (!abilityButton.interactable) return;

        PlayerPrefs.SetString("SelectedAbility", selectedAbility);
        PlayerPrefs.Save();

        OnAbilitySelected?.Invoke();
    }

    public void RefreshFromSave()
    {
        if (!PlayerPrefs.HasKey("SelectedAbility"))
        {
            // No ability chosen yet
            abilityButton.interactable = true;
            abilityImage.color = Color.white;
            SetUpgradeButtonsInteractable(false);
            return;
        }

        string savedAbility = PlayerPrefs.GetString("SelectedAbility");

        if (savedAbility == selectedAbility)
        {
            // THIS ability is selected
            isSelected = true;
            isHovered = false;

            abilityButton.interactable = false;
            abilityImage.color = selectedColor;

            SetUpgradeButtonsInteractable(true);
        }
        else
        {
            // Another ability was selected
            isSelected = false;
            isHovered = false;

            abilityButton.interactable = false;
            SetUpgradeButtonsInteractable(false);
        }
    }

    private void OnDestroy()
    {
        OnAbilitySelected -= RefreshFromSave;
    }

}
