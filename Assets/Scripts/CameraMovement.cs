using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public Transform player;

    public Vector3 offset = new Vector3(0, 2, -5);

    public float mouseSensitivity = 100f;

    public float minPitch = -30f;
    public float maxPitch = 60f;

    //rotation values
    private float yaw = 0f;
    private float pitch = 0f;

   void Start()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = false;
    #endif

        Cursor.lockState = CursorLockMode.Locked;
        yaw = transform.eulerAngles.y;
    }
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // clamp pitch
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        //create rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // apply rotation
        transform.position = player.position + rotation * offset;
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}
