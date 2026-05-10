using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CardSO cardSO;
    public CardType type;
    [SerializeReference] public List<CardComponent> components = new List<CardComponent>();
    public bool passiveCard = false;

    [Header("Visual Settings")]
    public Image cardImage;
    public TextMeshProUGUI valueText;

    [Header("Hover Settings")]
    public float scaleFactor = 1.1f; // Ne kadar büyüyecek?
    public GameObject glowEffect;   // Kenar parýltýsý objesi (baþlangýçta kapalý)
    Color onHover = Color.white;
    Color disabled = new Color(1,1,1,0);
    private Vector3 originalScale;

    [Header("Countdown Card Settings")]
    public bool countdownCard = false;
    public bool actionEveryCount = false;
    public int count = 3;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void InitializeCard()
    {
        gameObject.name = cardSO.name;
        type = cardSO.type;
        passiveCard = cardSO.passiveCard;
        cardImage.sprite = cardSO.cardSprite;
        countdownCard = cardSO.countdownCard;
        actionEveryCount = cardSO.actionEveryCount;
        count = cardSO.count;
        components.Clear();
        components = cardSO.components;
        if (valueText != null && cardSO.value != 0) valueText.text = cardSO.value.ToString();
    }
    public T GetData<T>() where T : CardComponent
    {
        return components.OfType<T>().FirstOrDefault();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CardGameManager.Instance.playersTurn || CardGameManager.Instance.playerCards.Count < CardGameManager.Instance.minCardsToPlay) return;
        if (!CardGameManager.Instance.gameStarted) return;
        if (passiveCard) return;
        if (CardGameManager.Instance.playersTurn && CardGameManager.Instance.canPlay)
        {
            CardGameManager.Instance.canPlay = false;
            glowEffect.GetComponent<Image>().DOColor(disabled, 0.15f);
            cardImage.DOColor(disabled, 0.15f);
            valueText.transform.DOScale(2f, 0.65f);
            valueText.DOColor(disabled, 0.65f);
            Invoke(nameof(UseCard), 1f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CardGameManager.Instance.playersTurn || CardGameManager.Instance.playerCards.Count < CardGameManager.Instance.minCardsToPlay || !CardGameManager.Instance.canPlay) return;
        if (!CardGameManager.Instance.gameStarted) return;
        if (passiveCard) return;
        transform.DOScale(originalScale * scaleFactor, 0.4f);
        //transform.localScale = originalScale * scaleFactor;
        //if (glowEffect != null) glowEffect.SetActive(true);
        if (glowEffect != null) glowEffect.GetComponent<Image>().DOColor(onHover, 0.4f);
        // Kartý hiyerarþide en üste taþý ki diðer kartlarýn altýnda kalmasýn
        //transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, 0.4f);
        //transform.localScale = originalScale;
        if (glowEffect != null) glowEffect.GetComponent<Image>().DOColor(disabled, 0.4f);
    }
    public void UseCard()
    {
        if (countdownCard && !actionEveryCount && count == 1 || countdownCard && actionEveryCount || !countdownCard)
        {
            foreach (var comp in components)
            {
                comp.Use();
            }
        }


        if (countdownCard)
        {
            if (CardGameManager.Instance.playersTurn) CardGameManager.Instance.playerCountdowns.Add(gameObject);
            else CardGameManager.Instance.dolvarisCountdowns.Add(gameObject);
            count--;
            if (count == 0)
            {
                //if (CardGameManager.Instance.playersTurn) CardGameManager.Instance.playerCountdowns.Remove(gameObject);
                //else CardGameManager.Instance.dolvarisCountdowns.Remove(gameObject);
            }
        }
        else
        {
            //if (CardGameManager.Instance.playersTurn) CardGameManager.Instance.playerCards.Remove(gameObject);
            //else CardGameManager.Instance.dolvarisCards.Remove(gameObject);
        }

        if (!countdownCard && !passiveCard) StartCoroutine(CardGameManager.Instance.ChangeTurn());

        if (countdownCard && count == 0 || !countdownCard) RemoveCard();
        
    }

    void RemoveCard()
    {
        cardImage.enabled = false;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Hangi listede olduðunu biliyorsa kendini oradan sildirir
        if (CardGameManager.Instance != null)
        {
            if (CardGameManager.Instance.playerCards.Contains(this.gameObject))
                CardGameManager.Instance.playerCards.Remove(this.gameObject);

            else if (CardGameManager.Instance.dolvarisCards.Contains(this.gameObject))
                CardGameManager.Instance.dolvarisCards.Remove(this.gameObject);
        }
    }
}

public enum CardType
{
    Action,
    Fate
}

