using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrenosController : EnemyBase
{

    [Header("Movement")]
    public float chaseSpeed = 4f;
    public float rotationSpeed = 180f;
    public float attackRotationSpeed = 540f;
    public float gravity = -20f;

    private float _verticalVelocity = 0f;
    [HideInInspector] public bool canMove = true;
    private CharacterController _cc;


    [Header("Action Tick System")]
    [Tooltip("How often (seconds) the boss evaluates whether to act.")]
    public float tickInterval = 0.3f;

    [Tooltip("Define all possible actions with their probabilities and ranges.")]
    public List<ThrenosActionEntry> actionTable = new List<ThrenosActionEntry>
    {
        // Defaults — override in Inspector
        // Phase 1
        new ThrenosActionEntry { actionName = "GroundSlash", probability = 0.25f, minRange = 7f,   maxRange = 999f },
        new ThrenosActionEntry { actionName = "Sway",        probability = 0.35f, minRange = 0f,   maxRange = 2.8f },
        // Phase 2
        new ThrenosActionEntry { actionName = "Dash",        probability = 0.30f, minRange = 6f,   maxRange = 999f },
        new ThrenosActionEntry { actionName = "KatanaSlash", probability = 0.40f, minRange = 0f,   maxRange = 3f   },
    };

    private float _tickTimer = 0f;


    [Header("Threnos — Phases")]
    [SerializeField, Range(0f, 1f)] private float phase2HealthThreshold = 0.4f;
    [HideInInspector] public bool isPhase2 = false;


    [Header("Phase 1 — Great Sword")]
    public GameObject heavySwordGO;
    public WeaponHitbox greatSwordHitbox;  // drag the child GO with BoxCollider here
    public float swayDamage = 15f;
    public float meleeRange = 2.8f;

    [Header("Phase 2 — Katana")]
    public GameObject katanaGO;
    public WeaponHitbox katanaHitbox;      // drag the katana child GO here
    public float katanaDamage = 20f;
    public float katanaMeleeRange = 3f;


    [Header("Phase 1 — Ground Slash")]
    public GameObject groundSlashVFXPrefab;
    public Transform groundSlashSpawnPoint;
    public float groundSlashSpeed = 8f;
    public float groundSlashLifetime = 4f;
    public float groundSlashMinRange = 7f;
    public float groundSlashDamage = 20f;


    [Header("Phase 2 — Dash Attack")]
    public float dashTriggerRange = 6f;
    public float dashSpeed = 18f;
    public float dashMaxDuration = 0.6f;
    public float dashDamageRange = 2.2f;
    public float dashDamage = 28f;
    public float dashHitRadius = 1.6f;
    public float dashRecoveryTime = 0.5f;
    public float dashKnockbackHorizontal = 12f;
    public float dashKnockbackVertical = 6f;
    public string anim_DashAttack = "DashAttack";
    public string playerLayerName = "Player";
    public string enemyLayerName = "Enemy";
    public MeshTrail meshTrail;


    [Header("Phase 2 — Transition FX")]
    public GameObject phase2TransitionVFX;


    [Header("Animator Parameters")]
    public string anim_GroundSlash = "GroundSlash";
    public string anim_Sway = "Sway";
    public string anim_Phase2Intro = "Phase2Intro";
    public string anim_KatanaSlash_01 = "KatanaSlash01";
    public string anim_KatanaSlash_02 = "KatanaSlash02";

    [HideInInspector] public bool isActing;

    public ThrenosIdleState TIdleState = new ThrenosIdleState();
    public ThrenosChaseState TChaseState = new ThrenosChaseState();
    public ThrenosCombatState TCombatState = new ThrenosCombatState();

    [SerializeField] private GameObject exitPortal;

    protected override void Start()
    {
        base.Start();

        _cc = GetComponent<CharacterController>();
        if (_cc == null) _cc = gameObject.AddComponent<CharacterController>();

        // Wire up hitboxes
        InitHitbox(greatSwordHitbox, swayDamage, applyKnockback: false);
        InitHitbox(katanaHitbox, katanaDamage, applyKnockback: false);

        heavySwordGO?.SetActive(true);
        katanaGO?.SetActive(false);

        if (meshTrail == null)
            meshTrail = GetComponentInChildren<MeshTrail>();

        TransitionTo(TChaseState);
    }

    protected override void Update()
    {
        if (isDead || GameManager.Instance.gameState != EGameState.INGAME) return;

        distanceToPlayer = player != null
            ? Vector3.Distance(transform.position, player.position)
            : float.MaxValue;

        if (!isPhase2 && GetHealthPercent() <= phase2HealthThreshold)
            StartCoroutine(EnterPhase2());

        currentState?.Tick(this);

        if (!isActing && player != null)
            SmoothFacePlayer(rotationSpeed);

        // Tick the action evaluator
        _tickTimer -= Time.deltaTime;
        if (_tickTimer <= 0f)
        {
            _tickTimer = tickInterval;
            EvaluateActionTick();
        }

        if(currentHealth <=0 && exitPortal != null)
        {
            exitPortal.SetActive(true);
        }

        ApplyGravity();
        UpdateHealthUI();
    }

    // ════════════════════════════════════════════════════════════════
    //  Tick evaluator
    // ════════════════════════════════════════════════════════════════

    // Called every tickInterval seconds.
    // Filters the action table by current phase and distance,
    // shuffles eligible entries, rolls probability for each,
    // executes the first one that succeeds.
    private void EvaluateActionTick()
    {
        if (isActing || isDead) return;

        // Build eligible list for this phase
        List<ThrenosActionEntry> eligible = new List<ThrenosActionEntry>();

        foreach (ThrenosActionEntry entry in actionTable)
        {
            // Filter by phase prefix so phase 1 actions don't fire in phase 2 and vice versa
            bool isPhase1Action = entry.actionName == "GroundSlash" || entry.actionName == "Sway";
            bool isPhase2Action = entry.actionName == "Dash" || entry.actionName == "KatanaSlash";

            if (!isPhase2 && !isPhase1Action) continue;
            if (isPhase2 && !isPhase2Action) continue;

            // Filter by range band
            if (distanceToPlayer < entry.minRange || distanceToPlayer > entry.maxRange) continue;

            eligible.Add(entry);
        }

        if (eligible.Count == 0) return;

        // Shuffle so no action gets priority by list order
        for (int i = eligible.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (eligible[i], eligible[j]) = (eligible[j], eligible[i]);
        }

        // Roll each eligible action — execute the first success
        foreach (ThrenosActionEntry entry in eligible)
        {
            if (Random.value <= entry.probability)
            {
                ExecuteAction(entry.actionName);
                return; // only one action per tick
            }
        }
        // If no roll succeeded this tick, boss continues chasing — next tick tries again
    }

    private void ExecuteAction(string actionName)
    {
        switch (actionName)
        {
            case "GroundSlash": StartCoroutine(Attack_GroundSlash()); break;
            case "Sway": StartCoroutine(Attack_Sway()); break;
            case "Dash": StartCoroutine(Attack_Dash()); break;
            case "KatanaSlash": StartCoroutine(Attack_KatanaSlash()); break;
            default:
                Debug.LogWarning($"[Threnos] Unknown action: {actionName}");
                break;
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Animation event receivers
    //  Wire these in the Animation window on each clip.
    //  OpenHitWindow_Sword can be called up to 3 times per animation.
    //  Each call resets the hit list — one hit per window phase.
    // ════════════════════════════════════════════════════════════════

    public void OpenHitWindow_Sword()
    {
        if (greatSwordHitbox != null)
        {
            greatSwordHitbox.OpenWindow();
            Debug.LogWarning("Sword hit window open!!!");
        }
    }

    public void CloseHitWindow_Sword()
    {
        if (greatSwordHitbox != null) greatSwordHitbox.CloseWindow();
    }

    public void OpenHitWindow_Katana()
    {
        if (katanaHitbox != null) katanaHitbox.OpenWindow();
    }

    public void CloseHitWindow_Katana()
    {
        if (katanaHitbox != null) katanaHitbox.CloseWindow();
    }

    // ════════════════════════════════════════════════════════════════
    //  Phase 1 — Ground Slash
    // ════════════════════════════════════════════════════════════════

    private IEnumerator Attack_GroundSlash()
    {
        isActing = true;
        canMove = false;

        yield return StartCoroutine(TurnToFacePlayer(attackRotationSpeed));
        animator.SetTrigger(anim_GroundSlash);

        // Coroutine waits for clip length — tune to match your animation
        yield return new WaitForSeconds(2.1f);

        canMove = true;
        isActing = false;
    }

    // Called by animation event — spawns the VFX projectile at the right frame
    public void SpawnGroundSlash()
    {
        if (groundSlashVFXPrefab == null) return;

        Vector3 spawnPos = groundSlashSpawnPoint != null
            ? groundSlashSpawnPoint.position
            : transform.position + transform.forward;

        Vector3 dir = player.position - spawnPos;
        dir.y = 0f;
        Quaternion rot = dir.sqrMagnitude > 0.001f
            ? Quaternion.LookRotation(dir.normalized)
            : transform.rotation;

        GameObject slash = Instantiate(groundSlashVFXPrefab, spawnPos, rot);
        GroundSlashProjectile proj = slash.GetComponent<GroundSlashProjectile>();
        if (proj != null)
            proj.Launch(rot * Vector3.forward, groundSlashSpeed, groundSlashLifetime);
    }

    // ════════════════════════════════════════════════════════════════
    //  Phase 1 — Sway
    // ════════════════════════════════════════════════════════════════

    private IEnumerator Attack_Sway()
    {
        isActing = true;
        canMove = false;

        yield return StartCoroutine(TurnToFacePlayer(attackRotationSpeed));
        animator.SetTrigger(anim_Sway);

        // Hitbox opened/closed by animation events on the sway clip.
        // Wait for full animation to finish before clearing isActing.
        yield return new WaitForSeconds(1.5f); // tune to clip length

        CloseHitWindow_Sword(); // safety close in case event was missed
        canMove = true;
        isActing = false;
    }

    // ════════════════════════════════════════════════════════════════
    //  Phase 2 — Katana Slash
    // ════════════════════════════════════════════════════════════════

    private IEnumerator Attack_KatanaSlash()
    {
        isActing = true;
        canMove = false;

        yield return StartCoroutine(TurnToFacePlayer(attackRotationSpeed));

        // Random.value is a float 0.0–1.0 — correct way to do a 50/50
        string chosenTrigger = Random.value <= 0.5f ? anim_KatanaSlash_01 : anim_KatanaSlash_02;
        animator.SetTrigger(chosenTrigger);

        // Wait one frame so the Animator has time to transition into the new state
        // before we try to read its clip length — without this, GetCurrentClipLength
        // can still return the previous state's length
        yield return null;
        yield return null;

        float clipLength = GetCurrentClipLength();

        // If we couldn't read the clip, fall back to a safe default
        if (clipLength <= 0f) clipLength = 1.2f;

        yield return new WaitForSeconds(clipLength);

        CloseHitWindow_Katana();
        canMove = true;
        isActing = false;
    }

    // Returns the length of whatever clip is currently playing on layer 0
    private float GetCurrentClipLength()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // stateInfo.length gives the clip length, dividing by speed accounts for
        // any speed multiplier set on the state in the Animator Controller
        float speed = animator.speed > 0f ? animator.speed : 1f;
        return stateInfo.length / speed;
    }

    // ════════════════════════════════════════════════════════════════
    //  Phase 2 — Dash
    //  Dash still uses direct distance check for hit confirmation —
    //  it moves too fast for a stationary trigger collider to catch.
    // ════════════════════════════════════════════════════════════════

    private IEnumerator Attack_Dash()
    {
        isActing = true;
        canMove = false;

        yield return StartCoroutine(TurnToFacePlayer(attackRotationSpeed));

        Vector3 dashDir = transform.forward;
        dashDir.y = 0f;
        dashDir.Normalize();

        animator.SetBool(anim_DashAttack, true);

        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        int enemyLayer = LayerMask.NameToLayer(enemyLayerName);
        Physics.IgnoreLayerCollision(enemyLayer, playerLayer, true);

        SetMeshTrailActive(true);

        float elapsed = 0f;
        bool hitConnected = false;

        while (elapsed < dashMaxDuration)
        {
            elapsed += Time.deltaTime;

            if (Vector3.Distance(transform.position, player.position) <= dashDamageRange)
            {
                hitConnected = true;
                break;
            }

            _cc.Move(dashDir * dashSpeed * Time.deltaTime);
            yield return null;
        }

        if (hitConnected)
            ApplyDashDamage();

        SetMeshTrailActive(false);
        Physics.IgnoreLayerCollision(enemyLayer, playerLayer, false);

        yield return new WaitForSeconds(dashRecoveryTime);

        animator.SetBool(anim_DashAttack, false);
        canMove = true;
        isActing = false;
    }

    private void ApplyDashDamage()
    {
        Vector3 origin = transform.position + transform.forward;

        Collider[] hits = Physics.OverlapSphere(
            origin, dashHitRadius,
            LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore
        );

        foreach (Collider col in hits)
        {
            var ph = col.GetComponentInParent<PlayerHealthManager>()
                  ?? col.GetComponent<PlayerHealthManager>();
            if (ph == null) continue;

            ph.ModifyHealth(-dashDamage);
            if (ph.deflectsDamage) TakeDamage(dashDamage * 0.5f);

            var tpc = col.GetComponentInParent<ThirdPersonController>()
                   ?? col.GetComponent<ThirdPersonController>();
            if (tpc != null)
            {
                Vector3 knockDir = (col.transform.position - transform.position).normalized;
                tpc.ApplyKnockback(knockDir, dashKnockbackHorizontal, dashKnockbackVertical);
            }
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  Phase 2 Transition
    // ════════════════════════════════════════════════════════════════

    private IEnumerator EnterPhase2()
    {
        isPhase2 = true;
        isActing = true;
        canMove = false;

        if (phase2TransitionVFX != null)
            Instantiate(phase2TransitionVFX, transform.position, Quaternion.identity);

        animator.SetTrigger(anim_Phase2Intro);

        yield return new WaitForSeconds(0.8f);
        heavySwordGO?.SetActive(false);
        katanaGO?.SetActive(true);

        yield return new WaitForSeconds(1.2f);

        canMove = true;
        isActing = false;
    }

    // ════════════════════════════════════════════════════════════════
    //  Movement API — used by ThrenosCombatState
    // ════════════════════════════════════════════════════════════════

    public void MoveTowardsPlayer(float speed = -1f)
    {
        if (!canMove || player == null || isDead) return;
        float s = speed > 0f ? speed : chaseSpeed;
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        _cc.Move(dir.normalized * s * Time.deltaTime);
    }

    public void RotateTowards(Vector3 dir, float speed)
    {
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, Quaternion.LookRotation(dir), speed * Time.deltaTime
        );
    }

    public void SmoothFacePlayer(float speed)
    {
        if (player == null) return;
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f) RotateTowards(dir, speed);
    }

    // ════════════════════════════════════════════════════════════════
    //  Gravity
    // ════════════════════════════════════════════════════════════════

    private void ApplyGravity()
    {
        if (_cc == null) return;
        if (_cc.isGrounded && _verticalVelocity < 0f) _verticalVelocity = -2f;
        else _verticalVelocity += gravity * Time.deltaTime;
        if (!isActing) _cc.Move(new Vector3(0f, _verticalVelocity * Time.deltaTime, 0f));
    }

    // ════════════════════════════════════════════════════════════════
    //  Utility
    // ════════════════════════════════════════════════════════════════

    private void InitHitbox(WeaponHitbox hitbox, float dmg, bool applyKnockback)
    {
        if (hitbox == null) return;
        hitbox.owner = this;
        hitbox.damage = dmg;
        hitbox.applyKnockback = applyKnockback;
    }

    private IEnumerator TurnToFacePlayer(float speed)
    {
        if (player == null) yield break;
        while (true)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.001f) break;
            Quaternion target = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, speed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, target) < 2f) break;
            yield return null;
        }
    }

    private void SetMeshTrailActive(bool active)
    {
        if (meshTrail != null) meshTrail.SetTrailActive(active);
    }
}