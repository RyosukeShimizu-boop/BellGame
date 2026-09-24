using UnityEngine;

public class HitPointTypeChanger : MonoBehaviour
{
    [SerializeField]
    private GameObject hammerVisual;

    [SerializeField]
    private GameObject woodVisual;

    public void SetHammer()
    {
        if (hammerVisual != null)
        {
            hammerVisual.SetActive(true);
        }

        if (woodVisual != null)
        {
            woodVisual.SetActive(false);
        }
    }

    public void SetWood()
    {
        if (hammerVisual != null)
        {
            hammerVisual.SetActive(false);
        }

        if (woodVisual != null)
        {
            woodVisual.SetActive(true);
        }
    }
}