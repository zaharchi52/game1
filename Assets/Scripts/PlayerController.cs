using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerController — отвечает за передвижение игрока и простую систему взаимодействия (по нажатию E).
/// Подходит для top-down / Undertale-подобного управления.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Скорость передвижения игрока (ед./сек). Подбери на глаз.")]
    public float speed = 4f;

    [Tooltip("Радиус поиска интерактивных объектов (E).")]
    public float interactRadius = 1.0f;

    [Header("Settings")]
    [Tooltip("Если true — управление инвертировано (используется для галлюцинаций в малинке).")]
    public bool controlsInverted = false;

    // приватные поля
    Rigidbody2D rb;           // кешируем Rigidbody2D для физики
    Vector2 moveVelocity;     // целевая скорость движения

    void Awake()
    {
        // Получаем компонент Rigidbody2D при старте (обязателен благодаря RequireComponent)
        rb = GetComponent<Rigidbody2D>();

        // Защитная настройка: отключаем гравитацию на всякий случай
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true; // блокируем вращение тела
        }
    }

    void Update()
    {
        // Считываем ввод с осей (по умолчанию в проекте оси "Horizontal" и "Vertical" настроены)
        // Используем GetAxisRaw для «жёсткого» (без сглаживания) управления — чаще приятнее в ретро-играх.
        float h = Input.GetAxisRaw("Horizontal"); // A/D, ←/→
        float v = Input.GetAxisRaw("Vertical");   // W/S, ↑/↓

        // Инвертирование управления (для головоломки с малиной)
        if (controlsInverted)
        {
            h = -h;
            v = -v;
        }

        // Нормируем движение, чтобы диагональ не была быстрее (нормализуем вектор)
        Vector2 input = new Vector2(h, v);
        if (input.magnitude > 1f) input = input.normalized;

        moveVelocity = input * speed;

        // Обработка взаимодействия (нажатие E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void FixedUpdate()
    {
        // Применяем движение через Rigidbody2D — так физика будет корректной
        if (rb != null)
        {
            rb.velocity = moveVelocity;
        }
    }

    /// <summary>
    /// Попытка взаимодействия с ближайшим Interactable в пределах interactRadius.
    /// Ожидается, что объекты, с которыми можно взаимодействовать, имеют компонент Interactable (скрипт).
    /// </summary>
    void TryInteract()
    {
        // Находим все коллайдеры в радиусе
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);

        Interactable nearest = null;
        float bestDist = float.MaxValue;

        foreach (var c in hits)
        {
            // Игнорируем самого себя
            if (c.gameObject == this.gameObject) continue;

            // Пытаемся получить компонент Interactable (его создадим позже)
            Interactable it = c.GetComponent<Interactable>();
            if (it != null)
            {
                float d = Vector2.Distance(transform.position, c.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = it;
                }
            }
        }

        if (nearest != null)
        {
            nearest.Interact();
        }
        else
        {
            // Для отладки — можно вывести подсказку, что рядом нет интерактивного объекта
            Debug.Log("Нет объектов для взаимодействия (E).");
        }
    }

    // Визуализация радиуса взаимодействия в редакторе (не влияет на игру)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}