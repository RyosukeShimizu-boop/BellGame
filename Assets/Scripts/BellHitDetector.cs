using UnityEngine;

public class BellHitDetector : MonoBehaviour
{
    [SerializeField]
    private PendulumSpeed pendulumSpeed;

    [SerializeField]
    private float minimumHitSpeed = 1.0f;

    [SerializeField]
    private float hitCooldown = 0.2f;

    private float cooldownTimer;
    private bool isInsideBell;

    private void FixedUpdate()
    {
        if (cooldownTimer > 0.0f)
        {
            cooldownTimer -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger Enter: {other.name}");

        if (!other.CompareTag("Bell"))
        {
            return;
        }

        isInsideBell = true;

        if (cooldownTimer > 0.0f)
        {
            Debug.Log("鐘に入ったがクールタイム中");
            return;
        }

        float hitSpeed = pendulumSpeed.Speed;

        if (hitSpeed < minimumHitSpeed)
        {
            Debug.Log(
                $"弱い接触のため無効。先端速度: {hitSpeed:F2}"
            );
            return;
        }

        cooldownTimer = hitCooldown;

        Debug.Log(
            $"有効な鐘ヒット！先端速度: {hitSpeed:F2}"
        );
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Trigger Exit: {other.name}");

        if (!other.CompareTag("Bell"))
        {
            return;
        }

        isInsideBell = false;

        Debug.Log("HitPointが鐘の判定範囲から退出");
    }
}