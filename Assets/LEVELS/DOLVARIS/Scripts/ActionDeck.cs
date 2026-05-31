using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionDeck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image glowEffect;
    Color onHover = new Color(1, 1, 1, 1);
    Color disabled = new Color(1, 1, 1, 0);
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CardGameManager.Instance.playersTurn || !CardGameManager.Instance.canPlay) return;
        if (!CardGameManager.Instance.gameStarted) return;
        //transform.localScale = originalScale * scaleFactor;
        //if (glowEffect != null) glowEffect.SetActive(true);
        if (glowEffect != null) glowEffect.GetComponent<Image>().DOColor(onHover, 0.4f);
        // Kartý hiyerarþide en üste taþý ki diðer kartlarýn altýnda kalmasýn
        //transform.SetAsLastSibling();
        if (CardGameManager.Instance.playerCards.Count >= CardGameManager.Instance.maxCardsInHand)
        {
            if(CardGameManager.Instance.playerCards[0].GetComponent<Card>().removeIndicator != null) CardGameManager.Instance.playerCards[0].GetComponent<Card>().removeIndicator.DOColor(onHover, 0.5f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (glowEffect != null) glowEffect.GetComponent<Image>().DOColor(disabled, 0.4f);
        if (CardGameManager.Instance.playerCards[0].GetComponent<Card>().removeIndicator != null) CardGameManager.Instance.playerCards[0].GetComponent<Card>().removeIndicator.DOColor(disabled, 0.5f);
    }

    public void DisableGlow()
    {
        if (glowEffect != null) glowEffect.GetComponent<Image>().DOColor(disabled, 0.4f);
    }
}
