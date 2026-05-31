using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Attach to the child GameObject that has the BoxCollider (isTrigger = true).
// ThrenosController calls OpenWindow() / CloseWindow() via animation events.
// Each Open clears the hit list — so calling it 3 times per animation
// gives 3 independent hit opportunities, player can be hit once per window.
public class WeaponHitbox : MonoBehaviour
{
    [HideInInspector] public ThrenosController owner;
    [HideInInspector] public float damage;
    [HideInInspector] public bool applyKnockback;

    private bool isWindowOpen = false;
    private HashSet<Collider> hitThisWindow = new HashSet<Collider>();

    // ── Called by animation events ───────────────────────────────────

    // Open a fresh hit window — resets who has been hit.
    // Call this up to 3 times per animation at each moment a hit is possible.
    public void OpenWindow()
    {
        isWindowOpen = true;
        hitThisWindow.Clear();
    }

    // Close the window — no more hits until next OpenWindow()
    public void CloseWindow()
    {
        isWindowOpen = false;
        hitThisWindow.Clear();
    }

    // ── Trigger detection ────────────────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        if (!isWindowOpen) return;
        if (hitThisWindow.Contains(other)) return;

        // Walk up the hierarchy — player collider may be a child
        PlayerHealthManager ph = other.GetComponentInChildren<PlayerHealthManager>()
                              ?? other.GetComponent<PlayerHealthManager>();
        if (ph == null)
        {
            Debug.LogWarning("HealthManager not found!! in WeaponHitbox");
            return;
        }

        hitThisWindow.Add(other);
        ph.ModifyHealth(-damage);

        if (ph.deflectsDamage)
            owner?.TakeDamage(damage * 0.5f);

        if (applyKnockback && owner != null)
        {
            ThirdPersonController tpc = other.GetComponentInParent<ThirdPersonController>()
                                     ?? other.GetComponent<ThirdPersonController>();
            if (tpc != null)
            {
                Vector3 knockDir = (other.transform.position - owner.transform.position).normalized;
                tpc.ApplyKnockback(knockDir,
                                   owner.dashKnockbackHorizontal,
                                   owner.dashKnockbackVertical);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!isWindowOpen) return;
        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(bc.center, bc.size);
        }
    }
}