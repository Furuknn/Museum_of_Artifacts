using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam : MonoBehaviour
{
    private FlashlightStatsBase statsRuntime;
    public enum BeamType { Narrow, Wide }
    public BeamType beamType;

    [Header("Beam Stats")]
    private float _speed;
    private float _damage;
    private float _lifetime;
    private float _cooldown;
    private float _doubleDamageChance;
    private bool _canDoubleDamage;

    public LayerMask hitLayers;

    [Header("Wide Beam Settings")]
    [Tooltip("Defines which axes (X, Y, Z) the scaling will apply to. Use 1 for 'On' and 0 for 'Off'.")]
    public Vector3 expansionAxes = new Vector3(1, 0, 1);
    private float _expansionMultiplier;
    public AnimationCurve scaleCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private Vector3 fireDirection;
    private float distanceTraveled;
    private Vector3 initialScale;

    private float skinOffset = 0.05f; // to avoid clipping

    [SerializeField] private GameObject hitEffectPrefab;

    private List<GameObject> alreadyHitTargets = new List<GameObject>();

    public void InitializeDirection(Camera cam)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        fireDirection = ray.direction.normalized;
    }

    private void Start()
    {
        initialScale = transform.localScale;
        statsRuntime = WeaponStatsManager.Instance.flashlightStatsRuntime;

        if (beamType == BeamType.Narrow)
        {
            _speed = statsRuntime.narrowSpeed;
            _damage = statsRuntime.narrowDamage;
            _lifetime = statsRuntime.narrowLifetime;
            _cooldown = statsRuntime.narrowCooldown;
            _canDoubleDamage = statsRuntime.canDoubleDamage;
            _doubleDamageChance = statsRuntime.doubleDamageChance;
        }
        else if (beamType == BeamType.Wide)
        {
            _speed = statsRuntime.wideSpeed;
            _damage = statsRuntime.wideDamage;
            _lifetime = statsRuntime.wideLifetime;
            _cooldown = statsRuntime.wideCooldown;
            _expansionMultiplier = statsRuntime.wideExpansionMultiplier;
        }

        Destroy(gameObject, _lifetime);

        if (fireDirection == Vector3.zero)
            fireDirection = transform.forward;
    }

    private void Update()
    {
        float moveDistance = _speed * Time.deltaTime;
        float currentRadius = transform.localScale.x * 1f;

        // RAYCAST FOR COLLISION
        // In Update(), replace the narrow beam block
        if (beamType == BeamType.Narrow)
        {
            if (Physics.SphereCast(transform.position, currentRadius, fireDirection,
                out RaycastHit hit, moveDistance + skinOffset, hitLayers))
            {
                // Collider can exist on a destroyed object during the deferred destroy window
                if (hit.collider == null || hit.collider.gameObject == null) return;

                HandleImpact(hit.collider.gameObject, hit.point, hit.normal);
                return;
            }
        }
        else if (beamType == BeamType.Wide)
        {
            // Wide Beam: Hits EVERYTHING in its path (Piercing)
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, currentRadius, fireDirection, moveDistance + skinOffset, hitLayers);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == null || hit.collider.gameObject == null) return;

                HandleImpact(hit.collider.gameObject, hit.point, hit.normal);
            }
        }

        transform.position += fireDirection * moveDistance;
        distanceTraveled += moveDistance;

        if (beamType == BeamType.Wide)
            ApplyScaleExpansion();
    }

    // --- CENTRALIZED IMPACT LOGIC ---
    private void HandleImpact(GameObject hitObj, Vector3 hitPoint, Vector3 hitNormal)
    {
        // hitObj can be a destroyed object reference during Unity's deferred destroy window
        if (hitObj == null) return;

        if (beamType == BeamType.Wide)
        {
            if (alreadyHitTargets.Contains(hitObj)) return;
            alreadyHitTargets.Add(hitObj);
        }

        IDamageable damageable = hitObj.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(GetDamage());

        if (beamType == BeamType.Narrow)
        {
            SpawnHitVFX(hitPoint, hitNormal);
            Destroy(gameObject);
        }
        else if (beamType == BeamType.Wide && damageable != null)
        {
            SpawnHitVFX(hitPoint, hitNormal);
        }
    }

    private void SpawnHitVFX(Vector3 position, Vector3 normal)
    {
        if (hitEffectPrefab == null) return;

        Quaternion rot = normal != Vector3.zero ? Quaternion.LookRotation(normal) : Quaternion.identity;
        GameObject hitVFX = Instantiate(hitEffectPrefab, position, rot);
        Destroy(hitVFX, 2f);
    }
    // --------------------------------

    private float GetDamage()
    {
        // Explicitly lock the double damage chance to the Narrow beam only
        if (beamType == BeamType.Narrow && _canDoubleDamage && Random.value < _doubleDamageChance)
            return _damage * 2f;

        return _damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleImpact(other.gameObject, transform.position, Vector3.zero);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 hitPoint = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
        Vector3 hitNormal = collision.contactCount > 0 ? collision.GetContact(0).normal : Vector3.zero;

        HandleImpact(collision.gameObject, hitPoint, hitNormal);
    }

    private void ApplyScaleExpansion()
    {
        float timeProgress = distanceTraveled / (_speed * _lifetime);
        float curveValue = scaleCurve.Evaluate(timeProgress);

        float finalScale = initialScale.x * (1f + curveValue * (_expansionMultiplier - 1f));

        Vector3 newScale = initialScale;
        newScale.x = Mathf.Lerp(initialScale.x, finalScale, expansionAxes.x);
        newScale.y = Mathf.Lerp(initialScale.y, finalScale, expansionAxes.y);
        newScale.z = Mathf.Lerp(initialScale.z, finalScale, expansionAxes.z);

        transform.localScale = newScale;
    }

    private void OnDrawGizmos()
    {
        float radius = transform.localScale.x * 1f;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);

        float simulatedDistance = (_speed > 0 ? _speed : 20f) * 0.016f;
        Gizmos.color = Color.red;
        Vector3 direction = fireDirection != Vector3.zero ? fireDirection : transform.forward;

        Gizmos.DrawRay(transform.position, direction * (simulatedDistance + skinOffset));

        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(transform.position + (direction * (simulatedDistance + skinOffset)), radius);
    }
}