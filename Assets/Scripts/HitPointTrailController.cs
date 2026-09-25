using UnityEngine;
// ======================================= //
// HitPointTrailController.cs
// HitPoint‚Ì‹OÕ‚Ì•\¦‚ğØ‚è‘Ö‚¦‚·‚éˆ—
// ======================================= //

[RequireComponent(typeof(TrailRenderer))]
public class HitPointTrailController : MonoBehaviour
{
    private TrailRenderer trailRenderer;

    // ƒQ[ƒ€ŠJnˆ— //
    private void Awake()
    {
        trailRenderer =
            GetComponent<TrailRenderer>();

        DisableTrail();
    }

    // ‹OÕ‚ğ—LŒø‰» //
    public void EnableTrail()
    {
        if (trailRenderer == null)
        {
            return;
        }

        // ‘O‰ñ‚Ì‹OÕ‚ğíœ
        trailRenderer.Clear();

        // V‚µ‚¢‹OÕ‚Ì¶¬‚ğŠJn
        trailRenderer.emitting = true;
    }

    // ‹OÕ‚ğ–³Œø‰» //
    public void DisableTrail()
    {
        if (trailRenderer == null)
        {
            return;
        }

        // V‚µ‚¢‹OÕ‚Ì¶¬‚ğ’â~
        trailRenderer.emitting = false;

        // c‚Á‚Ä‚¢‚é‹OÕ‚ğíœ
        trailRenderer.Clear();
    }
}