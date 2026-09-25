using UnityEngine;
using UnityEngine.EventSystems;
// ======================================= //
// PendulumSetUpController.cs
// 二重振り子全般処理
// ======================================= //

public class PendulumSetupController : MonoBehaviour
{
    private enum DragTarget
    {
        None,
        FirstJoint,
        Hammer
    }

    [Header("Initial Setup Angle")]
    [SerializeField]
    private float secondInitialAngle = 90.0f;

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

    [Header("Start And Stop Buttons")]
    [SerializeField]
    private GameObject startButton;

    [SerializeField]
    private GameObject stopButton;

    [Header("UI")]
    [SerializeField]
    private GameObject leftButton;

    [SerializeField]
    private GameObject rightButton;

    [Header("Mass By Length")]
    [SerializeField]
    private float firstMassPerUnit = 0.5f;

    [SerializeField]
    private float secondMassPerUnit = 0.5f;

    [SerializeField]
    private float minimumMass = 0.2f;

    [SerializeField]
    private float maximumMass = 10.0f;

    [Header("Joint Visual")]
    [SerializeField]
    private Transform jointVisual;

    [SerializeField]
    private float jointVisualOffsetX = 0.0f;

    [SerializeField]
    private float jointVisualOffsetY = 0.0f;

    [SerializeField]
    private float jointVisualPositionZ = -0.5f;

    [SerializeField]
    private GameObject hammerButton;

    [SerializeField]
    private GameObject woodButton;

    private DragTarget currentDragTarget;
    private bool setupMode = true;

    private Vector3 savedRootPosition;
    private Quaternion savedRootRotation;

    private float savedFirstJointAngle;
    private float savedSecondJointAngle;

    private Vector3 savedFirstVisualPosition;
    private Vector3 savedFirstVisualScale;
    private Vector3 savedFirstColliderCenter;
    private Vector3 savedFirstColliderSize;

    private Vector3 savedSecondVisualPosition;
    private Vector3 savedSecondVisualScale;
    private Vector3 savedSecondColliderCenter;
    private Vector3 savedSecondColliderSize;

    private Vector3 savedSecondParentAnchorPosition;
    private Vector3 savedHitPointPosition;

    private bool hasSavedSetup;

    private float savedFirstMass;
    private float savedSecondMass;

    // ゲーム開始時処理 //
    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        SetJointAngle(
            secondPendulum,
            secondInitialAngle
        );

        firstPendulum.useGravity = false;
        secondPendulum.useGravity = false;

        StopPhysics();

        if (startButton != null)
        {
            startButton.SetActive(true);
        }

        if (stopButton != null)
        {
            stopButton.SetActive(false);
        }
    }

    // 更新処理 //
    private void Update()
    {
        if (!setupMode)
        {
            return;
        }

        // UIを押している時は
        // 振り子操作を無効化
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Vector3 mouseWorldPosition =
            GetMouseWorldPosition();

        if (Input.GetMouseButtonDown(0))
        {
            SelectDragTarget(
                mouseWorldPosition
            );
        }

        if (Input.GetMouseButton(0))
        {
            UpdateDraggedPart(
                mouseWorldPosition
            );
        }

        if (Input.GetMouseButtonUp(0))
        {
            currentDragTarget =
                DragTarget.None;
        }
    }

    // ドラッグ対象決定 //
    private void SelectDragTarget(Vector3 mouseWorldPosition)
    {
        // クリック位置と対象位置を計算
        float firstJointDistance = Vector2.Distance(
            mouseWorldPosition,
            firstJointHandle.position
        );

        float hammerDistance = Vector2.Distance(
            mouseWorldPosition,
            hammerHandle.position
        );

        // ドラッグ対象を決定
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

    // ドラッグ更新処理 //
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

    // 第一間接更新処理 //
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

    // 第一関節の長さを変更する処理 //
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

        if (jointVisual != null)
        {
            jointVisual.localPosition =
                new Vector3(
                    jointVisualOffsetX,
                    -length + jointVisualOffsetY,
                    jointVisualPositionZ
                );

            // 親の伸縮などが残っていても画像サイズを一定に保つ
            jointVisual.localScale =
                Vector3.one;
        }

        // 親側と子側の関節軸を統一
        secondPendulum.anchorRotation =
            Quaternion.Euler(0.0f, 90.0f, 0.0f);

        secondPendulum.parentAnchorRotation =
            Quaternion.Euler(0.0f, 90.0f, 0.0f);

        // 変更したColliderとMassから物理特性を再計算
        Physics.SyncTransforms();
        RefreshFirstPendulumPhysics();

    }

    // 第二関節の長さを変更する処理 //
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

        // 変更したColliderとMassから物理特性を再計算
        Physics.SyncTransforms();
        RefreshSecondPendulumPhysics();
    }

    // 角度設定 //
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

    // マウスの位置獲得 //
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

    // 物理演算終了 //
    private void StopPhysics()
    {
        firstPendulum.linearVelocity = Vector3.zero;
        firstPendulum.angularVelocity = Vector3.zero;

        secondPendulum.linearVelocity = Vector3.zero;
        secondPendulum.angularVelocity = Vector3.zero;
    }

    // 1stの重心と慣性再計算 //
    private void RefreshFirstPendulumPhysics()
    {
        if (firstPendulum == null)
        {
            return;
        }

        firstPendulum.ResetCenterOfMass();
        firstPendulum.ResetInertiaTensor();
    }

    // 2ndの重心と慣性再計算 //
    private void RefreshSecondPendulumPhysics()
    {
        if (secondPendulum == null)
        {
            return;
        }

        secondPendulum.ResetCenterOfMass();
        secondPendulum.ResetInertiaTensor();
    }

    // 重心と慣性再計算 //
    private void RefreshAllPendulumPhysics()
    {
        Physics.SyncTransforms();

        RefreshFirstPendulumPhysics();
        RefreshSecondPendulumPhysics();

        Physics.SyncTransforms();
    }

    // シミュレーション開始(STARTボタンを押した時) //
    public void StartSimulation()
    {
        if (!setupMode)
        {
            return;
        }

        SaveCurrentSetup();

        setupMode = false;
        currentDragTarget = DragTarget.None;

        StopPhysics();

        /*
         * START直前の最終的なCollider、Mass、
         * Anchorの状態を物理エンジンへ反映する。
         */
        RefreshAllPendulumPhysics();

        StopPhysics();

        firstPendulum.useGravity = true;
        secondPendulum.useGravity = true;

        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        if (stopButton != null)
        {
            stopButton.SetActive(true);
        }

        if (leftButton != null)
        {
            leftButton.SetActive(false);
        }

        if (rightButton != null)
        {
            rightButton.SetActive(false);
        }

        if (hammerButton != null)
        {
            hammerButton.SetActive(false);
        }

        if (woodButton != null)
        {
            woodButton.SetActive(false);
        }
    }

    // シミュレーション終了(STOPボタンを押した時) //
    public void StopSimulation()
    {
        // 先に重力と現在の速度を止める
        firstPendulum.useGravity = false;
        secondPendulum.useGravity = false;

        StopPhysics();

        // STARTを押した瞬間に保存した状態へ戻す
        RestoreSavedSetup();

        setupMode = true;
        currentDragTarget = DragTarget.None;

        if (startButton != null)
        {
            startButton.SetActive(true);
        }

        if (stopButton != null)
        {
            stopButton.SetActive(false);
        }

        if (leftButton != null)
        {
            leftButton.SetActive(true);
        }

        if (rightButton != null)
        {
            rightButton.SetActive(true);
        }

        if (hammerButton != null)
        {
            hammerButton.SetActive(true);
        }

        if (woodButton != null)
        {
            woodButton.SetActive(true);
        }
    }
    
    // シミュレーション開始時のPendulumRootの位置を保存(STARTボタンを押した時) //
    private void SaveCurrentSetup()
    {
        savedRootPosition =
            transform.position;

        savedRootRotation =
            transform.rotation;

        if (firstPendulum.dofCount > 0)
        {
            savedFirstJointAngle =
                firstPendulum.jointPosition[0];
        }

        if (secondPendulum.dofCount > 0)
        {
            savedSecondJointAngle =
                secondPendulum.jointPosition[0];
        }

        savedFirstVisualPosition =
            firstVisual.localPosition;

        savedFirstVisualScale =
            firstVisual.localScale;

        savedFirstColliderCenter =
            firstCollider.center;

        savedFirstColliderSize =
            firstCollider.size;

        savedSecondVisualPosition =
            secondVisual.localPosition;

        savedSecondVisualScale =
            secondVisual.localScale;

        savedSecondColliderCenter =
            secondCollider.center;

        savedSecondColliderSize =
            secondCollider.size;

        savedSecondParentAnchorPosition =
            secondPendulum.parentAnchorPosition;

        savedHitPointPosition =
            hammerHandle.localPosition;

        savedFirstMass =
            firstPendulum.mass;

        savedSecondMass =
            secondPendulum.mass;

        hasSavedSetup = true;
    }

    // 保存したPendulumRootの位置をロード(STOPボタンを押した時) //
    private void RestoreSavedSetup()
    {
        if (!hasSavedSetup)
        {
            return;
        }

        // 先に物理を完全停止
        firstPendulum.useGravity = false;
        secondPendulum.useGravity = false;

        StopPhysics();

        // ルートを開始前の位置へ戻す
        ArticulationBody rootBody =
            GetComponent<ArticulationBody>();

        rootBody.TeleportRoot(
            savedRootPosition,
            savedRootRotation
        );

        rootBody.linearVelocity =
            Vector3.zero;

        rootBody.angularVelocity =
            Vector3.zero;

        // 長さとVisual、Colliderを復元
        firstVisual.localPosition =
            savedFirstVisualPosition;

        firstVisual.localScale =
            savedFirstVisualScale;

        firstCollider.center =
            savedFirstColliderCenter;

        firstCollider.size =
            savedFirstColliderSize;

        secondVisual.localPosition =
            savedSecondVisualPosition;

        secondVisual.localScale =
            savedSecondVisualScale;

        secondCollider.center =
            savedSecondColliderCenter;

        secondCollider.size =
            savedSecondColliderSize;

        firstPendulum.mass =
            savedFirstMass;

        secondPendulum.mass =
            savedSecondMass;

        secondPendulum.matchAnchors = false;

        secondPendulum.anchorPosition =
            Vector3.zero;

        secondPendulum.parentAnchorPosition =
            savedSecondParentAnchorPosition;

        hammerHandle.localPosition =
            savedHitPointPosition;

        SetJointPositionRadians(
            firstPendulum,
            savedFirstJointAngle
        );

        SetJointPositionRadians(
            secondPendulum,
            savedSecondJointAngle
        );

        // 復元したCollider形状から物理特性を再計算
        RefreshAllPendulumPhysics();

        StopPhysics();
    }

    // 保存しておいた関節角度を復元する処理 //
    private void SetJointPositionRadians(
    ArticulationBody body,
    float angleRadians)
    {
        if (body == null || body.dofCount == 0)
        {
            return;
        }

        ArticulationReducedSpace position =
            body.jointPosition;

        position[0] = angleRadians;

        body.jointPosition = position;

        ArticulationReducedSpace velocity =
            body.jointVelocity;

        velocity[0] = 0.0f;

        body.jointVelocity = velocity;
    }

    // シミュレーションを止める処理(速度超過で折れた時) //
    public void FreezeSimulation()
    {
        setupMode = false;
        currentDragTarget = DragTarget.None;

        firstPendulum.useGravity = false;
        secondPendulum.useGravity = false;

        StopPhysics();
    }
}