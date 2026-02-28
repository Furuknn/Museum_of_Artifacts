using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightStickScript : WeaponBase
{
    Animator animator;

    bool canDealDamage;
    List<GameObject> dealtDamage;
    
    private NightStickStatsBase statsRuntime;
    [SerializeField] float stickLenght;

    [Header("Combo Ayarları")]
    [SerializeField] private List<AttackSO> combo;
    [SerializeField] private float comboResetWindow = 1.0f;
    
    private int comboCounter;
    private float nextAttackTime = 0f;
    private float resetComboTime = 0f; 

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        canDealDamage = false;
        dealtDamage = new List<GameObject>();

        if (WeaponStatsManager.Instance != null && WeaponStatsManager.Instance.nightStickStatsRuntime != null)
        {
            statsRuntime = WeaponStatsManager.Instance.nightStickStatsRuntime;
        }
    }

    void Update()
    {
        if (canDealDamage)
        {
            RaycastHit hit;
            int layerMask = 1 << 7; // Enemy Layer

            float currentLength = (statsRuntime != null) ? 2f : stickLenght;

            if (Physics.Raycast(transform.position, -(-transform.right), out hit, currentLength, layerMask))
            {
                if (!dealtDamage.Contains(hit.transform.gameObject))
                {
                    dealtDamage.Add(hit.transform.gameObject);
                    EnemyScript enemy = hit.transform.gameObject.GetComponent<EnemyScript>();
                    if (enemy != null)
                    {
                        var playerStats = GameManager.instance.characters[GameManager.instance.currentHeroIndex].playerStatisticsSO;
                        
                        // Hasar Hesaplama
                        float weaponDamageMult = (statsRuntime != null) ? statsRuntime.damageMultiply : 1f;
                        float totalDamage = playerStats.damage * playerStats.damageMultiplier * weaponDamageMult;
                        
                        enemy.TakeDamage(totalDamage);
                    }
                }
            }
        }
        
        if (Time.time > resetComboTime && comboCounter > 0)
        {
           // comboCounter = 0; 
        }
    }

    public override void MainAttack()
    {
        if (Time.time < nextAttackTime) return;

        if (Time.time > resetComboTime)
        {
            comboCounter = 0;
        }

        PerformAttack();
    }

    private void PerformAttack()
    {
        if (combo.Count == 0) return;

        AttackSO currentAttack = combo[comboCounter];
        animator.runtimeAnimatorController = currentAttack.animatorOV;
        animator.Play("Attack", 0, 0);

        
        float animDuration = GetClipLength(currentAttack.animatorOV);
        
        float cooldown = (statsRuntime != null) ? statsRuntime.attackCooldown : 0.5f;

        nextAttackTime = Time.time + animDuration + cooldown;

        resetComboTime = nextAttackTime + comboResetWindow;

        comboCounter++;
        if (comboCounter >= combo.Count)
        {
            comboCounter = 0;
        }
    }
    private float GetClipLength(AnimatorOverrideController ov)
    {
        if (ov == null) return 1f; // Güvenlik

        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        ov.GetOverrides(overrides);

        foreach (var pair in overrides)
        {
            if (pair.Value != null)
            {
                return pair.Value.length;
            }
        }

        return 1f;
    }
    

    public override void SecondaryAttack()//Dash
    {
        
    }
    public override void UltimateAttack() { }

    public void StartDealDamage()
    {
        canDealDamage = true;
        dealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
        // ThirdPersonController.instance.isAttacking = false; // Gerekirse aç
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - (-transform.right) * stickLenght);
    }
}