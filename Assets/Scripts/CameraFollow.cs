using UnityEngine;

/// <summary>
/// ������ ������ ������� �� �������, ��������� ��� �� ������.
/// ����� ����������� �������� ��������.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;    // �����
    public float smoothSpeed = 5f; // �������� ����������

    private Vector3 offset;     // �������� (���� �����)

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
