using TMPro;
using UnityEngine;
using UnityEngine.UI;
// ======================================= //
// BellHP.cs
// 鐘のHP(残煩悩)管理
// ======================================= //

public class BellHP : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    private int maximumHealth = 108;

    [SerializeField]
    private int currentHealth;

    [Header("UI")]
    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private TMP_Text healthText;

    [Header("Game")]
    [SerializeField]
    private GameTimer gameTimer;

    private bool isDestroyed;

    public int MaximumHealth => maximumHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDestroyed => isDestroyed;

    // ゲーム開始時処理 //
    private void Awake()
    {
        ResetHealth();
    }

    // ダメージを与える(煩悩を減らす)処理 //
    public void TakeDamage(int damage)
    {
        // HPが0になった後はBellHP側では処理しない
        // 以降のダメージはBellHitDetectorから
        // GameTimer.AddBenefit()へ渡す
        if (isDestroyed)
        {
            return;
        }

        // 万が一ダメージが-値だった場合0にする
        damage = Mathf.Max(damage, 0);

        if (damage <= 0)
        {
            return;
        }

        // 煩悩を減らす処理
        currentHealth = Mathf.Max(
            currentHealth - damage,
            0
        );

        // ゲージ更新処理
        UpdateHealthUI();

        // 煩悩が0になった時の処理
        if (currentHealth <= 0)
        {
            isDestroyed = true;
            OnBellDestroyed();
        }
    }

    // HP(煩悩)を初期値に戻す
    public void ResetHealth()
    {
        currentHealth = maximumHealth;
        isDestroyed = false;

        // HPゲージを再表示
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
        }

        // HP数値テキストを再表示
        if (healthText != null)
        {
            healthText.gameObject.SetActive(true);
        }

        UpdateHealthUI();
    }

    // HP(煩悩)ゲージ更新処理 //
    private void UpdateHealthUI()
    {
        // slider更新
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maximumHealth;
            healthBar.value = currentHealth;
        }

        // Text更新
        if (healthText != null)
        {
            healthText.text =
                $"残り煩悩 {currentHealth}";
        }
    }

    // HP(煩悩)が0になった時の処理
    private void OnBellDestroyed()
    {
        // HPゲージを非表示
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        // HP数値テキストを非表示
        if (healthText != null)
        {
            healthText.gameObject.SetActive(false);
        }

        // ご利益タイムを開始
        if (gameTimer != null)
        {
            gameTimer.StartBenefitTime();
        }
        else
        {
            Debug.LogError(
                "BellHPのGame Timerが未設定です。"
            );
        }
    }
}