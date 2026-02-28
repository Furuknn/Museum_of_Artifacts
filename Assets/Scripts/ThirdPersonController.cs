using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    public static ThirdPersonController instance { get; private set; }

    // Kamera stillerini tanımlıyoruz
    public enum CameraStyle
    {
        Combat, // Serbest koşu (Karakter bastığın yöne döner)
        Shooter // Nişan alma (Karakter hep ileri bakar, yan yan yürür)
    }

    [Header("Camera Settings")]
    public CameraStyle currentCameraStyle; // Editörden veya kodla değiştirebilirsin
    [SerializeField] Transform cam;

    [Header("Movement")]
    [SerializeField] public CharacterController characterController;
    [SerializeField, Range(0f, 100f)] private float speed = 10f;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothTime = 0.5f;
    private float rotationVelocity;

    [Header("Sprint")]
    [SerializeField] private float sprintMultiplier = 1.6f;
    private bool isSprinting;
    private float baseSpeed;

    [Header("Jump and Gravity")]
    [SerializeField, Range(-100f, 100f)] private float gravity = -9.81f;
    [SerializeField, Range(0f, 50f)] private float jumpForce = 5f;
    [SerializeField, Range(1f, 2f)] private float groundRange;
    [SerializeField] private LayerMask groundLayer;

    [Header("Interact")]
    [SerializeField, Range(0f, 100f)] private float camRange = 20f;
    [SerializeField, Range(0f, 100f)] private float camStartOffset = 1f;
    [SerializeField] private GameObject interactionIndicator;

    [Header("Input & Animation")]
    public PlayerInputActions playerInputActions;
    [SerializeField] private Animator animator;
    public bool isAttacking;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Jump.performed += JumpHandle;
        playerInputActions.Player.Interact.performed += InteractHandle;
        playerInputActions.Player.MenuTrigger.performed += MenuToggle;
        playerInputActions.Player.MainAttack.performed += MainAttack;
        playerInputActions.Player.SecondaryAttack.performed += SecondaryAttack;
        playerInputActions.Player.Sprint.started += SprintStart;
        playerInputActions.Player.Sprint.canceled += SprintEnd;

        instance = this;
    }

    void OnEnable()
    {
        playerInputActions.Player.Enable();
    }

    void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    void Update()
    {
        ApplyGravity();

        // Shooter modundaysak karakter hareket etmese bile kameranın baktığı yere dönsün
        if (currentCameraStyle == CameraStyle.Shooter)
        {
            RotatePlayerToCameraForward();
        }

        CheckInteractable();
    }

    void FixedUpdate()
    {
        MovementHandle();
        CameraRay();
    }

    #region Movement and TPS Camera
    void MovementHandle()
    {
        if (isAttacking) return;

        Vector2 movementInput = playerInputActions.Player.Movement.ReadValue<Vector2>();
        Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

        // Eğer hareket girdisi varsa
        if (direction.magnitude >= 0.1f)
        {
            if (animator != null) animator.SetBool("isMoving", true);

            // --- COMBAT MODU (Serbest Hareket) ---
            if (currentCameraStyle == CameraStyle.Combat)
            {
                // Karakteri hareket ettiği yöne döndür (Örn: S'ye basınca arkasını döner)
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);

                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                characterController.Move(moveDir.normalized * speed * Time.deltaTime);
            }
            // --- SHOOTER MODU (Strafe Hareket) ---
            else if (currentCameraStyle == CameraStyle.Shooter)
            {
                // Karakterin yönünü zaten Update içinde RotatePlayerToCameraForward ile kilitledik.
                // Burada sadece sağa/sola/ileri/geri yürümesini sağlıyoruz (Dönmeden).

                Vector3 moveDir = cam.forward * direction.z + cam.right * direction.x;
                moveDir.y = 0; // Yükseklik değişimini engelle



                float finalSpeed = speed;

                if (isSprinting && direction.magnitude >= 0.1f)
                {
                    finalSpeed *= sprintMultiplier;
                }

                characterController.Move(moveDir.normalized * finalSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (animator != null) animator.SetBool("isMoving", false);
        }
    }

    private void SprintStart(InputAction.CallbackContext context)
    {
        if (isAttacking) return;

        isSprinting = true;

        if (animator != null)
            animator.SetBool("isSprinting", true);
    }

    private void SprintEnd(InputAction.CallbackContext context)
    {
        isSprinting = false;

        if (animator != null)
            animator.SetBool("isSprinting", false);
    }

    // Shooter modu için karakteri zorla kameranın baktığı yöne döndürür
    private void RotatePlayerToCameraForward()
    {
        Vector3 camForward = cam.forward;
        camForward.y = 0f;

        if (camForward.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(camForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f); // Hızlı dönüş
    }
    #endregion

    #region Jump
    public void JumpHandle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isAttacking) return;

        if (isPlayerOnGround())
        {
            gravity = jumpForce;
            characterController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
            if (animator != null) animator.SetBool("isJumping", true);
        }
    }

    bool isPlayerOnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundRange, LayerMask.GetMask("Ground"));
    }
    #endregion

    #region Gravity
    void ApplyGravity()
    {
        if (isPlayerOnGround() && gravity < 0)
        {
            gravity = -2f;
            if (isPlayerOnGround() && animator != null)
            {
                animator.SetBool("isFalling", false);
                animator.SetBool("isJumping", false);
            }
        }
        else
        {
            if (gravity < 0 && !isPlayerOnGround() && animator != null)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
            }
        }
        gravity += -9.81f * Time.deltaTime;
        characterController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
    }
    #endregion

    #region Interact & Actions
    void InteractHandle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        RaycastHit hit;
        if (Physics.Raycast(GetRayStartOrgin(), cam.forward, out hit, camRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null) interactable.Interact();
        }
    }

    void CheckInteractable()
    {
        RaycastHit hit;

        if (Physics.Raycast(GetRayStartOrgin(), cam.forward, out hit, camRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            interactionIndicator.SetActive(interactable != null);
        }
        else
        {
            // IMPORTANT: turn it off when raycast hits nothing
            interactionIndicator.SetActive(false);
        }
    }


    private Vector3 GetRayStartOrgin()
    {
        return cam.position + (cam.forward * camStartOffset);
    }

    private void CameraRay()
    {
        Debug.DrawRay(GetRayStartOrgin(), cam.forward * camRange, Color.red);
    }

    public void MenuToggle(InputAction.CallbackContext context)
    {
        UIManager.instance.PauseMenuToggle();
    }

    public void MainAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IWeapon onHand = GetComponentInChildren<IWeapon>();
            if (onHand != null) onHand.MainAttack();
        }
    }

    public void SecondaryAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IWeapon onHand = GetComponentInChildren<IWeapon>();
            if (onHand != null) onHand.SecondaryAttack();
        }
    }

    public void UltimateAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IWeapon onHand = GetComponentInChildren<IWeapon>();
            if (onHand != null) onHand.UltimateAttack();
        }
    }

    public void GetAnimatorCompononet()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetPlayerSpeed(float newSpeed)
    {
        this.speed = newSpeed;
    }

    public float GetPlayerSpeed()
    {
        return speed;
    }
    #endregion
}