using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    [Header("Pausing")]
    [SerializeField] bool isPaused;

    [Header("Animation")]
    [SerializeField] Animator animator;
    [SerializeField] float animationSmoothingTime;

    [Header("Camera")]
    [SerializeField] Transform playerCam;

    [Header("Movement")]
    [SerializeField] float mouseSens = 3.5f;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float runSpeed;
    [SerializeField] float runBuildUpSpeed;
    [SerializeField] float gravity = -13f;

    [Header("Mouse")]
    [SerializeField] bool lockCursor = true;
    [SerializeField][Range(0.0f,0.5f)] float moveSmoothTime = .3f;
    [SerializeField][Range(0.0f,0.5f)] float mouseSmoothTime = .03f;

    [Header("Jumping")]
    [SerializeField] AnimationCurve jumpFallOff;
    [SerializeField] float jumpMultiplier;

    [Header("KeyCodes")]
    [SerializeField] KeyCode runKey;
    [SerializeField] KeyCode jumpKey;
    [SerializeField] KeyCode pauseKey;

    [Header("Slope")]
    [SerializeField] float slopeForce;
    [SerializeField] float slopeForceRayLength;

    float moveSpeed;
    float cameraPitch = 0.0f;
    float velocityY = 0.0f;
    CharacterController controller;
    bool isJumping;


    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;

    #endregion

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {


        if (isPaused == false)
        {
            MouseLook();
            Movement();
        }
        PauseController();
    }

    void MouseLook()
    {
        Vector2 targetMouseDelta = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSens;

        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        playerCam.localEulerAngles = Vector3.right * cameraPitch;

        transform.Rotate(currentMouseDelta.x * mouseSens * Vector3.up);
    }

    void PauseController()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (lockCursor)
                lockCursor = false;
            else lockCursor = true;
        }
        if (lockCursor)
        {
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Movement()
    {
        Vector2 targetDir = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (controller.isGrounded)
            velocityY = 0.0f;

        velocityY += gravity * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x)  * moveSpeed + Vector3.up * velocityY;

        if(targetDir.y != 0 || targetDir.x != 0 && OnSlope())
        {
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }

        controller.Move(velocity * Time.deltaTime);


        JumpInput();
        SetMovementSpeed();
    }

    void SetMovementSpeed()
    {
        if(Input.GetKey(runKey))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, Time.deltaTime * runBuildUpSpeed);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, Time.deltaTime * runBuildUpSpeed);
        }
    }

    private bool OnSlope()
    {
        if(isJumping) { return false; }


        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, controller.height / 2 * slopeForceRayLength))
        {
            if (hit.normal != Vector3.up)
            {
                return true;

            }
        }
        return false;

    }

    void JumpInput()
    {
        if(Input.GetKeyDown(jumpKey) && !isJumping)
        {
            isJumping = true;
        }
    }

    //disabled
    /*
    IEnumerator JumpEvent()
    {
        controller.slopeLimit = 90.0f;
        float timeInAir = 0.0f;

        do
        {
            animator.SetBool("Jumping", true);
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            controller.Move(jumpForce * jumpMultiplier * Time.deltaTime * Vector3.up);
            timeInAir += Time.deltaTime;
            yield return null;
        } while (!controller.isGrounded && controller.collisionFlags != CollisionFlags.Above);
        animator.SetBool("Jumping", false);
        controller.slopeLimit = 45f;
        isJumping = false;
    }
    */
}
