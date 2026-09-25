using UnityEngine;
// ======================================= //
// BellHitDetector.cs
// HitPointがBellに当たった時の処理
// ======================================= //

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

    [Header("Camera Shake")]
    [SerializeField]
    private CameraShake cameraShake;

    private float cooldownTimer;

    [Header("Damage")]
    [SerializeField]
    private BellHP bellHealth;

    [Header("Damage Display")]
    [SerializeField]
    private DamagePopupSpawner damagePopupSpawner;

    [SerializeField]
    private Transform damagePopupPosition;

    [Header("Game")]
    [SerializeField]
    private GameTimer gameTimer;

    [Header("Hit Sound")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip bellHitClip;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float hitVolume = 1.0f;

    private bool hitDetectionEnabled;

    // ゲーム開始時処理 //
    private void Awake()
    {
        if (bellRigidbody == null)
        {
            bellRigidbody =
                GetComponent<Rigidbody>();
        }

        if (audioSource == null)
        {
            audioSource =
                GetComponent<AudioSource>();
        }

        // START前はヒット判定を無効化
        hitDetectionEnabled = false;
    }

    // 当たり判定のクール時間処理
    private void FixedUpdate()
    {
        if (cooldownTimer > 0.0f)
        {
            cooldownTimer -= Time.fixedDeltaTime;
        }
    }

    // 当たった際の処理
    private void OnTriggerEnter(Collider other)
    {
        // START前なら終了
        if (!hitDetectionEnabled)
        {
            return;
        }

        // HitPointと衝突したか確認
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

        // 衝突時の速度を取得
        float hitSpeed = pendulumSpeed.Speed;

        // 基準値より弱い衝突は無視
        if (hitSpeed < minimumHitSpeed)
        {
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

        // 衝突時のHitPointの角度を獲得
        Vector3 hitDirection =
            hitVelocity.normalized;

        // 鐘に与える衝撃の強さ
        float impulseStrength = Mathf.Min(
            hitSpeed * impactMultiplier,
            maximumImpulse
        );

        // 方向と強さを合体
        Vector3 impulse =
            hitDirection * impulseStrength;

        // 鐘を動かす
        bellRigidbody.AddForce(
            impulse,
            ForceMode.Impulse
        );

        // 速度からダメージを計算する処理
        int damage = CalculateDamage(hitSpeed);

        if (damage > 0)
        {
            // 煩悩-かご利益+か判定
            if (gameTimer != null &&
                gameTimer.IsBenefitTime)
            {
                // HPが0になった後は、ご利益として加算
                gameTimer.AddBenefit(damage);

                Debug.Log(
                    $"ご利益へ加算: {damage}"
                );
            }
            else if (bellHealth != null)
            {
                // HPが残っている間はHPを減らす
                bellHealth.TakeDamage(damage);
            }
            else
            {
                Debug.LogError(
                    "BellHealthが未設定です。"
                );
            }
        }

        // SE再生
        if (damage > 0 &&
            audioSource != null &&
            bellHitClip != null)
        {
            audioSource.PlayOneShot(
                bellHitClip,
                hitVolume
            );
        }

        // カメラを揺らす
        if (damage > 0 &&
            cameraShake != null)
        {
            cameraShake.ShakeByDamage(
                damage
            );
        }

        // ダメージ表記を出す
        if (damage > 0 &&
            damagePopupSpawner != null)
        {
            Vector3 popupPosition =
                damagePopupPosition != null
                ? damagePopupPosition.position
                : transform.position;

            damagePopupSpawner.ShowDamage(
                damage,
                popupPosition
            );
        }

        cooldownTimer = hitCooldown;
    }

    private void OnTriggerExit(Collider other)
    {
        if (hitPointCollider != null &&
            other != hitPointCollider)
        {
            return;
        }
    }

    // 衝突時の速度からダメージを計算 //
    private int CalculateDamage(float hitSpeed)
    {
        if (hitSpeed < 1.0f)
        {
            return 0;
        }

        if (hitSpeed <= 10.0f)
        {
            return CalculateRangeDamage(
                hitSpeed,
                1.0f,
                10.0f,
                1,
                5
            );
        }

        if (hitSpeed <= 20.0f)
        {
            return CalculateRangeDamage(
                hitSpeed,
                11.0f,
                20.0f,
                6,
                10
            );
        }

        if (hitSpeed <= 30.0f)
        {
            return CalculateRangeDamage(
                hitSpeed,
                21.0f,
                30.0f,
                11,
                15
            );
        }

        if (hitSpeed <= 40.0f)
        {
            return CalculateRangeDamage(
                hitSpeed,
                31.0f,
                40.0f,
                16,
                20
            );
        }

        if (hitSpeed <= 50.0f)
        {
            return CalculateRangeDamage(
                hitSpeed,
                41.0f,
                50.0f,
                21,
                25
            );
        }

        if (hitSpeed <= 60.0f)
        {
            return CalculateRangeDamage(
                hitSpeed,
                51.0f,
                60.0f,
                25,
                30
            );
        }

        return 32;
    }

    private int CalculateRangeDamage(
     float speed,
     float minimumSpeed,
     float maximumSpeed,
     int minimumDamage,
     int maximumDamage)
    {
        float rate = Mathf.InverseLerp(
            minimumSpeed,
            maximumSpeed,
            speed
        );

        float calculatedDamage = Mathf.Lerp(
            minimumDamage,
            maximumDamage,
            rate
        );

        return Mathf.RoundToInt(calculatedDamage);
    }

    // 鐘の当たり判定を有効化 //
    public void EnableHitDetection()
    {
        hitDetectionEnabled = true;
        cooldownTimer = 0.0f;

        Debug.Log("Bellのヒット判定を有効化");
    }

    // 鐘の当たり判定を無効化 //
    public void DisableHitDetection()
    {
        hitDetectionEnabled = false;
        cooldownTimer = 0.0f;

        Debug.Log("Bellのヒット判定を無効化");
    }
}