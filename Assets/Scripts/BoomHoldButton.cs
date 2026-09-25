using UnityEngine;
using UnityEngine.EventSystems;
// ======================================= //
// BoomHoldButton.cs
// 移動ボタンを押している間、クレーンを移動させるスクリプト
// ======================================= //

public class BoomHoldButton :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    public enum MoveDirection
    {
        Left,
        Right
    }

    [Header("Controller")]
    [SerializeField]
    private CraneBoomController controller;

    [Header("Direction")]
    [SerializeField]
    private MoveDirection moveDirection;

    // ボタンを押した瞬間の処理
    public void OnPointerDown(PointerEventData eventData)
    {
        if (controller == null)
        {
            return;
        }

        // 左右判定
        if (moveDirection == MoveDirection.Left)
        {
            controller.BeginMoveLeft();
        }
        else
        {
            controller.BeginMoveRight();
        }
    }

    // ボタンを離した瞬間 //
    public void OnPointerUp(PointerEventData eventData)
    {
        StopMovement();
    }

    // ボタンを押したままマウスを画面外に移動させた場合 //
    public void OnPointerExit(PointerEventData eventData)
    {
        StopMovement();
    }

    // ボタンｇｓ非表示になった時 //
    private void OnDisable()
    {
        StopMovement();
    }

    // クレーンの動きを止める処理 //
    private void StopMovement()
    {
        if (controller == null)
        {
            return;
        }

        if (moveDirection == MoveDirection.Left)
        {
            controller.EndMoveLeft();
        }
        else
        {
            controller.EndMoveRight();
        }
    }
}