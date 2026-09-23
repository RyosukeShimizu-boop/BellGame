using TMPro;
using UnityEngine;

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

    private void Start()
    {
        previousPosition = transform.position;

        if (dangerText != null)
        {
            dangerText.gameObject.SetActive(false);
        }
    }

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

    public void StopSpeedMonitoring()
    {
        monitoring = false;
        failureTriggered = false;

        ResetDangerousState();
    }

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

    private void ResetDangerousState()
    {
        dangerousTimer = 0.0f;

        if (dangerText != null)
        {
            dangerText.gameObject.SetActive(false);
        }
    }
}