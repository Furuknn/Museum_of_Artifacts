using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class LightBeamUltimate : MonoBehaviour
{
    private FlashlightStatsBase statsRuntime;

    private float _laserRange => WeaponStatsManager.Instance.flashlightStatsRuntime.ultimateRange;
    private float _laserDuration => WeaponStatsManager.Instance.flashlightStatsRuntime.ultimateDuration;
    private float _laserDamage => WeaponStatsManager.Instance.flashlightStatsRuntime.ultimateDamage;
    private float _laserTick => WeaponStatsManager.Instance.flashlightStatsRuntime.ultimateTick;
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

        // 1. Find visual endpoint — stop only at solid geometry, ignore enemies
        Vector3 endPoint = camPos + camDir * _laserRange;

        RaycastHit[] allHits = Physics.RaycastAll(camPos, camDir, _laserRange);
        System.Array.Sort(allHits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in allHits)
        {
            // If it's NOT a damageable, treat it as a wall and stop the beam visually
            if (hit.collider.GetComponent<IDamageable>() == null)
            {
                endPoint = hit.point - camDir*0.15f;
                break;
            }
            // Damageable? Skip it — beam passes through
        }

        // 2. Set laser positions
        laserLine.SetPosition(0, firePoint.position);
        laserLine.SetPosition(1, endPoint);

        // 3. Volumetric damage via OverlapCapsule
        if (!canDealDamage) return;

        damageTimer += Time.deltaTime;
        if (damageTimer >= _laserTick)
        {
            damageTimer = 0f;
            DealDamageInBeamVolume(firePoint.position, endPoint);
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

    private void DealDamageInBeamVolume(Vector3 startPos, Vector3 endPos)
    {
        Vector3 direction = (endPos - startPos).normalized;

        // Inset the capsule end-points by radius so the end-caps don't balloon out
        Vector3 capsulePoint0 = startPos + direction * fullWidth;
        Vector3 capsulePoint1 = endPos - direction * fullWidth;

        // Guard against zero-length beams
        if (Vector3.Dot(capsulePoint1 - startPos, direction) <= 0f)
            capsulePoint1 = capsulePoint0;

        Collider[] hitColliders = Physics.OverlapCapsule(capsulePoint0, capsulePoint1, fullWidth);

        HashSet<IDamageable> alreadyHit = new HashSet<IDamageable>();

        foreach (Collider col in hitColliders)
        {
            IDamageable damageable = col.GetComponent<IDamageable>();
            if (damageable != null && !alreadyHit.Contains(damageable))
            {
                damageable.TakeDamage(_laserDamage);
                alreadyHit.Add(damageable);
            }
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!isLaserActive || !canDealDamage || firePoint == null) return;

        Vector3 camPos = playerCam.transform.position;
        Vector3 camDir = playerCam.transform.forward;

        // Recalculate endPoint exactly as UpdateLaser does
        Vector3 endPoint = camPos + camDir * _laserRange;

        RaycastHit[] allHits = Physics.RaycastAll(camPos, camDir, _laserRange);
        System.Array.Sort(allHits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in allHits)
        {
            if (hit.collider.GetComponent<IDamageable>() == null)
            {
                endPoint = hit.point - camDir*0.15f;
                break;
            }
        }

        Vector3 startPos = firePoint.position;
        Vector3 direction = (endPoint - startPos).normalized;
        float radius = fullWidth;

        Vector3 capsulePoint0 = startPos + direction * radius;
        Vector3 capsulePoint1 = endPoint - direction * radius;
        if (Vector3.Dot(capsulePoint1 - startPos, direction) <= 0f)
            capsulePoint1 = capsulePoint0;

        // Draw the capsule body as a wire cylinder approximation
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.4f);
        DrawWireCapsule(capsulePoint0, capsulePoint1, radius);

        // Draw the LineRenderer path so you can compare
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPos, endPoint);

        // Draw end-cap centers
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(capsulePoint0, 0.05f);
        Gizmos.DrawWireSphere(capsulePoint1, 0.05f);
    }

    private void DrawWireCapsule(Vector3 p0, Vector3 p1, float radius)
    {
        // p0 and p1 are the sphere centers of the capsule
        Vector3 up = (p1 - p0).normalized;
        Vector3 forward = Vector3.Slerp(Vector3.forward, -Vector3.up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized;
        forward = Vector3.Cross(right, up).normalized;

        int segments = 20;

        // Draw two end-cap circles
        DrawCircle(p0, up, radius, segments);
        DrawCircle(p1, up, radius, segments);

        // Draw 4 longitudinal lines connecting the caps
        for (int i = 0; i < 4; i++)
        {
            float angle = i * Mathf.PI / 2f;
            Vector3 offset = (Mathf.Cos(angle) * right + Mathf.Sin(angle) * forward) * radius;
            Gizmos.DrawLine(p0 + offset, p1 + offset);
        }

        // Draw end-cap hemisphere arcs (2 planes each)
        DrawHemisphereArc(p0, up, right, radius, segments, false);
        DrawHemisphereArc(p0, up, forward, radius, segments, false);
        DrawHemisphereArc(p1, up, right, radius, segments, true);
        DrawHemisphereArc(p1, up, forward, radius, segments, true);
    }

    private void DrawCircle(Vector3 center, Vector3 normal, float radius, int segments)
    {
        Vector3 tangent = Vector3.Slerp(Vector3.forward, -Vector3.up, 0.5f);
        Vector3 right = Vector3.Cross(normal, tangent).normalized;
        Vector3 forward = Vector3.Cross(right, normal).normalized;

        Vector3 prev = center + right * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 next = center + (Mathf.Cos(angle) * right + Mathf.Sin(angle) * forward) * radius;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }

    private void DrawHemisphereArc(Vector3 center, Vector3 up, Vector3 axis, float radius, int segments, bool flipped)
    {
        int half = segments / 2;
        float flip = flipped ? 1f : -1f;
        Vector3 prev = center + axis * radius;

        for (int i = 1; i <= half; i++)
        {
            float angle = i * Mathf.PI / half;
            Vector3 next = center + (Mathf.Cos(angle) * axis + flip * Mathf.Sin(angle) * up) * radius;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
#endif
}
