using TMPro;
using UnityEngine;

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

    public void Initialize(int damage)
    {
        damageText.text = $"{damage}煩悩";

        elapsedTime = 0.0f;

        startPosition =
            rectTransform.anchoredPosition;

        damageText.fontSize =
            GetFontSize(damage);

        rectTransform.localScale =
            Vector3.one * startScale;

        Color color = originalColor;
        color.a = 1.0f;
        damageText.color = color;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

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