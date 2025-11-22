using UnityEngine;

public class TeleportActivator : MonoBehaviour
{
    [Tooltip("—юда перетащи все WrapTeleport2D (по 4 шт.)")]
    public WrapTeleport2D[] portals;

    [Tooltip("ќтключать ли этот триггер после активации (true обычно)")]
    public bool disableSelfAfterActivate = true;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        // ¬ключаем все порталы (если они были отключены)
        if (portals != null)
        {
            foreach (var p in portals)
            {
                if (p != null)
                    p.enabled = true;
            }
        }

        activated = true;
        Debug.Log("TeleportActivator: активировал все порталы");

        if (disableSelfAfterActivate)
            gameObject.SetActive(false);
    }
}
