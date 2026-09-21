using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BellReturnController : MonoBehaviour
{
    [Header("Spring Settings")]
    [SerializeField]
    private float springStrength = 40.0f;

    [SerializeField]
    private float dampingStrength = 12.0f;

    [Header("Stop Settings")]
    [SerializeField]
    private float stopDistance = 0.01f;

    [SerializeField]
    private float stopSpeed = 0.05f;

    private Rigidbody bellRigidbody;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        bellRigidbody = GetComponent<Rigidbody>();

        initialPosition = bellRigidbody.position;
        initialRotation = bellRigidbody.rotation;
    }

    private void FixedUpdate()
    {
        Vector3 displacement =
            initialPosition - bellRigidbody.position;

        // Z方向は復元計算に含めない
        displacement.z = 0.0f;

        // 初期位置へ戻すバネの力
        Vector3 springForce =
            displacement * springStrength;

        // 現在の移動を弱める減衰力
        Vector3 dampingForce =
            -bellRigidbody.linearVelocity
            * dampingStrength;

        Vector3 returnForce =
            springForce + dampingForce;

        bellRigidbody.AddForce(
            returnForce,
            ForceMode.Force
        );

        // 十分近く、十分遅くなったら完全に初期位置へ戻す
        if (displacement.sqrMagnitude
                <= stopDistance * stopDistance &&
            bellRigidbody.linearVelocity.sqrMagnitude
                <= stopSpeed * stopSpeed)
        {
            bellRigidbody.position =
                initialPosition;

            bellRigidbody.rotation =
                initialRotation;

            bellRigidbody.linearVelocity =
                Vector3.zero;

            bellRigidbody.angularVelocity =
                Vector3.zero;
        }
    }

    public void ResetImmediately()
    {
        bellRigidbody.position =
            initialPosition;

        bellRigidbody.rotation =
            initialRotation;

        bellRigidbody.linearVelocity =
            Vector3.zero;

        bellRigidbody.angularVelocity =
            Vector3.zero;
    }
}

