using UnityEngine;

public class movementPlayer : MonoBehaviour
{
    private Rigidbody rb;
    public float jumpForce = 100f;
    private bool isGrounded;
    public float moveSpeed = 5f;

    //player rotation speed
    public float rotationSpeed = 10f;

    // Reference to the player's camera
    public Transform cameraTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //get direction of camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        //remove vertical movement
        forward.y = 0f;
        right.y = 0f;
        
        //convert 
        forward.Normalize();
        right.Normalize();

        //combine input with camera direction
        Vector3 direction = forward * vertical + right * horizontal;

        //only move if keyboard input
        if(direction.magnitude > 0.1f)
        {
            //rotate player to face direction of movement
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation,
                rotationSpeed * Time.deltaTime);

            rb.linearVelocity = new Vector3(
                direction.x * moveSpeed, 
                rb.linearVelocity.y, 
                direction.z * moveSpeed);
        }else{
            rb.linearVelocity = new Vector3(
                0, 
                rb.linearVelocity.y, 
                0);
        }

        
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}