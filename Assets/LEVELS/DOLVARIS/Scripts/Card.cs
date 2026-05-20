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
<<<<<<< Updated upstream
=======
    public string user = "None";
>>>>>>> Stashed changes

    [Header("Visual Settings")]
    public Image cardImage;
    public TextMeshProUGUI valueText;
<<<<<<< Updated upstream
=======
    public Image removeIndicator;
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
        if (valueText != null && cardSO.value != 0) valueText.text = cardSO.value.ToString();
=======
        valueText.text = cardSO.value;
        valueText.color = cardSO.valueColor;
>>>>>>> Stashed changes
    }
    public T GetData<T>() where T : CardComponent
    {
        return components.OfType<T>().FirstOrDefault();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
<<<<<<< Updated upstream
=======
        if (type == CardType.Fate) return;
>>>>>>> Stashed changes
        if (!CardGameManager.Instance.playersTurn || CardGameManager.Instance.playerCards.Count < CardGameManager.Instance.minCardsToPlay) return;
        if (!CardGameManager.Instance.gameStarted) return;
        if (passiveCard) return;
        if (CardGameManager.Instance.playersTurn && CardGameManager.Instance.canPlay)
        {
            CardGameManager.Instance.canPlay = false;
<<<<<<< Updated upstream
=======
            CardGameManager.Instance.actionDeckButton.interactable = false;
>>>>>>> Stashed changes
            glowEffect.GetComponent<Image>().DOColor(disabled, 0.15f);
            cardImage.DOColor(disabled, 0.15f);
            valueText.transform.DOScale(2f, 0.65f);
            valueText.DOColor(disabled, 0.65f);
            Invoke(nameof(UseCard), 1f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
<<<<<<< Updated upstream
=======
        if (type == CardType.Fate) return;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        transform.DOScale(originalScale, 0.4f);
        //transform.localScale = originalScale;
=======
        if (type == CardType.Fate) return;
        transform.DOScale(originalScale, 0.4f);
>>>>>>> Stashed changes
        if (glowEffect != null) glowEffect.GetComponent<Image>().DOColor(disabled, 0.4f);
    }
    public void UseCard()
    {
<<<<<<< Updated upstream
        if (countdownCard && !actionEveryCount && count == 1 || countdownCard && actionEveryCount || !countdownCard)
=======
        if (type != CardType.Fate) RemoveCard();
        if (!countdownCard)
>>>>>>> Stashed changes
        {
            foreach (var comp in components)
            {
                comp.Use();
<<<<<<< Updated upstream
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
=======
                Debug.LogWarning(comp.ToString() + " used!");
>>>>>>> Stashed changes
            }
        }
        else
        {
<<<<<<< Updated upstream
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
=======
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
>>>>>>> Stashed changes
    }

    private void OnDestroy()
    {
        // Hangi listede olduðunu biliyorsa kendini oradan sildirir
<<<<<<< Updated upstream
        if (CardGameManager.Instance != null)
        {
            if (CardGameManager.Instance.playerCards.Contains(this.gameObject))
                CardGameManager.Instance.playerCards.Remove(this.gameObject);

            else if (CardGameManager.Instance.dolvarisCards.Contains(this.gameObject))
                CardGameManager.Instance.dolvarisCards.Remove(this.gameObject);
        }
=======
        
>>>>>>> Stashed changes
    }
}

public enum CardType
{
    Action,
    Fate
}

