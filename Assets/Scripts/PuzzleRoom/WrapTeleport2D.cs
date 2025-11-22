using UnityEngine;
using System.Collections;

public class WrapTeleport2D : MonoBehaviour
{
    public Transform destination;
    public Vector2 pushDirection; // ← вот это
    public float pushAmount = 0.3f;
    public float cooldown = 0.2f;

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // телепортируем
        other.transform.position = destination.position;

        // смещение внутрь комнаты
        other.transform.position += (Vector3)(pushDirection * pushAmount);

        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        col.enabled = false;
        yield return new WaitForSeconds(cooldown);
        col.enabled = true;
    }
}
