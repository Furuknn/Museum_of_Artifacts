using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Attack", menuName = "Enemy/Attack Data")]
public class EnemyAttackSO : ScriptableObject
{
    [Header("Attack Properties")]
    public float damage = 10f;
    
    [Header("Attack Cooldown")]
    [Tooltip("Bu saldırı yapıldıktan sonra ne kadar bekleyecek (saniye)")]
    public float attackCooldown = 2f;
    
    [Header("Animation")]
    public RuntimeAnimatorController animatorOV;
}