using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NightStickStat", menuName = "Weapons/NightStickStat")]
public class NightStickStatsBase : ScriptableObject
{
    public float mainAttackDamage = 15f;
    public float damageMultiply = 1f;
    public float attackSpeed = 1f;
    public float attackRange = 3f;

    [Header("M2 ABILITIES")]
    public bool isSmashActive = false;
    public float smashGroundDamage = 40f;
    public float smashGroundStunTime = 2f;
    public float smashGroundRadius = 2f;
    public float smashGroundCooldown = 20f;

    public bool isSpinActive = false;
    public float spinDamage = 20f;
    public float spinRadius = 2f;
    public float spinDuration = 3f;
    public float spinSpeed = 1f;
    public float spinPlayerSpeed = 1.5f;
    public bool spinHealthRegen = false;
    public float spinCooldown = 15f;

    public bool isDashActive = false;
    public float dashDamage = 30f;
    public float dashRange = 3f;
    public float dashWidth = 1f;
    public bool dashImmunity = false;
    public float dashCooldown = 15f;

    [Header("ULTI")]
    public bool isUltiActive = false;
    public float shieldDuration = 5f;
    public bool shieldSlowness = true;
    public bool damageDeflect = false;
    public float shieldCooldown = 30f;
}
