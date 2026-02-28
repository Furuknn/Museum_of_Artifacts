using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightUltimateShooter : MonoBehaviour, IBeam
{
    [SerializeField] private LightBeamUltimate beamUltimatePrefab;
    [SerializeField] private Transform firePoint;

    private float lastFireTime;
    private float _cooldown => WeaponStatsManager.Instance.flashlightStatsRuntime.ultimateCooldown;

    LightBeamUltimate spawnedLaser;

    private HUDManager hudManager;
    
    public void Shoot(Vector3 origin, Vector3 direction)
    {
        if (Time.time - lastFireTime < _cooldown) return;
        lastFireTime = Time.time;

        spawnedLaser = Instantiate(beamUltimatePrefab, Vector3.zero, Quaternion.identity);
        spawnedLaser.InitiateLaser(firePoint);

        if (hudManager != null)
        {
            int attackIndex = 2;
            
            hudManager.StartCooldown(_cooldown, attackIndex);
        }
    }

    private void Awake()
    {
        hudManager = FindAnyObjectByType<HUDManager>();
        lastFireTime = -_cooldown;
    }
    
}
