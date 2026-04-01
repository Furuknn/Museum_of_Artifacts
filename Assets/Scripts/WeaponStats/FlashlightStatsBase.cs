using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlashlightStats", menuName = "Weapons/FlashlightStats")]
public class FlashlightStatsBase : ScriptableObject
{
    [Header("[Main] Narrow Beam")]
    public float narrowDamage;
    public float narrowSpeed;
    public float narrowLifetime;
    public float narrowCooldown;
    public bool canDoubleDamage;
    public float doubleDamageChance;

    [Header("[Secondary] Expanding Beam")]
    public float wideDamage;
    public float wideSpeed;
    public float wideLifetime;
    public float wideCooldown;
    public float wideExpansionMultiplier;

    [Header("[Secondary] Light Bomb")]
    public float bombDamage;
    public float bombTick;
    public float bombLifetime;
    public float bombCooldown;
    public int bombAmount;

    [Header("[Ulti] Asteroid Destroyer")]
    public float ultimateDamage;
    public float ultimateTick;
    public float ultimateRange;
    public float ultimateDuration;
    public float ultimateCooldown;
    public float ultimateWindUpTime;
    public float ultimateHeaviness;
    public float ultimateCameraResistance;

}
