using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // ตัวละคร
    public float smoothSpeed = 5f; // ความลื่นไหล
    public Vector3 offset;         // ระยะห่าง

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}