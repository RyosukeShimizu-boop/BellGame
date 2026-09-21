using UnityEngine;

public class BellHitDetector : MonoBehaviour
{
    [Header("HitPoint References")]
    [SerializeField]
    private PendulumSpeed pendulumSpeed;

    [SerializeField]
    private Collider hitPointCollider;

    [Header("Bell Physics")]
    [SerializeField]
    private Rigidbody bellRigidbody;

    [SerializeField]
    private float impactMultiplier = 1.0f;

    [SerializeField]
    private float maximumImpulse = 20.0f;

    [Header("Hit Settings")]
    [SerializeField]
    private float minimumHitSpeed = 1.0f;

    [SerializeField]
    private float hitCooldown = 0.2f;

    private float cooldownTimer;

    private void Awake()
    {
        if (bellRigidbody == null)
        {
            bellRigidbody = GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        if (cooldownTimer > 0.0f)
        {
            cooldownTimer -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitPointCollider != null &&
            other != hitPointCollider)
        {
            return;
        }

        if (cooldownTimer > 0.0f)
        {
            return;
        }

        if (pendulumSpeed == null ||
            bellRigidbody == null)
        {
            Debug.LogError(
                "PendulumSpeedまたはBellのRigidbodyが未設定です。"
            );
            return;
        }

        float hitSpeed = pendulumSpeed.Speed;

        if (hitSpeed < minimumHitSpeed)
        {
            Debug.Log(
                $"弱い接触のため無効。速度: {hitSpeed:F2}"
            );
            return;
        }

        Vector3 hitVelocity =
            pendulumSpeed.Velocity;

        // 2D画面外へ動かないようZ成分を除外
        hitVelocity.z = 0.0f;

        if (hitVelocity.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Vector3 hitDirection =
            hitVelocity.normalized;

        float impulseStrength = Mathf.Min(
            hitSpeed * impactMultiplier,
            maximumImpulse
        );

        Vector3 impulse =
            hitDirection * impulseStrength;

        bellRigidbody.AddForce(
            impulse,
            ForceMode.Impulse
        );

        cooldownTimer = hitCooldown;

        Debug.Log(
            $"鐘ヒット！速度: {hitSpeed:F2}, " +
            $"方向: {hitDirection}, " +
            $"反動: {impulseStrength:F2}"
        );
    }

    private void OnTriggerExit(Collider other)
    {
        if (hitPointCollider != null &&
            other != hitPointCollider)
        {
            return;
        }

        Debug.Log("HitPointがBellから退出");
    }
}