using UnityEngine;
using UnityEngine.EventSystems;

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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (controller == null)
        {
            return;
        }

        if (moveDirection == MoveDirection.Left)
        {
            controller.BeginMoveLeft();
        }
        else
        {
            controller.BeginMoveRight();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopMovement();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopMovement();
    }

    private void OnDisable()
    {
        StopMovement();
    }

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