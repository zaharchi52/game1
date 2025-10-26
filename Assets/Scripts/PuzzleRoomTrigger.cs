using UnityEngine;

public class PuzzleRoomTrigger : MonoBehaviour
{
    private CameraFollow camFollow;
    private Camera cam;

    private void Awake()
    {
        camFollow = Camera.main.GetComponent<CameraFollow>();
        cam = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && camFollow != null)
        {
            // ��������� ���������� �� �������
            camFollow.SetFollow(false);

            // ���������� ������ �� �������
            cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);

            // ������ projection size
            if (cam.orthographic) cam.orthographicSize = 0.28f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && camFollow != null)
        {
            // ���������� ���������� ������� ������
            camFollow.SetFollow(true);
        }
    }
}

