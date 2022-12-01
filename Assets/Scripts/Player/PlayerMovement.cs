using JetBrains.Annotations;
using System;
using System.Collections;
using System.Configuration;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    public bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnim && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] bool canSprint = true;
    [SerializeField] bool canJump = true;
    [SerializeField] bool canCrouch = true;
    [SerializeField] bool canUseHeadbob = true;
    [SerializeField] bool canInteract = true;
    [SerializeField] bool useStamina = true;

    [Header("Controls")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] KeyCode interactKey = KeyCode.F;

    [Header("Movement Params")]
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField] float sprintSpeed = 10.0f;
    [SerializeField] float crouchSpeed = 3.0f;

    [Header("Look Params")]
    [SerializeField, Range(1, 10)] private float lookSpeed = 2.0f;

    [SerializeField, Range(1, 180)] private float upperLookLimit = 80f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80f;

    [SerializeField] private bool lockCursor = true;

    [Header("Health Params")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float timeBeforeRegenStarts = 3f;
    [SerializeField] float healthValueIncrement = 1f;
    [SerializeField] float healthTimeIncrement = 0.1f;
    float currentHealth;
    Coroutine RegenHealth;
    public static Action<float> OnTakeDamage;
    public static Action<float> OnDamage;
    public static Action<float> OnHeal;

    [Header("Stamina Params")]
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float staminaUseMultiplier = 5f;
    [SerializeField] float timeBeforeStaminaRegenStarts = 5f;
    [SerializeField] float staminaValueIncrement = 2f;
    [SerializeField] float staminaTimeIncrement = 0.1f;
    float currentStamina;
    private Coroutine regeneratingStamina;
    public static Action<float> OnStaminaChange;

    [Header("Jump Params")]
    [SerializeField] float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouch Params")]
    [SerializeField] float crouchHeight = 1.35f;
    [SerializeField] float standingHeight = 2.7f;
    [SerializeField] float timeToCrouch = .25f;
    [SerializeField] Vector3 crouchingCenter = new Vector3(0, .5f, 0);
    [SerializeField] Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnim;

    [Header("Headbob Params")]
    [SerializeField] float walkBobSpeed = 14f;
    [SerializeField] float walkBobAmount = 0.05f;

    [SerializeField] float sprintBobSpeed = 18f;
    [SerializeField] float sprintBobAmount = 0.1f;

    [SerializeField] float crouchBobSpeed = 8f;
    [SerializeField] float crouchBobAmount = 0.025f;
    float defaultYPos = 0;
    float timer;

    [Header("Interaction")]
    [SerializeField] Vector3 interactionRayPoint = default;
    [SerializeField] float interactionDistance = default;
    [SerializeField] LayerMask interactionLayer = default;
    Interactable currentInteractable;

    private Camera playerCam;
    private CharacterController characterController;

    private Vector3 moveDir;
    private Vector2 currentInput;

    private float rotX = 0;

    public static PlayerMovement instance;

    private void OnEnable()
    {
        OnTakeDamage += ApplyDamage;
    }

    private void OnDisable()
    {
        OnTakeDamage -= ApplyDamage;
    }

    private void Awake()
    {
        instance = this;
        playerCam = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCam.transform.localPosition.y;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        LockCursor();
    }

    void LockCursor()
    {
       Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        HandlePause();
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canJump)
                HandleJump();

            if (canCrouch)
                HandleCrouch();

            if (canUseHeadbob)
                HandleHeadBob();

            if(canInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }

            if (useStamina)
                HandleStamina();

            ApplyFinalMovements();
        }
    }

    void HandlePause()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (lockCursor)
                lockCursor = false;
            else lockCursor = true;
        }
        if (lockCursor)
        {
            CanMove = true;
            LockCursor();
        }
        else
        {
            CanMove = false;
            UnlockCursor();
        }
    }

    void HandleInteractionCheck()
    {
        if(Physics.Raycast(playerCam.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if(hit.collider.gameObject.layer == 12 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable) currentInteractable.OnFocus();
            }
        }
        else if(currentInteractable)
        { 
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    void HandleInteractionInput()
    {
        if(Input.GetKeyDown(interactKey) && currentInteractable != null 
            && Physics.Raycast(playerCam.ViewportPointToRay(interactionRayPoint), 
            out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract();
        }
    }

    void HandleMovementInput()
    {
        currentInput = new Vector2(
            (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed)
            * Input.GetAxis("Vertical"), 
            (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed)
            * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDir.y;
        moveDir = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDir.y = moveDirectionY;
    }

    void HandleMouseLook()
    {
        rotX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotX = Mathf.Clamp(rotX, -upperLookLimit, lowerLookLimit);
        playerCam.transform.localRotation = Quaternion.Euler(rotX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    void HandleJump()
    {
        if (ShouldJump) moveDir.y = jumpForce;
    }

    void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }

    void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;

        if(Mathf.Abs(moveDir.x) > 0.1f || Mathf.Abs(moveDir.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCam.transform.localPosition = new Vector3(
                playerCam.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCam.transform.localPosition.z);
        }
    }

    void HandleStamina()
    {
        if(IsSprinting && currentInput != Vector2.zero)
        {
            if(regeneratingStamina != null)
            {
                StopCoroutine(RegenerateStamina());
                regeneratingStamina = null;
            }
            currentStamina -= staminaUseMultiplier * Time.deltaTime;

            if (currentStamina < 0)
                currentStamina = 0;

            OnStaminaChange?.Invoke(currentStamina);

            if (currentStamina <= 0)
                canSprint = false;
        }

        if(!IsSprinting && currentStamina < maxStamina && regeneratingStamina == null)
        {
            regeneratingStamina = StartCoroutine(RegenerateStamina());
        }
    }

    private void ApplyDamage(float dmg)
    {
        currentHealth -= dmg;
        OnDamage?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            KillPlayer();
        }
        else if (RegenHealth != null)
            StopCoroutine(RegenerateHealth());

        RegenHealth = StartCoroutine(RegenerateHealth());
    }

    private void KillPlayer()
    {
        currentHealth = 0;
        if(RegenHealth != null)
        {
            StopCoroutine(RegenerateHealth());
            print("Dead");
        }
    }

    void ApplyFinalMovements()
    {
        if (!characterController.isGrounded) moveDir.y -= gravity * Time.deltaTime;
        characterController.Move(moveDir * Time.deltaTime);
    }

    IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCam.transform.position, Vector3.up, 1f))
            yield break;

        duringCrouchAnim = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while(timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnim = false;
    }

    IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(timeBeforeRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(healthTimeIncrement);

        while(currentHealth < maxHealth)
        {
            currentHealth += healthValueIncrement;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            OnHeal?.Invoke(currentHealth);
            yield return timeToWait;
        }

        RegenHealth = null;
    }

    IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(staminaTimeIncrement);

        while(currentStamina < maxStamina)
        {
            if (currentStamina > 0)
                canSprint = true;

            currentStamina += staminaValueIncrement;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;

            OnStaminaChange?.Invoke(currentStamina);

            yield return timeToWait;
        }

        regeneratingStamina = null;
    }
}
