using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Statistics/Create a Statistic")]
public class PlayerStatisticsSO : ScriptableObject
{
    public float maxHealth;
    public float damage;
    public float damageMultiplier;
    public float xpMultiplier;
    public float xpRequiredToLevelUp;
}
