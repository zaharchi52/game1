using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorCell
{
    public int X;
    public int Y;

    public bool WallLeft = true;
    public bool WallBottom = true;

    public bool Visited = false;
    public int DistanceFromStart;
}

public class MazeGenerator
{
    [Header("Размер лабиринта")]
    public int Width = 9;
    public int Height = 9;

    [Header("Комната (загадка)")]
    public bool EnableRoom = true;                // включить/выключить создание комнаты
    public int RoomWidth = 1;                     // ширина комнаты (в клетках)
    public int RoomHeight = 1;                    // высота комнаты (в клетках)
    public bool CenterRoom = true;                // разместить по центру лабиринта
    public Vector2Int RoomPosition = new Vector2Int(0, 0); // координаты (если не центр)
    public bool AddRoomEntrance = true;           // добавить вход в комнату
    public string EntranceSide = "Bottom";        // сторона входа ("Bottom", "Top", "Left", "Right")

    public MazeGeneratorCell[,] GenerateMaze()
    {
        MazeGeneratorCell[,] maze = new MazeGeneratorCell[Width, Height];

        // создаём клетки
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                maze[x, y] = new MazeGeneratorCell { X = x, Y = y };
            }
        }

        // убираем правую и верхнюю границы (для корректного отображения)
        for (int x = 0; x < maze.GetLength(0); x++)
            maze[x, Height - 1].WallLeft = false;

        for (int y = 0; y < maze.GetLength(1); y++)
            maze[Width - 1, y].WallBottom = false;

        // генерируем основной лабиринт
        RemoveWallsWithBacktracker(maze);

        // создаём комнату (если включена)
        if (EnableRoom)
            CreateRoom(maze);

        // добавляем выход из лабиринта
        PlaceMazeExit(maze);

        return maze;
    }

    // Алгоритм "обратного трекера"
    private void RemoveWallsWithBacktracker(MazeGeneratorCell[,] maze)
    {
        MazeGeneratorCell current = maze[0, 0];
        current.Visited = true;
        current.DistanceFromStart = 0;

        Stack<MazeGeneratorCell> stack = new Stack<MazeGeneratorCell>();

        do
        {
            List<MazeGeneratorCell> unvisitedNeighbours = new List<MazeGeneratorCell>();

            int x = current.X;
            int y = current.Y;

            if (x > 0 && !maze[x - 1, y].Visited) unvisitedNeighbours.Add(maze[x - 1, y]);
            if (y > 0 && !maze[x, y - 1].Visited) unvisitedNeighbours.Add(maze[x, y - 1]);
            if (x < Width - 2 && !maze[x + 1, y].Visited) unvisitedNeighbours.Add(maze[x + 1, y]);
            if (y < Height - 2 && !maze[x, y + 1].Visited) unvisitedNeighbours.Add(maze[x, y + 1]);

            if (unvisitedNeighbours.Count > 0)
            {
                MazeGeneratorCell chosen = unvisitedNeighbours[UnityEngine.Random.Range(0, unvisitedNeighbours.Count)];
                RemoveWall(current, chosen);

                chosen.Visited = true;
                stack.Push(chosen);
                current = chosen;
                chosen.DistanceFromStart = stack.Count;
            }
            else
            {
                current = stack.Pop();
            }
        } while (stack.Count > 0);
    }

    private void RemoveWall(MazeGeneratorCell a, MazeGeneratorCell b)
    {
        if (a.X == b.X)
        {
            if (a.Y > b.Y) a.WallBottom = false;
            else b.WallBottom = false;
        }
        else
        {
            if (a.X > b.X) a.WallLeft = false;
            else b.WallLeft = false;
        }
    }

    private void PlaceMazeExit(MazeGeneratorCell[,] maze)
    {
        // РАСКОМЕНТИТЬ ДЛЯ СОЗДАНИЯ ВЫХОДА ИЗ ЛАБИРИНТА
        //MazeGeneratorCell furthest = maze[0, 0];

        for (int x = 0; x < maze.GetLength(0); x++)
        {
            //if (maze[x, Height - 2].DistanceFromStart > furthest.DistanceFromStart) furthest = maze[x, Height - 2];
            //if (maze[x, 0].DistanceFromStart > furthest.DistanceFromStart) furthest = maze[x, 0];
        }

        for (int y = 0; y < maze.GetLength(1); y++)
        {
            //if (maze[Width - 2, y].DistanceFromStart > furthest.DistanceFromStart) furthest = maze[Width - 2, y];
            //if (maze[0, y].DistanceFromStart > furthest.DistanceFromStart) furthest = maze[0, y];
        }

        //if (furthest.X == 0) furthest.WallLeft = false;
        //else if (furthest.Y == 0) furthest.WallBottom = false;
        //else if (furthest.X == Width - 2) maze[furthest.X + 1, furthest.Y].WallLeft = false;
        //else if (furthest.Y == Height - 2) maze[furthest.X, furthest.Y + 1].WallBottom = false;
    }

    // Создание комнаты для загадки
    private void CreateRoom(MazeGeneratorCell[,] maze)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        int startX, startY;

        // вычисляем позицию комнаты
        if (CenterRoom)
        {
            startX = (width - RoomWidth) / 2;
            startY = (height - RoomHeight) / 2;
        }
        else
        {
            startX = Mathf.Clamp(RoomPosition.x, 1, width - RoomWidth - 1);
            startY = Mathf.Clamp(RoomPosition.y, 1, height - RoomHeight - 1);
        }

        int endX = Mathf.Min(startX + RoomWidth, width - 1);
        int endY = Mathf.Min(startY + RoomHeight, height - 1);
       
        // удаляем стены внутри комнаты
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                maze[x, y].WallLeft = false;
                maze[x, y].WallBottom = false;

                if (x < width - 1)
                    maze[x + 1, y].WallLeft = false;

                if (y < height - 1)
                    maze[x, y + 1].WallBottom = false;
            }
        }

        // добавляем вход
        if (AddRoomEntrance)
            CreateRoomEntrance(maze, startX, startY, endX, endY);

        Debug.Log($"Комната создана ({startX},{startY}) ? ({endX},{endY}), вход: {EntranceSide}");
    }

    private void CreateRoomEntrance(MazeGeneratorCell[,] maze, int startX, int startY, int endX, int endY)
    {
        int entranceX = (startX + endX) / 2;
        int entranceY = (startY + endY) / 2;

        switch (EntranceSide)
        {
            case "Bottom":
                maze[entranceX, startY - 1].WallBottom = false;
                break;
            case "Top":
                maze[entranceX, endY].WallBottom = false;
                break;
            case "Left":
                maze[startX - 1, entranceY].WallLeft = false;
                break;
            case "Right":
                maze[endX, entranceY].WallLeft = false;
                break;
        }
    }
}
