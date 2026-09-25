using TMPro;
using UnityEngine;
// ======================================= //
// DamagePopup.cs
// 与えたダメージを表示する処理
// ======================================= //

public class DamagePopup : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField]
    private float lifeTime = 1.0f;

    [SerializeField]
    private float moveDistance = 80.0f;

    [SerializeField]
    private float startScale = 1.3f;

    [SerializeField]
    private float endScale = 1.0f;

    private RectTransform rectTransform;
    private TMP_Text damageText;

    private Vector2 startPosition;
    private Color originalColor;

    private float elapsedTime;

    [Header("Three Level Font Size")]
    [SerializeField]
    private float smallFontSize = 72.0f;

    [SerializeField]
    private float mediumFontSize = 108.0f;

    [SerializeField]
    private float largeFontSize = 144.0f;

    [SerializeField]
    private int mediumDamageThreshold = 11;

    [SerializeField]
    private int largeDamageThreshold = 21;

    // ゲーム開始時処理 //
    private void Awake()
    {
        rectTransform =
            GetComponent<RectTransform>();

        damageText =
            GetComponent<TMP_Text>();

        startPosition =
            rectTransform.anchoredPosition;

        originalColor =
            damageText.color;
    }

    // 生成直後に呼び出す処理 //
    public void Initialize(int damage)
    {
        // テキスト表示
        damageText.text = $"{damage}煩悩";

        elapsedTime = 0.0f;

        // 出現場所を獲得
        startPosition =
            rectTransform.anchoredPosition;

        // ダメージによって表示するサイズを変更
        damageText.fontSize =
            GetFontSize(damage);

        // 初期スケール
        rectTransform.localScale =
            Vector3.one * startScale;

        Color color = originalColor;
        color.a = 1.0f;
        damageText.color = color;
    }

    // 更新処理 //
    private void Update()
    {
        // 経過時間加算
        elapsedTime += Time.deltaTime;

        // 進行率
        float rate = Mathf.Clamp01(
            elapsedTime / lifeTime
        );

        // 少し上方向へ移動
        rectTransform.anchoredPosition =
            startPosition +
            Vector2.up * moveDistance * rate;

        // 最初は少し大きく、徐々に通常サイズへ
        float currentScale = Mathf.Lerp(
            startScale,
            endScale,
            rate
        );

        rectTransform.localScale =
            Vector3.one * currentScale;

        // 後半で透明にする
        float alpha = 1.0f;
        if (rate >= 0.4f)
        {
            alpha = Mathf.InverseLerp(
                1.0f,
                0.4f,
                rate
            );
        }

        Color color = originalColor;
        color.a = alpha;
        damageText.color = color;

        if (elapsedTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    // ダメージ別サイズ切り替え処理 //
    private float GetFontSize(int damage)
    {
        if (damage >= largeDamageThreshold)
        {
            return largeFontSize;
        }

        if (damage >= mediumDamageThreshold)
        {
            return mediumFontSize;
        }

        return smallFontSize;
    }
}