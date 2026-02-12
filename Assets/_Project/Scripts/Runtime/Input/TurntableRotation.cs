using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TurntableRotation : MonoBehaviour
{
    public Transform target;
    public float rotateSpeed = 180f;

    bool dragging;
    Vector2 lastPos;

    void Update()
    {
        if (target == null) return;
        if (Mouse.current == null) return;

        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (overUI)
            {
                dragging = false;
                return;
            }

            dragging = true;
            lastPos = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
            dragging = false;

        if (dragging && !overUI)
        {
            Vector2 pos = Mouse.current.position.ReadValue();
            float dx = pos.x - lastPos.x;
            lastPos = pos;

            target.Rotate(0f, -dx * rotateSpeed * Time.deltaTime, 0f, Space.World);
        }
    }
}
