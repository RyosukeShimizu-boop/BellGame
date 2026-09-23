using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Damage Range")]
    [SerializeField]
    private int minimumDamage = 1;

    [SerializeField]
    private int maximumDamage = 32;

    [Header("Shake Strength")]
    [SerializeField]
    private float minimumStrength = 0.02f;

    [SerializeField]
    private float maximumStrength = 0.35f;

    [Header("Shake Duration")]
    [SerializeField]
    private float minimumDuration = 0.08f;

    [SerializeField]
    private float maximumDuration = 0.35f;

    [Header("Shake Feel")]
    [SerializeField]
    private float frequency = 35.0f;

    private Vector3 originalLocalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    public void ShakeByDamage(int damage)
    {
        float damageRate = Mathf.InverseLerp(
            minimumDamage,
            maximumDamage,
            damage
        );

        // 小ダメージを控えめにし、大ダメージを強調
        damageRate = Mathf.Pow(
            damageRate,
            1.4f
        );

        float strength = Mathf.Lerp(
            minimumStrength,
            maximumStrength,
            damageRate
        );

        float duration = Mathf.Lerp(
            minimumDuration,
            maximumDuration,
            damageRate
        );

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        // 以前の揺れによる位置ずれをリセット
        transform.localPosition =
            originalLocalPosition;

        shakeCoroutine = StartCoroutine(
            ShakeCoroutine(
                duration,
                strength
            )
        );
    }

    private IEnumerator ShakeCoroutine(
        float duration,
        float strength)
    {
        float elapsedTime = 0.0f;

        // 毎回異なるノイズ位置から開始
        float seedX = Random.Range(
            0.0f,
            1000.0f
        );

        float seedY = Random.Range(
            0.0f,
            1000.0f
        );

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / duration
            );

            // 終了へ近づくほど揺れを弱くする
            float fade = 1.0f - progress;
            fade *= fade;

            float noiseTime =
                elapsedTime * frequency;

            float offsetX =
                Mathf.PerlinNoise(
                    seedX + noiseTime,
                    0.0f
                ) * 2.0f - 1.0f;

            float offsetY =
                Mathf.PerlinNoise(
                    0.0f,
                    seedY + noiseTime
                ) * 2.0f - 1.0f;

            Vector3 offset = new Vector3(
                offsetX,
                offsetY,
                0.0f
            );

            transform.localPosition =
                originalLocalPosition +
                offset * strength * fade;

            yield return null;
        }

        transform.localPosition =
            originalLocalPosition;

        shakeCoroutine = null;
    }

    public void ResetCameraPosition()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }

        transform.localPosition =
            originalLocalPosition;
    }
}