using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public int width = 200; 
    public int height = 200;
    public float cellSize = 0.1f;

    private PathNode[,] grid;

    void Awake() 
    { 
        GenerateGrid(); 
        StartCoroutine(RefreshGridRoutine());
    }

    IEnumerator RefreshGridRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            GenerateGrid();
        }
    }

    public void GenerateGrid()
    {
        grid = new PathNode[width, height];
        Vector3 origin = transform.position;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPoint = GetWorldPosition(x, y);

                Collider2D hit = Physics2D.OverlapCircle(worldPoint, cellSize * 0.45f, obstacleLayer);
            
                bool isWalkable = (hit == null);
                grid[x, y] = new PathNode(x, y, isWalkable);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 size = new Vector3(width * cellSize, height * cellSize, 0);
        Vector3 center = transform.position + size * 0.5f;
        Gizmos.DrawWireCube(center, size);

        if (grid != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = grid[x, y].isWalkable ? new Color(1,1,1,0.2f) : Color.red;
                    Gizmos.DrawWireCube(GetWorldPosition(x, y), new Vector3(cellSize, cellSize, 0));
                }
            }
        }
        else 
        {
            Gizmos.color = Color.gray;
            for (int x = 0; x < Mathf.Min(width, 10); x++)
            {
                for (int y = 0; y < Mathf.Min(height, 10); y++)
                {
                    Gizmos.DrawWireCube(GetWorldPosition(x, y), new Vector3(cellSize, cellSize, 0));
                }
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        float xPos = x * cellSize + (cellSize * 0.5f);
        float yPos = y * cellSize + (cellSize * 0.5f);
        return transform.position + new Vector3(xPos, yPos, 0);
    }

    public PathNode GetNodeFromWorldPoint(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - transform.position.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - transform.position.y) / cellSize);
        return grid[Mathf.Clamp(x, 0, width - 1), Mathf.Clamp(y, 0, height - 1)];
    }

    public void ResetNodes()
    {
        if (grid == null) return;
        foreach (var node in grid)
        {
            node.gCost = int.MaxValue;
            node.parent = null;
        }
    }

    public List<PathNode> GetNeighbors(PathNode node)
    {
        List<PathNode> neighbors = new List<PathNode>();
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };
        for (int i = 0; i < 4; i++)
        {
            int nx = node.x + dx[i], ny = node.y + dy[i];
            if (nx >= 0 && nx < width && ny >= 0 && ny < height) neighbors.Add(grid[nx, ny]);
        }
        return neighbors;
    }
}