using UnityEngine;

public class CraneBoomController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform cranePivot;

    [SerializeField]
    private ArticulationBody pendulumRoot;

    [SerializeField]
    private ArticulationBody firstPendulum;

    [Header("Boom Settings")]
    [SerializeField]
    private float boomRadius = 5.0f;

    [SerializeField]
    private float boomAngle = 60.0f;

    [SerializeField]
    private float minimumAngle = 20.0f;

    [SerializeField]
    private float maximumAngle = 85.0f;

    [SerializeField]
    private float angleSpeed = 30.0f;

    [Header("Optional Visual")]
    [SerializeField]
    private Transform boomVisual;

    private bool moveLeft;
    private bool moveRight;
    private bool setupMode = true;

    private void Start()
    {
        InitializeFromCurrentPosition();

        UpdateBoomVisual(
            cranePivot.position,
            GetPendulumConnectionPosition()
        );
    }

    private void Update()
    {
        if (!setupMode)
        {
            return;
        }

        float direction = 0.0f;

        if (moveLeft)
        {
            direction += 1.0f;
        }

        if (moveRight)
        {
            direction -= 1.0f;
        }

        if (Mathf.Approximately(direction, 0.0f))
        {
            return;
        }

        boomAngle +=
            direction * angleSpeed * Time.deltaTime;

        boomAngle = Mathf.Clamp(
            boomAngle,
            minimumAngle,
            maximumAngle
        );

        UpdateBoomPosition();
    }

    private void UpdateBoomPosition()
    {
        if (cranePivot == null ||
            pendulumRoot == null)
        {
            return;
        }

        float angleRadians =
            boomAngle * Mathf.Deg2Rad;

        Vector3 pivotPosition =
            cranePivot.position;

        Vector3 rootPosition = new Vector3(
            pivotPosition.x -
                Mathf.Sin(angleRadians) * boomRadius,

            pivotPosition.y +
                Mathf.Cos(angleRadians) * boomRadius,

            pendulumRoot.transform.position.z
        );

        /*
         * ArticulationBodyのルートは、
         * Transformを直接変更せずTeleportRootで移動する。
         */
        pendulumRoot.TeleportRoot(
            rootPosition,
            Quaternion.identity
        );

        pendulumRoot.linearVelocity =
            Vector3.zero;

        pendulumRoot.angularVelocity =
            Vector3.zero;

        UpdateBoomVisual(
            pivotPosition,
            GetPendulumConnectionPosition()
        );
    }

    private void UpdateBoomVisual(
    Vector3 pivotPosition,
    Vector3 rootPosition)
    {
        if (boomVisual == null)
        {
            return;
        }

        Vector3 difference =
            rootPosition - pivotPosition;

        float length = difference.magnitude;

        if (length < 0.001f)
        {
            return;
        }

        // Armの中心をCranePivotとPendulumRootの中間へ置く
        boomVisual.position =
            (pivotPosition + rootPosition) * 0.5f;

        // CranePivotからPendulumRootへ向ける
        float angle =
            Mathf.Atan2(
                difference.y,
                difference.x
            ) * Mathf.Rad2Deg;

        boomVisual.rotation =
            Quaternion.Euler(
                0.0f,
                0.0f,
                angle - 90.0f
            );

        // Xは太さ、Yは長さ
        boomVisual.localScale =
            new Vector3(
                0.15f,
                length,
                1.0f
            );
    }

    public void BeginMoveLeft()
    {
        moveLeft = true;
    }

    public void EndMoveLeft()
    {
        moveLeft = false;
    }

    public void BeginMoveRight()
    {
        moveRight = true;
    }

    public void EndMoveRight()
    {
        moveRight = false;
    }

    public void StopSetup()
    {
        setupMode = false;
        moveLeft = false;
        moveRight = false;
    }

    public void ResumeSetup()
    {
        setupMode = true;
        moveLeft = false;
        moveRight = false;
    }

    private void InitializeFromCurrentPosition()
    {
        if (cranePivot == null || pendulumRoot == null)
        {
            Debug.LogError(
                "CranePivotまたはPendulumRootが設定されていません。"
            );
            return;
        }

        Vector3 offset =
            pendulumRoot.transform.position -
            cranePivot.position;

        boomRadius = new Vector2(
            offset.x,
            offset.y
        ).magnitude;

        if (boomRadius < 0.001f)
        {
            Debug.LogError(
                "CranePivotとPendulumRootが同じ位置です。"
            );
            return;
        }

        /*
         * 使用している円弧の式
         *
         * X = PivotX - sin(angle) * radius
         * Y = PivotY + cos(angle) * radius
         *
         * に対応する逆算
         */
        boomAngle = Mathf.Atan2(
            -offset.x,
            offset.y
        ) * Mathf.Rad2Deg;

        /*
         * ここではClampしない。
         * UpdateBoomPositionも呼ばない。
         *
         * 起動時のPendulumRootの位置を
         * そのまま維持するため。
         */
    }

    private Vector3 GetPendulumConnectionPosition()
    {
        if (firstPendulum == null)
        {
            return pendulumRoot.transform.position;
        }

        /*
         * anchorPositionは1stPendulumのローカル座標なので、
         * TransformPointでワールド座標へ変換する。
         */
        return firstPendulum.transform.TransformPoint(
            firstPendulum.anchorPosition
        );
    }
}
