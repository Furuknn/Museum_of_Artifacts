using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class SkipTurnButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SpriteRenderer glowEffect;
    Color onHover = new Color(1, 1, 0, 1);
    Color disabled = new Color(1, 1, 0, 0);

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CardGameManager.Instance.playersTurn || !CardGameManager.Instance.canPlay) return;
        if (!CardGameManager.Instance.gameStarted) return;
        if (glowEffect != null) glowEffect.GetComponent<SpriteRenderer>().DOColor(disabled, 0.4f);
        StartCoroutine(CardGameManager.Instance.SkipTurn());
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CardGameManager.Instance.playersTurn || !CardGameManager.Instance.canPlay) return;
        if (!CardGameManager.Instance.gameStarted) return;
        //transform.localScale = originalScale * scaleFactor;
        //if (glowEffect != null) glowEffect.SetActive(true);
        if (glowEffect != null) glowEffect.GetComponent<SpriteRenderer>().DOColor(onHover, 0.4f);
        // Kartý hiyerarþide en üste taþý ki diðer kartlarýn altýnda kalmasýn
        //transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (glowEffect != null) glowEffect.GetComponent<SpriteRenderer>().DOColor(disabled, 0.4f);
    }

    public void DisableGlow()
    {
        if (glowEffect != null) glowEffect.GetComponent<SpriteRenderer>().DOColor(disabled, 0.4f);
    }
}
