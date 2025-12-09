using UnityEngine;
using System.Collections.Generic;

public class DisableCollidersByLayer : MonoBehaviour
{
    [Tooltip("Имя слоя, коллайдеры которого нужно отключить (например 'Labyrinth')")]
    public string layerName = "Labyrinth";

    [Tooltip("Игрок (только для проверки)")]
    public string playerTag = "Player";

    [Tooltip("Отключать только один раз")]
    public bool oneTime = true;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (activated && oneTime) return;

        DisableLayerColliders();
        activated = true;
    }

    public void DisableLayerColliders()
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer < 0)
        {
            Debug.LogError($"DisableCollidersByLayer: слой '{layerName}' не найден");
            return;
        }

        Collider2D[] all = FindObjectsOfType<Collider2D>(true);
        int count = 0;
        foreach (var c in all)
        {
            if (c.gameObject.layer == layer)
            {
                c.enabled = false;
                count++;
            }
        }

        Debug.Log($"DisableCollidersByLayer: отключено {count} Collider2D на слое {layerName}");
    }

    
}
