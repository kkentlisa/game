using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssistantAStar : MonoBehaviour
{
    public Transform playerTransform;
    public GridManager gridManager;
    public float speed = 3f;
    
    private List<PathNode> currentPath = new List<PathNode>();
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(UpdatePathRoutine());
    }

    IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
            if (playerTransform != null && gridManager != null)
            {
                FindPath(transform.position, playerTransform.position);
            }
            yield return new WaitForSeconds(0.2f); 
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        gridManager.ResetNodes(); 
        PathNode startNode = gridManager.GetNodeFromWorldPoint(startPos);
        PathNode targetNode = gridManager.GetNodeFromWorldPoint(targetPos);

        if (startNode == targetNode || !targetNode.isWalkable) 
        {
            currentPath.Clear();
            return;
        }

        List<PathNode> openSet = new List<PathNode> { startNode };
        HashSet<PathNode> closedSet = new HashSet<PathNode>();
        startNode.gCost = 0;

        while (openSet.Count > 0)
        {
            PathNode curr = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < curr.fCost || (openSet[i].fCost == curr.fCost && openSet[i].hCost < curr.hCost))
                    curr = openSet[i];
            }

            openSet.Remove(curr);
            closedSet.Add(curr);

            if (curr == targetNode) 
            { 
                RetracePath(startNode, targetNode); 
                return; 
            }

            foreach (PathNode neighbor in gridManager.GetNeighbors(curr))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

                int newCostToNeighbor = curr.gCost + 1;
                if (newCostToNeighbor < neighbor.gCost)
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = Mathf.Abs(neighbor.x - targetNode.x) + Mathf.Abs(neighbor.y - targetNode.y);
                    neighbor.parent = curr;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
    }

    void RetracePath(PathNode start, PathNode end)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode curr = end;
        while (curr != start)
        {
            path.Add(curr);
            curr = curr.parent;
        }
        path.Reverse();
        currentPath = path;
    }

    void FixedUpdate()
    {
        if (currentPath != null && currentPath.Count > 0)
        {
            Vector3 targetPos = gridManager.GetWorldPosition(currentPath[0].x, currentPath[0].y);
            Vector2 direction = ((Vector2)targetPos - rb.position).normalized;
            
            rb.linearVelocity = direction * speed;

            if (Vector2.Distance(rb.position, targetPos) < 0.08f)
            {
                currentPath.RemoveAt(0);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}