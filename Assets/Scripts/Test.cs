using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            $"Bell Trigger Enter: {other.name}"
        );
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(
            $"Bell Trigger Stay: {other.name}"
        );
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(
            $"Bell Trigger Exit: {other.name}"
        );
    }
}
