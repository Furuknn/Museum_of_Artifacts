using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Image = UnityEngine.UI.Image;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour, IEnemy, IDamageable
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform player;
    [HideInInspector] public float distanceToPlayer;

    private IEnemyState currentState;
    public EnemyIdleState IdleState = new EnemyIdleState();
    public EnemyChaseState ChaseState = new EnemyChaseState();
    public EnemyAttackState AttackState = new EnemyAttackState();
    public EnemyStunState StunState = new EnemyStunState();
    public EnemyDeathState DeathState = new EnemyDeathState();

    [Header("Stats")]
    public float maxHealth = 100f;
    public float chaseRange = 15f;
    public float attackRange = 2.5f;
    public float separationRadius = 1.8f; 
    public float separationForce = 6f; 
    [HideInInspector] public float currentHealth;
    [HideInInspector] public bool isDead;

    [Header("Attacks")]
    public List<EnemyAttackSO> attackPatterns;
    public bool randomisePatterns = false;
    [HideInInspector] public int comboIndex;
    [HideInInspector] public float lastAttackTime;
    [HideInInspector] public bool isDealtDamageWindowOpen;
    private List<GameObject> hitThisSwing = new List<GameObject>();

    [Header("Weapon")]
    public Transform[] weaponPoints;
    public float weaponRange = 1.2f;
    public float weaponRadius = 0.4f;

    [Header("Stun")]
    public bool stunOnHit = false;
    public float stunDuration = 0.8f;
    private Coroutine activeStunRoutine;
    private float cachedStunClipLength = 1f;

    [field: SerializeField] public GameObject healthBarRoot  { get; private set; }
    [field: SerializeField] public Image      healthBarFill  { get; private set; }
    [field: SerializeField] public Image      healthBarDelayedFill { get; private set; }

    [Header("Health Bar")]
    [SerializeField] private float fillSpeed = 4f;
    [SerializeField] private float delayedFillDelay = 0.5f;
    [SerializeField] private float delayedFillSpeed = 2f;
    [SerializeField] private float hideHealthBarAfter = 4f; // Time before fading starts
    [SerializeField] private float healthBarFadeSpeed = 2f; // How fast it fades
    private CanvasGroup healthBarCanvasGroup;
    private float lastDamageTime;
    private Camera mainCam;

    [Header("Damage Numbers")]
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private float damageNumberHeightOffset = 1.5f;

    [Header("XP")]
    [SerializeField] private GameObject xpOrbPrefab;
    [SerializeField] private float xpReward = 10f;

    [Header("Boss")]
    public bool isBoss = false;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mainCam = Camera.main;
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        player = FindObjectOfType<ThirdPersonController>()?.transform;

        if (healthBarRoot != null)
        {
            // --- NEW CANVAS GROUP LOGIC ---
            healthBarCanvasGroup = healthBarRoot.GetComponent<CanvasGroup>();
            if (healthBarCanvasGroup == null)
            {
                healthBarCanvasGroup = healthBarRoot.AddComponent<CanvasGroup>();
            }
            healthBarCanvasGroup.alpha = 1f; // Ensure it starts fully visible mathematically
            // ------------------------------

            healthBarRoot.SetActive(false);

            if (healthBarFill != null) healthBarFill.fillAmount = 1f;
            if (healthBarDelayedFill != null) healthBarDelayedFill.fillAmount = 1f;
        }

        CacheStunClipLength();
        TransitionTo(IdleState);
    }

    protected virtual void Update()
    {
        if (isDead || GameManager.Instance.gameState != EGameState.INGAME) return;

        distanceToPlayer = player != null
            ? Vector3.Distance(transform.position, player.position)
            : float.MaxValue;

        currentState?.Tick(this);
        ApplySeparation();
        UpdateHealthUI();
        CheckWeaponHits();
    }

    protected virtual void FixedUpdate()
    {
        if (isDead) return;
        currentState?.FixedTick(this);
    }


    public void TransitionTo(IEnemyState next)
    {
        currentState?.Exit(this);
        currentState = next;
        currentState.Enter(this);
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        lastDamageTime = Time.time;

        GetComponent<EnemyHitFeedback>()?.OnHit();
        SpawnDamageNumber(damage);

        // --- UPDATED UI LOGIC ---
        if (healthBarRoot != null)
        {
            healthBarRoot.SetActive(true);
            if (healthBarCanvasGroup != null)
            {
                healthBarCanvasGroup.alpha = 1f; // Instantly pop back to full opacity
            }
        }

        if (currentHealth <= 0f) { Death(); return; }

        if (stunOnHit) ApplyStun(stunDuration);
    }

    public virtual void ApplyStun(float duration)
    {
        if (isDead) return;
        if (activeStunRoutine != null) StopCoroutine(activeStunRoutine);
        activeStunRoutine = StartCoroutine(StunRoutine(duration));
    }

    public virtual void Death()
    {
        if (isDead) return;
        isDead = true;
        TransitionTo(DeathState);

        SubLevelManager.Instance?.CheckEnemyList(gameObject);

        if (isBoss)
        {
            //LevelManager.Instance.ReturnFromLevel();
            //LevelManager.Instance.DestroyCurrentLevel();
            //LevelManager.Instance.ReturnWithWinFromLevel();
            SubLevelManager.Instance.WinCondition();
        }

        LyposEnemy lyposEnemy = GetComponent<LyposEnemy>();
        if (lyposEnemy != null) lyposEnemy.OnDie();
    }


    public float GetHealthPercent() => currentHealth / maxHealth;

    public void DestroyEnemy()
    {
        if (xpOrbPrefab != null)
        {
            var orb = Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);
            orb.GetComponent<XpGainObject>().xpGain = xpReward;
        }
        Destroy(gameObject);
    }

    // REPLACE ApplySeparation entirely
    private void ApplySeparation()
    {
        if (!agent.isActiveAndEnabled || isDead) return;

        Collider[] neighbours = Physics.OverlapSphere(
            transform.position, separationRadius,
            LayerMask.GetMask("Enemy"),  // make sure your player is NOT on this layer
            QueryTriggerInteraction.Ignore
        );

        Vector3 push = Vector3.zero;
        int count = 0;

        foreach (Collider col in neighbours)
        {
            if (col.gameObject == gameObject) continue;

            // Hard-exclude the player regardless of layer setup,
            // so a misconfigured layer can never cause pushback
            if (col.GetComponent<ThirdPersonController>() != null) continue;

            // Also skip anything that isn't an enemy — obstacles, props etc.
            if (col.GetComponent<EnemyBase>() == null) continue;

            Vector3 away = transform.position - col.transform.position;
            float dist = away.magnitude;

            if (dist < 0.001f)
            {
                away = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
                dist = 0.1f;
            }

            push += away.normalized * (1f - dist / separationRadius);
            count++;
        }

        if (count == 0) return;

        push /= count;
        push.y = 0f;
        agent.Move(push * separationForce * Time.deltaTime);
    }


    public void OpenDamageWindow()
    {
        isDealtDamageWindowOpen = true;
        hitThisSwing.Clear();
    }

    public void CloseDamageWindow()
    {
        isDealtDamageWindowOpen = false;
        hitThisSwing.Clear();
    }

    private void CheckWeaponHits()
    {
        if (!isDealtDamageWindowOpen || weaponPoints == null) return;

        EnemyAttackSO data = GetCurrentAttackData();
        if (data == null) return;

        foreach (Transform wp in weaponPoints)
        {
            // SphereCast gives the hit detection actual volume —
            // a raycast from a weapon bone is a single pixel in world space
            // and almost always misses unless perfectly aligned.
            // 0.4f radius is a good starting point; expose it as a field if needed.
            RaycastHit[] hits = Physics.SphereCastAll(
                wp.position,
                0.4f,               // detection radius — tweak per enemy type
                wp.up,
                weaponRange,
                LayerMask.GetMask("Player"), // only check the player layer
                QueryTriggerInteraction.Ignore
            );

            // Draw so you can see the sweep in Scene view while playtesting
            Debug.DrawRay(wp.position, wp.up * weaponRange, Color.red, 0.1f);

            foreach (RaycastHit hit in hits)
            {
                if (hitThisSwing.Contains(hit.collider.gameObject)) continue;

                var playerHealth = hit.collider.GetComponent<PlayerHealthManager>();
                if (playerHealth != null)
                {
                    hitThisSwing.Add(hit.collider.gameObject);
                    playerHealth.ModifyHealth(-data.damage);

                    if (playerHealth.deflectsDamage)
                        TakeDamage(data.damage * 0.5f);
                }
            }
        }
    }

    public EnemyAttackSO GetCurrentAttackData()
    {
        if (attackPatterns == null || attackPatterns.Count == 0) return null;
        return attackPatterns[Mathf.Clamp(comboIndex, 0, attackPatterns.Count - 1)];
    }

    public void AdvanceCombo()
    {
        if (randomisePatterns)
            comboIndex = Random.Range(0, attackPatterns.Count);
        else
        {
            comboIndex++;
            if (comboIndex >= attackPatterns.Count) comboIndex = 0;
        }
    }


    private void UpdateHealthUI()
    {
        if (healthBarFill == null || healthBarRoot == null || !healthBarRoot.activeSelf) return;

        float target = GetHealthPercent();

        healthBarRoot.transform.rotation = Quaternion.LookRotation(
            healthBarRoot.transform.position - mainCam.transform.position
        );

        healthBarFill.fillAmount = Mathf.MoveTowards(
            healthBarFill.fillAmount, target, fillSpeed * Time.deltaTime
        );

        if (healthBarDelayedFill != null && Time.time - lastDamageTime >= delayedFillDelay)
        {
            healthBarDelayedFill.fillAmount = Mathf.MoveTowards(
                healthBarDelayedFill.fillAmount, target, delayedFillSpeed * Time.deltaTime
            );
        }

        // --- NEW FADE OUT LOGIC ---
        if (healthBarCanvasGroup != null)
        {
            // If enough time has passed since the last hit, start fading
            if (Time.time - lastDamageTime >= hideHealthBarAfter)
            {
                healthBarCanvasGroup.alpha = Mathf.MoveTowards(
                    healthBarCanvasGroup.alpha, 0f, healthBarFadeSpeed * Time.deltaTime
                );

                // Disable the GameObject entirely once it's invisible to save resources
                if (healthBarCanvasGroup.alpha <= 0f)
                {
                    healthBarRoot.SetActive(false);
                }
            }
        }
        // --------------------------
    }


    private IEnumerator StunRoutine(float duration)
    {
        TransitionTo(StunState);

        float multiplier = cachedStunClipLength > 0f
            ? cachedStunClipLength / duration
            : 1f;

        animator.SetFloat("stunSpeed", multiplier);
        animator.ResetTrigger("stun");
        animator.SetTrigger("stun");

        yield return new WaitForSeconds(duration);

        animator.ResetTrigger("stun");
        activeStunRoutine = null;

        if (!isDead) TransitionTo(IdleState);
    }

    private void CacheStunClipLength()
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.ToLower().Contains("stun"))
            {
                cachedStunClipLength = clip.length;
                return;
            }
        }

        Debug.LogWarning($"{name}: No stun clip found, defaulting to 1s.");
        cachedStunClipLength = 1f;
    }

    private void SpawnDamageNumber(float damage)
    {
        if (damageNumberPrefab == null) return;

        Vector3 pos = transform.position + Vector3.up * damageNumberHeightOffset;
        var go = Instantiate(damageNumberPrefab, pos, Quaternion.identity);
        go.GetComponent<EnemyDealedDamageUI>()?.Initialize(damage, 1f);
    }

    public void SetAnimMove(bool idle, bool walk, bool chase)
    {
        animator.SetBool("isIdle", idle);
        animator.SetBool("isMoving", walk);
        animator.SetBool("isChasing", chase);
    }


    private void OnEnable()
    {
        GameManager.OnGameStopped += OnGameStopped;
        GameManager.OnGameContinued += OnGameContinued;
    }

    private void OnDisable()
    {
        GameManager.OnGameStopped -= OnGameStopped;
        GameManager.OnGameContinued -= OnGameContinued;
    }

    private void OnGameStopped()
    {
        if (agent.isActiveAndEnabled) agent.isStopped = true;
        animator.speed = 0f;
        if (activeStunRoutine != null)
        {
            StopCoroutine(activeStunRoutine);
            activeStunRoutine = null;
        }
    }

    private void OnGameContinued()
    {
        if (!isDead && agent.isActiveAndEnabled) agent.isStopped = false;
        animator.speed = 1f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (weaponPoints == null) return;
        foreach (Transform wp in weaponPoints)
            Gizmos.DrawLine(wp.position, wp.position + wp.up * weaponRange);

        Gizmos.color = new Color(1, 0.5f, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}