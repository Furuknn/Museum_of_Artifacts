using System.Collections;
using UnityEngine;

// Attach this to your ground slash VFX prefab.
// Call Launch() from ThrenosController.SpawnGroundSlash()
public class GroundSlashProjectile : MonoBehaviour
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private float hitRadius = 1.2f;
    [SerializeField] private float slowDownRate = 0.01f;
    [SerializeField] private float detectingDistance = 0.1f;

    private Vector3 direction;
    private float speed;
    private float lifetime;
    private Rigidbody rb;

    // Already-hit list so the projectile only damages the player once
    private bool hasHitPlayer;
    private bool stopped;

    public void Launch(Vector3 dir, float spd, float life)
    {
        direction = dir.normalized;
        direction.y = 0f;
        speed = spd;
        lifetime = life;
        stopped = false;

        //StartCoroutine(LifetimeRoutine());
    }
    private void Start()
    {

        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            rb.velocity = direction * speed;
            StartCoroutine(SlowDown());
        }

        Destroy(gameObject, lifetime+2);
    }
    private void FixedUpdate()
    {
        if (!stopped)
        {
            // Snap to ground surface
            Vector3 rayOrigin = transform.position + Vector3.up * 1f;
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 1f + detectingDistance, LayerMask.GetMask("Ground")))
            {
                Vector3 pos = rb.position;
                pos.y = hit.point.y;
                rb.MovePosition(pos);
            }
        }

        CheckHit();
    }

    private void CheckHit()
    {
        if (hasHitPlayer) return;

        Collider[] hits = Physics.OverlapSphere(
            transform.position, hitRadius,
            LayerMask.GetMask("Player"),
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider col in hits)
        {
            var ph = col.GetComponent<PlayerHealthManager>();
            if (ph == null) continue;

            ph.ModifyHealth(-damage);
            hasHitPlayer = true;
            break;
        }
    }
    private IEnumerator SlowDown()
    {
        float t = 0f;
        Vector3 initialVelocity = direction * speed;

        while (t < 1f)
        {
            t += slowDownRate;
            rb.velocity = Vector3.Lerp(initialVelocity, Vector3.zero, t);
            yield return new WaitForSeconds(0.1f);
        }

        rb.velocity = Vector3.zero;
        stopped = true;
    }
    private IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}