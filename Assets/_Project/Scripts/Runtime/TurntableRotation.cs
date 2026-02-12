using UnityEngine;

public class TurntableRotation : MonoBehaviour
{
    public Transform target;
    public float rotateSpeed = 180f;
    bool dragging;

    void Update()
    {
        if (target == null) return;

        if (Input.GetMouseButtonDown(0)) dragging = true;
        if (Input.GetMouseButtonUp(0)) dragging = false;

        if (dragging)
        {
            float dx = Input.GetAxis("Mouse X");
            target.Rotate(0f, -dx * rotateSpeed * Time.deltaTime, 0f, Space.World);
        }
    }
}
