using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.right;
    public float moveDistance = 3f;
    public float speed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, moveDistance);
        transform.position = startPos + moveDirection.normalized * offset;
    }
}