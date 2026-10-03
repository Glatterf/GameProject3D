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
    public KeyCode sprintKeyAlt = KeyCode.RightShift;

    private bool IsSprinting => Input.GetKey(sprintKey) || Input.GetKey(sprintKeyAlt);

    [Header("Moving Platform")]
    public LayerMask platformLayer;
    public float platformCheckDistance = 0.3f;
    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;
    private Quaternion lastPlatformRotation;

    [Header("Footsteps")]
    public float walkStepInterval = 0.5f;
    public float sprintStepInterval = 0.3f;
    public float crouchStepInterval = 0.7f;
    private float footstepTimer = 0f;
    private string currentTerrainType = "soil";

    // Reference to the player's camera
    public Transform cameraTransform;

    // stored each frame so UpdateAnimator can read them
    private Vector3 currentDirection;
    private float currentMoveSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        controller.height = standingHeight;
        controller.center = new Vector3(0, standingHeight / 2f, 0);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBGM();
        else
            Debug.LogError("AudioManager not found in scene!");
    }

    void Update()
    {
        HandleGroundAndGravity();
        HandlePlatformMovement();
        HandleCrouch();
        HandleMovementAndRotation();
        HandleJump();
        HandleFootsteps();
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
            if (animator != null)
            {
                animator.SetFloat("LandSpeed", landSpeed);
                animator.SetTrigger("LandTrigger");
            }

            // landing thud
            PlayFootstepNow();
        }

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // keeps controller grounded reliably

        velocity.y += gravity * Time.deltaTime;

        wasGroundedLastFrame = isGrounded;
    }

    void HandlePlatformMovement()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (isGrounded && Physics.Raycast(origin, Vector3.down, out hit, platformCheckDistance + 0.1f, platformLayer))
        {
            Transform hitPlatform = hit.transform;

            if (hitPlatform != currentPlatform)
            {
                currentPlatform = hitPlatform;
                lastPlatformPosition = currentPlatform.position;
                lastPlatformRotation = currentPlatform.rotation;
            }
            else
            {
                Vector3 platformDeltaPosition = currentPlatform.position - lastPlatformPosition;
                controller.Move(platformDeltaPosition);

                Quaternion platformDeltaRotation = currentPlatform.rotation * Quaternion.Inverse(lastPlatformRotation);
                transform.rotation = platformDeltaRotation * transform.rotation;

                lastPlatformPosition = currentPlatform.position;
                lastPlatformRotation = currentPlatform.rotation;
            }
        }
        else
        {
            currentPlatform = null;
        }
    }

    void HandleCrouch()
    {
        bool wantsToCrouch = Input.GetKey(crouchKey);

        if (wantsToCrouch != isCrouching && isGrounded)
        {
            isCrouching = wantsToCrouch;

            float newHeight = isCrouching ? crouchHeight : standingHeight;
            controller.height = newHeight;
            controller.center = new Vector3(0, newHeight / 2f, 0);
        }
    }

    void HandleMovementAndRotation()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * vertical + right * horizontal;

        float currentSpeed = IsSprinting ? sprintSpeed : moveSpeed;

        currentDirection = direction;
        currentMoveSpeed = currentSpeed;

        if (direction.magnitude > 0.1f)
        {
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
            if (animator != null) animator.SetTrigger("JumpTrigger");
        }
    }

    // ---------------- FOOTSTEPS ----------------

    void HandleFootsteps()
    {
        bool isMoving = currentDirection.magnitude > 0.1f;

        if (isGrounded && isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstepNow();

                footstepTimer = isCrouching ? crouchStepInterval
                              : IsSprinting ? sprintStepInterval
                              : walkStepInterval;
            }
        }
        else
        {
            footstepTimer = 0f; // first step plays immediately when you start moving
        }
    }

    void PlayFootstepNow()
    {
        if (AudioManager.Instance == null) return;

        DetectTerrainType();
        AudioManager.Instance.PlayFootstep(currentTerrainType);
    }

    // Uses an overlap check (not a raycast) because a raycast can't detect
    // trigger boxes the player is already standing inside.
    void DetectTerrainType()
    {
        // Start above the player so the ray begins OUTSIDE the trigger boxes
        Vector3 origin = transform.position + Vector3.up * 3f;
        float rayLength = 4f; // reaches about 1 unit below the feet

        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            Vector3.down,
            rayLength,
            ~0,
            QueryTriggerInteraction.Collide);

        Debug.DrawRay(origin, Vector3.down * rayLength, Color.red, 0.5f);

        bool onWater = false;
        bool onRock = false;
        float feetY = transform.position.y;

        foreach (RaycastHit hit in hits)
        {
            // ignore the player's own collider
            if (hit.collider.transform.IsChildOf(transform)) continue;

            // ignore surfaces that are well below the feet
            if (hit.point.y < feetY - 0.5f) continue;

            if (hit.collider.GetComponentInParent<WaterZone>() != null)
                onWater = true;
            else if (hit.collider.gameObject.name.ToLower().Contains("rock"))
                onRock = true;
        }

        // priority: water > rock > soil
        if (onWater) currentTerrainType = "water";
        else if (onRock) currentTerrainType = "rock";
        else currentTerrainType = "soil";

        Debug.Log($"Terrain: {currentTerrainType}");
    }
    void UpdateAnimator()
    {
        if (animator == null) return;

        float speedValue = currentDirection.magnitude > 0.1f ? currentMoveSpeed : 0f;
        animator.SetFloat("Speed", speedValue);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsSprinting", IsSprinting);
    }
}