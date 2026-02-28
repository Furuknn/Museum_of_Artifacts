using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class EnemyScript : MonoBehaviour, IDamageable
{
    [SerializeField] private bool isEnemyCanDesicion;
    [Header("Animation")]
    private Animator animator;

    [Header("Health")]
    public float maxHealth = 100f;
    private float enemyHealth;
    private bool isEnemyDying = false;

    [Header("Health Bar")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private float reduceSpeed = 2f;

    [Header("Health Bar Fade")]
    [SerializeField] private Image healthBarFadeImage;
    [SerializeField] private float fadeDelay = 0.4f;
    [SerializeField] private float fadeSpeed = 1.5f;
    private float lastDamageTime;

    [SerializeField] private GameObject playerGO;
    [SerializeField] private float patrolMinTime = 1f;
    [SerializeField] private float patrolMaxTime = 3f;

    private Camera cam;



    private enum State
    {
        chase,
        attack,
        idle
    }
    [Header("Enemey Movement")]
    [SerializeField] private State currentState;
    [SerializeField] private float lastAttackTime;
    [SerializeField, Range(0f, 100f)] private float enemyMovementSpeed = 5f;
    [SerializeField, Range(0f, 100f)] private float enemyChasingSpeed = 7f;
    [SerializeField, Range(0f, 50f)] private float rotationSpeed = 5f;
    [SerializeField] private float chaseRange = 15f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 1.5f;
    private bool isRotating = true;
    private Quaternion targetRotation;
    private float patrolTimer;

    [Header("DamageUI")]
    [SerializeField] private GameObject DamageUI;
    [SerializeField] private float minDamageUIScale = 1.0f;
    [SerializeField] private float maxDamageUIScale = 2.0f;
    [SerializeField] private float maxHealthRatioForMaxScale = 0.3f;

    [Header("Attack")]
    [SerializeField] private bool isPatternRandom;
    [SerializeField] private List<EnemyAttackSO> enemyAttackSOs;
    [SerializeField] private bool isEnemyCanDealDamage;
    private List<GameObject> dealtDamage;
    [SerializeField, Range(0f, 10f)] private float enemyWeaponRange;
    [SerializeField] private Transform[] enemyWeapon;
    [SerializeField] private bool afterTakeDamageIsEnemyGetStun;
    [SerializeField, Range(0, 5f)] private float stunTime = 1f;
    private float currentActiveDamage;
    private float stunClipDuration;
    private Coroutine currentStunCoroutine;
    [Header("Attack Cooldown")]
    float lastClickedTime;
    //float lastComboTime;
    int comboCounter;
    float maxCombatTime = 0.5f;
    float maxClickTime = 0.2f;
    [Header("Xp Gave")]
    [SerializeField] private GameObject xpAssetPrefab;
    [SerializeField, Range(0.1f, 100f)] private float xpGave = 10f;

    [SerializeField] private bool isEnemyABoss;
    void Awake()
    {
        animator = GetComponent<Animator>();
        isEnemyCanDesicion = true;
        dealtDamage = new List<GameObject>();
    }
    void Start()
    {
        cam = Camera.main;
        enemyHealth = maxHealth;

        if (healthBarFadeImage != null)
        {
            healthBarFadeImage.fillAmount = 1f;
        }
        /*healthBar = GameObject.Find("/healthBar");
        healthBarImage = GameObject.Find("/foregroundHB").GetComponent<Image>();*/
        /*if (playerGO == null)
        {
            FindPlayerGO();
        }*/

        FindPlayerGO();
        PickNewPatrolDirection();
        UpdateStunClipDuration();

    }

    void Update()
    {

        HealthUI();
        //ApprochingToThePlayer();
        DesicionAI();

        CheckDealtDamage();
    }
    private void FindPlayerGO()
    {
        playerGO = FindObjectOfType<ThirdPersonController>().gameObject;

    }
    private void HealthUI()
    {
        if (!healthBar.activeSelf) return;

        float targetFill = enemyHealth / maxHealth;

        // MAIN BAR (fast)
        healthBarImage.fillAmount = Mathf.MoveTowards(
            healthBarImage.fillAmount,
            targetFill,
            reduceSpeed * Time.deltaTime
        );

        // FACE CAMERA
        healthBar.transform.rotation =
            Quaternion.LookRotation(healthBar.transform.position - cam.transform.position);

        // FADE BAR (delayed)
        if (healthBarFadeImage == null)
            return;

        if (Time.time - lastDamageTime < fadeDelay)
            return;

        healthBarFadeImage.fillAmount = Mathf.MoveTowards(
            healthBarFadeImage.fillAmount,
            targetFill,
            fadeSpeed * Time.deltaTime
        );
    }


    private void DesicionAI()
    {
        if (enemyHealth <= 0 && isEnemyDying == false) Death(); //Sometimes enemy not death while after he take damage then suddenly attack
        if (GameManager.instance.gameState != EGameState.INGAME || !isEnemyCanDesicion) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerGO.transform.position);

        switch (currentState)
        {
            case State.idle:
                Patrol();
                if (distanceToPlayer <= chaseRange)
                    currentState = State.chase;
                break;

            case State.chase:
                MoveToPlayer();

                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.attack;
                    ChangeMovementAnimatorParameters(false, false, false);
                }
                // DÜZELTME 2: Hysteresis (Tolerans) Eklendi
                // Kovalamayı bırakması için menzilden biraz daha (örneğin +2 birim) uzaklaşması lazım.
                // Bu, sınırda titremeyi engeller.
                else if (distanceToPlayer > chaseRange + 2f)
                {
                    currentState = State.idle;
                }
                break;

            case State.attack:
                Attack();
                if (distanceToPlayer > attackRange + 1f && !isEnemyCanDealDamage)
                {
                    currentState = State.chase;
                    FinishEnemyDealingDamage();
                }
                break;
        }
    }

    private void Patrol()
    {
        //PATROL 
        if (isRotating)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            ChangeMovementAnimatorParameters(true, false, false);//IDLE,WALK,CHASE
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                isRotating = false;
                patrolTimer = Random.Range(patrolMinTime, patrolMaxTime);
            }
        }
        else
        {
            ChangeMovementAnimatorParameters(false, true, false);//IDLE,WALK,CHASE
            transform.Translate(Vector3.forward * enemyMovementSpeed * Time.deltaTime);
            patrolTimer -= Time.deltaTime;
            if (patrolTimer <= 0)
            {
                PickNewPatrolDirection();
            }
        }

    }
    private void PickNewPatrolDirection()
    {
        float randomY = Random.Range(0, 360);
        targetRotation = Quaternion.Euler(0, randomY, 0);
        isRotating = true;
    }
    private void MoveToPlayer()
    {
        if (isEnemyCanDealDamage) return;
        //approching to player
        Vector3 direction = playerGO.transform.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            ChangeMovementAnimatorParameters(false, false, true);//IDLE,WALK,CHASE
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        }
        transform.Translate(Vector3.forward * enemyChasingSpeed * Time.deltaTime);

    }
    private void Attack()
    {
        ChangeMovementAnimatorParameters(false, false, false);
        Vector3 direction = playerGO.transform.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        // Mevcut saldırının verisini alıyoruz
        var currentAttackData = enemyAttackSOs[comboCounter];

        // DÜZELTME: Sabit attackCooldown yerine SO'dan gelen cooldown'ı kullanıyoruz
        if (Time.time - lastClickedTime > currentAttackData.attackCooldown && !isEnemyCanDealDamage)
        {
            if (enemyAttackSOs == null || enemyAttackSOs.Count == 0) return;

            // Animasyonu ve hasarı uygula
            animator.runtimeAnimatorController = currentAttackData.animatorOV;
            animator.Play("Attack_1", 0, 0f);

            currentActiveDamage = currentAttackData.damage;
            lastClickedTime = Time.time;

            Debug.Log($"{currentAttackData.name} yapıldı. Bekleme süresi: {currentAttackData.attackCooldown}sn");

            // Kombo sırasını ilerlet
            if (isPatternRandom)
                comboCounter = Random.Range(0, enemyAttackSOs.Count);
            else
            {
                comboCounter++;
                if (comboCounter >= enemyAttackSOs.Count) comboCounter = 0;
            }
        }
    }
    /*private void ApprochingToThePlayer()
    {
        if (playerGO == null && GameManager.instance.gameState != EGameState.INGAME)
        {
            FindPlayerGO();
            return;
        }
        if (GameManager.instance.gameState == EGameState.INGAME)
        {
            Vector3 playerPosition = new Vector3(playerGO.transform.position.x, transform.position.y, playerGO.transform.position.z);
            Vector3 direction = (playerPosition - transform.position ).normalized;
            transform.position += direction * Time.deltaTime * enemyMovementSpeed;
            
        }


    }*/


    public void TakeDamage(float damage)//TAKEN DAMAGE
    {
        if (isEnemyDying) return;

        enemyHealth -= damage;
        lastDamageTime = Time.time;

        Debug.Log("Enemy Dealed damage" + damage);

        GetComponent<EnemyHitFeedback>()?.OnHit();

        ShowOnUI(damage);
        if (enemyHealth <= 0)
        {
            Death();
            if (SubLevelManager.instace != null)
            {
                SubLevelManager.instace.CheckEnemyList(gameObject);
            }

            return;
        }

        if (!healthBar.activeSelf)
        {
            healthBar.SetActive(true);
        }

        if (afterTakeDamageIsEnemyGetStun)
        {
            ApplyStun();
        }
    }
    private void Death()
    {
        isEnemyDying = true;
        Debug.Log("Düşman Öldü");
        ChangeMovementAnimatorParameters(false, false, false);
        isEnemyCanDesicion = false;
        FinishEnemyDealingDamage();
        if (healthBar.activeSelf)
        {
            healthBar.SetActive(false);
        }
        animator.SetTrigger("death");
        if (isEnemyABoss)
        {
            ReturnLevel();
        }
    }
    public void DestroyEnemy()
    {
        if (xpAssetPrefab != null)
        {
            GameObject xpGainGameObject = Instantiate(xpAssetPrefab, transform.position, Quaternion.identity);
            xpGainGameObject.GetComponent<XpGainObject>().xpGain = xpGave;
        }

        Destroy(gameObject);
    }
    public void ApplyStun()
    {
        // DÜZELTME 1: Çakışmayı Önleme
        // Eğer halihazırda işleyen bir Stun varsa onu durdur.
        if (currentStunCoroutine != null)
        {
            StopCoroutine(currentStunCoroutine);
        }
        // Yeni Stun'ı başlat ve değişkene ata.
        currentStunCoroutine = StartCoroutine(Stun());
    }
    IEnumerator Stun()
    {
        ChangeMovementAnimatorParameters(false, false, false);
        isEnemyCanDesicion = false;
        FinishEnemyDealingDamage();

        float multiplier = stunClipDuration / stunTime;
        if (stunTime <= 0) multiplier = 1;

        animator.SetFloat("stunSpeed", multiplier);

        animator.ResetTrigger("stun");
        animator.SetTrigger("stun");
        yield return new WaitForSeconds(stunTime);

        animator.ResetTrigger("stun");
        isEnemyCanDesicion = true;
        currentStunCoroutine = null;
    }

    void ShowOnUI(float damage)
    {
        float damageRatio = damage / maxHealth;

        float normalizedScaleRatio = Mathf.Clamp01(damageRatio / maxHealthRatioForMaxScale);

        float finalScale = Mathf.Lerp(minDamageUIScale, maxDamageUIScale, normalizedScaleRatio);

        Vector3 spawnPos = transform.position + Vector3.up * 1.5f;

        GameObject dealedDamageUI = Instantiate(DamageUI, spawnPos, Quaternion.identity);

        var dealedDamageUIScript = dealedDamageUI.GetComponent<enemyDealedDamageUI>();

        if (dealedDamageUIScript != null)
        {
            dealedDamageUIScript.Initialize(damage, finalScale);
        }

    }

    public void StartEnemyDealingDamage()
    {
        isEnemyCanDealDamage = true;
        //Debug.Log($"{gameObject.name}: StartEnemyDealingDamage() Can deal damage: {isEnemyCanDealDamage}");
        Debug.Log("StartEnemyDealingDamage: Başladı");
    }
    public void FinishEnemyDealingDamage()
    {
        isEnemyCanDealDamage = false;
        dealtDamage.Clear();
        Debug.Log("FinishEnemyDealingDamage: Bitti");
        //Debug.Log($"{gameObject.name}: FinishEnemyDealingDamage() Can deal damage: {isEnemyCanDealDamage}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (Transform weapon in enemyWeapon)
        {
            Gizmos.DrawLine(weapon.position, weapon.position - (-weapon.up) * enemyWeaponRange);
        }

    }

    #region Animation Method
    private void ChangeMovementAnimatorParameters(bool isIdle, bool isWalking, bool isChasing)
    {
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isMoving", isWalking);
        animator.SetBool("isChasing", isChasing);
    }
    #endregion
    private void CheckDealtDamage()
    {
        if (!isEnemyCanDealDamage) return;

        foreach (Transform weapon in enemyWeapon)
        {
            Vector3 direction = weapon.up;

            RaycastHit[] hits = Physics.RaycastAll(weapon.position, direction, enemyWeaponRange);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == gameObject) continue;

                if (!dealtDamage.Contains(hit.collider.gameObject))
                {
                    var playerHealth = hit.collider.GetComponent<PlayerHealthManager>();

                    if (playerHealth != null)
                    {
                        dealtDamage.Add(hit.collider.gameObject);

                        playerHealth.ModifyHealth(-currentActiveDamage);

                        Debug.Log($"Player vuruldu! Hasar: {currentActiveDamage}");
                    }
                }
            }
        }
    }
    private void UpdateStunClipDuration()
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;

        // Animator'daki tüm klipleri bir diziye al
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            // Clip isminde "Stun" geçiyorsa veya adı tam olarak "Stun" ise
            // (Küçük/büyük harf duyarlılığını kaldırmak için ToLower yapıyoruz)
            if (clip.name.ToLower().Contains("stun"))
            {
                stunClipDuration = clip.length;
                // Debug.Log($"Stun animasyonu bulundu: {clip.name}, Süresi: {stunClipDuration}");
                return; // Bulduk, döngüden çık
            }
        }

        // Eğer bulunamazsa hata vermemesi için varsayılan bir değer ata
        Debug.LogWarning(gameObject.name + ": 'Stun' isminde bir animasyon klibi bulunamadı!");
        stunClipDuration = 1f;
    }

    public void ReturnLevel()
    {
        LevelManager.instance.ReturnFromLevel();
        LevelManager.instance.DestroyCurrentLevel();
        LevelManager.instance.ReturnWithWinFromLevel();
    }
}
