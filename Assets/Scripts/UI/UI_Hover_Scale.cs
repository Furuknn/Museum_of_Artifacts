// UI_Hover_Scale.cs
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Hover_Scale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Scale Settings")]
    [SerializeField] private float hoveredScale = 1.15f;
    [SerializeField] private float duration = 0.2f;

    [Header("Easing")]
    [SerializeField] private Ease hoverEase = Ease.OutBack;
    [SerializeField] private Ease returnEase = Ease.OutSine;

    [Header("Pressed Color")]
    [SerializeField] private Image image;
    [SerializeField] private Color pressedColor = Color.green;

    [Tooltip("Leave empty if this is not an ability")]
    public string abilityKeyName;

    public bool IsChosen { get; private set; }

    private Vector3 originalScale;
    private Color originalColor;
    


    private void Awake()
    {
        originalScale = transform.localScale;
        originalColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsChosen) return;
        transform.DOKill();
        transform.DOScale(originalScale * hoveredScale, duration).SetEase(hoverEase);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsChosen) return;
        transform.DOKill();
        transform.DOScale(originalScale, duration).SetEase(returnEase);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsChosen)
        {
            // clicking again deselects
            Deselect();
            AbilitySelectionManager.Instance.OnAbilityDeselected();
        }
        else
        {
            Select();
            AbilitySelectionManager.Instance.OnAbilitySelected(this);
        }
    }

    public void Select()
    {
        IsChosen = true;
        image.color = pressedColor;
        transform.DOKill();
        transform.DOScale(originalScale * hoveredScale, duration).SetEase(hoverEase);
    }

    public void Deselect()
    {
        IsChosen = false;
        image.color = originalColor;
        transform.DOKill();
        transform.DOScale(originalScale, duration).SetEase(returnEase);
    }

#if UNITY_EDITOR
    public void EditorPreviewHover()
    {
        originalScale = transform.localScale;
        transform.DOKill();
        transform.DOScale(originalScale * hoveredScale, duration).SetEase(hoverEase).SetUpdate(true);
    }

    public void EditorPreviewReturn()
    {
        transform.DOKill();
        transform.DOScale(originalScale, duration).SetEase(returnEase).SetUpdate(true);
    }
#endif
}