using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeamShooter : MonoBehaviour, IBeam
{
    [SerializeField] private LightBeam beamPrefab;
    [SerializeField] private Transform firePoint;
    private float lastFireTime;
    float _cooldown =>
    beamPrefab.beamType == LightBeam.BeamType.Narrow
        ? WeaponStatsManager.Instance.flashlightStatsRuntime.narrowCooldown
        : WeaponStatsManager.Instance.flashlightStatsRuntime.wideCooldown;

    private HUDManager hudManager;


    private void Awake()
    {
        hudManager = FindAnyObjectByType<HUDManager>();
    }
    public void Shoot(Vector3 origin, Vector3 direction)
    {
        if (Time.time - lastFireTime < _cooldown) return;

        lastFireTime = Time.time;

        LightBeam beam = Instantiate(beamPrefab, firePoint.position, Quaternion.LookRotation(direction));

        if (hudManager != null)
        {
            int attackIndex =
                beamPrefab.beamType == LightBeam.BeamType.Narrow ? 0 :
                beamPrefab.beamType == LightBeam.BeamType.Wide ? 1 : -1;

            if (attackIndex != -1)
                hudManager.StartCooldown(_cooldown, attackIndex);
        }
    }
}
