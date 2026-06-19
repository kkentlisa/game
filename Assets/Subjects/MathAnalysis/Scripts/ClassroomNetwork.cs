using UnityEngine;
using System.Collections.Generic;

public class ClassroomNetwork : MonoBehaviour
{
    [System.Serializable]
    public class Node
    {
        public string name;
        public Vector2 position;
        public List<int> connectedNodeIndices = new List<int>();
    }

    public List<Node> allNodes = new List<Node>();

    public int GetClosestNodeIndex(Vector2 position)
    {
        int closestIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < allNodes.Count; i++)
        {
            float dist = Vector2.Distance(position, allNodes[i].position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    void OnDrawGizmos()
    {
        if (allNodes == null) return;

        for (int i = 0; i < allNodes.Count; i++)
        {
            Gizmos.color = Color.green;
            Vector3 nodePos = new Vector3(allNodes[i].position.x, allNodes[i].position.y, 0f);
            Gizmos.DrawSphere(nodePos, 0.15f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(nodePos + Vector3.up * 0.2f, i.ToString());
#endif

            Gizmos.color = Color.yellow;
            foreach (int connectedIndex in allNodes[i].connectedNodeIndices)
            {
                if (connectedIndex >= 0 && connectedIndex < allNodes.Count)
                {
                    Vector3 connectedPos = new Vector3(allNodes[connectedIndex].position.x, allNodes[connectedIndex].position.y, 0f);
                    Gizmos.DrawLine(nodePos, connectedPos);
                }
            }
        }
    }
}