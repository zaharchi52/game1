using UnityEngine;
using System.Collections.Generic;

public class PuzzleRoomEntry : MonoBehaviour
{
    [Tooltip("Коллайдеры комнаты (только их НЕ отключаем).")]
    public Collider2D[] puzzleRoomColliders;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (activated) return;

        DisableLabyrinthColliders();
        activated = true;

        Debug.Log("Лабиринт отключён, комната активна");
    }

    void DisableLabyrinthColliders()
    {
        // собираем комнатные коллайдеры в список
        HashSet<Collider2D> roomSet = new HashSet<Collider2D>(puzzleRoomColliders);

        // получаем ВСЕ Collider2D в сцене
        Collider2D[] all = FindObjectsOfType<Collider2D>(true);

        int disabled = 0;

        foreach (Collider2D col in all)
        {
            // не трогаем комнату
            if (roomSet.Contains(col)) continue;

            // не трогаем триггер входа
            if (col == GetComponent<Collider2D>()) continue;

            // Отключаем ТОЛЬКО Collider2D
            col.enabled = false;
            disabled++;
        }

        Debug.Log("Отключено коллайдеров лабиринта: " + disabled);
    }
}
