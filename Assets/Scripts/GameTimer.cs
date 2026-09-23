using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField]
    private float timeLimit = 30.0f;

    [SerializeField]
    private float remainingTime;

    [Header("UI")]
    [SerializeField]
    private TMP_Text timeText;

    [Header("Timeout")]
    [SerializeField]
    private PendulumSetupController pendulumController;

    [SerializeField]
    private BellHP bellHP;

    [Header("Game Over UI")]
    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private TMP_Text gameOverTitle;

    [SerializeField]
    private TMP_Text remainText;

    [Header("Bell Display")]
    [SerializeField]
    private GameObject bellObject;

    [SerializeField]
    private PendulumSpeed pendulumSpeed;

    [Header("Super Benefit Effect")]
    [SerializeField]
    private SuperBenefitEffect superBenefitEffect;

    [Header("Clear UI")]
    [SerializeField]
    private GameObject clearPanel;

    [SerializeField]
    private TMP_Text clearTitle;

    [SerializeField]
    private TMP_Text clearBenefitText;

    [Header("Break UI")]
    [SerializeField]
    private GameObject breakPanel;

    [SerializeField]
    private TMP_Text breakTitle;

    [SerializeField]
    private TMP_Text breakMessage;

    private bool isRunning;
    private bool isTimeUp;

    public float RemainingTime => remainingTime;
    public bool IsRunning => isRunning;
    public bool IsTimeUp => isTimeUp;

    [Header("Benefit Time")]
    [SerializeField]
    private float bonusTime = 10.0f;

    [SerializeField]
    private TMP_Text benefitText;

    [Header("Speed Over")]
    [SerializeField]
    private PendulumSetupController OverpendulumController;

    [SerializeField]
    private BellHitDetector bellHitDetector;

    [SerializeField]
    private HitPointTrailController hitPointTrailController;

    private bool isBenefitTime;
    private bool isGameFinished;
    private int totalBenefit;

    public bool IsBenefitTime => isBenefitTime;
    public bool IsGameFinished => isGameFinished;
    public int TotalBenefit => totalBenefit;

    private void Awake()
    {
        ResetTimer();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        if (breakPanel != null)
        {
            breakPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isRunning || isGameFinished)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0.0f)
        {
            remainingTime = 0.0f;

            UpdateTimerText();
            TimeUp();

            return;
        }

        UpdateTimerText();
    }

    public void StartTimer()
    {
        remainingTime = timeLimit;

        isRunning = true;
        isTimeUp = false;
        isBenefitTime = false;
        isGameFinished = false;

        totalBenefit = 0;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        if (breakPanel != null)
        {
            breakPanel.SetActive(false);
        }

        if (benefitText != null)
        {
            benefitText.gameObject.SetActive(false);
            benefitText.text = "‚²—˜‰v 0";
        }

        if (bellObject != null)
        {
            bellObject.SetActive(true);
        }

        UpdateTimerText();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        remainingTime = timeLimit;

        isRunning = false;
        isTimeUp = false;
        isBenefitTime = false;
        isGameFinished = false;

        totalBenefit = 0;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        if (breakPanel != null)
        {
            breakPanel.SetActive(false);
        }

        if (benefitText != null)
        {
            benefitText.gameObject.SetActive(false);
            benefitText.text = "‚²—˜‰v 0";
        }

        if (bellObject != null)
        {
            bellObject.SetActive(true);
        }

        UpdateTimerText();
    }


    private void TimeUp()
    {
        if (isGameFinished)
        {
            return;
        }

        isRunning = false;
        isTimeUp = true;
        isGameFinished = true;

        // HP‚ð0‚É‚Å‚«‚Ä‚¢‚½ê‡
        if (isBenefitTime)
        {
            ShowGameClear();
        }
        else
        {
            ShowTimeUpResult();
        }
    }

    private void UpdateTimerText()
    {
        if (timeText == null)
        {
            return;
        }

        timeText.text =
            $"”N–¾‚¯‚Ü‚Å‚ ‚Æ {remainingTime:F0}";
    }

    public void SpeedOverGameOver()
    {
        if (isGameFinished)
        {
            return;
        }

        isRunning = false;
        isTimeUp = true;
        isGameFinished = true;

        // ‘¬“xŠÄŽ‹‚ð’âŽ~
        if (pendulumSpeed != null)
        {
            pendulumSpeed.StopSpeedMonitoring();
        }

        // Bell‚Ö‚Ìƒqƒbƒg”»’è‚ð’âŽ~
        if (bellHitDetector != null)
        {
            bellHitDetector.DisableHitDetection();
        }

        // ‹OÕ‚ð’âŽ~‚µ‚ÄÁ‹Ž
        if (hitPointTrailController != null)
        {
            hitPointTrailController.DisableTrail();
        }

        // U‚èŽq‚ðŒ»ÝˆÊ’u‚Å’âŽ~
        if (pendulumController != null)
        {
            pendulumController.FreezeSimulation();
        }

        // ‘¼‚ÌŒ‹‰ÊPanel‚ð”ñ•\Ž¦
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        // Ü‚ê‚½ê‡‚ÌƒeƒLƒXƒg‚ðÝ’è
        if (breakTitle != null)
        {
            breakTitle.text = "GAME OVER";
        }

        if (breakMessage != null)
        {
            breakMessage.text = "à–Ø‚ªÜ‚ê‚½";
        }

        // BreakPanel‚ÖØ‚è‘Ö‚¦‚é
        if (breakPanel != null)
        {
            breakPanel.SetActive(true);
        }

        Debug.Log(
            "‘¬“x’´‰ß‚É‚æ‚èà–Ø‚ªÜ‚ê‚Ü‚µ‚½B"
        );
    }


    public void StartBenefitTime()
    {
        if (!isRunning ||
            isBenefitTime ||
            isGameFinished)
        {
            return;
        }

        isBenefitTime = true;
        totalBenefit = 0;

        // §ŒÀŽžŠÔ‚ð10•b’Ç‰Á
        remainingTime += bonusTime;

        // ƒXƒ[‚Æ•¶Žš‰‰o
        if (superBenefitEffect != null)
        {
            superBenefitEffect.PlayEffect();
        }
        else
        {
            Debug.LogError(
                "GameTimer‚ÌSuper Benefit Effect‚ª–¢Ý’è‚Å‚·B"
            );
        }

        UpdateTimerText();
        UpdateBenefitText();

        Debug.Log(
            $"ƒX[ƒp[‚²—˜‰vƒ^ƒCƒ€ŠJŽnB" +
            $"Žc‚èŽžŠÔ‚É{bonusTime:F0}•b’Ç‰Á"
        );
    }
    public void AddBenefit(int damage)
    {
        if (!isRunning)
        {
            Debug.LogWarning(
                "ƒ^ƒCƒ}[’âŽ~’†‚Ì‚½‚ßA‚²—˜‰v‚ð‰ÁŽZ‚Å‚«‚Ü‚¹‚ñB"
            );

            return;
        }

        if (!isBenefitTime)
        {
            Debug.LogWarning(
                "‚²—˜‰vƒ^ƒCƒ€‚Å‚Í‚ ‚è‚Ü‚¹‚ñB"
            );

            return;
        }

        if (isGameFinished)
        {
            return;
        }

        damage = Mathf.Max(damage, 0);

        if (damage <= 0)
        {
            return;
        }

        totalBenefit += damage;

        UpdateBenefitText();

        Debug.Log(
            $"‚²—˜‰v +{damage}A" +
            $"‡Œv‚²—˜‰v: {totalBenefit}"
        );
    }

    private void UpdateBenefitText()
    {
        if (benefitText == null)
        {
            return;
        }

        bool shouldShow =
            isRunning &&
            isBenefitTime &&
            !isGameFinished;

        benefitText.gameObject.SetActive(
            shouldShow
        );

        if (shouldShow)
        {
            benefitText.text =
                $"‚²—˜‰v {totalBenefit}";
        }
    }

    private void ShowGameClear()
    {
        Debug.Log(
            $"ƒQ[ƒ€ƒNƒŠƒABÅI‚²—˜‰v: {totalBenefit}"
        );

        // ’Êí‚ÌƒQ[ƒ€ƒI[ƒo[Panel‚Í‰B‚·
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (clearTitle != null)
        {
            clearTitle.text =
                "HAPPY NEW YEAR!";
        }

        if (clearBenefitText != null)
        {
            clearBenefitText.text =
                $"Šl“¾‚µ‚½‚²—˜‰v {totalBenefit}";
        }

        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
        }

        if (benefitText != null)
        {
            benefitText.gameObject.SetActive(false);
        }

        if (bellObject != null)
        {
            bellObject.SetActive(false);
        }
    }

    private void ShowTimeUpResult()
    {
        Debug.Log("ŽžŠÔØ‚ê");

        if (gameOverTitle != null)
        {
            gameOverTitle.text =
                "HAPPY NEW YEAR";
        }

        if (remainText != null &&
            bellHP != null)
        {
            remainText.text =
                $"âP‚¦‚È‚©‚Á‚½”Ï”Y " +
                $"{bellHP.CurrentHealth}";
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (bellObject != null)
        {
            bellObject.SetActive(false);
        }

        if (pendulumSpeed != null)
        {
            pendulumSpeed.StopSpeedMonitoring();
        }
    }
}