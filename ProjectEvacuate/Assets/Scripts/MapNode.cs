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
}