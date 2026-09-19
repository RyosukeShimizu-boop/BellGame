using UnityEngine;

public class PendulumSpeed : MonoBehaviour
{
    [SerializeField]
    private float speed;

    public float Speed => speed;

    private Vector3 previousPosition;

    private void Start()
    {
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        speed = Vector3.Distance(
            transform.position,
            previousPosition
        ) / Time.fixedDeltaTime;

        previousPosition = transform.position;
    }
}