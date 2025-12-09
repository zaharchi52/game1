using UnityEngine;

public class PuzzlePassage : MonoBehaviour
{
    private PuzzleRoomController controller;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        controller = GetComponentInParent<PuzzleRoomController>();
    }

    public void SetNormal(Sprite sprite)
    {
        sr.sprite = sprite;
    }

    public void SetCorrect(Sprite sprite)
    {
        sr.sprite = sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        controller.PlayerEntered(this);
    }
}
