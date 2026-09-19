using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PendulumEnergyAssist : MonoBehaviour
{
    [Header("補助設定")]
    [SerializeField]
    private float assistTorque = 10.0f;

    [SerializeField]
    private float energyTolerance = 0.02f;

    private Rigidbody2D rigidBody2D;

    // 開始時のエネルギー
    private float targetEnergy;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();

        rigidBody2D.linearDamping = 0.0f;
        rigidBody2D.angularDamping = 0.0f;
    }

    private void Start()
    {
        targetEnergy = CalculateEnergy();
    }

    private void FixedUpdate()
    {
        float currentEnergy = CalculateEnergy();

        // 目標エネルギーを維持できている場合は補助しない
        if (currentEnergy >= targetEnergy * (1.0f - energyTolerance))
        {
            return;
        }

        float angularVelocity = rigidBody2D.angularVelocity;

        // 完全停止に近い場合は方向を決められないので補助しない
        if (Mathf.Abs(angularVelocity) < 0.1f)
        {
            return;
        }

        float direction = Mathf.Sign(angularVelocity);

        rigidBody2D.AddTorque(
            direction * assistTorque,
            ForceMode2D.Force
        );
    }

    private float CalculateEnergy()
    {
        // 並進運動エネルギー
        float linearEnergy =
            0.5f *
            rigidBody2D.mass *
            rigidBody2D.linearVelocity.sqrMagnitude;

        // 角速度を度からラジアンへ変換
        float angularVelocityRadians =
            rigidBody2D.angularVelocity * Mathf.Deg2Rad;

        // 回転運動エネルギー
        float rotationalEnergy =
            0.5f *
            rigidBody2D.inertia *
            angularVelocityRadians *
            angularVelocityRadians;

        // 位置エネルギー
        float potentialEnergy =
            rigidBody2D.mass *
            Mathf.Abs(Physics2D.gravity.y) *
            rigidBody2D.worldCenterOfMass.y;

        return linearEnergy + rotationalEnergy + potentialEnergy;
    }
}