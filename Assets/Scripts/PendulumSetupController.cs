using UnityEngine;

public class PendulumSetupController : MonoBehaviour
{
    private enum DragTarget
    {
        None,
        FirstJoint,
        Hammer
    }

    [Header("Camera")]
    [SerializeField]
    private Camera mainCamera;

    [Header("Physics Bodies")]
    [SerializeField]
    private ArticulationBody firstPendulum;

    [SerializeField]
    private ArticulationBody secondPendulum;

    [Header("visuals")]
    [SerializeField]
    private Transform firstVisual;

    [SerializeField]
    private Transform secondVisual;

    [Header("Physical Shapes")]
    [SerializeField]
    private BoxCollider firstCollider;

    [SerializeField]
    private BoxCollider secondCollider;

    [Header("Drag Handles")]
    [SerializeField]
    private Transform firstJointHandle;

    [SerializeField]
    private Transform hammerHandle;

    [Header("Length Limits")]
    [SerializeField]
    private float minimumFirstLength = 0.5f;

    [SerializeField]
    private float minimumSecondLength = 0.5f;

    [Header("Rod Thickness")]
    [SerializeField]
    private float firstThickness = 0.2f;

    [SerializeField]
    private float secondThickness = 0.2f;

    [Header("Click Detection")]
    [SerializeField]
    private float handleRadius = 0.5f;

    private DragTarget currentDragTarget;
    private bool setupMode = true;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // �Z�b�e�B���O���͕����^�����~�߂�
        firstPendulum.useGravity = false;
        secondPendulum.useGravity = false;

        StopPhysics();
    }

    private void Update()
    {
        if (!setupMode)
        {
            return;
        }

        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        if (Input.GetMouseButtonDown(0))
        {
            SelectDragTarget(mouseWorldPosition);
        }

        if (Input.GetMouseButton(0))
        {
            UpdateDraggedPart(mouseWorldPosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            currentDragTarget = DragTarget.None;
        }
    }

    private void SelectDragTarget(Vector3 mouseWorldPosition)
    {
        float firstJointDistance = Vector2.Distance(
            mouseWorldPosition,
            firstJointHandle.position
        );

        float hammerDistance = Vector2.Distance(
            mouseWorldPosition,
            hammerHandle.position
        );

        if (hammerDistance <= handleRadius)
        {
            currentDragTarget = DragTarget.Hammer;
            return;
        }

        if (firstJointDistance <= handleRadius)
        {
            currentDragTarget = DragTarget.FirstJoint;
            return;
        }

        currentDragTarget = DragTarget.None;
    }

    private void UpdateDraggedPart(Vector3 mouseWorldPosition)
    {
        switch (currentDragTarget)
        {
            case DragTarget.FirstJoint:
                UpdateFirstPendulum(mouseWorldPosition);
                break;

            case DragTarget.Hammer:
                UpdateSecondPendulum(mouseWorldPosition);
                break;
        }
    }

    private void UpdateFirstPendulum(Vector3 mouseWorldPosition)
    {
        Vector3 rootPosition = transform.position;

        Vector2 direction = new Vector2(
            mouseWorldPosition.x - rootPosition.x,
            mouseWorldPosition.y - rootPosition.y
        );

        float length = Mathf.Max(
            direction.magnitude,
            minimumFirstLength
        );

        Vector2 normalizedDirection = direction.normalized;

        Vector3 jointWorldPosition = new Vector3(
            rootPosition.x + normalizedDirection.x * length,
            rootPosition.y + normalizedDirection.y * length,
            rootPosition.z
        );

        // 1st�̃��[���h�p�x
        float worldAngle =
            Mathf.Atan2(
                normalizedDirection.y,
                normalizedDirection.x
            ) * Mathf.Rad2Deg + 90.0f;

        ApplyFirstLength(length);

        SetJointAngle(
            firstPendulum,
            worldAngle
        );

        StopPhysics();
    }

    private void UpdateSecondPendulum(
        Vector3 mouseWorldPosition)
    {
        Vector3 jointPosition =
            secondPendulum.transform.position;

        Vector2 direction = new Vector2(
            mouseWorldPosition.x - jointPosition.x,
            mouseWorldPosition.y - jointPosition.y
        );

        float length = Mathf.Max(
            direction.magnitude,
            minimumSecondLength
        );


        Vector2 normalizedDirection = direction.normalized;

        Vector3 hammerWorldPosition = new Vector3(
            jointPosition.x + normalizedDirection.x * length,
            jointPosition.y + normalizedDirection.y * length,
            jointPosition.z
        );

        float secondWorldAngle =
            Mathf.Atan2(
                normalizedDirection.y,
                normalizedDirection.x
            ) * Mathf.Rad2Deg + 90.0f;

        float firstWorldAngle =
            firstPendulum.transform.eulerAngles.z;

        float relativeAngle =
            Mathf.DeltaAngle(
                firstWorldAngle,
                secondWorldAngle
            );

        ApplySecondLength(length);

        hammerHandle.position =
            hammerWorldPosition;

        SetJointAngle(
            secondPendulum,
            relativeAngle
        );

        StopPhysics();
    }

    private void ApplyFirstLength(float length)
    {
        // 1stの見た目
        firstVisual.localPosition =
            new Vector3(0.0f, -length * 0.5f, 0.0f);

        firstVisual.localScale =
            new Vector3(
                firstThickness,
                length,
                firstThickness
            );

        // 1stの物理形状
        firstCollider.center =
            new Vector3(0.0f, -length * 0.5f, 0.0f);

        firstCollider.size =
            new Vector3(
                firstThickness,
                length,
                firstThickness
            );

        /*
         * 2ndPendulumのTransformは動かさない。
         * 1st側にある第2関節の位置を変更する。
         */
        secondPendulum.matchAnchors = false;

        secondPendulum.anchorPosition =
            Vector3.zero;

        secondPendulum.parentAnchorPosition =
            new Vector3(0.0f, -length, 0.0f);

        // 親側と子側の関節軸を統一
        secondPendulum.anchorRotation =
            Quaternion.Euler(0.0f, 90.0f, 0.0f);

        secondPendulum.parentAnchorRotation =
            Quaternion.Euler(0.0f, 90.0f, 0.0f);
    }

    private void ApplySecondLength(float length)
    {
        secondVisual.localPosition =
            new Vector3(
                0.0f,
                -length * 0.5f,
                0.0f
            );

        secondVisual.localScale =
            new Vector3(
                secondThickness,
                length,
                secondThickness
            );

        secondCollider.center =
            new Vector3(
                0.0f,
                -length * 0.5f,
                0.0f
            );

        secondCollider.size =
            new Vector3(
                secondThickness,
                length,
                secondThickness
            );

        hammerHandle.localPosition =
            new Vector3(
                0.0f,
                -length,
                0.0f
            );
    }

    private void SetJointAngle(
    ArticulationBody body,
    float angleDegrees)
    {
        if (body == null || body.dofCount == 0)
        {
            return;
        }

        ArticulationReducedSpace jointPosition =
            body.jointPosition;

        // Anchor Rotation Y = 90では、
        // 画面上の角度とArticulationの回転方向が逆になる
        jointPosition[0] =
            -angleDegrees * Mathf.Deg2Rad;

        body.jointPosition = jointPosition;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Plane movementPlane =
            new Plane(Vector3.forward, transform.position);

        Ray ray = mainCamera.ScreenPointToRay(
            Input.mousePosition
        );

        if (movementPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPosition =
                ray.GetPoint(distance);

            worldPosition.z = transform.position.z;

            return worldPosition;
        }

        return transform.position;
    }

    private void StopPhysics()
    {
        firstPendulum.linearVelocity = Vector3.zero;
        firstPendulum.angularVelocity = Vector3.zero;

        secondPendulum.linearVelocity = Vector3.zero;
        secondPendulum.angularVelocity = Vector3.zero;
    }

    public void StartSimulation()
    {
        setupMode = false;
        currentDragTarget = DragTarget.None;

        StopPhysics();

        firstPendulum.useGravity = true;
        secondPendulum.useGravity = true;
    }
}