using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public int width = 200; 
    public int height = 200;
    public float cellSize = 0.1f;

    private PathNode[,] grid;

    void Awake() { GenerateGrid(); }

    public void GenerateGrid()
    {
        grid = new PathNode[width, height];
        
        Vector3 origin = transform.position;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xPos = x * cellSize + (cellSize * 0.5f);
                float yPos = y * cellSize + (cellSize * 0.5f);
                Vector3 worldPoint = origin + new Vector3(xPos, yPos, 0f);

                Collider2D hit = Physics2D.OverlapCircle(worldPoint, cellSize * 0.1f, obstacleLayer);
            
                bool isWalkable = (hit == null);
                grid[x, y] = new PathNode(x, y, isWalkable);
            }
        }
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

    public PathNode GetNodeFromWorldPoint(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - transform.position.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - transform.position.y) / cellSize);
        
        return grid[Mathf.Clamp(x, 0, width - 1), Mathf.Clamp(y, 0, height - 1)];
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        float xPos = x * cellSize + (cellSize * 0.5f);
        float yPos = y * cellSize + (cellSize * 0.5f);
        return transform.position + new Vector3(xPos, yPos, 0);
    }

    public List<PathNode> GetNeighbors(PathNode node)
    {
        List<PathNode> neighbors = new List<PathNode>();
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = node.x + dx[i];
            int ny = node.y + dy[i];

            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                neighbors.Add(grid[nx, ny]);
            }
        }
        return neighbors;
    }

    void OnDrawGizmos()
    {
        if (grid == null) return;
        foreach (var n in grid)
        {
            Gizmos.color = n.isWalkable ? new Color(0, 1, 0, 0.2f) : new Color(1, 0, 0, 0.5f);
            Gizmos.DrawCube(GetWorldPosition(n.x, n.y), Vector3.one * (cellSize * 0.9f));
        }
    }
}