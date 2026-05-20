<<<<<<< Updated upstream
using System.Collections;
=======
 using System.Collections;
>>>>>>> Stashed changes
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
<<<<<<< Updated upstream
=======
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;
using UnityEditor.Localization.Plugins.XLIFF.V12;
>>>>>>> Stashed changes

public class CardGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int minCardsToPlay = 3;
    public int maxCardsInHand = 5;
<<<<<<< Updated upstream
    public bool gameStarted = false;
    public bool playersTurn = true;
    public bool canPlay = true;
    public Transform playerHand;
    public Transform fateCardsParent;
    public Transform actionCardsParent;

    public static CardGameManager Instance;
=======
    public int dolvarisMaxCards = 5;
    public bool gameStarted = false;
    bool gameEnded = false;
    public bool playersTurn = true;
    public bool canPlay = true;
    
    public Transform actionCardsParent;
    public Transform turnIndicator;
    public Material moveActiveMat;
    public Material moveDisabledMat;
    public List<MeshRenderer> playerMoveIndicators;
    public List<MeshRenderer> dolvarisMoveIndicators;
    public UnityEngine.UI.Button actionDeckButton;
    public GameObject startScreen;
    public GameObject winScreen;
    public GameObject loseScreen;

    public static CardGameManager Instance;

    [Header("VISUALS")]
    public Light playerEffectLight;
    public Light dolvarisEffectLight;
    public TextMeshProUGUI cardEffect;

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
    public Transform playerHand;
    public Transform playerCountdownParent;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
    public Transform dolvarisCountdownParent;
>>>>>>> Stashed changes
    public TextMeshProUGUI dolvarisHealthText;
    public TextMeshProUGUI dolvarisDefenceText;

    [Header("CARDS")]
    public CardDatabase database;
    public GameObject cardPrefab;
<<<<<<< Updated upstream
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
=======
    public GameObject fatePrefab;
    public GameObject countdownPrefab;
    public GameObject bombPrefab;
    public List<GameObject> actionDeck;
    public List<GameObject> fateDeck;
    public Transform fateDeckPos;
    public Transform fateCardParent;
    public Transform fateDisplayPos;
    public CardSO harperCard;
    public CardSO zaddyCard;

    [Header("FATES")]
    public Card currentFate;
    public Coroutine fateRoutine;
    public bool noDefence = false;
    public bool noHeal = false;
    public float fateDamageMulti = 1f;
    public int drawAndHeal = 0;
    public float lifeStealPercentage = 0f;
    public float thornsDamagePercentage = 0f;
    public float healBonusPercentage = 0f;
    public float defenceBonusPercentage = 0f;

    [Header("CHARACTERS")]
    int harper_health = 100;
    int harper_defence = 50;
    float harper_passiveDamageMultiplier = 1.2f;
    public GameObject harperText;
    int zaddy_health = 100;
    int zaddy_defence = 100;
    float zaddy_passiveDefenceMultiplier = 1.2f;
    public GameObject zaddyText;


    public void OnClick_RestartGame()
    {
        SceneManager.LoadScene("dolvarisScene");
    }

    public void OnClick_ChooseCharacter(string player)
    {
        if (player == "Harper") this.player = CardPlayerCharacter.harper;
        else if (player == "Zaddy") this.player = CardPlayerCharacter.zaddy;

        StartGame();
    }

    public void StartGame()
>>>>>>> Stashed changes
    {
        if (player == CardPlayerCharacter.harper)
        {
            PLAYER_MAXHEALTH = harper_health;
            PLAYER_MAXDEFENCE = harper_defence;
            playerHealth = harper_health;
            playerDefence = harper_defence;
            defenceMulti = 1f;
<<<<<<< Updated upstream
            damageMulti = 1.2f;
=======
            damageMulti = harper_passiveDamageMultiplier;
            harperText.SetActive(true);
>>>>>>> Stashed changes
        }
        else if (player == CardPlayerCharacter.zaddy)
        {
            PLAYER_MAXHEALTH = zaddy_health;
            PLAYER_MAXDEFENCE = zaddy_defence;
            playerHealth = zaddy_health;
            playerDefence = zaddy_defence;
            damageMulti = 1f;
<<<<<<< Updated upstream
            defenceMulti = 1.2f;
        }

        PrepareDeck();
        UpdateUIs();
    }

    void UpdateUIs()
=======
            defenceMulti = zaddy_passiveDefenceMultiplier;
            zaddyText.SetActive(true);
        }

        PrepareDeck();



        UpdateUIs();
    }
    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        startScreen.SetActive(true);
    }

    void ResetFate()
    {
        if (player == CardPlayerCharacter.harper)
        {
            PLAYER_MAXHEALTH = harper_health;
            PLAYER_MAXDEFENCE = harper_defence;
            defenceMulti = 1f;
            damageMulti = harper_passiveDamageMultiplier;
        }
        else if (player == CardPlayerCharacter.zaddy)
        {
            PLAYER_MAXHEALTH = zaddy_health;
            PLAYER_MAXDEFENCE = zaddy_defence;
            damageMulti = 1f;
            defenceMulti = zaddy_passiveDefenceMultiplier;
        }

        maxCardsInHand = 5;
        noDefence = false;
        noHeal = false;
        fateDamageMulti = 1f;
        drawAndHeal = 0;
        lifeStealPercentage = 0;
        thornsDamagePercentage = 0;
        healBonusPercentage = 0;
        defenceBonusPercentage = 0;
    }

    public void UpdateUIs()
>>>>>>> Stashed changes
    {
        playerHealthText.text = playerHealth.ToString();
        playerDefenceText.text = playerDefence.ToString();
        dolvarisHealthText.text = dolvarisHealth.ToString();
        dolvarisDefenceText.text = dolvarisDefence.ToString();
    }
<<<<<<< Updated upstream
    void PrepareDeck()
    {

        actionDeck.Clear();
        foreach (var cardData in database.allCards)
=======

    public void PickFate()
    {
        fateRoutine = StartCoroutine(PickFateRoutine());
    }

    public IEnumerator PickFateRoutine()
    {
        canPlay = false;
        if (currentFate != null)
        {
            currentFate.cardImage.DOColor(new Color(1,1,1,0), 0.5f);
            currentFate.valueText.DOColor(new Color(1, 1, 1, 0), 0.5f);
            yield return new WaitForSeconds(0.5f);
            currentFate.RemoveCard();
        }
        ResetFate();
        GameObject fate = fateDeck[0];
        
        currentFate = fate.GetComponent<Card>();

        yield return new WaitForSeconds(1f);
        fate.transform.SetParent(fateCardParent);
        fate.transform.DOMove(fateDisplayPos.position, 1f);
        fate.transform.DORotateQuaternion(fateDisplayPos.rotation, 1f);
        fateDeck.RemoveAt(0);
        currentFate.UseCard();
        UpdateUIs();
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));

        fate.transform.DOMove(fateCardParent.position, 1f);
        fate.transform.DORotateQuaternion(fateCardParent.rotation, 1f);

        UpdateUIs();
        
        yield return new WaitForSeconds(1f);

        gameStarted = true;
        fateRoutine = null;
        if (playersTurn) canPlay = true;
        if (playersTurn) actionDeckButton.interactable = true;
    }
    void PrepareDeck()
    {
        actionDeck.Clear();
        fateDeck.Clear();
        foreach (var cardData in database.actionCards)
>>>>>>> Stashed changes
        {
            for (int i = 0; i < cardData.inDeckAmount; i++)
            {
                Card card = Instantiate(cardPrefab).GetComponent<Card>();
                card.cardSO = cardData;
                card.InitializeCard();
                actionDeck.Add(card.gameObject);
            }
        }
<<<<<<< Updated upstream
        ShuffleDeck();
    }

=======
        foreach (var cardData in database.fateCards)
        {
            for (int i = 0; i < cardData.inDeckAmount; i++)
            {
                Card card = Instantiate(fatePrefab).GetComponent<Card>();
                card.cardSO = cardData;
                card.InitializeCard();
                fateDeck.Add(card.gameObject);
                card.transform.position = fateDeckPos.position;
                card.transform.rotation = fateDeckPos.rotation;
            }
        }
        ShuffleFates();
        ShuffleDeck();
        
    }

    void ShuffleFates()
    {
        // Desteyi karýþtýrmak çekme iþlemini kolaylaþtýrýr
        for (int i = 0; i < fateDeck.Count; i++)
        {
            GameObject temp = fateDeck[i];
            int randomIndex = Random.Range(i, fateDeck.Count);
            fateDeck[i] = fateDeck[randomIndex];
            fateDeck[randomIndex] = temp;
        }

    }
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        if (!gameStarted) StartCoroutine(DealCards(3));
    }

=======

        CardEffect("Cards over! Shuffling new deck...", Color.white, 0.9f);

        Invoke(nameof(DrawCharacterCard),1f);
        
        if (!gameStarted) StartCoroutine(DealCards(3));
    }

    public IEnumerator SkipTurn()
    {
        actionDeckButton.interactable = false;
        canPlay = false;
        if (playersTurn) playerMoveCount++;
        else dolvarisMoveCount++;
        if (playerMoveCount > 5) playerMoveCount = 5;
        if (dolvarisMoveCount > 5) dolvarisMoveCount = 5;
        playersTurn = !playersTurn;

        if (playersTurn)
        {
            foreach (MeshRenderer mesh in dolvarisMoveIndicators)
            {
                mesh.material = moveDisabledMat;
            }
        }
        else
        {
            foreach (MeshRenderer mesh in playerMoveIndicators)
            {
                mesh.material = moveDisabledMat;
            }
        }

        if (playersTurn) turnIndicator.DOLocalRotate(new Vector3(0, 180, 0), 1f);
        else turnIndicator.DOLocalRotate(Vector3.zero, 1f);
        yield return new WaitForSeconds(1f);
        if (playersTurn) Debug.LogWarning("PLAYERS TURN.");
        else Debug.LogWarning("DOLVARIS TURN.");
        if (playersTurn)
        {
            foreach (var count in playerCountdowns)
            {
                yield return new WaitForSeconds(0.5f);
                if (count.activeSelf) count.GetComponent<CardCountdown>().CountdownEvent();
                yield return new WaitForSeconds(0.5f);
            }
            playerCountdowns.RemoveAll(item => !item.activeSelf);
        }
        else
        {
            foreach (var count in dolvarisCountdowns)
            {
                yield return new WaitForSeconds(0.5f);
                count.GetComponent<CardCountdown>().CountdownEvent();
                yield return new WaitForSeconds(0.5f);
            }
            dolvarisCountdowns.RemoveAll(item => !item.activeSelf);
        }
        AdjustMoveIndicators();

        if (!playersTurn) Invoke(nameof(DolvarisPlaysRandom), 2f);
        else
        {
            canPlay = true;
            actionDeckButton.interactable = true;
        }
    }

    public void AdjustHandCount(int count)
    {
        maxCardsInHand = count;

        if (playerCards.Count > maxCardsInHand)
        {
            int r = playerCards.Count - maxCardsInHand;
            for (int i = 0; i < r; i++)
            {
                playerCards[i].GetComponent<Card>().RemoveCard();
            }
        }
    }

    void DrawCharacterCard()
    {
        if (player == CardPlayerCharacter.harper)
        {
            Card card = Instantiate(cardPrefab).GetComponent<Card>();
            card.cardSO = harperCard;
            card.InitializeCard();
            playerCards.Add(card.gameObject);
            card.gameObject.transform.SetParent(playerHand, false);
        }
        else if (player == CardPlayerCharacter.zaddy)
        {
            Card card = Instantiate(cardPrefab).GetComponent<Card>();
            card.cardSO = zaddyCard;
            card.InitializeCard();
            playerCards.Add(card.gameObject);
            card.gameObject.transform.SetParent(playerHand, false);
        }
        if (playerCards.Count > maxCardsInHand) playerCards[0].GetComponent<Card>().RemoveCard();
    }
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
                actionDeck[0].GetComponent<Card>().user = "Player";
>>>>>>> Stashed changes
                playerCards.Add(actionDeck[0]);
                actionDeck[0].transform.SetParent(playerHand, false);
                actionDeck.RemoveAt(0);

            }
            else
            {
<<<<<<< Updated upstream
=======
                actionDeck[0].GetComponent<Card>().user = "Dolvaris";
>>>>>>> Stashed changes
                dolvarisCards.Add(actionDeck[0]);
                Instantiate(dolvarisCardObject, dolvarisHand);
                actionDeck.RemoveAt(0);

            }
            toPlayer = !toPlayer;
        }
<<<<<<< Updated upstream

        gameStarted = true;
    }


    void Lose()
    {

=======
        //gameStarted = true;
        AdjustMoveIndicators();
        PickFate();
        //if (playersTurn && gameStarted) actionDeckButton.interactable = true;
        
    }

    public void AdjustMoveIndicators()
    {
        if (playersTurn)
        {
            int i = 0;
            foreach (MeshRenderer ind in playerMoveIndicators)
            {
                if (i < playerMoveCount) ind.material = moveActiveMat;
                else ind.material = moveDisabledMat;
                i++;
            }
        }
        else
        {
            int i = 0;
            foreach (MeshRenderer ind in dolvarisMoveIndicators)
            {
                if (i < dolvarisMoveCount) ind.material = moveActiveMat;
                else ind.material = moveDisabledMat;
                i++;
            }
        }
    }
    void Lose()
    {
        gameEnded = true;
        canPlay = false;
        loseScreen.SetActive(true);
>>>>>>> Stashed changes
    }

    void Win()
    {
<<<<<<< Updated upstream

=======
        gameEnded = true;
        canPlay = false;
        winScreen.SetActive(true);
>>>>>>> Stashed changes
    }

    public IEnumerator ChangeTurn()
    {
<<<<<<< Updated upstream
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
=======
        if (gameEnded) yield break;

        yield return new WaitUntil(() => fateRoutine == null);

        if (playersTurn && playerMoveCount > 1)
        {
            playerMoveCount--;
            canPlay = true;
            actionDeckButton.interactable = true;
            AdjustMoveIndicators();
            yield break;
        }
        else if (!playersTurn && dolvarisMoveCount > 1)
        {
            actionDeckButton.interactable = false;
            dolvarisMoveCount--;
            Invoke(nameof(DolvarisPlaysRandom), 2f);
            AdjustMoveIndicators();
            yield break;
        }
        else playersTurn = !playersTurn;

        

        if (playersTurn)
        {
            foreach (MeshRenderer mesh in dolvarisMoveIndicators)
            {
                mesh.material = moveDisabledMat;
            }
        }
        else
        {
            foreach (MeshRenderer mesh in playerMoveIndicators)
            {
                mesh.material = moveDisabledMat;
            }
        }
        if (playersTurn) turnIndicator.DOLocalRotate(new Vector3(0, 180, 0), 1f);
        else turnIndicator.DOLocalRotate(Vector3.zero, 1f);
        yield return new WaitForSeconds(1f);
        if (playersTurn) Debug.LogWarning("PLAYERS TURN.");
        else Debug.LogWarning("DOLVARIS TURN.");
        if (playersTurn)
        {
            foreach (var count in playerCountdowns)
            {
                yield return new WaitForSeconds(0.5f);
                if (count.activeSelf) count.GetComponent<CardCountdown>().CountdownEvent();
                yield return new WaitForSeconds(0.5f);
            }
            playerCountdowns.RemoveAll(item => !item.activeSelf);
        }
        else
        {
            foreach (var count in dolvarisCountdowns)
            {
                yield return new WaitForSeconds(0.5f);
                count.GetComponent<CardCountdown>().CountdownEvent();
                yield return new WaitForSeconds(0.5f);
            }
            dolvarisCountdowns.RemoveAll(item => !item.activeSelf);
        }
        AdjustMoveIndicators();

        if (!playersTurn) Invoke(nameof(DolvarisPlaysRandom), 2f);
        else
        {
            canPlay = true;
            actionDeckButton.interactable=true;
        }
>>>>>>> Stashed changes
    }

    void DolvarisPlaysRandom()
    {
<<<<<<< Updated upstream
=======
        if (gameEnded) return;

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    public void DrawCard(int count)
    {
        if (playersTurn)
        {
            for (int i = 0; i < count; i++)
            {
                 playerCards.Add(actionDeck[0]);
=======

    public void DrawCard(int count)
    {
        actionDeckButton.interactable = false;
        if (playersTurn && canPlay)
        {
            for (int i = 0; i < count; i++)
            {
                canPlay = false;
                actionDeck[0].GetComponent<Card>().user = "Player";
                playerCards.Add(actionDeck[0]);
>>>>>>> Stashed changes
                 actionDeck[0].transform.SetParent(playerHand, false);
                 actionDeck.RemoveAt(0);

                if (playerCards.Count > maxCardsInHand)
                {
<<<<<<< Updated upstream
                    Destroy(playerCards[0]);
                }
=======
                    playerCards[0].GetComponent<Card>().RemoveCard();
                }

                if (drawAndHeal > 0) HealPlayer(drawAndHeal, false, false);
                UpdateUIs();
>>>>>>> Stashed changes
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
<<<<<<< Updated upstream
=======
                actionDeck[0].GetComponent<Card>().user = "Dolvaris";
>>>>>>> Stashed changes
                dolvarisCards.Add(actionDeck[0]);
                Instantiate(dolvarisCardObject, dolvarisHand);
                actionDeck.RemoveAt(0);

<<<<<<< Updated upstream
                if (dolvarisCards.Count > maxCardsInHand)
                {
                    Destroy(dolvarisCards[0]);
=======
                if (dolvarisCards.Count > dolvarisMaxCards)
                {
                    dolvarisCards[0].GetComponent<Card>().RemoveCard();
>>>>>>> Stashed changes
                }
            }
        }
        if (actionDeck.Count <= 0) PrepareDeck();
        StartCoroutine(ChangeTurn());
    }

<<<<<<< Updated upstream
=======
    public void DrawCardEvent(int count)
    {
        if (playersTurn)
        {
            for (int i = 0; i < count; i++)
            {
                actionDeck[0].GetComponent<Card>().user = "Player";
                playerCards.Add(actionDeck[0]);
                actionDeck[0].transform.SetParent(playerHand, false);
                actionDeck.RemoveAt(0);

                if (playerCards.Count > maxCardsInHand)
                {
                    playerCards[0].GetComponent<Card>().RemoveCard();
                }
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                actionDeck[0].GetComponent<Card>().user = "Dolvaris";
                dolvarisCards.Add(actionDeck[0]);
                Instantiate(dolvarisCardObject, dolvarisHand);
                actionDeck.RemoveAt(0);

                if (dolvarisCards.Count > dolvarisMaxCards)
                {
                    dolvarisCards[0].GetComponent<Card>().RemoveCard();
                }
            }
        }

        CardEffect("+" + count + " Cards", Color.yellow, 0.9f);

        if (actionDeck.Count <= 0) PrepareDeck();
    }
>>>>>>> Stashed changes
    public void DestroyCountdown()
    {
        if (playersTurn)
        {
<<<<<<< Updated upstream
            foreach (var card in dolvarisCountdowns)
            {
                Destroy(card);
=======
            foreach (var count in dolvarisCountdowns)
            {
                count.GetComponent<CardCountdown>().RemoveCountdown();
>>>>>>> Stashed changes
            }
            dolvarisCountdowns.Clear();
        }
        else
        {
<<<<<<< Updated upstream
            foreach (var card in playerCountdowns)
            {
                Destroy(card);
            }
            playerCountdowns.Clear();
        }
=======
            foreach (var count in playerCountdowns)
            {
                count.GetComponent<CardCountdown>().RemoveCountdown();
            }
            playerCountdowns.Clear();
        }

        CardEffect("COUNTDOWNS DESTROYED!", Color.white, 0.9f);
>>>>>>> Stashed changes
    }

    public void AddMove(int value)
    {
        if (playersTurn) playerMoveCount += value;
        else dolvarisMoveCount += value;
<<<<<<< Updated upstream
=======

        CardEffect("+" + (value-1) + " Moves", Color.yellow, 0.8f);
    }

    void CardEffect(string text, Color color, float time)
    {
        cardEffect.text = text;
        cardEffect.color = color;
        cardEffect.DOColor(color, time).OnComplete(() => cardEffect.DOColor(new Color(1,1,1,0),time/2));
        cardEffect.transform.DOScale(1.5f, time);
>>>>>>> Stashed changes
    }
    public void Heal(int value, bool healDefence, bool maxHeal)
    {
        if (playersTurn) HealPlayer(value, healDefence, maxHeal);
        else HealDolvaris(value, healDefence, maxHeal);
<<<<<<< Updated upstream

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
=======
        
       
        UpdateUIs();
    }

    public void PlayerHealthHealFeedback()
    {
        //playerHealthText.DOColor(Color.red, 0.5f).OnComplete(() => playerHealthText.DOColor(Color.green, 0.3f));
        playerHealthText.transform.DOScale(1.25f, 0.5f).OnComplete(() => playerHealthText.transform.DOScale(1f, 0.5f));
        playerEffectLight.color = Color.green;
        playerEffectLight.DOIntensity(25f, 0.5f).OnComplete(() => playerEffectLight.DOIntensity(0, 0.5f));
    }

    public void PlayerDefenceHealFeedback()
    {
        //playerDefenceText.DOColor(Color.white, 0.3f).OnComplete(() => playerDefenceText.DOColor(Color.cyan, 0.3f));
        playerDefenceText.transform.DOScale(1.25f, 0.5f).OnComplete(() => playerDefenceText.transform.DOScale(1f, 0.5f));
        playerEffectLight.color = Color.cyan;
        playerEffectLight.DOIntensity(25f, 0.5f).OnComplete(() => playerEffectLight.DOIntensity(0, 0.5f));
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            playerDefence += (int)(value * defenceMulti);
=======
            if (noDefence) return;
            playerDefence += (int)(value * defenceMulti);
            if (defenceBonusPercentage > 0) playerDefence += (int)(value * defenceBonusPercentage);
>>>>>>> Stashed changes
            PlayerDefenceHealFeedback();
        }
        else
        {
<<<<<<< Updated upstream
            playerHealth += value;
=======
            if (noHeal) return;
            playerHealth += value;
            if (healBonusPercentage > 0) playerHealth += (int)(value * healBonusPercentage);
>>>>>>> Stashed changes
            PlayerHealthHealFeedback();
        }

        if (playerDefence > PLAYER_MAXDEFENCE) playerDefence = PLAYER_MAXDEFENCE;
        if (playerHealth > PLAYER_MAXHEALTH) playerHealth = PLAYER_MAXHEALTH;

<<<<<<< Updated upstream
=======
        if (healDefence)
        {
            if (maxHeal) CardEffect("Max Defense!", Color.cyan, 0.8f);
            else CardEffect("+" + value + " Defense", Color.cyan, 0.8f);
        }
        else
        {
            if (maxHeal) CardEffect("Max Heal!", Color.green, 0.8f);
            else CardEffect("+" + value + " Heal", Color.green, 0.8f);
        }

        UpdateUIs();
>>>>>>> Stashed changes
    }

    void DolvarisHealthHealFeedback()
    {
        //playerHealthText.DOColor(Color.red, 0.5f).OnComplete(() => playerHealthText.DOColor(Color.green, 0.3f));
<<<<<<< Updated upstream
        dolvarisHealthText.transform.DOScale(1.1f, 0.5f).OnComplete(() => dolvarisHealthText.transform.DOScale(1f, 0.5f));
=======
        dolvarisHealthText.transform.DOScale(1.25f, 0.5f).OnComplete(() => dolvarisHealthText.transform.DOScale(1f, 0.5f));
        dolvarisEffectLight.color = Color.green;
        dolvarisEffectLight.DOIntensity(25f, 0.5f).OnComplete(() => dolvarisEffectLight.DOIntensity(0, 0.5f));
>>>>>>> Stashed changes
    }

    void DolvarisDefenceHealFeedback()
    {
        //playerDefenceText.DOColor(Color.white, 0.3f).OnComplete(() => playerDefenceText.DOColor(Color.cyan, 0.3f));
<<<<<<< Updated upstream
        dolvarisDefenceText.transform.DOScale(1.1f, 0.5f).OnComplete(() => dolvarisDefenceText.transform.DOScale(1f, 0.5f));
=======
        dolvarisDefenceText.transform.DOScale(1.25f, 0.5f).OnComplete(() => dolvarisDefenceText.transform.DOScale(1f, 0.5f));
        dolvarisEffectLight.color = Color.cyan;
        dolvarisEffectLight.DOIntensity(25f, 0.5f).OnComplete(() => dolvarisEffectLight.DOIntensity(0, 0.5f));
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    }

    public void Damage(int damage, bool trueDamage)
    {
        if (!playersTurn)
        {
            DamagePlayer(damage, trueDamage);
        }
        else DamageDolvaris(damage, trueDamage);
=======

        if (healDefence)
        {
            if (maxHeal) CardEffect("Max Defense!", Color.cyan, 0.8f);
            else CardEffect("+" + value + " Defense", Color.cyan, 0.8f);
        }
        else
        {
            if (maxHeal) CardEffect("Max Heal!", Color.green, 0.8f);
            else CardEffect("+" + value + " Heal", Color.green, 0.8f);
        }

        UpdateUIs();
    }

    public void Damage(int damage, bool trueDamage, bool comboDamage)
    {
        if (!playersTurn)
        {
            DamagePlayer(damage, trueDamage, comboDamage);
        }
        else DamageDolvaris(damage, trueDamage, comboDamage);

        
>>>>>>> Stashed changes

        UpdateUIs();

    }

    void PlayerHealthDamageFeedback()
    {
        playerHealthText.DOColor(Color.red, 0.2f).OnComplete(() => playerHealthText.DOColor(Color.green, 0.2f));
<<<<<<< Updated upstream
        playerHealthText.transform.DOScale(1.25f, 0.2f).OnComplete(() => playerHealthText.transform.DOScale(1f, 0.2f));
=======
        playerHealthText.transform.DOScale(1.5f, 0.2f).OnComplete(() => playerHealthText.transform.DOScale(1f, 0.2f));
        playerEffectLight.color = Color.red;
        playerEffectLight.DOIntensity(25f, 0.3f).OnComplete(() => playerEffectLight.DOIntensity(0, 0.3f));
>>>>>>> Stashed changes
    }

    void PlayerDefenceDamageFeedback()
    {
        playerDefenceText.DOColor(Color.white, 0.2f).OnComplete(() => playerDefenceText.DOColor(Color.cyan, 0.2f));
<<<<<<< Updated upstream
        playerDefenceText.transform.DOScale(1.25f, 0.2f).OnComplete(() => playerDefenceText.transform.DOScale(1f, 0.2f));
    }
    public void DamagePlayer(int damage, bool trueDamage)
=======
        playerDefenceText.transform.DOScale(1.5f, 0.2f).OnComplete(() => playerDefenceText.transform.DOScale(1f, 0.2f));
        playerEffectLight.color = Color.red;
        playerEffectLight.DOIntensity(25f, 0.3f).OnComplete(() => playerEffectLight.DOIntensity(0, 0.3f));
    }
    public void DamagePlayer(int damage, bool trueDamage, bool comboDamage)
>>>>>>> Stashed changes
    {
        foreach (GameObject card in playerCards)
        {
            if (card.GetComponent<Card>().GetData<card_ignoreattack>() != null)
            {
                card.GetComponent<Card>().UseCard();
<<<<<<< Updated upstream
=======

                CardEffect("BLOCKED!", Color.blue, 0.9f);

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            if (playerDefence < 0)
            {
                playerHealth += playerDefence;
                PlayerHealthDamageFeedback();
            }
        }
        else
        {
=======
            if (playerDefence < 0) playerDefence = 0;
        }
        else
        {
            playerDefence = 0;
>>>>>>> Stashed changes
            playerHealth -= damage;
            PlayerHealthDamageFeedback();
        }

<<<<<<< Updated upstream
        UpdateUIs();

        if (playerHealth < 0) Lose();
=======
        if (comboDamage) CardEffect(damage * 2 + " COMBO DAMAGE!", Color.magenta, 0.6f);
        else if (trueDamage) CardEffect(damage + " True Damage!", Color.red, 0.6f);
        else CardEffect(damage + " Damage", Color.red, 0.6f);

        UpdateUIs();

        if (playerHealth <= 0)
        {
            playerHealth = 0;
            UpdateUIs();
            Lose();
        }
>>>>>>> Stashed changes
    }

    void DolvarisHealthDamageFeedback()
    {
        dolvarisHealthText.DOColor(Color.red, 0.2f).OnComplete(() => dolvarisHealthText.DOColor(Color.green, 0.2f));
<<<<<<< Updated upstream
        dolvarisHealthText.transform.DOScale(1.25f, 0.2f).OnComplete(() => dolvarisHealthText.transform.DOScale(1f, 0.2f));
=======
        dolvarisHealthText.transform.DOScale(1.5f, 0.2f).OnComplete(() => dolvarisHealthText.transform.DOScale(1f, 0.2f));
        dolvarisEffectLight.color = Color.red;
        dolvarisEffectLight.DOIntensity(25f, 0.3f).OnComplete(() => dolvarisEffectLight.DOIntensity(0, 0.3f));
>>>>>>> Stashed changes
    }

    void DolvarisDefenceDamageFeedback()
    {
        dolvarisDefenceText.DOColor(Color.white, 0.2f).OnComplete(() => dolvarisDefenceText.DOColor(Color.cyan, 0.2f));
<<<<<<< Updated upstream
        dolvarisDefenceText.transform.DOScale(1.25f, 0.2f).OnComplete(() => dolvarisDefenceText.transform.DOScale(1f, 0.2f));
    }

    public void DamageDolvaris(int damage, bool trueDamage)
=======
        dolvarisDefenceText.transform.DOScale(1.5f, 0.2f).OnComplete(() => dolvarisDefenceText.transform.DOScale(1f, 0.2f));
        dolvarisEffectLight.color = Color.red;
        dolvarisEffectLight.DOIntensity(25f, 0.3f).OnComplete(() => dolvarisEffectLight.DOIntensity(0, 0.3f));
    }

    public void DamageDolvaris(int damage, bool trueDamage, bool comboDamage)
>>>>>>> Stashed changes
    {
        foreach (GameObject card in dolvarisCards)
        {
            if (card.GetComponent<Card>().GetData<card_ignoreattack>() != null)
            {
                card.GetComponent<Card>().UseCard();
                Destroy(dolvarisHand.GetChild(0).gameObject);
<<<<<<< Updated upstream
=======
                CardEffect("BLOCKED!", Color.blue, 0.9f);
>>>>>>> Stashed changes
                return;
            }
        }

        if (trueDamage)
        {
<<<<<<< Updated upstream
            dolvarisHealth -= (int)(damage * damageMulti);
=======
            dolvarisHealth -= (int)(damage * damageMulti * fateDamageMulti);
>>>>>>> Stashed changes
            DolvarisHealthDamageFeedback();

        }
        else if (dolvarisDefence > 0)
        {
<<<<<<< Updated upstream
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
=======
            dolvarisDefence -= (int)(damage * damageMulti * fateDamageMulti);
            
            DolvarisDefenceDamageFeedback();
            if (dolvarisDefence < 0) dolvarisDefence = 0;
        }
        else
        {
            dolvarisDefence = 0;
            dolvarisHealth -= (int)(damage * damageMulti * fateDamageMulti);
            DolvarisHealthDamageFeedback();
        }

        if (comboDamage) CardEffect(damage * 2 * damageMulti * fateDamageMulti + " COMBO DAMAGE!", Color.magenta, 0.6f);
        else if (trueDamage) CardEffect(damage * damageMulti * fateDamageMulti + " True Damage!", Color.red, 0.6f);
        else CardEffect(damage * damageMulti * fateDamageMulti + " Damage", Color.red, 0.6f);

        if (lifeStealPercentage > 0) HealPlayer((int)(damage * lifeStealPercentage), false, false);
        if (thornsDamagePercentage > 0) DamagePlayer((int)(damage * thornsDamagePercentage), false, false);

        UpdateUIs();

        if (dolvarisHealth <= 0)
        {
            dolvarisHealth = 0;
            UpdateUIs();
            Win();
        }
>>>>>>> Stashed changes
    }

}

public enum CardPlayerCharacter
{
    harper,
    zaddy
}