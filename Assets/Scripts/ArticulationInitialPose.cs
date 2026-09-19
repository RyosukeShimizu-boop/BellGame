using UnityEngine;

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

    private void Start()
    {
        SetJointAngle(firstPendulum, firstAngle);
        SetJointAngle(secondPendulum, secondRelativeAngle);
    }

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

        ArticulationReducedSpace jointPosition =
            body.jointPosition;

        jointPosition[0] =
            angleDegrees * Mathf.Deg2Rad;

        body.jointPosition = jointPosition;
    }
}
