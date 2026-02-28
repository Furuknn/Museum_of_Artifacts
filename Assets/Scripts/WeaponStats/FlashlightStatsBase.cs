using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlashlightStats", menuName = "Weapons/FlashlightStats")]
public class FlashlightStatsBase : ScriptableObject
{
    [Header("Narrow Beam")]
    public float narrowDamage;
    public float narrowSpeed;
    public float narrowLifetime;
    public float narrowCooldown;

    [Header("Wide Beam")]
    public float wideDamage;
    public float wideSpeed;
    public float wideLifetime;
    public float wideCooldown;
    public float wideExpansionMultiplier;

    [Header("Ultimate Beam")]
    public float ultimateDamage;
    public float ultimateRange;
    public float ultimateDuration;
    public float ultimateCooldown;
    public float ultimateWindUpTime;
    public float ultimateHeaviness;
    public float ultimateCameraResistance;

    [Header("Light Bomb")]
    public float bombDamage;
    public float bombTick;
    public float bombLifetime;
    public float bombCooldown;
    public int bombAmount;
}
