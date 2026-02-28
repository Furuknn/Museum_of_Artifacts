using System.Collections;
using UnityEngine;

public class UpgradeDescriptionManager : MonoBehaviour
{
    public static UpgradeDescriptionManager Instance;

    [SerializeField] private UI_UpgradeDescriptionPanel panelPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector2 offset = new Vector2(20f, 0f);

    private UI_UpgradeDescriptionPanel currentPanel;

    [SerializeField] private float hoverDelay = 0.15f;

    private Coroutine showRoutine;
    private UI_UpgradeButton pendingButton;

    void Awake()
    {
        Instance = this;
    }

    public void Show(UI_UpgradeButton button)
    {
        pendingButton = button;

        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showRoutine = StartCoroutine(ShowWithDelay());
    }

    IEnumerator ShowWithDelay()
    {
        yield return new WaitForSecondsRealtime(hoverDelay);

        if (pendingButton == null)
            yield break;

        HideInstant();

        currentPanel = Instantiate(panelPrefab, canvas.transform);
        currentPanel.SetDescription(pendingButton.Description, pendingButton.SkillCost);

        RectTransform buttonRect = pendingButton.GetComponent<RectTransform>();
        RectTransform panelRect = currentPanel.GetComponent<RectTransform>();

        panelRect.position = buttonRect.position + (Vector3)offset;
        ClampToScreen(panelRect);
    }

    public void Hide()
    {
        pendingButton = null;

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }

        HideInstant();
    }

    private void HideInstant()
    {
        if (currentPanel != null)
        {
            Destroy(currentPanel.gameObject);
            currentPanel = null;
        }
    }

    private void ClampToScreen(RectTransform panelRect)
    {
        Canvas.ForceUpdateCanvases();

        Vector3[] corners = new Vector3[4];
        panelRect.GetWorldCorners(corners);

        Vector3 offset = Vector3.zero;

        if (corners[0].x < 0)
            offset.x = -corners[0].x;

        if (corners[2].x > Screen.width)
            offset.x = Screen.width - corners[2].x;

        if (corners[0].y < 0)
            offset.y = -corners[0].y;

        if (corners[2].y > Screen.height)
            offset.y = Screen.height - corners[2].y;

        panelRect.position += offset;
    }
}
