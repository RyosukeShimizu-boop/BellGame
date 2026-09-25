using UnityEngine;
// ======================================= //
// ArticulationInitialPose.cs
// 各振り子の初期位置を確定する処理
// 各振り子の角度と長さを変更する処理	
// ======================================= //

public class ArticulationInitialPose : MonoBehaviour
{
    [Header("振り子")]
    [SerializeField]
    private ArticulationBody firstPendulum;

    [SerializeField]
    private ArticulationBody secondPendulum;

    [Header("初期角度")]
    [SerializeField]
    private float firstAngle = 120.0f;

    [SerializeField]
    private float secondRelativeAngle = -150.0f;

    // ゲーム開始時に初期位置を確定 //
    private void Start()
    {
        SetJointAngle(firstPendulum, firstAngle);
        SetJointAngle(secondPendulum, secondRelativeAngle);
    }

    // 各振り子の角度と位置を変更する //
    private void SetJointAngle(
        ArticulationBody body,
        float angleDegrees)
    {
        if (body == null)
        {
            Debug.LogError("ArticulationBodyが設定されていません。");
            return;
        }

        if (body.dofCount == 0)
        {
            Debug.LogError(
                $"{body.name}の回転自由度がありません。"
            );
            return;
        }

        // 現在の角度を取得
        ArticulationReducedSpace jointPosition =
            body.jointPosition;

        // 度→ラジアン変換
        jointPosition[0] =
            angleDegrees * Mathf.Deg2Rad;

        // 実際に反映
        body.jointPosition = jointPosition;
    }
}
