using UnityEngine;
// ======================================= //
// CraneBoomController.cs
// 実際にクレーンのアームを動かす処理
// ======================================= //


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

    // ゲーム開始時処理 //
    private void Start()
    {
        // 現在位置から初期値を計算
        InitializeFromCurrentPosition();

        // アーム表示を更新
        UpdateBoomVisual(
            cranePivot.position,
            GetPendulumConnectionPosition()
        );
    }

    // 更新処理 //
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

        // 角度を更新
        boomAngle +=
            direction * angleSpeed * Time.deltaTime;

        // 角度の最小値と最大値の範囲内に制限
        boomAngle = Mathf.Clamp(
            boomAngle,
            minimumAngle,
            maximumAngle
        );

        // 位置を更新
        UpdateBoomPosition();
    }

    // 振り子の根本を移動させる処理 //
    private void UpdateBoomPosition()
    {
        if (cranePivot == null ||
            pendulumRoot == null)
        {
            return;
        }

        // 度からラジアンへ変換
        float angleRadians =
            boomAngle * Mathf.Deg2Rad;

        // 回転中心の位置を取得
        Vector3 pivotPosition =
            cranePivot.position;

        // PendulumRootの座標計算
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

    // クレーンアームの見た目を更新する処理 //
    private void UpdateBoomVisual(
    Vector3 pivotPosition,
    Vector3 rootPosition)
    {
        if (boomVisual == null)
        {
            return;
        }

        // クレーンの根元から、振り子との接続位置へ向かうベクトル
        Vector3 difference =
            rootPosition - pivotPosition;

        // アームの長さ
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

    // 左へ動かす
    public void BeginMoveLeft()
    {
        moveLeft = true;
    }

    // 左移動を終了
    public void EndMoveLeft()
    {
        moveLeft = false;
    }

    // 右へ動かす
    public void BeginMoveRight()
    {
        moveRight = true;
    }

    // 右移動を終了
    public void EndMoveRight()
    {
        moveRight = false;
    }

    // STARTボタンを押したときの処理
    public void StopSetup()
    {
        setupMode = false;
        moveLeft = false;
        moveRight = false;
    }

    // STOPボタンを押したときの処理
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
