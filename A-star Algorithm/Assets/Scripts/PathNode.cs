/*
 * Цей клас моделює вузол на сітці
 */

public class PathNode
{
    private readonly Grid<PathNode> _grid;
    
    public readonly int x;
    public readonly int y;

    public int gCost; // Відстань від початкового вузла
    public int hCost; // Еврістика
    public int fCost; // Сума gCost та hCost

    public bool isWalkable;
    public bool isStart;
    public bool isEnd;
    public PathNode cameFromNode;

    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        _grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public void SetIsWalkable()
    {
        isWalkable = !isWalkable;
        _grid.TriggerGridObjectChanged(x, y);
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}