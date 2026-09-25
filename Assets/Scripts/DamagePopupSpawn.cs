using UnityEngine;
// ======================================= //
// DamagePopupSpawner.cs
// ダメージ表示を生成する処理
// ======================================= //

public class DamagePopupSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private DamagePopup damagePopupPrefab;

    [SerializeField]
    private Canvas targetCanvas;

    [SerializeField]
    private Camera mainCamera;

    [Header("Position")]
    [SerializeField]
    private Vector2 screenOffset =
        new Vector2(0.0f, 50.0f);

    private RectTransform canvasRectTransform;

    // ゲーム開始時処理
    private void Awake()
    {
        if (targetCanvas == null)
        {
            targetCanvas =
                GetComponent<Canvas>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        canvasRectTransform =
            targetCanvas.GetComponent<RectTransform>();
    }

    // ダメージ表記を出現させる処理 //
    public void ShowDamage(
        int damage,
        Vector3 worldPosition)
    {
        if (damagePopupPrefab == null ||
            targetCanvas == null ||
            mainCamera == null)
        {
            Debug.LogError(
                "DamagePopupの参照が設定されていません。"
            );

            return;
        }

        // Screen座標変換
        Vector3 screenPosition =
            mainCamera.WorldToScreenPoint(
                worldPosition
            );

        // カメラの後ろなら生成しない
        if (screenPosition.z < 0.0f)
        {
            return;
        }

        Camera canvasCamera = null;

        if (targetCanvas.renderMode !=
            RenderMode.ScreenSpaceOverlay)
        {
            canvasCamera =
                targetCanvas.worldCamera;
        }

        // UI座標変換
        RectTransformUtility
            .ScreenPointToLocalPointInRectangle(
                canvasRectTransform,
                screenPosition,
                canvasCamera,
                out Vector2 localPosition
            );

        // ダメージ表示出現処理
        DamagePopup popup = Instantiate(
            damagePopupPrefab,
            targetCanvas.transform
        );
        
        // 出現位置獲得
        RectTransform popupRect =
            popup.GetComponent<RectTransform>();

        // 表示位置設定
        popupRect.anchoredPosition =
            localPosition + screenOffset;

        // スケール初期化
        popupRect.localScale =
            Vector3.one;

        // 出現処理
        popup.Initialize(damage);
    }
}