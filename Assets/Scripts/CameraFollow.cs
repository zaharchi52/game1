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

    void LateUpdate()
    {
        if (target == null) return;

        // ����� ������� ������ (� ������ ��������)
        Vector3 desiredPosition = target.position + offset;

        // ������� �������� (Lerp)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // ��������� ��������� ������
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
