using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBomb : MonoBehaviour
{
    private FlashlightStatsBase statsRuntime;

    [Header("References")]
    [SerializeField] private GameObject bombShell;
    [SerializeField] private GameObject bombCore;

    [Header("Explosion Settings")]
    [SerializeField] private float expansionMultiplier = 3f;
    [SerializeField] private float expansionSpeed = 1f;
    [SerializeField] private float coreScaleMultiplier = 2f;
    [SerializeField] private AnimationCurve expansionCurve;

    [Header("Shrink Settings")]
    [SerializeField] private float shrinkSpeed = 1f;
    [SerializeField] private AnimationCurve shrinkCurve;

    [Header("Collision")]
    [SerializeField] private LayerMask hitLayers;

    [Header("Light Bomb Stats")]
    private float _bombDamage;
    private float _bombTick;
    private float _bombLifetime;
    private float _bombCooldown;
    private int _bombAmount;

    private bool hasExploded = false;
    private Vector3 initialScale;
    private MeshCollider damageCollider;
    private readonly List<IDamageable> enemiesInside = new();
    private Coroutine damageRoutine;


    private void Awake()
    {
        if (bombShell == null)
            Debug.LogError("Bomb shell has not been assigned!");

        initialScale = bombShell.transform.localScale;
    }

    private void Start()
    {
        statsRuntime = WeaponStatsManager.Instance.flashlightStatsRuntime;

        _bombDamage = statsRuntime.bombDamage;
        _bombTick = statsRuntime.bombTick;
        _bombLifetime = statsRuntime.bombLifetime;
        _bombCooldown = statsRuntime.bombCooldown;
        _bombAmount = statsRuntime.bombAmount;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;

        if (((1 << collision.gameObject.layer) & hitLayers) == 0)
            return;

        hasExploded = true;

        bombCore.transform.localScale *= coreScaleMultiplier;

        // Stop physics
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Stick bomb to surface
        transform.position = collision.contacts[0].point;

        SetupDamageCollider();

        damageRoutine = StartCoroutine(DamageTickRoutine());

        StartCoroutine(ExplosionRoutine());
    }

    private void SetupDamageCollider()
    {
        damageCollider = bombShell.AddComponent<MeshCollider>();
        damageCollider.convex = true;
        damageCollider.isTrigger = true;
    }
    private IEnumerator ExplosionRoutine()
    {
        // -------- EXPAND --------
        yield return StartCoroutine(Expand());

        // -------- LIFETIME --------
        yield return new WaitForSeconds(_bombLifetime);

        // -------- SHRINK --------
        yield return StartCoroutine(Shrink());

        Destroy(gameObject);
    }

    private IEnumerator Expand()
    {
        float timer = 0f;
        float duration = 1f / Mathf.Max(0.01f, expansionSpeed);
        Vector3 targetScale = initialScale * expansionMultiplier;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            float curveValue = expansionCurve.Evaluate(t);
            bombShell.transform.localScale =
                Vector3.LerpUnclamped(initialScale, targetScale, curveValue);

            yield return null;
        }

        bombShell.transform.localScale = targetScale;
    }

    private IEnumerator Shrink()
    {
        float timer = 0f;
        float duration = 1f / Mathf.Max(0.01f, shrinkSpeed);

        Vector3 startScale = bombShell.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            float curveValue = shrinkCurve.Evaluate(t);
            bombShell.transform.localScale =
                Vector3.LerpUnclamped(startScale, targetScale, curveValue);

            yield return null;
        }

        bombShell.transform.localScale = targetScale;
    }

    private IEnumerator DamageTickRoutine()
    {
        WaitForSeconds wait = new(_bombTick);

        while (true)
        {
            for (int i = enemiesInside.Count - 1; i >= 0; i--)
            {
                if (enemiesInside[i] == null)
                {
                    enemiesInside.RemoveAt(i);
                    continue;
                }

                enemiesInside[i].TakeDamage(_bombDamage);
            }

            yield return wait;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !enemiesInside.Contains(damageable))
        {
            enemiesInside.Add(damageable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            enemiesInside.Remove(damageable);
        }
    }
}
