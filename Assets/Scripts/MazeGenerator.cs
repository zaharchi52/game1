using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// MazeGenerator � ������ ��������� �������� � 2D.
/// �������� �� ������ ��������� ������ � ������� (Depth-First Search, ����������� backtracker).
/// </summary>
public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    public int width = 15;              // ������ ��������� (� �������)
    public int height = 15;             // ������ ��������� (� �������)
    public float cellSize = 1f;         // ������ ������ (���������� ����� �������)

    [Header("Prefabs")]
    public GameObject wallPrefab;       // ������ �����
    public GameObject floorPrefab;      // ������ ����

    [Header("Objects")]
    public GameObject player;           // ������ �� ������
    public GameObject entrancePrefab;   // ����
    public GameObject exitPrefab;       // �����

    // ���������� ����������
    private int[,] maze;                // 0 = �����, 1 = ����
    private Vector2Int startCell;       // ����� ������ (����)
    private Vector2Int endCell;         // ����� ������

    void Start()
    {
        GenerateMaze();
        InstantiateMaze();
        PlacePlayer();
    }

    // --- 1. ��������� ��������� ��������� ---
    void GenerateMaze()
    {
        maze = new int[width, height];

        // ��������� �� �������
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 0;

        // ��������� ������
        startCell = new Vector2Int(1, 1);

        // ���������� ���� ����������
        Carve(startCell.x, startCell.y);

        // ����� � ��������������� ����
        endCell = new Vector2Int(width - 2, height - 2);
    }

    // �������� carve � ����������� ������ � ��������� ����
    void Carve(int x, int y)
    {
        // �������� ������ ��� ����
        maze[x, y] = 1;

        // ��������� ������� �����������
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
                // ������� ����� ����� ������� � ��������� �������
                maze[x + dir.x, y + dir.y] = 1;
                Carve(nx, ny);
            }
        }
    }

    // ��������, ������ �� �� �������
    bool IsInside(int x, int y)
    {
        return x > 0 && y > 0 && x < width - 1 && y < height - 1;
    }

    // ������������� ������ �����������
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

    // --- 2. �������� �������� �� ����� ---
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
                    // �����
                    GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity, mazeParent);
                    var sr = wall.GetComponent<SpriteRenderer>();
                    if (sr) sr.sortingOrder = 1; // ����� ������ ����
                }
                else
                {
                    // ���
                    GameObject floor = Instantiate(floorPrefab, pos, Quaternion.identity, mazeParent);
                    var sr = floor.GetComponent<SpriteRenderer>();
                    if (sr) sr.sortingOrder = 0; // ��� ���� ����
                }
            }
        }

        // ��������� ���� � �����
        Vector3 startPos = new Vector3(startCell.x * cellSize, startCell.y * cellSize, 0);
        Vector3 endPos = new Vector3(endCell.x * cellSize, endCell.y * cellSize, 0);

        if (entrancePrefab)
            Instantiate(entrancePrefab, startPos, Quaternion.identity, mazeParent);

        if (exitPrefab)
            Instantiate(exitPrefab, endPos, Quaternion.identity, mazeParent);
    }

    // --- 3. ��������� ������ ---
    void PlacePlayer()
    {
        if (player)
        {
            player.transform.position = new Vector3(startCell.x * cellSize, startCell.y * cellSize, 0);
        }
    }
}
