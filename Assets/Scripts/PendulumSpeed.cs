using TMPro;
using UnityEngine;
// ======================================= //
// PendulumSpeed.cs
// 振り子速度処理
// ======================================= //

public class PendulumSpeed : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField]
    private float speed;

    [SerializeField]
    private Vector3 velocity;

    [Header("Speed Failure")]
    [SerializeField]
    private float dangerousSpeed = 61.0f;

    [SerializeField]
    private float dangerousDuration = 3.0f;

    [SerializeField]
    private TMP_Text dangerText;

    [SerializeField]
    private GameTimer gameTimer;

    private Vector3 previousPosition;
    private float dangerousTimer;
    private bool monitoring;
    private bool failureTriggered;

    public float Speed => speed;
    public Vector3 Velocity => velocity;
    public float DangerousTimer => dangerousTimer;

    // 開始時処理 //
    private void Start()
    {
        previousPosition = transform.position;

        if (dangerText != null)
        {
            dangerText.gameObject.SetActive(false);
        }
    }

    // 物理更新処理 //
    private void FixedUpdate()
    {
        velocity =
            (transform.position - previousPosition)
            / Time.fixedDeltaTime;

        speed = velocity.magnitude;
        previousPosition = transform.position;

        if (!monitoring || failureTriggered)
        {
            return;
        }

        UpdateDangerousSpeed();
    }

    // 危険速度になった時の更新処理 //
    private void UpdateDangerousSpeed()
    {
        if (speed >= dangerousSpeed)
        {
            dangerousTimer += Time.fixedDeltaTime;

            // 2秒経過したら、0を表示せずゲームオーバー
            if (dangerousTimer >= dangerousDuration)
            {
                failureTriggered = true;
                monitoring = false;

                if (dangerText != null)
                {
                    dangerText.gameObject.SetActive(false);
                }

                if (gameTimer != null)
                {
                    gameTimer.SpeedOverGameOver();
                }

                return;
            }

            UpdateDangerText();
        }
        else
        {
            ResetDangerousState();
        }
    }

    // 速度監視開始処理 //
    public void BeginSpeedMonitoring()
    {
        previousPosition = transform.position;

        velocity = Vector3.zero;
        speed = 0.0f;
        dangerousTimer = 0.0f;

        failureTriggered = false;
        monitoring = true;

        if (dangerText != null)
        {
            dangerText.gameObject.SetActive(false);
        }
    }

    // 速度監視終了処理 //
    public void StopSpeedMonitoring()
    {
        monitoring = false;
        failureTriggered = false;

        ResetDangerousState();
    }

    // 速度計測を初期状態へ戻す処理 //
    public void ResetMeasurement()
    {
        previousPosition = transform.position;
        velocity = Vector3.zero;
        speed = 0.0f;
        dangerousTimer = 0.0f;
        failureTriggered = false;

        if (dangerText != null)
        {
            dangerText.gameObject.SetActive(false);
        }
    }

    // 「折れそう」文字表示処理 //
    private void UpdateDangerText()
    {
        if (dangerText == null)
        {
            return;
        }

        float remainingDangerTime =
            dangerousDuration - dangerousTimer;

        int displayNumber =
            Mathf.CeilToInt(remainingDangerTime);

        // 0以下は表示しない
        if (displayNumber <= 0)
        {
            dangerText.gameObject.SetActive(false);
            return;
        }

        dangerText.gameObject.SetActive(true);

        dangerText.text =
            $"折れそう {displayNumber}";
    }

    // 「折れそう」文字消す処理 //
    private void ResetDangerousState()
    {
        dangerousTimer = 0.0f;

        if (dangerText != null)
        {
            dangerText.gameObject.SetActive(false);
        }
    }
}