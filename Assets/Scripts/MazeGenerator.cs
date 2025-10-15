using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// MazeGenerator — создаёт случайный лабиринт в 2D.
/// Работает на основе алгоритма поиска в глубину (Depth-First Search, рекурсивный backtracker).
/// </summary>
public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    public int width = 15;              // ширина лабиринта (в клетках)
    public int height = 15;             // высота лабиринта (в клетках)
    public float cellSize = 1f;         // размер клетки (расстояние между стенами)

    [Header("Prefabs")]
    public GameObject wallPrefab;       // префаб стены
    public GameObject floorPrefab;      // префаб пола

    [Header("Objects")]
    public GameObject player;           // ссылка на игрока
    public GameObject entrancePrefab;   // вход
    public GameObject exitPrefab;       // выход

    // внутренние переменные
    private int[,] maze;                // 0 = стена, 1 = путь
    private Vector2Int startCell;       // точка старта (вход)
    private Vector2Int endCell;         // точка выхода

    void Start()
    {
        GenerateMaze();
        InstantiateMaze();
        PlacePlayer();
    }

    // --- 1. Генерация структуры лабиринта ---
    void GenerateMaze()
    {
        maze = new int[width, height];

        // Заполняем всё стенами
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 0;

        // Начальная клетка
        startCell = new Vector2Int(1, 1);

        // Генерируем путь рекурсивно
        Carve(startCell.x, startCell.y);

        // Выход — противоположный угол
        endCell = new Vector2Int(width - 2, height - 2);
    }

    // Алгоритм carve — рекурсивный проход с удалением стен
    void Carve(int x, int y)
    {
        // Помечаем клетку как путь
        maze[x, y] = 1;

        // Случайный порядок направлений
        List<Vector2Int> directions = new List<Vector2Int>()
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };
        Shuffle(directions);

        foreach (var dir in directions)
        {
            int nx = x + dir.x * 2;
            int ny = y + dir.y * 2;

            if (IsInside(nx, ny) && maze[nx, ny] == 0)
            {
                // Удаляем стену между текущей и следующей клеткой
                maze[x + dir.x, y + dir.y] = 1;
                Carve(nx, ny);
            }
        }
    }

    // Проверка, внутри ли мы массива
    bool IsInside(int x, int y)
    {
        return x > 0 && y > 0 && x < width - 1 && y < height - 1;
    }

    // Перемешивание списка направлений
    void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            Vector2Int temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }

    // --- 2. Создание объектов на сцене ---
    void InstantiateMaze()
    {
        Transform mazeParent = new GameObject("Maze").transform;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * cellSize, y * cellSize, 0);

                if (maze[x, y] == 0)
                {
                    // Стена
                    GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity, mazeParent);
                    var sr = wall.GetComponent<SpriteRenderer>();
                    if (sr) sr.sortingOrder = 1; // стена поверх пола
                }
                else
                {
                    // Пол
                    GameObject floor = Instantiate(floorPrefab, pos, Quaternion.identity, mazeParent);
                    var sr = floor.GetComponent<SpriteRenderer>();
                    if (sr) sr.sortingOrder = 0; // пол ниже стен
                }
            }
        }

        // Добавляем вход и выход
        Vector3 startPos = new Vector3(startCell.x * cellSize, startCell.y * cellSize, 0);
        Vector3 endPos = new Vector3(endCell.x * cellSize, endCell.y * cellSize, 0);

        if (entrancePrefab)
            Instantiate(entrancePrefab, startPos, Quaternion.identity, mazeParent);

        if (exitPrefab)
            Instantiate(exitPrefab, endPos, Quaternion.identity, mazeParent);
    }

    // --- 3. Размещаем игрока ---
    void PlacePlayer()
    {
        if (player)
        {
            player.transform.position = new Vector3(startCell.x * cellSize, startCell.y * cellSize, 0);
        }
    }
}
