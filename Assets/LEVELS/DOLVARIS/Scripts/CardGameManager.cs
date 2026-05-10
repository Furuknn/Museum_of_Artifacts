using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int minCardsToPlay = 3;
    public int maxCardsInHand = 5;
    public bool gameStarted = false;
    public bool playersTurn = true;
    public bool canPlay = true;
    public Transform playerHand;
    public Transform fateCardsParent;
    public Transform actionCardsParent;

    public static CardGameManager Instance;
    [Header("PLAYER STATS")]
    public CardPlayerCharacter player;
    public float damageMulti = 1f;
    public float defenceMulti = 1f;
    public int PLAYER_MAXHEALTH;
    public int PLAYER_MAXDEFENCE;
    public int playerHealth = 80;
    public int playerDefence = 100;
    public int playerMoveCount = 1;
    public List<GameObject> playerCards;
    public List<GameObject> playerCountdowns;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerDefenceText;

    [Header("DOLVARIS STATS")]
    public int DOLVARIS_MAXHEALTH;
    public int DOLVARIS_MAXDEFENCE;
    public int dolvarisHealth = 150;
    public int dolvarisDefence = 150;
    public int dolvarisMoveCount = 1;
    public List<GameObject> dolvarisCards;
    public List<GameObject> dolvarisCountdowns;
    public GameObject dolvarisCardObject;
    public Transform dolvarisHand;
    public TextMeshProUGUI dolvarisHealthText;
    public TextMeshProUGUI dolvarisDefenceText;

    [Header("CARDS")]
    public CardDatabase database;
    public GameObject cardPrefab;
    public List<GameObject> actionDeck;
    public List<GameObject> fateDeck;

    [Header("CHARACTERS")]
    int harper_health = 120;
    int harper_defence = 60;
    float harper_passiveDamageMultiplier = 1.2f;
    int zaddy_health = 80;
    int zaddy_defence = 100;
    float zaddy_passiveDefenceMultiplier = 1.2f;

    


    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        if (player == CardPlayerCharacter.harper)
        {
            PLAYER_MAXHEALTH = harper_health;
            PLAYER_MAXDEFENCE = harper_defence;
            playerHealth = harper_health;
            playerDefence = harper_defence;
            defenceMulti = 1f;
            damageMulti = 1.2f;
        }
        else if (player == CardPlayerCharacter.zaddy)
        {
            PLAYER_MAXHEALTH = zaddy_health;
            PLAYER_MAXDEFENCE = zaddy_defence;
            playerHealth = zaddy_health;
            playerDefence = zaddy_defence;
            damageMulti = 1f;
            defenceMulti = 1.2f;
        }

        PrepareDeck();
        UpdateUIs();
    }

    void UpdateUIs()
    {
        playerHealthText.text = playerHealth.ToString();
        playerDefenceText.text = playerDefence.ToString();
        dolvarisHealthText.text = dolvarisHealth.ToString();
        dolvarisDefenceText.text = dolvarisDefence.ToString();
    }
    void PrepareDeck()
    {

        actionDeck.Clear();
        foreach (var cardData in database.allCards)
        {
            for (int i = 0; i < cardData.inDeckAmount; i++)
            {
                Card card = Instantiate(cardPrefab).GetComponent<Card>();
                card.cardSO = cardData;
                card.InitializeCard();
                actionDeck.Add(card.gameObject);
            }
        }
        ShuffleDeck();
    }

    void ShuffleDeck()
    {
        // Desteyi karýþtýrmak çekme iþlemini kolaylaþtýrýr
        for (int i = 0; i < actionDeck.Count; i++)
        {
            GameObject temp = actionDeck[i];
            int randomIndex = Random.Range(i, actionDeck.Count);
            actionDeck[i] = actionDeck[randomIndex];
            actionDeck[randomIndex] = temp;
        }
        if (!gameStarted) StartCoroutine(DealCards(3));
    }

    IEnumerator DealCards(int count)
    {
        yield return new WaitForSeconds(2);
        bool toPlayer = false;
        for (int i = 0; i < count*2; i++)
        {
            int rnd = Random.Range(0, actionDeck.Count);
            yield return new WaitForSeconds(0.8f);
            if (toPlayer)
            {
                playerCards.Add(actionDeck[0]);
                actionDeck[0].transform.SetParent(playerHand, false);
                actionDeck.RemoveAt(0);

            }
            else
            {
                dolvarisCards.Add(actionDeck[0]);
                Instantiate(dolvarisCardObject, dolvarisHand);
                actionDeck.RemoveAt(0);

            }
            toPlayer = !toPlayer;
        }

        gameStarted = true;
    }


    void Lose()
    {

    }

    void Win()
    {

    }

    public IEnumerator ChangeTurn()
    {
        if (playersTurn && playerMoveCount > 1)
        {
            playerMoveCount--;
        }
        else if (!playersTurn && dolvarisMoveCount > 1)
        {
            dolvarisMoveCount--;
        }
        else playersTurn = !playersTurn;

        if (playersTurn)
        {
            foreach (GameObject card in playerCountdowns)
            {
                card.GetComponent<Card>().UseCard();
            }

            canPlay = true;
        }
        else
        {
            foreach (GameObject card in dolvarisCountdowns)
            {
                card.GetComponent<Card>().UseCard();
            }
        }

        if (!playersTurn) Invoke(nameof(DolvarisPlaysRandom), 1f);
        yield return null;
    }

    void DolvarisPlaysRandom()
    {
        if (dolvarisCards.Count < minCardsToPlay)
        {
            DrawCard(1);
            return;
        }
        int rnd = Random.Range(0, dolvarisCards.Count);
        while (true)
        {
            if (dolvarisCards[rnd].GetComponent<Card>().GetData<card_ignoreattack>() == null)
            {
                dolvarisCards[rnd].GetComponent<Card>().UseCard();
                break;
            }
            else rnd = Random.Range(0, dolvarisCards.Count);
        }
        Destroy(dolvarisHand.GetChild(0).gameObject);
    }
    public void DrawCard(int count)
    {
        if (playersTurn)
        {
            for (int i = 0; i < count; i++)
            {
                 playerCards.Add(actionDeck[0]);
                 actionDeck[0].transform.SetParent(playerHand, false);
                 actionDeck.RemoveAt(0);

                if (playerCards.Count > maxCardsInHand)
                {
                    Destroy(playerCards[0]);
                }
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                dolvarisCards.Add(actionDeck[0]);
                Instantiate(dolvarisCardObject, dolvarisHand);
                actionDeck.RemoveAt(0);

                if (dolvarisCards.Count > maxCardsInHand)
                {
                    Destroy(dolvarisCards[0]);
                }
            }
        }
        if (actionDeck.Count <= 0) PrepareDeck();
        StartCoroutine(ChangeTurn());
    }

    public void DestroyCountdown()
    {
        if (playersTurn)
        {
            foreach (var card in dolvarisCountdowns)
            {
                Destroy(card);
            }
            dolvarisCountdowns.Clear();
        }
        else
        {
            foreach (var card in playerCountdowns)
            {
                Destroy(card);
            }
            playerCountdowns.Clear();
        }
    }

    public void AddMove(int value)
    {
        if (playersTurn) playerMoveCount += value;
        else dolvarisMoveCount += value;
    }
    public void Heal(int value, bool healDefence, bool maxHeal)
    {
        if (playersTurn) HealPlayer(value, healDefence, maxHeal);
        else HealDolvaris(value, healDefence, maxHeal);

        UpdateUIs();
    }

    void PlayerHealthHealFeedback()
    {
        //playerHealthText.DOColor(Color.red, 0.5f).OnComplete(() => playerHealthText.DOColor(Color.green, 0.3f));
        playerHealthText.transform.DOScale(1.1f, 0.5f).OnComplete(() => playerHealthText.transform.DOScale(1f, 0.5f));
    }

    void PlayerDefenceHealFeedback()
    {
        //playerDefenceText.DOColor(Color.white, 0.3f).OnComplete(() => playerDefenceText.DOColor(Color.cyan, 0.3f));
        playerDefenceText.transform.DOScale(1.1f, 0.5f).OnComplete(() => playerDefenceText.transform.DOScale(1f, 0.5f));
    }

    public void HealPlayer(int value, bool healDefence, bool maxHeal)
    {
        if (maxHeal)
        {
            if (healDefence)
            {
                playerDefence = PLAYER_MAXDEFENCE;
                PlayerDefenceHealFeedback();
            }
            else
            {
                playerHealth = PLAYER_MAXHEALTH;
                PlayerHealthHealFeedback();
            }
            return;
        }
        if (healDefence)
        {
            playerDefence += (int)(value * defenceMulti);
            PlayerDefenceHealFeedback();
        }
        else
        {
            playerHealth += value;
            PlayerHealthHealFeedback();
        }

        if (playerDefence > PLAYER_MAXDEFENCE) playerDefence = PLAYER_MAXDEFENCE;
        if (playerHealth > PLAYER_MAXHEALTH) playerHealth = PLAYER_MAXHEALTH;

    }

    void DolvarisHealthHealFeedback()
    {
        //playerHealthText.DOColor(Color.red, 0.5f).OnComplete(() => playerHealthText.DOColor(Color.green, 0.3f));
        dolvarisHealthText.transform.DOScale(1.1f, 0.5f).OnComplete(() => dolvarisHealthText.transform.DOScale(1f, 0.5f));
    }

    void DolvarisDefenceHealFeedback()
    {
        //playerDefenceText.DOColor(Color.white, 0.3f).OnComplete(() => playerDefenceText.DOColor(Color.cyan, 0.3f));
        dolvarisDefenceText.transform.DOScale(1.1f, 0.5f).OnComplete(() => dolvarisDefenceText.transform.DOScale(1f, 0.5f));
    }

    public void HealDolvaris(int value, bool healDefence, bool maxHeal)
    {
        if (maxHeal)
        {
            if (healDefence)
            {
                dolvarisDefence = DOLVARIS_MAXDEFENCE;
                DolvarisDefenceHealFeedback();
            }
            else
            {
                dolvarisHealth = DOLVARIS_MAXHEALTH;
                DolvarisHealthHealFeedback();
            }
            return;
        }
        if (healDefence)
        {
            dolvarisDefence += value;
            DolvarisDefenceHealFeedback();
        }
        else
        {
            dolvarisHealth += value;
            DolvarisHealthHealFeedback();
        }

        if (dolvarisDefence > DOLVARIS_MAXDEFENCE) dolvarisDefence = DOLVARIS_MAXDEFENCE;
        if (dolvarisHealth > DOLVARIS_MAXHEALTH) dolvarisHealth = DOLVARIS_MAXHEALTH;
    }

    public void Damage(int damage, bool trueDamage)
    {
        if (!playersTurn)
        {
            DamagePlayer(damage, trueDamage);
        }
        else DamageDolvaris(damage, trueDamage);

        UpdateUIs();

    }

    void PlayerHealthDamageFeedback()
    {
        playerHealthText.DOColor(Color.red, 0.2f).OnComplete(() => playerHealthText.DOColor(Color.green, 0.2f));
        playerHealthText.transform.DOScale(1.25f, 0.2f).OnComplete(() => playerHealthText.transform.DOScale(1f, 0.2f));
    }

    void PlayerDefenceDamageFeedback()
    {
        playerDefenceText.DOColor(Color.white, 0.2f).OnComplete(() => playerDefenceText.DOColor(Color.cyan, 0.2f));
        playerDefenceText.transform.DOScale(1.25f, 0.2f).OnComplete(() => playerDefenceText.transform.DOScale(1f, 0.2f));
    }
    public void DamagePlayer(int damage, bool trueDamage)
    {
        foreach (GameObject card in playerCards)
        {
            if (card.GetComponent<Card>().GetData<card_ignoreattack>() != null)
            {
                card.GetComponent<Card>().UseCard();
                return;
            }
        }

        if (trueDamage)
        {
            playerHealth -= damage;

            PlayerHealthDamageFeedback();
        }
        else if (playerDefence > 0)
        {
            playerDefence -= damage;
            PlayerDefenceDamageFeedback();
            if (playerDefence < 0)
            {
                playerHealth += playerDefence;
                PlayerHealthDamageFeedback();
            }
        }
        else
        {
            playerHealth -= damage;
            PlayerHealthDamageFeedback();
        }

        UpdateUIs();

        if (playerHealth < 0) Lose();
    }

    void DolvarisHealthDamageFeedback()
    {
        dolvarisHealthText.DOColor(Color.red, 0.2f).OnComplete(() => dolvarisHealthText.DOColor(Color.green, 0.2f));
        dolvarisHealthText.transform.DOScale(1.25f, 0.2f).OnComplete(() => dolvarisHealthText.transform.DOScale(1f, 0.2f));
    }

    void DolvarisDefenceDamageFeedback()
    {
        dolvarisDefenceText.DOColor(Color.white, 0.2f).OnComplete(() => dolvarisDefenceText.DOColor(Color.cyan, 0.2f));
        dolvarisDefenceText.transform.DOScale(1.25f, 0.2f).OnComplete(() => dolvarisDefenceText.transform.DOScale(1f, 0.2f));
    }

    public void DamageDolvaris(int damage, bool trueDamage)
    {
        foreach (GameObject card in dolvarisCards)
        {
            if (card.GetComponent<Card>().GetData<card_ignoreattack>() != null)
            {
                card.GetComponent<Card>().UseCard();
                Destroy(dolvarisHand.GetChild(0).gameObject);
                return;
            }
        }

        if (trueDamage)
        {
            dolvarisHealth -= (int)(damage * damageMulti);
            DolvarisHealthDamageFeedback();

        }
        else if (dolvarisDefence > 0)
        {
            dolvarisDefence -= (int)(damage * damageMulti);
            DolvarisDefenceDamageFeedback();
            if (dolvarisDefence < 0)
            {
                dolvarisHealth += dolvarisDefence;
                DolvarisHealthDamageFeedback();
            }
        }
        else
        {
            dolvarisHealth -= (int)(damage * damageMulti);
            DolvarisHealthDamageFeedback();
        }

        UpdateUIs();

        if (dolvarisHealth < 0) Win();
    }

}

public enum CardPlayerCharacter
{
    harper,
    zaddy
}