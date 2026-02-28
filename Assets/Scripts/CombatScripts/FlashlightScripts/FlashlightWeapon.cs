using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightWeapon : WeaponBase
{
    [Header("Shooters")]
    [SerializeField] private LightBeamShooter narrowShooter;
    [SerializeField] private LightBeamShooter wideShooter;
    [SerializeField] private LightBombShooter bombShooter;
    [SerializeField] private LightUltimateShooter ultimateShooter;

    [Header("Input System")]
    [SerializeField] private InputActionReference secondaryAttackInput;

    [Header("Aiming Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask aimLayerMask;
    [SerializeField] private float maxAimDistance = 100f;

    private Camera cam;
    private bool isHoldingBomb = false;

    private const string AbilityKey = "SelectedAbility";



    private void Start()
    {
        cam = Camera.main;

        ApplySelectedAbility();
    }

    private void OnEnable()
    {
        UI_AbilityButton.OnAbilitySelected += ApplySelectedAbility;

        // Subscribe to Input Events
        if (secondaryAttackInput != null)
        {
            secondaryAttackInput.action.started += OnSecondaryStarted;   // Button Down
            secondaryAttackInput.action.canceled += OnSecondaryCanceled; // Button Up
            secondaryAttackInput.action.performed += OnSecondaryPerformed; // Button Clicked (for wide beam)
            secondaryAttackInput.action.Enable();
        }
    }

    private void OnDisable()
    {
        UI_AbilityButton.OnAbilitySelected -= ApplySelectedAbility;
        // Unsubscribe to prevent memory leaks
        if (secondaryAttackInput != null)
        {
            secondaryAttackInput.action.started -= OnSecondaryStarted;
            secondaryAttackInput.action.canceled -= OnSecondaryCanceled;
            secondaryAttackInput.action.performed -= OnSecondaryPerformed;
            secondaryAttackInput.action.Disable();
        }
    }

    private void Update()
    {
        // Only run trajectory logic if we are actively holding the bomb button
        if (isHoldingBomb && bombShooter.gameObject.activeSelf)
        {
            Vector3 aimDir = GetAimDirection();
            bombShooter.PredictTrajectory(firePoint.position, aimDir);
            bombShooter.IsOnCooldown();
        }
    }

    private void ApplySelectedAbility()
    {
        string ability = PlayerPrefs.GetString(AbilityKey);

        // Disable everything first
        narrowShooter.gameObject.SetActive(true); // always available
        wideShooter.gameObject.SetActive(false);
        bombShooter.gameObject.SetActive(false);

        isHoldingBomb = false;

        switch (ability)
        {
            case "WideBeam":
                wideShooter.gameObject.SetActive(true);
                break;

            case "LightBomb":
                bombShooter.gameObject.SetActive(true);
                break;
        }
    }

    private void OnSecondaryStarted(InputAction.CallbackContext ctx)
    {
        // Button Pressed Down: Start aiming if using Bomb
        if (bombShooter.gameObject.activeSelf)
        {
            isHoldingBomb = true;
        }
    }

    private void OnSecondaryCanceled(InputAction.CallbackContext ctx)
    {
        // Button Released: Fire if using Bomb
        if (bombShooter.enabled && isHoldingBomb)
        {
            isHoldingBomb = false;

            // Fire exactly where we are aiming now
            Vector3 aimDir = GetAimDirection();
            bombShooter.Shoot(firePoint.position, aimDir);
        }
    }

    private void OnSecondaryPerformed(InputAction.CallbackContext ctx)
    {
        // Standard "Click": Fire Wide Beam if that mode is active
        // We use Performed here because Wide Beam is instant, not hold-release
        if (wideShooter.gameObject.activeSelf)
        {
            SecondaryAttack();
        }
    }

    private Vector3 GetAimDirection()
    {
        // Center of screen aim
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, aimLayerMask))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(maxAimDistance);

        return (targetPoint - firePoint.position).normalized;
    }
    public override void MainAttack()
    {
        narrowShooter.Shoot(firePoint.position, GetAimDirection());
    }

    public override void SecondaryAttack()
    {
        if (wideShooter.gameObject.activeSelf)
            wideShooter.Shoot(firePoint.position, GetAimDirection());

    }

    public override void UltimateAttack()
    {
        ultimateShooter.Shoot(firePoint.position, GetAimDirection());
    }

    private void OnDrawGizmos()
    {
        if (firePoint != null && cam != null)
        {
            Gizmos.color = Color.yellow;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            // FPS Mode visualization: 
            // Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            Vector3 target = ray.GetPoint(maxAimDistance);
            if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance))
            {
                target = hit.point;
            }

            Gizmos.DrawLine(firePoint.position, target);
            Gizmos.DrawWireSphere(target, 0.2f);
        }
    }
}
