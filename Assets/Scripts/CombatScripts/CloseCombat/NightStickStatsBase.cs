using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NightStickStat", menuName = "Weapons/NightStickStat")]
public class NightStickStatsBase : ScriptableObject
{
    public bool canDash = false;
    public float damageMultiply = 1f;
    public float attackCooldown = 0.5f;
}
