using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    public List<MapNode> FindPath(MapNode startNode, MapNode targetNode)
    {
        List<MapNode> openSet = new List<MapNode>();
        HashSet<MapNode> closedSet = new HashSet<MapNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            MapNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                // 寻找路径最短的Node
                if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (MapNode neighbor in GetNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private List<MapNode> RetracePath(MapNode startNode, MapNode endNode)
    {
        List<MapNode> path = new List<MapNode>();
        MapNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private int GetDistance(MapNode nodeA, MapNode nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }

        return 14 * distX + 10 * (distY - distX);
    }

    private List<MapNode> GetNeighbors(MapNode node)
    {
        // 此处需实现一个返回节点邻居的函数，需要根据你的地图和节点结构进行实现。
        // 示例：
        // return grid.GetNeighbors(node);
        throw new System.NotImplementedException();
    }
}
