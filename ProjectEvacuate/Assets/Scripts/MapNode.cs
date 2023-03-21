using System;
using UnityEngine;
using System.Collections.Generic;

public class MapNode
{
    public int gridX;
    public int gridY;
    public bool walkable;
    public int gCost;
    public int hCost;
    public MapNode parent;

    public MapNode(bool _walkable, int _gridX, int _gridY)
    {
        walkable = _walkable;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int FCost
    {
        get { return gCost + hCost; }
    }

    public List<MapNode> GetNeighbors()
    {
        return MapMgr.MapInstance.GetNeighbors(this);
    }

    public void Print()
    {
        Debug.Log(String.Concat("x:", gridX.ToString(), " y:", gridY.ToString()));
    }

    public Vector3 getMapNodeWorldPosition()
    {
        return MapMgr.MapInstance.getMapNodeWorldPosition(this);
    }
}