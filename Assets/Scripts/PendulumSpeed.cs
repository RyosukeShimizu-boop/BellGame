using UnityEngine;

public class PendulumSpeed : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private Vector3 velocity;

    public float Speed => speed;
    public Vector3 Velocity => velocity;

    private Vector3 previousPosition;

    private void Start()
    {
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        velocity =
            (transform.position - previousPosition)
            / Time.fixedDeltaTime;

        speed = velocity.magnitude;

        previousPosition = transform.position;
    }

    public void ResetMeasurement()
    {
        previousPosition = transform.position;
        velocity = Vector3.zero;
        speed = 0.0f;
    }
}