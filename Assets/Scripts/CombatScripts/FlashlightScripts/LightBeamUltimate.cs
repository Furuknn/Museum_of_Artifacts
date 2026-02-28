using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class LightBeamUltimate : MonoBehaviour
{
    private FlashlightStatsBase statsRuntime;

    private float _laserRange => WeaponStatsManager.Instance.flashlightStatsRuntime.ultimateRange;
    private float _laserDuration => WeaponStatsManager.Instance.flashlightStatsRuntime.ultimateDuration;
    private float _laserDamage => WeaponStatsManager.Instance.flashlightStatsRuntime.ultimateDamage;
    private float _ultimateHeaviness => WeaponStatsManager.Instance.flashlightStatsRuntime.ultimateHeaviness;
    private float cameraResistanceMultiplier => WeaponStatsManager.Instance.flashlightStatsRuntime.ultimateCameraResistance;

    [Header("Beam Width Settings")]
    [SerializeField] private float thinWidth = 0.01f;
    [SerializeField] private float fullWidth = 0.25f;
    [SerializeField] private float expandTime = 0.07f;

    [Header("Beam Width Curve")]
    [SerializeField]
    private AnimationCurve widthExpandCurve =
    AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Damage Tick")]
    [SerializeField] private float damageTick = 0.25f;
    private float damageTimer;

    [Header("Line Renderer")]
    [SerializeField] private LineRenderer laserLine;

    [Header("Material Flash Settings")]
    [SerializeField] private Material beamMaterial;
    [SerializeField] private Color baseColor = Color.red;
    [SerializeField] private float baseIntensity = 1f;
    [SerializeField] private float emissionPulse = 1f;
    [SerializeField] private float pulseSpeed = 4f;

    private Material materialInstance;
    private static readonly int emissionID = Shader.PropertyToID("_EmissionColor");


    [Header("Camera")]
    [SerializeField] private Camera playerCam;
    private CinemachineFreeLook freeLookCam;

    private float baseXSpeed;
    private float baseYSpeed;



    [SerializeField] private GameObject hitEffectPrefab;
    private GameObject hitVFX;

    // Internal
    private Transform firePoint;
    private bool isLaserActive = false;
    private float playerBaseSpeed;
    private bool canDealDamage = false;

    #region Unity Methods

    private void Awake()
    {
        if (playerCam == null)
            playerCam = Camera.main;

        if(freeLookCam==null)
            freeLookCam=FindAnyObjectByType<CinemachineFreeLook>();

        if (laserLine == null)
            laserLine = GetComponent<LineRenderer>();

        SetupMaterial();
        laserLine.enabled = false;

        statsRuntime = WeaponStatsManager.Instance.flashlightStatsRuntime;

        
    }

    private void Start()
    {
        
    }

    void Update()
    {
        if (isLaserActive)
            UpdateLaser();
    }

    private void LateUpdate()
    {
        UpdateEmissionPulse();
    }

    #endregion

    #region Setup

    private void SetupMaterial()
    {
        if (beamMaterial == null)
        {
            materialInstance = laserLine.material;
            baseColor = materialInstance.color;
        }
        else
        {
            materialInstance = Instantiate(beamMaterial);
            laserLine.material = materialInstance;
        }

        materialInstance.EnableKeyword("_EMISSION");
        materialInstance.SetColor(emissionID, baseColor * baseIntensity);
    }
    #endregion

    #region Laser Logic
    public void InitiateLaser(Transform firePoint)
    {
        this.firePoint = firePoint;

        laserLine.enabled = true;
        laserLine.positionCount = 2;

        // Start ultra-thin (energy gathered)
        laserLine.startWidth = thinWidth;
        laserLine.endWidth = thinWidth;

        canDealDamage = false;

        if (freeLookCam != null)
        {
            baseXSpeed = freeLookCam.m_XAxis.m_MaxSpeed;
            baseYSpeed = freeLookCam.m_YAxis.m_MaxSpeed;

            freeLookCam.m_XAxis.m_MaxSpeed *= cameraResistanceMultiplier;
            freeLookCam.m_YAxis.m_MaxSpeed *= cameraResistanceMultiplier;
        }

        isLaserActive = true;

        StartCoroutine(ExpandLaserWidth());
        StartCoroutine(StopLaserAfterDelay());
    }

    private IEnumerator ExpandLaserWidth()
    {
        float t = 0f;

        while (t < expandTime)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / expandTime);

            float curveValue = widthExpandCurve.Evaluate(normalized);
            float width = Mathf.Lerp(thinWidth, fullWidth, curveValue);

            laserLine.startWidth = width;
            laserLine.endWidth = width * 0.85f;

            yield return null;
        }

        ThirdPersonController controller = FindAnyObjectByType<ThirdPersonController>();
        playerBaseSpeed = controller.GetPlayerSpeed();

        float newSpeed = controller.GetPlayerSpeed() / _ultimateHeaviness;
        controller.SetPlayerSpeed(newSpeed);

        laserLine.startWidth = fullWidth;
        laserLine.endWidth = fullWidth * 0.85f;

        canDealDamage = true;
    }


    private void UpdateLaser()
    {
        Vector3 camPos = playerCam.transform.position;
        Vector3 camDir = playerCam.transform.forward;

        // 1. Find visual endpoint (first hit only)
        Vector3 endPoint = camPos + camDir * _laserRange;

        if (Physics.Raycast(camPos, camDir, out RaycastHit firstHit, _laserRange))
        {
            endPoint = firstHit.point;
        }

        // 2. Set laser positions
        Vector3 start = firePoint.position;
        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, endPoint);

        // 3. Piercing damage (RaycastAll)
        if (!canDealDamage) return;

        damageTimer += Time.deltaTime;

        if (damageTimer >= damageTick)
        {
            damageTimer = 0f;

            RaycastHit[] hits = Physics.RaycastAll(camPos, camDir, _laserRange);
            foreach (RaycastHit hit in hits)
            {
                hit.collider.GetComponent<IDamageable>()
                    ?.TakeDamage(_laserDamage);
            }
        }

        // 4. Hit VFX at visual endpoint
        if (hitEffectPrefab != null)
        {
            if (hitVFX == null)
                hitVFX = Instantiate(hitEffectPrefab, endPoint, Quaternion.identity);
            else
                hitVFX.transform.position = endPoint;
        }
    }



    private IEnumerator StopLaserAfterDelay()
    {
        yield return new WaitForSeconds(_laserDuration);

        isLaserActive = false;
        laserLine.enabled = false;

        ThirdPersonController controller = FindAnyObjectByType<ThirdPersonController>();
        controller.SetPlayerSpeed(playerBaseSpeed);

        if (freeLookCam != null)
        {
            freeLookCam.m_XAxis.m_MaxSpeed = baseXSpeed;
            freeLookCam.m_YAxis.m_MaxSpeed = baseYSpeed;
        }

        if (hitVFX != null)
            Destroy(hitVFX);

        Destroy(gameObject);
    }
    #endregion

    #region Visuals

    private void UpdateEmissionPulse()
    {
        if (materialInstance == null || !laserLine.enabled)
            return;

        float sine = Mathf.Sin(Time.time * pulseSpeed);
        float intensity = Mathf.Max(0f, baseIntensity + sine * emissionPulse);

        materialInstance.SetColor(emissionID, baseColor * intensity);
    }

    #endregion
}
