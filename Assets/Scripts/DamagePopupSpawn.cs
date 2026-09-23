using UnityEngine;

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

    public void ShowDamage(
        int damage,
        Vector3 worldPosition)
    {
        if (damagePopupPrefab == null ||
            targetCanvas == null ||
            mainCamera == null)
        {
            Debug.LogError(
                "DamagePopupÇÃéQè∆Ç™ê›íËÇ≥ÇÍÇƒÇ¢Ç‹ÇπÇÒÅB"
            );

            return;
        }

        Vector3 screenPosition =
            mainCamera.WorldToScreenPoint(
                worldPosition
            );

        // ÉJÉÅÉâÇÃå„ÇÎÇ»ÇÁê∂ê¨ÇµÇ»Ç¢
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

        RectTransformUtility
            .ScreenPointToLocalPointInRectangle(
                canvasRectTransform,
                screenPosition,
                canvasCamera,
                out Vector2 localPosition
            );

        DamagePopup popup = Instantiate(
            damagePopupPrefab,
            targetCanvas.transform
        );

        RectTransform popupRect =
            popup.GetComponent<RectTransform>();

        popupRect.anchoredPosition =
            localPosition + screenOffset;

        popupRect.localScale =
            Vector3.one;

        popup.Initialize(damage);
    }
}