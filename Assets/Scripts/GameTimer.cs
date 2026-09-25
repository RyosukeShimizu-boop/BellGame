using TMPro;
using UnityEngine;
// ======================================= //
// GameTimer.cs
// ゲーム全体の進行管理を処理
// ======================================= //

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

    [Header("Sunrise Background")]
    [SerializeField]
    private GameObject sunriseBackground;

    [Header("Benefit BGM")]
    [SerializeField]
    private AudioSource benefitBgmSource;

    public bool IsBenefitTime => isBenefitTime;
    public bool IsGameFinished => isGameFinished;
    public int TotalBenefit => totalBenefit;

    // ゲーム開始時処理 //
    private void Awake()
    {
        ResetTimer();

        // 全パネル非表示
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

        // 日の出背景非表示
        if (sunriseBackground != null)
        {
            sunriseBackground.SetActive(false);
        }
    }

    // 更新処理 //
    private void Update()
    {
        if (!isRunning || isGameFinished)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        // 時間切れ処理
        if (remainingTime <= 0.0f)
        {
            remainingTime = 0.0f;

            UpdateTimerText();
            TimeUp();

            return;
        }

        // タイマーテキスト更新
        UpdateTimerText();
    }

    // タイマースタート(STARTボタンで呼び出し) //
    public void StartTimer()
    {
        remainingTime = timeLimit;

        isRunning = true;
        isTimeUp = false;
        isBenefitTime = false;
        isGameFinished = false;

        totalBenefit = 0;

        // 全パネル非表示
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

        // 「ご利益」テキスト非表示
        if (benefitText != null)
        {
            benefitText.gameObject.SetActive(false);
            benefitText.text = "ご利益 0";
        }

        // 鐘有効化
        if (bellObject != null)
        {
            bellObject.SetActive(true);
        }

        // 日の出背景非表示
        if (sunriseBackground != null)
        {
            sunriseBackground.SetActive(false);
        }

        // タイマー更新
        UpdateTimerText();
    }

    // タイマーストップ(STOPボタンで呼び出し) //
    public void StopTimer()
    {
        // 日の出背景非表示
        if (sunriseBackground != null)
        {
            sunriseBackground.SetActive(false);
        }

        isRunning = false;
    }

    // タイマーリセット(STOPボタンで呼び出し) //
    public void ResetTimer()
    {
        remainingTime = timeLimit;

        isRunning = false;
        isTimeUp = false;
        isBenefitTime = false;
        isGameFinished = false;

        totalBenefit = 0;

        // 全パネル非表示
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

        // 「ご利益」文字非表示
        if (benefitText != null)
        {
            benefitText.gameObject.SetActive(false);
            benefitText.text = "ご利益 0";
        }

        // 鐘表示
        if (bellObject != null)
        {
            bellObject.SetActive(true);
        }

        // 日の出背景非表示
        if (sunriseBackground != null)
        {
            sunriseBackground.SetActive(false);
        }

        // ご利益タイムBGMストップ
        if (benefitBgmSource != null)
        {
            benefitBgmSource.Stop();
        }

        // タイマーテキスト更新
        UpdateTimerText();
    }

    // タイムアップ処理 //
    private void TimeUp()
    {
        if (isGameFinished)
        {
            return;
        }

        isRunning = false;
        isTimeUp = true;
        isGameFinished = true;

        // クリアかどうか判定
        if (isBenefitTime)
        {
            ShowGameClear();
        }
        else
        {
            ShowTimeUpResult();
        }
    }

    // タイマーテキスト更新処理 //
    private void UpdateTimerText()
    {
        if (timeText == null)
        {
            return;
        }

        timeText.text =
            $"年明けまであと {remainingTime:F0}";
    }

    // 折れた際のパネル表示 //
    public void SpeedOverGameOver()
    {
        if (isGameFinished)
        {
            return;
        }

        isRunning = false;
        isTimeUp = true;
        isGameFinished = true;

        // 速度監視を停止
        if (pendulumSpeed != null)
        {
            pendulumSpeed.StopSpeedMonitoring();
        }

        // Bellへのヒット判定を停止
        if (bellHitDetector != null)
        {
            bellHitDetector.DisableHitDetection();
        }

        // 軌跡を停止して消去
        if (hitPointTrailController != null)
        {
            hitPointTrailController.DisableTrail();
        }

        // 振り子を現在位置で停止
        if (pendulumController != null)
        {
            pendulumController.FreezeSimulation();
        }

        // 他の結果Panelを非表示
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        // 折れた場合のテキストを設定
        if (breakTitle != null)
        {
            breakTitle.text = "GAME OVER";
        }

        if (breakMessage != null)
        {
            breakMessage.text = "折れた";
        }

        if (sunriseBackground != null)
        {
            sunriseBackground.SetActive(false);
        }

        // BreakPanelへ切り替える
        if (breakPanel != null)
        {
            breakPanel.SetActive(true);
        }
    }

    // スーパーご利益タイムスタート処理 //
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

        // 制限時間を10秒追加
        remainingTime += bonusTime;

        // 日の出背景を表示
        if (sunriseBackground != null)
        {
            sunriseBackground.SetActive(true);
        }

        // ご利益タイム用BGMを最初から再生
        if (benefitBgmSource != null)
        {
            benefitBgmSource.Stop();
            benefitBgmSource.time = 0.0f;
            benefitBgmSource.Play();
        }

        // スーパーご利益タイム演出
        if (superBenefitEffect != null)
        {
            superBenefitEffect.PlayEffect();
        }

        // テキスト関連更新処理
        UpdateTimerText();
        UpdateBenefitText();
    }
    
    // ご利益追加処理 //
    public void AddBenefit(int damage)
    {
        if (!isRunning)
        {
            return;
        }

        if (!isBenefitTime)
        {
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

        // テキスト更新処理
        UpdateBenefitText();
    }

    // 「ご利益」文字更新処理 //
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
                $"ご利益 {totalBenefit}";
        }
    }

    // クリアパネル表示処理 //
    private void ShowGameClear()
    {

        // 通常のゲームオーバーPanelは隠す
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
                $"獲得したご利益 {totalBenefit}";
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

    // 煩悩が残っていた時のパネル表示処理 //
    private void ShowTimeUpResult()
    {
        Debug.Log("時間切れ");

        if (gameOverTitle != null)
        {
            gameOverTitle.text =
                "HAPPY NEW YEAR";
        }

        if (remainText != null &&
            bellHP != null)
        {
            remainText.text =
                $"祓えなかった煩悩 " +
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