using System.Collections;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager instance { get; private set; }
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    private Image inGameHealthBar;
    private float reduceSpeed = 2f;

    [Header("Passive Healing")]
    [SerializeField] private float passiveHealDelay = 5f;
    [SerializeField] private float passiveHealAmount = 5f;
    [SerializeField] private float passiveHealTick = 1f;

    [Header("Health Bar Fade")]
    private Image healthbarFade;
    [SerializeField] private float fadeDelay = 0.5f;
    [SerializeField] private float fadeSpeed = 1.5f;

    private float lastDamageTime;
    private Coroutine passiveHealRoutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        inGameHealthBar = UIManager.instance.GetInGameHealthBar();
        healthbarFade = UIManager.instance.GetInGameHealthBarFade();
        Debug.Log($"healthBar UI: {inGameHealthBar.gameObject.name}");
        maxHealth = GameManager.instance.characters[GameManager.instance.currentHeroIndex].playerStatisticsSO.maxHealth;
        health = maxHealth;
        Debug.Log($"PlayerHealthManager: {health}");

        if (healthbarFade != null)
        {
            healthbarFade.fillAmount = 1f;
        }
    }
    void Update()
    {
        UpdateHealthUI();

        UpdateHealthFadeUI();

        TryStartPassiveHeal();

        //to test the passive heal
        if (Input.GetKeyDown(KeyCode.N))
        {
            ModifyHealth(-10);
        }
    }

    public void ModifyHealth(float amount)//MUST BE + or -
    {
        Debug.Log($"ModifyHealth: Player's health has been modified amount: {amount}");

        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (amount < 0)
        {
            lastDamageTime = Time.time;

            // Stop passive heal immediately
            if (passiveHealRoutine != null)
            {
                StopCoroutine(passiveHealRoutine);
                passiveHealRoutine = null;
            }
        }

        ChechkCurrentHealth();
    }

    private void ChechkCurrentHealth()
    {
        if (health <= 0)
        {
            if (LevelManager.instance.isPlayerGetFirstWin)
            {
                Debug.Log("Lose but not at all");

                ResetAllStatsOfPlayer();//RESET ALL STATS
                //LevelManager.instance.BackToTheFormerPosition();
                LevelManager.instance.ReturnFromLevel();
                LevelManager.instance.DestroyCurrentLevel();
                LevelManager.instance.ReturnWithLoseFromLevel();
            }
            else
            {
                KillPlayer();
            }

        }
    }

    // if the player didn't receive damage in passiveHealDelay time, start healing routine
    private void TryStartPassiveHeal()
    {
        if (health >= maxHealth)
            return;

        if (passiveHealRoutine != null)
            return;

        if (Time.time - lastDamageTime >= passiveHealDelay)
        {
            passiveHealRoutine = StartCoroutine(PassiveHealRoutine());
        }
    }

    //Passively heal the player until it reaches max health or the player gets hit
    private IEnumerator PassiveHealRoutine()
    {
        while (health < maxHealth)
        {
            health += passiveHealAmount;
            health = Mathf.Clamp(health, 0, maxHealth);

            yield return new WaitForSeconds(passiveHealTick);
        }

        passiveHealRoutine = null;
    }

    public void KillPlayer()
    {
        GameManager.instance.GameOverLose();
    }
    private void UpdateHealthUI()
    {
        if (inGameHealthBar == null) return;

        //Debug.Log(health / maxHealth);
        inGameHealthBar.fillAmount = Mathf.MoveTowards(inGameHealthBar.fillAmount, health / maxHealth, reduceSpeed * Time.deltaTime);
    }

    //Updates the grey health fade bar
    private void UpdateHealthFadeUI()
    {
        if (healthbarFade == null)
            return;

        float targetFill = health / maxHealth;

        // Wait before fading
        if (Time.time - lastDamageTime < fadeDelay)
            return;

        // Smoothly move grey bar toward real health
        healthbarFade.fillAmount = Mathf.MoveTowards(
            healthbarFade.fillAmount,
            targetFill,
            fadeSpeed * Time.deltaTime
            );
    }
    private void ResetAllStatsOfPlayer()
    {
        health = maxHealth;
        PlayerXpManagement.instance.ResetXp();
        //THERE WILL BE RESET SKILL TREE
    }
}
