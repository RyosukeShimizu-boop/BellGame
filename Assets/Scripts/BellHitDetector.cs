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

        // STARTëOÇÕÉqÉbÉgîªíËÇñ≥å¯âª
        hitDetectionEnabled = false;
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
        if (!hitDetectionEnabled)
        {
            return;
        }

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
                "PendulumSpeedÇ‹ÇΩÇÕBellÇÃRigidbodyÇ™ñ¢ê›íËÇ≈Ç∑ÅB"
            );
            return;
        }

        float hitSpeed = pendulumSpeed.Speed;

        if (hitSpeed < minimumHitSpeed)
        {
            return;
        }

        Vector3 hitVelocity =
            pendulumSpeed.Velocity;

        // 2DâÊñ äOÇ÷ìÆÇ©Ç»Ç¢ÇÊÇ§Zê¨ï™ÇèúäO
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

        int damage = CalculateDamage(hitSpeed);

        if (damage > 0)
        {
            if (gameTimer != null &&
                gameTimer.IsBenefitTime)
            {
                // HPÇ™0Ç…Ç»Ç¡ÇΩå„ÇÕÅAÇ≤óòâvÇ∆ÇµÇƒâ¡éZ
                gameTimer.AddBenefit(damage);

                Debug.Log(
                    $"Ç≤óòâvÇ÷â¡éZ: {damage}"
                );
            }
            else if (bellHealth != null)
            {
                // HPÇ™écÇ¡ÇƒÇ¢ÇÈä‘ÇÕHPÇå∏ÇÁÇ∑
                bellHealth.TakeDamage(damage);
            }
            else
            {
                Debug.LogError(
                    "BellHealthÇ™ñ¢ê›íËÇ≈Ç∑ÅB"
                );
            }
        }

        if (damage > 0 &&
            audioSource != null &&
            bellHitClip != null)
        {
            audioSource.PlayOneShot(
                bellHitClip,
                hitVolume
            );
        }

        if (damage > 0 &&
            cameraShake != null)
        {
            cameraShake.ShakeByDamage(
                damage
            );
        }

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

    public void EnableHitDetection()
    {
        hitDetectionEnabled = true;
        cooldownTimer = 0.0f;

        Debug.Log("BellÇÃÉqÉbÉgîªíËÇóLå¯âª");
    }

    public void DisableHitDetection()
    {
        hitDetectionEnabled = false;
        cooldownTimer = 0.0f;

        Debug.Log("BellÇÃÉqÉbÉgîªíËÇñ≥å¯âª");
    }
}