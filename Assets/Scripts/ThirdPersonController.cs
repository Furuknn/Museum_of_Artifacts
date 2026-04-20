using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    public static ThirdPersonController Instance { get; private set; }

    // Kamera stillerini tanımlıyoruz
    public enum CameraStyle
    {
        Combat, // Serbest koşu (Karakter bastığın yöne döner)
        Shooter // Nişan alma (Karakter hep ileri bakar, yan yan yürür)
    }

    [Header("Camera Settings")]
    public CameraStyle currentCameraStyle; // Editörden veya kodla değiştirebilirsin
    [SerializeField] Transform cam;
    public CinemachineFreeLook freeLook;
    private Camera mainCam;

    [Header("Movement")]
    [SerializeField] public CharacterController characterController;
    [SerializeField, Range(0f, 100f)] private float speed = 10f;
    public float speedMultiplier = 1f;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothTime = 0.5f;
    private float rotationVelocity;
    private bool canMove = true;
    bool isLanded = true;

    [Header("Sprint")]
    [SerializeField] private float sprintMultiplier = 1.6f;
    private bool isSprinting;
    private float baseSpeed;

    [Header("Jump & Gravity")]
    [SerializeField, Range(0f, 50f)] private float jumpForce = 12f;
    [SerializeField, Range(-100f, 0f)] private float gravityValue = -25f;
    [SerializeField, Range(1f, 5f)] private float fallMultiplier = 2.2f;   // Snappier fall
    [SerializeField, Range(0f, 1f)] private float coyoteTime = 0.15f;      // Forgiveness after walking off a ledge
    [SerializeField, Range(0f, 1f)] private float jumpBufferTime = 0.12f;  // Forgiveness for early jump press
    [SerializeField] private LayerMask groundLayer;
    [SerializeField, Range(0f, 1f)] private float groundCheckRadius = 0.3f;
    [SerializeField] private Transform groundCheckOrigin;

    public bool canJump = true;

    private float verticalVelocity;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool wasGrounded;
    private bool isGrounded;
    private bool canApplyGravity = true;

    [Header("Interact")]
    [SerializeField, Range(0f, 100f)] private float interactRange = 20f;
    [SerializeField] private GameObject interactionIndicator;

    private IInteractable currentInteractable;
    private bool canInteract = true;

    [Header("Input & Animation")]
    public PlayerInputActions playerInputActions;
    [SerializeField] private Animator animator;
    public bool isAttacking;

    private bool canAttack = true;

    [Header("No Clip")]
    [SerializeField]private float noClipSpeedMultiplier = 5f;
    [SerializeField]private float noClipJumpForce = 20f;
    private bool isNoClip;
    private float originalMultiplier;
    private float originalJumpForce;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Jump.performed += JumpHandle;
        playerInputActions.Player.Interact.performed += InteractHandle;
        playerInputActions.Player.MenuTrigger.performed += MenuToggle;
        playerInputActions.Player.MainAttack.performed += MainAttack;
        playerInputActions.Player.SecondaryAttack.performed += SecondaryAttack;
        playerInputActions.Player.UltimateAttack.performed += UltimateAttack;
        playerInputActions.Player.Sprint.started += SprintStart;
        playerInputActions.Player.Sprint.canceled += SprintEnd;

        Instance = this;

        mainCam = Camera.main;
    }

    private void OnGameStopped()
    {
        canMove = false;
        canJump = false;
        canInteract = false;
        canAttack = false;
        canApplyGravity = false;

        if (animator != null)
            animator.speed = 0f;
    }
    
    private void OnGameContinued()
    {
        canMove = true;
        canJump = true;
        canInteract = true;
        canAttack = true;
        canApplyGravity = true;

        if(animator!=null)
            animator.speed = 1f;
    }

    void OnEnable()
    {
        playerInputActions.Player.Enable();
        GameManager.OnGameStopped += OnGameStopped;
        GameManager.OnGameContinued += OnGameContinued;
    }

    void OnDisable()
    {
        playerInputActions.Player.Disable();
        GameManager.OnGameStopped -= OnGameStopped;
        GameManager.OnGameContinued -= OnGameContinued;
    }

    void Update()
    {
        HandleGravityAndJump();

        // Shooter modundaysak karakter hareket etmese bile kameranın baktığı yere dönsün
        if (currentCameraStyle == CameraStyle.Shooter)
        {
            RotatePlayerToCameraForward();
        }

        CameraRay();
        CheckInteractable();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.End))
        {
            if (isNoClip)
            {
                isNoClip = false;
                speedMultiplier = originalMultiplier; // restore speed
                jumpForce = originalJumpForce;
                SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ignore Raycast")); // or whatever your normal layer is
            }
            else
            {
                isNoClip = true;
                originalMultiplier = speedMultiplier; // save before overwriting
                originalJumpForce= jumpForce;

                speedMultiplier = noClipSpeedMultiplier;
                jumpForce = noClipJumpForce;
                SetLayerRecursively(gameObject, LayerMask.NameToLayer("NoClip"));
            }
        }
#endif
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    void FixedUpdate()
    {
        if(canMove)
            MovementHandle();
    }

    #region Movement and TPS Camera

    public void SetSpeed(float value)
    {
        speedMultiplier = value;
    }

    void MovementHandle()
    {
        if (isAttacking) return;
        isSprinting = Input.GetKey(KeyCode.LeftShift);
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
                float finalSpeed = speed * speedMultiplier;

                if (isSprinting && direction.magnitude >= 0.1f)
                {
                    finalSpeed *= sprintMultiplier;
                }

                characterController.Move(moveDir.normalized * finalSpeed * Time.deltaTime);
            }
            // --- SHOOTER MODU (Strafe Hareket) ---
            else if (currentCameraStyle == CameraStyle.Shooter)
            {
                // Karakterin yönünü zaten Update içinde RotatePlayerToCameraForward ile kilitledik.
                // Burada sadece sağa/sola/ileri/geri yürümesini sağlıyoruz (Dönmeden).

                Vector3 moveDir = cam.forward * direction.z + cam.right * direction.x;
                moveDir.y = 0; // Yükseklik değişimini engelle

                float finalSpeed = speed * speedMultiplier;

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

    void ChangeFOV(float targetFov, float duration)
    {
        // FreeLook referansını alalım (Daha temiz kod için)
        var freeLook = ThirdPersonController.Instance.freeLook;

        // DOTween.To(Getter, Setter, TargetValue, Duration)
        DOTween.To(() => freeLook.m_Lens.FieldOfView,
                   x => freeLook.m_Lens.FieldOfView = x,
                   targetFov,
                   duration)
               .SetEase(Ease.OutQuad);
    }

    private void SprintStart(InputAction.CallbackContext context)
    {
        if (isAttacking) return;

        isSprinting = true;

        if (animator != null)
        {
            ChangeFOV(65, 0.1f);
            animator.SetBool("isSprinting", true);
            animator.SetFloat("moveSpeed", 1.4f);
        }
    }

    private void SprintEnd(InputAction.CallbackContext context)
    {
        isSprinting = false;

        if (animator != null)
        {
            ChangeFOV(60, 0.1f);
            animator.SetBool("isSprinting", false);
            animator.SetFloat("moveSpeed", 1f);
        }
    }

    // Shooter modu için karakteri zorla kameranın baktığı yöne döndürür
    public void RotatePlayerToCameraForward()
    {
        Vector3 camForward = mainCam.transform.forward;
        camForward.y = 0f; // was camForward.x = 0f

        //if (camForward.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(camForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
    }
    #endregion

    #region Jump

    private bool CheckGrounded()
    {
        if (isNoClip) return true;

        Vector3 origin = groundCheckOrigin != null
            ? groundCheckOrigin.position
            : transform.position + Vector3.up * 0.1f;

        // Slightly offset origin upward so the sphere can detect ground even on slopes
        return Physics.SphereCast(
            origin + Vector3.up * (groundCheckRadius + 0.05f),
            groundCheckRadius,
            Vector3.down,
            out _,
            groundCheckRadius + 0.1f,   // max distance
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    void HandleGravityAndJump()
    {
        if (!canApplyGravity) return;

        isGrounded = CheckGrounded();

        // ── Coyote time: keep the "was grounded" window alive briefly
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        // ── Jump buffer: store the press for a brief window
        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.deltaTime;

        // ── Landing detection (was airborne, now grounded)
        if (!wasGrounded && isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;     // Small snap-to-ground value — kills float without slamming
            if (animator != null)
            {
                animator.ResetTrigger("Jump");
                animator.SetTrigger("Land");
                animator.SetBool("isFalling", false);
                animator.SetBool("isJumping", false);
            }
            isLanded = true;
            StartCoroutine(LandSlowness());
        }

        // ── Consume jump buffer if we have coyote window
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            ExecuteJump();
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        // ── Gravity: fall faster than you rise (much better game feel)
        float currentGravity = (!isGrounded && verticalVelocity < 0f)
            ? gravityValue * fallMultiplier
            : gravityValue;

        verticalVelocity += currentGravity * Time.deltaTime;
        verticalVelocity = Mathf.Max(verticalVelocity, -50f);   // Terminal velocity cap

        // ── Airborne animation states
        if (animator != null)
        {
            animator.SetBool("isJumping", !isGrounded && verticalVelocity > 0f);
            animator.SetBool("isFalling", !isGrounded && verticalVelocity < -2f);
        }

        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        wasGrounded = isGrounded;
    }
    public void JumpHandle(InputAction.CallbackContext context)
    {
        if (!canJump || !context.performed || isAttacking) return;

        jumpBufferTimer = jumpBufferTime;   // Always store the press

        // If we already have a coyote window, jump immediately
        if (coyoteTimer > 0f)
        {
            ExecuteJump();
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
        // Otherwise the buffer will catch it in HandleGravityAndJump when we land
    }

    private void ExecuteJump()
    {
        verticalVelocity = jumpForce;
        isLanded = false;

        if (isNoClip) verticalVelocity = jumpForce;   // NoClip still works

        if (animator != null)
        {
            animator.ResetTrigger("Land");
            animator.SetTrigger("Jump");
            animator.SetBool("isJumping", true);
            animator.SetBool("isFalling", false);
        }
    }

    IEnumerator LandSlowness()
    {
        float tempSprintMultiplier = sprintMultiplier;
        sprintMultiplier = 1.35f;
        yield return new WaitForSeconds(0.25f);
        sprintMultiplier = tempSprintMultiplier;
    }

    #endregion

    #region Interact & Actions
    void InteractHandle(InputAction.CallbackContext context)
    {
        if (!canInteract || !context.performed) return;
        if (currentInteractable == null) return;
        if (!currentInteractable.isInteractable()) return;

        currentInteractable.Interact();
    }

    void CheckInteractable()
    {
        currentInteractable = null;

        Ray screenRay = GetCrosshairRay();
        Vector3 targetPoint;

        if (Physics.Raycast(screenRay, out RaycastHit screenHit, interactRange))
            targetPoint = screenHit.point;
        else
            targetPoint = screenRay.origin + screenRay.direction * interactRange;

        Vector3 eyePosition = transform.position + Vector3.up * 1.6f;
        Vector3 directionToTarget = (targetPoint - eyePosition).normalized;
        Ray eyeRay = new Ray(eyePosition, directionToTarget);

        if (Physics.Raycast(eyeRay, out RaycastHit eyeHit, interactRange))
            currentInteractable = eyeHit.collider.GetComponent<IInteractable>();

        // One line covers: null, exists but not interactable, exists and interactable
        bool showIndicator = currentInteractable != null && currentInteractable.isInteractable();
        interactionIndicator.SetActive(showIndicator);

        Debug.DrawRay(eyeRay.origin, eyeRay.direction * interactRange, Color.green);
    }

    private Ray GetCrosshairRay()
    {
        return mainCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
    }

    private void CameraRay()
    {
        Ray ray = GetCrosshairRay();
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red);
        //Debug.DrawWireSphere(transform.position, interactSphereRadius); // needs Gizmos
    }

    public void MenuToggle(InputAction.CallbackContext context)
    {
        UIManager.Instance.PauseMenuToggle();
    }

    public void MainAttack(InputAction.CallbackContext context)
    {
        if (!canAttack) return;

        if (context.performed)
        {
            IWeapon onHand = GetComponentInChildren<IWeapon>();
            if (onHand != null) onHand.MainAttack();
        }
    }

    public void SecondaryAttack(InputAction.CallbackContext context)
    {
        if (!canAttack) return;

        if (context.performed)
        {
            IWeapon onHand = GetComponentInChildren<IWeapon>();
            if (onHand != null) onHand.SecondaryAttack();
        }
    }

    public void UltimateAttack(InputAction.CallbackContext context)
    {
        if (!canAttack) return;

        if (context.performed)
        {
            IWeapon onHand = GetComponentInChildren<IWeapon>();
            if (onHand != null) onHand.UltimateAttack();
        }
    }

    public void GetAnimatorComponent()
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