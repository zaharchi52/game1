using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    public Cell CellPrefab;                 // Префаб обычной ячейки
    public GameObject HighWallSpritePrefab; // Префаб спрайта верхней стены (без коллайдера)
    public GameObject PuzzleRoomPrefab;     // Префаб комнаты с загадкой

    public float cellSpacing = 1f;
    public Vector2 originOffset = Vector2.zero;

    private void Start()
    {
        MazeGenerator generator = new MazeGenerator();
        MazeGeneratorCell[,] maze = generator.GenerateMaze();

        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        GameObject mazeRoot = new GameObject("MazeRoot");

        // === 1️⃣ Генерация обычных клеток ===
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = new Vector3(x * cellSpacing, y * cellSpacing, 0f) + (Vector3)originOffset;
                Cell c = Instantiate(CellPrefab, worldPos, Quaternion.identity, mazeRoot.transform);
                c.name = $"Cell_{x}_{y}";

                // Физические стены
                if (c.WallLeft != null) c.WallLeft.SetActive(maze[x, y].WallLeft);
                if (c.WallBottom != null) c.WallBottom.SetActive(maze[x, y].WallBottom);

                // === HighWallPrefab с учётом нижней клетки ===
                if (HighWallSpritePrefab != null && y > 0)
                {
                    MazeGeneratorCell belowCell = maze[x, y - 1];

                    bool hasLeftPassage = (x > 0) ? !maze[x - 1, y - 1].WallLeft : false;
                    bool hasRightPassage = (x < width - 1) ? !maze[x + 1, y - 1].WallLeft : false;

                    bool placeHighWallSprite = false;
                    if (maze[x, y].WallBottom) placeHighWallSprite = true; // верхняя клетка имеет нижнюю стену
                    if (y == height - 1) placeHighWallSprite = true;       // край лабиринта

                    if (placeHighWallSprite)
                    {
                        Vector3 spritePos = new Vector3(x * cellSpacing, y * cellSpacing, 0f) + (Vector3)originOffset;
                        GameObject highWallSprite = Instantiate(HighWallSpritePrefab, spritePos, Quaternion.identity, mazeRoot.transform);

                        highWallSprite.transform.localScale = new Vector3(cellSpacing, highWallSprite.transform.localScale.y, 1f);
                        highWallSprite.transform.position += new Vector3(0f, -0.355f * cellSpacing, 0f);

                        SpriteRenderer sr = highWallSprite.GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            if (hasLeftPassage && hasRightPassage)
                                sr.sortingOrder = 4; // нижняя клетка имеет проходы слева и справа
                            else
                                sr.sortingOrder = 2; // стандартный случай
                        }
                    }
                }
            }
        }

        // === 2️⃣ Комната с загадкой ===
        if (PuzzleRoomPrefab != null)
        {
            int centerX = width / 2;
            int centerY = height / 2;

            Vector2 roomPos = new Vector2(centerX * cellSpacing, centerY * cellSpacing) + originOffset;
            GameObject puzzleRoom = Instantiate(PuzzleRoomPrefab, roomPos, Quaternion.identity, mazeRoot.transform);
            puzzleRoom.name = "PuzzleRoom_Center";

            int roomRadius = 2;

            for (int dx = -roomRadius; dx <= roomRadius; dx++)
            {
                for (int dy = -roomRadius; dy <= roomRadius; dy++)
                {
                    int cx = centerX + dx;
                    int cy = centerY + dy;
                    if (cx >= 0 && cx < width && cy >= 0 && cy < height)
                    {
                        maze[cx, cy].WallLeft = false;
                        maze[cx, cy].WallBottom = false;
                    }
                }
            }

            int topY = centerY + roomRadius;
            int bottomY = centerY - roomRadius;
            int leftX = centerX - roomRadius;
            int rightX = centerX + roomRadius;

            if (topY < height) maze[centerX, topY].WallBottom = false;
            if (bottomY >= 0) maze[centerX, bottomY].WallBottom = false;
            if (leftX >= 0) maze[leftX, centerY].WallLeft = false;
            if (rightX < width) maze[rightX, centerY].WallLeft = false;
        }
    }
}
