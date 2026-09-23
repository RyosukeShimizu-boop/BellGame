using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class HitPointTrailController : MonoBehaviour
{
    private TrailRenderer trailRenderer;

    private void Awake()
    {
        trailRenderer =
            GetComponent<TrailRenderer>();

        DisableTrail();
    }

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