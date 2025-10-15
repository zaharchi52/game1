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

    void LateUpdate()
    {
        if (target == null) return;

        // Новая позиция камеры (с учётом смещения)
        Vector3 desiredPosition = target.position + offset;

        // Плавное движение (Lerp)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Фиксируем положение камеры
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
