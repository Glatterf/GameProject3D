using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class movementPlayer : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Jump")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGroundedLastFrame = true;

    [Header("Crouch")]
    public float standingHeight = 2f;
    public float crouchHeight = 1.2f;
    public KeyCode crouchKey = KeyCode.LeftControl;
    private bool isCrouching = false;

    [Header("Sprint")]
    public float sprintSpeed = 8f;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Moving Platform")]
    public LayerMask platformLayer;
    public float platformCheckDistance = 0.3f;
    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;
    private Quaternion lastPlatformRotation;

    // Reference to the player's camera
    public Transform cameraTransform;

    // stored each frame so UpdateAnimator can read them
    private Vector3 currentDirection;
    private float currentMoveSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // Force standing state to match the formula from the very first frame,
        // regardless of whatever values are currently sitting in the Inspector.
        controller.height = standingHeight;
        controller.center = new Vector3(0, standingHeight / 2f, 0);
    }

    void Update()
    {
        HandleGroundAndGravity();
        HandlePlatformMovement();
        HandleCrouch();
        HandleMovementAndRotation();
        HandleJump();
        UpdateAnimator();

        // apply vertical velocity (gravity/jump) every frame
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleGroundAndGravity()
    {
        isGrounded = controller.isGrounded;

        // Detect the exact frame of landing (was airborne last frame, grounded now)
        if (isGrounded && !wasGroundedLastFrame)
        {
            float landSpeed = currentDirection.magnitude > 0.1f ? currentMoveSpeed : 0f;
            animator.SetFloat("LandSpeed", landSpeed);
            animator.SetTrigger("LandTrigger");
        }

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // keeps controller grounded reliably

        velocity.y += gravity * Time.deltaTime;

        wasGroundedLastFrame = isGrounded;
    }

    void HandlePlatformMovement()
    {
        // short downward ray from the controller's base to detect what we're standing on
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (isGrounded && Physics.Raycast(origin, Vector3.down, out hit, platformCheckDistance + 0.1f, platformLayer))
        {
            Transform hitPlatform = hit.transform;

            if (hitPlatform != currentPlatform)
            {
                // just stepped onto a (possibly new) platform, so just record its transform for next frame
                currentPlatform = hitPlatform;
                lastPlatformPosition = currentPlatform.position;
                lastPlatformRotation = currentPlatform.rotation;
            }
            else
            {
                // already standing on this platform, carry its movement into the controller
                Vector3 platformDeltaPosition = currentPlatform.position - lastPlatformPosition;
                controller.Move(platformDeltaPosition);

                // also carry rotation, so spinning platforms turn the player with them
                Quaternion platformDeltaRotation = currentPlatform.rotation * Quaternion.Inverse(lastPlatformRotation);
                transform.rotation = platformDeltaRotation * transform.rotation;

                lastPlatformPosition = currentPlatform.position;
                lastPlatformRotation = currentPlatform.rotation;
            }
        }
        else
        {
            // not standing on anything trackable, stop carrying platform movement
            currentPlatform = null;
        }
    }

    void HandleCrouch()
    {
        bool wantsToCrouch = Input.GetKey(crouchKey);

        // Only resize the physical collider on state change, and only while grounded
        // (resizing mid-air was the cause of the earlier sinking bug).
        if (wantsToCrouch != isCrouching && isGrounded)
        {
            isCrouching = wantsToCrouch;

            float newHeight = isCrouching ? crouchHeight : standingHeight;
            controller.height = newHeight;

            // Center.y MUST equal height/2 to keep the capsule's bottom anchored at the ground.
            controller.center = new Vector3(0, newHeight / 2f, 0);
        }
    }

    void HandleMovementAndRotation()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // get direction of camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // remove vertical component
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // combine input with camera direction
        Vector3 direction = forward * vertical + right * horizontal;

        float currentSpeed = Input.GetKey(sprintKey) ? sprintSpeed : moveSpeed;

        // store for animator
        currentDirection = direction;
        currentMoveSpeed = currentSpeed;

        if (direction.magnitude > 0.1f)
        {
            // rotate player to face direction of movement
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);

            Vector3 move = direction.normalized * currentSpeed;
            controller.Move(move * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("JumpTrigger");
        }
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        float speedValue = currentDirection.magnitude > 0.1f ? currentMoveSpeed : 0f;
        animator.SetFloat("Speed", speedValue);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsSprinting", Input.GetKey(sprintKey));
    }
}