using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        ResetHealth();
    }

    public void TakeDamage(int damage)
    {
        // HPが0になった後はBellHP側では処理しない
        // 以降のダメージはBellHitDetectorから
        // GameTimer.AddBenefit()へ渡す
        if (isDestroyed)
        {
            return;
        }

        damage = Mathf.Max(damage, 0);

        if (damage <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(
            currentHealth - damage,
            0
        );

        UpdateHealthUI();

        Debug.Log(
            $"Bell Damage: {damage}, " +
            $"HP: {currentHealth} / {maximumHealth}"
        );

        if (currentHealth <= 0)
        {
            isDestroyed = true;
            OnBellDestroyed();
        }
    }

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

        Debug.Log(
            $"Bell HPをリセット: " +
            $"{currentHealth} / {maximumHealth}"
        );
    }
    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maximumHealth;
            healthBar.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text =
                $"残り煩悩 {currentHealth}";
        }
    }

    private void OnBellDestroyed()
    {
        Debug.Log(
            "Bell HPが0になりました。" +
            "制限時間を追加して、ご利益タイムを開始します。"
        );

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