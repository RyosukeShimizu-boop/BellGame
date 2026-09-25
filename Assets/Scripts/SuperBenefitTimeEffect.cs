using System.Collections;
using TMPro;
using UnityEngine;
// ======================================= //
// SuperBenefitTimeEffect.cs
// スーパーご利益タイム演出処理
// ======================================= //

public class SuperBenefitEffect : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TMP_Text messageText;

    [Header("Text Position")]
    [SerializeField]
    private float startPositionX = 1200.0f;

    [SerializeField]
    private float centerPositionX = 0.0f;

    [SerializeField]
    private float endPositionX = -1200.0f;

    [Header("Animation Time")]
    [SerializeField]
    private float enterDuration = 0.25f;

    [SerializeField]
    private float waitDuration = 0.5f;

    [SerializeField]
    private float exitDuration = 0.25f;

    [Header("Slow Motion")]
    [SerializeField]
    [Range(0.01f, 1.0f)]
    private float slowTimeScale = 0.2f;

    [SerializeField]
    private float slowDuration = 1.0f;

    private RectTransform messageRectTransform;
    private Coroutine playCoroutine;

    private float originalTimeScale;
    private float originalFixedDeltaTime;

    // ゲーム開始時処理 //
    private void Awake()
    {
        originalTimeScale = 1.0f;
        originalFixedDeltaTime = Time.fixedDeltaTime;

        if (messageText != null)
        {
            messageRectTransform =
                messageText.GetComponent<RectTransform>();

            messageText.gameObject.SetActive(false);
        }
    }

    // 演出開始処理 //
    public void PlayEffect()
    {
        if (messageText == null)
        {
            return;
        }

        if (messageRectTransform == null)
        {
            messageRectTransform =
                messageText.GetComponent<RectTransform>();
        }

        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
        }

        RestoreTimeScale();

        playCoroutine =
            StartCoroutine(PlayEffectRoutine());
    }

    // 「スーパーご利益タイム」開始時処理 //
    private IEnumerator PlayEffectRoutine()
    {
        messageText.text =
            "スーパーご利益タイム";

        messageText.gameObject.SetActive(true);

        SetTextPositionX(startPositionX);

        StartSlowMotion();

        float effectStartTime =
            Time.realtimeSinceStartup;

        // 右側から中央へ移動
        yield return MoveTextRoutine(
            startPositionX,
            centerPositionX,
            enterDuration
        );

        // 中央で停止
        yield return new WaitForSecondsRealtime(
            waitDuration
        );

        // 中央から左側へ移動
        yield return MoveTextRoutine(
            centerPositionX,
            endPositionX,
            exitDuration
        );

        messageText.gameObject.SetActive(false);

        /*
         * 文字演出が1秒未満で終了した場合でも、
         * slowDurationまではスローを維持する。
         */
        float elapsedRealTime =
            Time.realtimeSinceStartup -
            effectStartTime;

        float remainingSlowTime =
            slowDuration - elapsedRealTime;

        if (remainingSlowTime > 0.0f)
        {
            yield return new WaitForSecondsRealtime(
                remainingSlowTime
            );
        }

        RestoreTimeScale();

        playCoroutine = null;
    }

    // 文字を移動させる処理 //
    private IEnumerator MoveTextRoutine(
        float fromX,
        float toX,
        float duration)
    {
        if (duration <= 0.0f)
        {
            SetTextPositionX(toX);
            yield break;
        }

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            // ゲーム速度を無視した本当の経過時間
            elapsedTime +=
                Time.unscaledDeltaTime;

            // 進行率
            float rate = Mathf.Clamp01(
                elapsedTime / duration
            );

            // 文字の移動する様子をスロー→普通→スローにする
            float smoothRate =
                Mathf.SmoothStep(
                    0.0f,
                    1.0f,
                    rate
                );

            // 現在位置計算
            float currentX = Mathf.Lerp(
                fromX,
                toX,
                smoothRate
            );

            SetTextPositionX(currentX);

            yield return null;
        }

        SetTextPositionX(toX);
    }

    // テキストのポジションを変更する処理 //
    private void SetTextPositionX(float positionX)
    {
        if (messageRectTransform == null)
        {
            return;
        }

        Vector2 position =
            messageRectTransform.anchoredPosition;

        position.x = positionX;

        messageRectTransform.anchoredPosition =
            position;
    }

    // スローにする処理 //
    private void StartSlowMotion()
    {
        Time.timeScale = slowTimeScale;

        Time.fixedDeltaTime =
            originalFixedDeltaTime *
            slowTimeScale;
    }

    // スロー解除 //
    private void RestoreTimeScale()
    {
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime =
            originalFixedDeltaTime;
    }

    // 演出終了(STOPボタンを押した時)
    public void ResetEffect()
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }

        RestoreTimeScale();

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        SetTextPositionX(startPositionX);
    }

    // スロー状態を残さないように //
    private void OnDisable()
    {
        /*
         * GameManager自体を非表示にする構成なら、
         * スロー状態が残らないように戻す。
         */
        RestoreTimeScale();
    }

    // スロー状態を残さないように //
    private void OnDestroy()
    {
        RestoreTimeScale();
    }
}