using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    public Cell CellPrefab;              // ������ Cell (� ����������� Cell.cs)
    public GameObject HighWallPrefab;    // ������ ���������� ������� ����� (��� ����������)

    public float cellSpacing = 1f;       // ���������� ����� �������� ����� (1 = �������� ������ � 1 unit)
    public Vector2 originOffset = Vector2.zero; // ����� ���� ����� � ���. (�� �������)

    private void Start()
    {
        MazeGenerator generator = new MazeGenerator();
        MazeGeneratorCell[,] maze = generator.GenerateMaze();

        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        // ������ �������� ��� �������
        GameObject mazeRoot = new GameObject("MazeRoot");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 worldPos = new Vector2(x * cellSpacing, y * cellSpacing) + originOffset;
                // instantiate cell prefab
                Cell c = Instantiate(CellPrefab, worldPos, Quaternion.identity, mazeRoot.transform);
                c.name = $"Cell_{x}_{y}";

                // ��������/��������� ���������� ����� (����������)
                if (c.WallLeft != null) c.WallLeft.SetActive(maze[x, y].WallLeft);
                if (c.WallBottom != null) c.WallBottom.SetActive(maze[x, y].WallBottom);

                // ������ ������� �����: ���� ��� ������� ������� (y+1) ���� ������ ����� -> ������ HighWall �� ������� (x, y+1)
                bool placeHighWall = false;
                if (y < height - 1)
                {
                    if (maze[x, y + 1].WallBottom) placeHighWall = true;
                }
                else
                {
                    // ���� ��� ������� ��� � ����� ������� ������� ����� �� ����
                    placeHighWall = true;
                }

                if (placeHighWall && HighWallPrefab != null)
                {
                    Vector2 highWallWorld = new Vector2(x * cellSpacing, (y + 1) * cellSpacing) + originOffset;
                    // optional: offset by 0.0.. e.g. highWallWorld.y += 0.0f;
                    Instantiate(HighWallPrefab, highWallWorld, Quaternion.identity, mazeRoot.transform);
                }
            }
        }
    }
}
