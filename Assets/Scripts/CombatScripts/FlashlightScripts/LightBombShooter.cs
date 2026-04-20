using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class LightBombShooter : MonoBehaviour, IBeam
{
    [Header("References")]
    [SerializeField] private LightBomb bombPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float lineWidth = 0.05f;

    [Header("Physics Settings")]
    [SerializeField] private float throwSpeed = 20f;  // Forward strength (based on crosshair)
    [SerializeField] private float upwardSpeed = 5f;  // Extra upward pop
    [SerializeField] private float drag = 0f;         // 0 for standard parabolic arc
    [SerializeField] private float mass = 1f;

    [Header("Prediction Settings")]
    [SerializeField] private int maxSteps = 50;      // How many points to draw
    [SerializeField] private float timeStep = 0.1f;  // Time between each point (smaller = smoother)
    [SerializeField] private LayerMask collisionLayer; // What stops the line?

    [Header("Trajectory Materials")]
    [SerializeField] private Material activeMat;
    [SerializeField] private Material passiveMat;

    private Vector3 currentDirection;
    private float lastFireTime;
    private LineRenderer lineRenderer;
    private HUDManager hudManager;

    private float _bombCooldown => WeaponStatsManager.Instance.flashlightStatsRuntime.bombCooldown;

    private void Awake()
    {
        GameObject lrObj = new GameObject("TrajectoryLine");
        lrObj.transform.SetParent(transform);

        lineRenderer = lrObj.AddComponent<LineRenderer>();
        lineRenderer.material = activeMat;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
        lineRenderer.staticShadowCaster = false;

        hudManager = FindAnyObjectByType<HUDManager>();
        lastFireTime = -_bombCooldown;
    }

    private Vector3 GetThrowVelocity(Vector3 aimDirection)
    {
        // 1. Get the base velocity exactly where the player is aiming (respecting up/down pitch)
        Vector3 baseVelocity = aimDirection.normalized * throwSpeed;

        // 2. Add the extra vertical "pop" to create the arc
        Vector3 finalVelocity = baseVelocity + (Vector3.up * upwardSpeed);

        return finalVelocity;
    }

    public void Shoot(Vector3 origin, Vector3 direction)
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        if (Time.time - lastFireTime < _bombCooldown) return;
        lastFireTime = Time.time;

        LightBomb bomb = Instantiate(bombPrefab, origin, Quaternion.identity);

        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = mass;
            rb.drag = drag;
            rb.velocity = GetThrowVelocity(direction);
        }

        if (hudManager != null)
        {
            int attackIndex = 1;
            float _cooldown = _bombCooldown;

            if (attackIndex != -1)
                hudManager.StartCooldown(_cooldown, attackIndex);
        }
    }

    public void PredictTrajectory(Vector3 origin, Vector3 direction)
    {
        if (lineRenderer == null) return;

        lineRenderer.enabled = true;

        Vector3 velocity = GetThrowVelocity(direction);
        Vector3 position = origin;

        List<Vector3> points = new List<Vector3>();
        points.Add(position);

        for (int i = 0; i < maxSteps; i++)
        {
            velocity = CalculateNewVelocity(velocity, drag, timeStep);
            Vector3 nextPosition = position + velocity * timeStep;

            if (Physics.Raycast(
                position,
                nextPosition - position,
                out RaycastHit hit,
                Vector3.Distance(position, nextPosition),
                collisionLayer))
            {
                points.Add(hit.point);
                break;
            }

            points.Add(nextPosition);
            position = nextPosition;
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        velocity += Physics.gravity * increment; // Apply Gravity
        velocity *= Mathf.Clamp01(1f - drag * increment); // Apply Drag
        return velocity;
    }

    private void OnDisable()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    public bool IsOnCooldown()
    {
        if (Time.time - lastFireTime < _bombCooldown)
        {
            lineRenderer.material = passiveMat;
            return true;
        }
        else
        {
            lineRenderer.material = activeMat;
            return false;
        }
    }
}