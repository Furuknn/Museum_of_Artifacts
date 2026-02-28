using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHighlight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text buttonText;

    [Header("Fill")]
    [SerializeField] private float fillTime = 0.4f;

    [Header("Scale")]
    [SerializeField] private float highlightScale = 1.1f;
    [SerializeField] private float scaleSpeed = 10f;

    [Header("Text Color")]
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color highlightTextColor = Color.yellow;

    private RectTransform rect;
    private Coroutine fillRoutine;

    private bool isHighlighted = false;

    private void Awake()
    {
        rect = transform as RectTransform;
        fillImage.fillAmount = 0f;
        buttonText.color = normalTextColor;
    }

    // Called when highlighted
    public void Highlight()
    {
        isHighlighted = true;


        fillImage.fillOrigin = 0;
        if (fillRoutine != null)
            StopCoroutine(fillRoutine);

        fillRoutine = StartCoroutine(FillTo(1f));

        buttonText.color = highlightTextColor;
    }

    // Called when un-highlighted
    public void UnHighlight()
    {
        isHighlighted = false;

        fillImage.fillOrigin = 1;
        if (fillRoutine != null)
            StopCoroutine(fillRoutine);

        fillRoutine = StartCoroutine(FillTo(0f));

        buttonText.color = normalTextColor;
    }

    private IEnumerator FillTo(float target)
    {
        float start = fillImage.fillAmount;
        float t = 0f;

        while (t < fillTime)
        {
            t += Time.unscaledDeltaTime;
            fillImage.fillAmount = Mathf.Lerp(start, target, t / fillTime);
            yield return null;
        }

        fillImage.fillAmount = target;
    }

    private void Update()
    {
        float targetScale = (isHighlighted) ? highlightScale : 1f;
        rect.localScale = Vector3.Lerp(
            rect.localScale,
            Vector3.one * targetScale,
            Time.unscaledDeltaTime * scaleSpeed
        );
    }
}
