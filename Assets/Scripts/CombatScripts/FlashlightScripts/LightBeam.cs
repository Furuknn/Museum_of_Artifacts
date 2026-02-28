using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam: MonoBehaviour
{
    private FlashlightStatsBase statsRuntime;
    public enum BeamType {Narrow,Wide}
    public BeamType beamType;

    [Header("Beam Stats")]
    private float _speed;
    private float _damage;
    private float _lifetime;
    private float _cooldown;
    public LayerMask hitLayers;

    [Header("Wide Beam Settings")]
    [Tooltip("Defines which axes (X, Y, Z) the scaling will apply to. Use 1 for 'On' and 0 for 'Off'.")]
    public Vector3 expansionAxes = new Vector3(1, 0, 1); // Default to X and Z (width/height)
    [Tooltip("The maximum multiplier for the scale at the end of the curve.")]
    private float _expansionMultiplier;
    [Tooltip("The curve evaluates the scale factor from 0 (start) to 1 (end of lifetime).")]
    public AnimationCurve scaleCurve = AnimationCurve.Linear(0, 0, 1, 1); // Changed default curve

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
        }
        else if (beamType==BeamType.Wide)
        {
            _speed= statsRuntime.wideSpeed;
            _damage= statsRuntime.wideDamage;
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

        if (beamType == BeamType.Narrow)
        {
            // Narrow Beam: Stops on the first thing it hits (Blocking)
            if (Physics.SphereCast(transform.position, currentRadius, fireDirection, out RaycastHit hit, moveDistance + skinOffset, hitLayers))
            {
                EnemyScript enemy = hit.collider.gameObject.GetComponent<EnemyScript>();
                if (enemy != null)
                {
                    enemy.TakeDamage(_damage);
                }

                // Destroy beam on impact
                Destroy(gameObject);

                if (hitEffectPrefab != null)
                {
                    GameObject hitVFX = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(hitVFX, 2f);
                }
                return; // Stop execution so we don't move or expand
            }
        }
        else if (beamType == BeamType.Wide)
        {
            // Wide Beam: Hits EVERYTHING in its path (Piercing)
            // SphereCastAll returns an array of everything hit in this frame's sweep
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, currentRadius, fireDirection, moveDistance + skinOffset, hitLayers);

            foreach (RaycastHit hit in hits)
            {
                GameObject hitObj = hit.collider.gameObject;

                // Check if we already hit this specific enemy instance
                if (!alreadyHitTargets.Contains(hitObj))
                {
                    EnemyScript enemy = hitObj.GetComponent<EnemyScript>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(_damage);
                        alreadyHitTargets.Add(hitObj); // Mark as hit

                        // Optional: Spawn hit effect for wide beam hitting an enemy
                        if (hitEffectPrefab != null)
                            Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                }
            }
        }

        transform.position += fireDirection * moveDistance;

        // Apply expansion over distance
        distanceTraveled += moveDistance;

        if (beamType==BeamType.Wide)
            ApplyScaleExpansion();

    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyScript enemy = other.gameObject.GetComponent<EnemyScript>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damage);
        }

        if (beamType == BeamType.Narrow) // sadece ince beamse temas anýnda yok et
        {
            Destroy(gameObject);
            GameObject hitVFX = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

            Destroy(hitVFX, 2f);
            return;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damage);
        }

        if (beamType == BeamType.Narrow) // sadece ince beamse temas anýnda yok et
        {
            Destroy(gameObject);
            GameObject hitVFX = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

            Destroy(hitVFX, 2f);
            return;
        }
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
        // Use the same radius logic as your Update loop
        float radius = transform.localScale.x * 1f;

        // 1. Draw the "Hit Sphere" at the projectile's current center
        Gizmos.color = Color.cyan; // Cyan for the sphere volume
        Gizmos.DrawWireSphere(transform.position, radius);

        // 2. Draw the "Cast Ray" (The path it checks this frame)
        // We simulate a frame's movement (e.g., 1/60th of a second) to see how far it checks ahead
        float simulatedDistance = (_speed > 0 ? _speed : 20f) * 0.016f;

        Gizmos.color = Color.red; // Red for the forward check direction
        Vector3 direction = fireDirection != Vector3.zero ? fireDirection : transform.forward;

        // Draw the ray from the center
        Gizmos.DrawRay(transform.position, direction * (simulatedDistance + skinOffset));

        // 3. (Optional) Draw the sphere at the END of the cast
        // This helps visualize the "Capsule" shape the cast effectively creates
        Gizmos.color = new Color(0, 1, 1, 0.3f); // Faint Cyan
        Gizmos.DrawWireSphere(transform.position + (direction * (simulatedDistance + skinOffset)), radius);
    }
}
