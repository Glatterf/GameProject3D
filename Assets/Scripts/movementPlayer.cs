using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class movementPlayer : MonoBehaviour
{
    private CharacterController controller;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Jump")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Crouch")]
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public Vector3 standingCenter = new Vector3(0, 1, 0);
    public Vector3 crouchCenter = new Vector3(0, 0.5f, 0);
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Sprint")]
    public float sprintSpeed = 8f;
    public KeyCode sprintKey = KeyCode.LeftShift;

    // Reference to the player's camera
    public Transform cameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleGroundAndGravity();
        HandleCrouch();
        HandleMovementAndRotation();
        HandleJump();

        // apply vertical velocity (gravity/jump) every frame
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleGroundAndGravity()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // keeps controller grounded reliably

        velocity.y += gravity * Time.deltaTime;
    }

    void HandleCrouch()
    {
        if (Input.GetKey(crouchKey))
        {
            controller.height = crouchHeight;
            controller.center = crouchCenter;
        }
        else
        {
            controller.height = standingHeight;
            controller.center = standingCenter;
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
        }
    }
}