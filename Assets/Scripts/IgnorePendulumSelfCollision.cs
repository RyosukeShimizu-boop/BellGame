using UnityEngine;
// ======================================= //
// IgnorePendulumSelfCollision.cs
// U‚èq“¯m‚ª‚Ô‚Â‚©‚ç‚È‚¢‚æ‚¤‚É‚·‚éˆ—
// ======================================= //

public class IgnorePendulumSelfCollision : MonoBehaviour
{
    [SerializeField]
    private Collider firstPendulumCollider;

    [SerializeField]
    private Collider secondPendulumCollider;

    [SerializeField]
    private Collider hitPointCollider;

    // ƒQ[ƒ€ŠJnˆ— //
    private void Awake()
    {
        if (firstPendulumCollider != null &&
            secondPendulumCollider != null)
        {
            Physics.IgnoreCollision(
                firstPendulumCollider,
                secondPendulumCollider,
                true
            );
        }

        if (secondPendulumCollider != null &&
            hitPointCollider != null)
        {
            Physics.IgnoreCollision(
                secondPendulumCollider,
                hitPointCollider,
                true
            );
        }

        if (firstPendulumCollider != null &&
            hitPointCollider != null)
        {
            Physics.IgnoreCollision(
                firstPendulumCollider,
                hitPointCollider,
                true
            );
        }
    }
}