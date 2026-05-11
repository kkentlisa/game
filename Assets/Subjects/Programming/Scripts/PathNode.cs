using UnityEngine;

public class PathNode
{
    public int x, y;
    public int gCost; 
    public int hCost; 
    public int fCost => gCost + hCost; 

    public bool isWalkable;
    public PathNode parent; 

    public PathNode(int x, int y, bool isWalkable)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
    }
}