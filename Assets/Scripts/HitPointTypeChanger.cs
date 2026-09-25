using UnityEngine;
// ======================================= //
// HitPointTypeChanger.cs
// HitPointのハンマー表示切替処理
// ======================================= //


public class HitPointTypeChanger : MonoBehaviour
{
    [SerializeField]
    private GameObject hammerVisual;

    [SerializeField]
    private GameObject woodVisual;

    // 鉄製ハンマー表示処理 //
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

    // 木槌表示処理
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