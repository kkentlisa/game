using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("Настройки сетки")]
    public int width = 20;
    public int height = 20;
    public float cellSize = 0.1f; 
    public Vector3 offset; 

    [Header("Префабы стен")]
    public GameObject wallPrefab; 
    private List<GameObject> spawnedWalls = new List<GameObject>();
    private MazeCell[,] maze;

    private class MazeCell
    {
        public int x, y;
        public bool visited = false;
        public GameObject[] walls = new GameObject[4];

        public MazeCell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    void Start()
    {
        offset = transform.position;
        StartCoroutine(MazeCycleRoutine());
    }

    IEnumerator MazeCycleRoutine()
    {
        while (true)
        {
            GenerateNewMaze();
            yield return new WaitForSeconds(15f);
        }
    }

    void GenerateNewMaze()
    {
        foreach (var wall in spawnedWalls) Destroy(wall);
        spawnedWalls.Clear();

        maze = new MazeCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = new MazeCell(x, y);
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellCenter = offset + new Vector3(x * cellSize + cellSize/2, y * cellSize + cellSize/2, 0);

                if (x < width - 1)
                {
                    GameObject w = Instantiate(wallPrefab, cellCenter + new Vector3(cellSize/2, 0, 0), Quaternion.identity, transform);
                    w.transform.localScale = new Vector3(0.1f, cellSize, 1);
                    maze[x, y].walls[1] = w;
                    maze[x + 1, y].walls[3] = w;
                    spawnedWalls.Add(w);
                }
                if (y < height - 1)
                {
                    GameObject w = Instantiate(wallPrefab, cellCenter + new Vector3(0, cellSize/2, 0), Quaternion.Euler(0, 0, 90), transform);
                    w.transform.localScale = new Vector3(0.1f, cellSize, 1);
                    maze[x, y].walls[0] = w;
                    maze[x, y + 1].walls[2] = w;
                    spawnedWalls.Add(w);
                }
            }
        }

        Stack<MazeCell> stack = new Stack<MazeCell>();
        MazeCell current = maze[0, 0];
        current.visited = true;

        int visitedCount = 1;
        int totalCells = width * height;

        while (visitedCount < totalCells)
        {
            List<MazeCell> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                MazeCell next = neighbors[Random.Range(0, neighbors.Count)];
                RemoveWallBetween(current, next);
                
                stack.Push(current);
                current = next;
                current.visited = true;
                visitedCount++;
            }
            else if (stack.Count > 0)
            {
                current = stack.Pop();
            }
        }
    }

    List<MazeCell> GetUnvisitedNeighbors(MazeCell cell)
    {
        List<MazeCell> neighbors = new List<MazeCell>();
        if (cell.y < height - 1 && !maze[cell.x, cell.y + 1].visited) neighbors.Add(maze[cell.x, cell.y + 1]); 
        if (cell.x < width - 1 && !maze[cell.x + 1, cell.y].visited) neighbors.Add(maze[cell.x + 1, cell.y]); 
        if (cell.y > 0 && !maze[cell.x, cell.y - 1].visited) neighbors.Add(maze[cell.x, cell.y - 1]); 
        if (cell.x > 0 && !maze[cell.x - 1, cell.y].visited) neighbors.Add(maze[cell.x - 1, cell.y]); 
        return neighbors;
    }

    void RemoveWallBetween(MazeCell a, MazeCell b)
    {
        if (a.x == b.x) 
        {
            if (b.y > a.y) a.walls[0].SetActive(false); else a.walls[2].SetActive(false);
        }
        else 
        {
            if (b.x > a.x) a.walls[1].SetActive(false); else a.walls[3].SetActive(false);
        }
    }
}