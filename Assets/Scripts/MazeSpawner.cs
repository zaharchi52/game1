// FULL UPDATED MazeSpawner.cs with SpritePrefab overlay and offset
using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    public Cell CellPrefab;                 // Префаб обычной ячейки
    public GameObject HighWallSpritePrefab; // Префаб верхней стены
    public GameObject SpritesPrefab;        // Префаб спрайтов поверх физических стен
    public GameObject PuzzleRoomPrefab;

    public float cellSpacing = 1f;
    public Vector2 originOffset = Vector2.zero;
    public Vector3 spriteOffset = new Vector3(0.771f, 0.453f, 0f); // смещение спрайтового префаба

    private void Start()
    {
        MazeGenerator generator = new MazeGenerator();
        MazeGeneratorCell[,] maze = generator.GenerateMaze();

        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        GameObject mazeRoot = new GameObject("MazeRoot");

        // === 1️⃣ Генерация клеток ===
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

                // === 2️⃣ SpritesPrefab поверх физических стен с offset ===
                if (SpritesPrefab != null)
                {
                    Vector3 spritePos = worldPos + spriteOffset;
                    GameObject spriteObj = Instantiate(SpritesPrefab, spritePos, Quaternion.identity, mazeRoot.transform);
                    spriteObj.name = $"Sprite_{x}_{y}";

                    Transform leftT = spriteObj.transform.Find("WallLeftSprite");
                    Transform bottomT = spriteObj.transform.Find("WallBottomSprite");

                    SpriteRenderer srLeft = leftT ? leftT.GetComponent<SpriteRenderer>() : null;
                    SpriteRenderer srBottom = bottomT ? bottomT.GetComponent<SpriteRenderer>() : null;

                    // LEFT WALL
                    if (maze[x, y].WallLeft && srLeft != null) { srLeft.enabled = true; srLeft.sortingOrder = 3; }
                    else if (srLeft != null) srLeft.enabled = false;

                    // BOTTOM WALL
                    if (maze[x, y].WallBottom && srBottom != null) { srBottom.enabled = true; srBottom.sortingOrder = 1; }
                    else if (srBottom != null) srBottom.enabled = false;

                    // === SortingOrder 3 если ниже есть два прохода ===
                    if (y > 0)
                    {
                        bool leftPass = (x > 0) ? !maze[x - 1, y - 1].WallLeft : false;
                        bool rightPass = (x < width - 1) ? !maze[x + 1, y - 1].WallLeft : false;

                        if (leftPass && rightPass)
                        {
                            if (srLeft && srLeft.enabled) srLeft.sortingOrder = 3;
                            if (srBottom && srBottom.enabled) srBottom.sortingOrder = 3;
                        }
                    }
                }

                // === 3️⃣ HighWallPrefab ===
                if (HighWallSpritePrefab != null && y > 0)
                {
                    bool placeHigh = false;
                    if (maze[x, y].WallBottom) placeHigh = true;
                    if (y == height - 1) placeHigh = true;

                    if (placeHigh)
                    {
                        Vector3 spritePos = new Vector3(x * cellSpacing, y * cellSpacing, 0f) + (Vector3)originOffset;
                        GameObject high = Instantiate(HighWallSpritePrefab, spritePos, Quaternion.identity, mazeRoot.transform);

                        high.transform.localScale = new Vector3(cellSpacing, high.transform.localScale.y, 1f);
                        high.transform.position += new Vector3(0f, -0.357f * cellSpacing, 0f);

                        SpriteRenderer hs = high.GetComponent<SpriteRenderer>();
                        if (hs != null)
                        {
                            bool leftPass = (x > 0) ? !maze[x - 1, y - 1].WallLeft : false;
                            bool rightPass = (x < width - 1) ? !maze[x + 1, y - 1].WallLeft : false;

                            if (leftPass && rightPass) hs.sortingOrder = 4;
                            else hs.sortingOrder = 2;
                        }
                    }
                }
            }
        }

        // === 4️⃣ Комната с загадкой ===
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