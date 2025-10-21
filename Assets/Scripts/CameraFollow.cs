using UnityEngine;

/// <summary>
/// Камера плавно следует за игроком, удерживая его по центру.
/// Можно настраивать скорость слежения.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;    // Игрок
    public float smoothSpeed = 5f; // Скорость следования

    private Vector3 offset;     // Смещение (если нужно)

    void Start()
    {
        if (target != null)
            offset = transform.position - target.position;
    }

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 0.15f);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
