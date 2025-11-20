using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    public Cell CellPrefab;                // Префаб обычной ячейки
    public GameObject HighWallSpritePrefab; // Префаб спрайта верхней стены (без коллайдера)
    public GameObject PuzzleRoomPrefab;    // Префаб комнаты с загадкой

    public float cellSpacing = 1f;         // Расстояние между клетками
    public Vector2 originOffset = Vector2.zero; // Смещение лабиринта

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
                Vector2 worldPos = new Vector2(x * cellSpacing, y * cellSpacing) + originOffset;
                Cell c = Instantiate(CellPrefab, worldPos, Quaternion.identity, mazeRoot.transform);


                c.name = $"Cell_{x}_{y}";

                // Физические стены
                if (c.WallLeft != null) c.WallLeft.SetActive(maze[x, y].WallLeft);
                if (c.WallBottom != null) c.WallBottom.SetActive(maze[x, y].WallBottom);

                // Визуальные верхние стены (спрайт без коллайдера)
                bool placeHighWallSprite = false;

                if (y < height - 1 && maze[x, y + 1].WallBottom) placeHighWallSprite = true;
                if (y == height - 1) placeHighWallSprite = true;

                if (placeHighWallSprite && HighWallSpritePrefab != null)
                {
                    Vector2 spritePos = new Vector2(x * cellSpacing, (y + 1) * cellSpacing) + originOffset;
                    GameObject highWallSprite = Instantiate(HighWallSpritePrefab, spritePos, Quaternion.identity, mazeRoot.transform);

                    // Подгоняем спрайт под ширину клетки
                    highWallSprite.transform.localScale = new Vector3(cellSpacing, highWallSprite.transform.localScale.y, 1f);

                    // Сдвиг спрайта вниз, чтобы перекрывать физическую стену
                    highWallSprite.transform.position += new Vector3(0f, -0.355f * cellSpacing, 0f);
                }
            }
        }

        // === 2️⃣ Добавляем комнату с загадкой ===
        if (PuzzleRoomPrefab != null)
        {
            int centerX = width / 2;
            int centerY = height / 2;

            Vector2 roomPos = new Vector2(centerX * cellSpacing, centerY * cellSpacing) + originOffset;
            GameObject puzzleRoom = Instantiate(PuzzleRoomPrefab, roomPos, Quaternion.identity, mazeRoot.transform);
            puzzleRoom.name = "PuzzleRoom_Center";

            int roomRadius = 2;  // радиус комнаты (в клетках)

            // Очищаем все стены в радиусе комнаты
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

            // Делаем проходы со всех сторон комнаты (верх, низ, лево, право)
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
