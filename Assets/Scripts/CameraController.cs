using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    public float targetOrthographicSize = 0.28f; // projection size ��� �������
    public float zoomSpeed = 2f;                  // �������� ����

    private Vector3 targetPosition;
    private bool isZooming = false;

    public void ZoomToRoom(Vector3 roomPosition)
    {
        targetPosition = new Vector3(roomPosition.x, roomPosition.y, mainCamera.transform.position.z);
        isZooming = true;
    }

    private void Update()
    {
        if (isZooming)
        {
            // ������� ����������� ������ � ������ �������
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * zoomSpeed);

            // ������� ���������� projection size
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetOrthographicSize, Time.deltaTime * zoomSpeed);

            // ���� ����� �������� ���� � �������������
            if (Vector3.Distance(mainCamera.transform.position, targetPosition) < 0.01f &&
                Mathf.Abs(mainCamera.orthographicSize - targetOrthographicSize) < 0.01f)
            {
                mainCamera.transform.position = targetPosition;
                mainCamera.orthographicSize = targetOrthographicSize;
                isZooming = false;
            }
        }
    }
}
