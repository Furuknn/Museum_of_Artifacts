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
    public string user = "None";

    [Header("Visual Settings")]
    public Image cardImage;
    public TextMeshProUGUI valueText;
    public Image removeIndicator;

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
        valueText.text = cardSO.value;
        valueText.color = cardSO.valueColor;
    }
    public T GetData<T>() where T : CardComponent
    {
        return components.OfType<T>().FirstOrDefault();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (type == CardType.Fate) return;
        if (!CardGameManager.Instance.playersTurn || CardGameManager.Instance.playerCards.Count < CardGameManager.Instance.minCardsToPlay) return;
        if (!CardGameManager.Instance.gameStarted) return;
        if (passiveCard) return;
        if (CardGameManager.Instance.playersTurn && CardGameManager.Instance.canPlay)
        {
            CardGameManager.Instance.canPlay = false;
            CardGameManager.Instance.actionDeckButton.interactable = false;
            glowEffect.GetComponent<Image>().DOColor(disabled, 0.15f);
            cardImage.DOColor(disabled, 0.15f);
            valueText.transform.DOScale(2f, 0.65f);
            valueText.DOColor(disabled, 0.65f);
            Invoke(nameof(UseCard), 1f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (type == CardType.Fate) return;
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
        if (type == CardType.Fate) return;
        transform.DOScale(originalScale, 0.4f);
        if (glowEffect != null) glowEffect.GetComponent<Image>().DOColor(disabled, 0.4f);
    }
    public void UseCard()
    {
        if (type != CardType.Fate) RemoveCard();
        CardGameManager.Instance.source.PlayOneShot(CardGameManager.Instance.playCardSound);
        if (!countdownCard)
        {
            foreach (var comp in components)
            {
                comp.Use();
                Debug.LogWarning(comp.ToString() + " used!");
            }
        }
        else
        {
            CardCountdown countdown;
            if (actionEveryCount) countdown = Instantiate(CardGameManager.Instance.countdownPrefab).GetComponent<CardCountdown>();
            else countdown = Instantiate(CardGameManager.Instance.bombPrefab).GetComponent<CardCountdown>();
            countdown.cardSO = cardSO;
            countdown.InitializeCountdown();
            if (user == "Player")
            {
                CardGameManager.Instance.playerCountdowns.Add(countdown.gameObject);
                countdown.transform.SetParent(CardGameManager.Instance.playerCountdownParent);
            }
            else
            {
                CardGameManager.Instance.dolvarisCountdowns.Add(countdown.gameObject);
                countdown.transform.SetParent(CardGameManager.Instance.dolvarisCountdownParent);
            }
        }

        if (!passiveCard && type != CardType.Fate) StartCoroutine(CardGameManager.Instance.ChangeTurn());

        
        
    }

    public void RemoveCard()
    {
        cardImage.enabled = false;
        if (CardGameManager.Instance != null)
        {
            if (CardGameManager.Instance.playerCards.Contains(this.gameObject))
            {
                CardGameManager.Instance.playerCards.Remove(this.gameObject);
                transform.SetParent(null);
            }
               

            else if (CardGameManager.Instance.dolvarisCards.Contains(this.gameObject))
            {
                CardGameManager.Instance.dolvarisCards.Remove(this.gameObject);
            }
                
        }
    }

    private void OnDestroy()
    {
        // Hangi listede olduðunu biliyorsa kendini oradan sildirir
        
    }
}

public enum CardType
{
    Action,
    Fate
}

