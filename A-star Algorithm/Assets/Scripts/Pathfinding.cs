/*
 * В цьому класі знаходяться алгоритми пошуку
 * Та методи для їх коректної роботи
 */

using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private const int MoveStraightCost = 10;
    private const int MoveDiagonalCost = 14; // Отримується за допомогою теореми Піфагора
    private static int _algorithmNumber;
    private static bool _diagonalNeighbours = true;

    public static Pathfinding Instance { get; private set; }

    private Grid<PathNode> _grid;
    private List<PathNode> _openList;
    private HashSet<PathNode> _closedList;

    public Pathfinding(int width, int height)
    {
        Instance = this;
        _grid = new Grid<PathNode>(width, height, 10f, Vector3.zero, (grid, x, y) => new PathNode(grid, x, y));
    }

    public Grid<PathNode> GetGrid()
    {
        return _grid;
    }

    private List<PathNode> AStarAlgorithm(int startX, int startY, int endX, int endY)
    {
        // var startTime = Time.realtimeSinceStartup;

        PathNode startNode = _grid.GetGridObject(startX, startY);
        PathNode endNode = _grid.GetGridObject(endX, endY);

        if (startNode == null || endNode == null)
        {
            return null;
        }

        _openList = new List<PathNode> { startNode };
        _closedList = new HashSet<PathNode>();

        for (int x = 0; x < _grid.GetWidth(); x++)
        {
            for (int y = 0; y < _grid.GetHeight(); y++)
            {
                PathNode pathNode = _grid.GetGridObject(x, y);
                
                pathNode.gCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);
        startNode.CalculateFCost();

        PathfindingDebugStepVisual.Instance.ClearSnapshots();
        PathfindingDebugStepVisual.Instance.TakeSnapshot(_grid, startNode, _openList, _closedList);

        while (_openList.Count > 0)
        {
            PathNode currentNode = GetLowestCostNode(_openList, Costs.FCost);
            if (currentNode == endNode)
            {
                // Дійшли до кінечного вузла
                // Debug.Log($"A* algorithm: {Time.realtimeSinceStartup - startTime}");

                PathfindingDebugStepVisual.Instance.TakeSnapshot(_grid, currentNode, _openList, _closedList);
                PathfindingDebugStepVisual.Instance.TakeSnapshotFinalPath(_grid, CalculatePath(endNode));
                return CalculatePath(endNode);
            }

            _openList.Remove(currentNode);
            _closedList.Add(currentNode);

            foreach (var neighbourNode in GetNeighbourList(currentNode, _diagonalNeighbours))
            {
                if (_closedList.Contains(neighbourNode))
                {
                    continue;
                }

                if (!neighbourNode.isWalkable)
                {
                    _closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistance(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!_openList.Contains(neighbourNode))
                    {
                        _openList.Add(neighbourNode);
                    }
                }

                PathfindingDebugStepVisual.Instance.TakeSnapshot(_grid, currentNode, _openList, _closedList);
            }
        }

        // Шлях не було знайдено
        PathfindingDebugStepVisual.Instance.ClearSnapshots();
        return null;
    }

    private List<PathNode> DijkstraAlgorithm(int startX, int startY, int endX, int endY)
    {
        // var startTime = Time.realtimeSinceStartup;
    
        PathNode startNode = _grid.GetGridObject(startX, startY);
        PathNode endNode = _grid.GetGridObject(endX, endY);
    
        if (startNode == null || endNode == null)
        {
            return null;
        }
    
        _openList = new List<PathNode>{ startNode };
        _closedList = new HashSet<PathNode>();
    
        for (int x = 0; x < _grid.GetWidth(); x++)
        {
            for (int y = 0; y < _grid.GetHeight(); y++)
            {
                PathNode pathNode = _grid.GetGridObject(x, y);
                
                pathNode.gCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }
    
        startNode.gCost = 0;
        startNode.hCost = 0;
        startNode.CalculateFCost();
    
        PathfindingDebugStepVisual.Instance.ClearSnapshots();
        PathfindingDebugStepVisual.Instance.TakeSnapshot(_grid, startNode, _openList, _closedList);
    
        while (_openList.Count > 0)
        {
            PathNode currentNode = GetLowestCostNode(_openList, Costs.FCost);
            if (currentNode == endNode)
            {
                // Дійшли до кінечного вузла
                // Debug.Log($"Dijkstra algorithm: {Time.realtimeSinceStartup - startTime}");
    
                PathfindingDebugStepVisual.Instance.TakeSnapshot(_grid, currentNode, _openList, _closedList);
                PathfindingDebugStepVisual.Instance.TakeSnapshotFinalPath(_grid, CalculatePath(endNode));
                return CalculatePath(endNode);
            }
    
            _openList.Remove(currentNode);
            _closedList.Add(currentNode);
    
            foreach (var neighbourNode in GetNeighbourList(currentNode, _diagonalNeighbours))
            {
                if (_closedList.Contains(neighbourNode))
                {
                    continue;
                }
    
                if (!neighbourNode.isWalkable)
                {
                    _closedList.Add(neighbourNode);
                    continue;
                }
    
                int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = 0;
                    neighbourNode.CalculateFCost();
                }
                
                if (!_openList.Contains(neighbourNode))
                {
                    _openList.Add(neighbourNode);
                }
    
                PathfindingDebugStepVisual.Instance.TakeSnapshot(_grid, currentNode, _openList, _closedList);
            }
        }
    
        // Шлях не було знайдено
        PathfindingDebugStepVisual.Instance.ClearSnapshots();
        return null;
    }
    
    private List<PathNode> BreadthFirstSearch(int startX, int startY, int endX, int endY)
    {
        // var startTime = Time.realtimeSinceStartup;
    
        PathNode startNode = _grid.GetGridObject(startX, startY);
        PathNode endNode = _grid.GetGridObject(endX, endY);
    
        if (startNode == null || endNode == null)
        {
            return null;
        }
    
        _openList = new List<PathNode> { startNode };
        _closedList = new HashSet<PathNode>();
    
        for (int x = 0; x < _grid.GetWidth(); x++)
        {
            for (int y = 0; y < _grid.GetHeight(); y++)
            {
                PathNode pathNode = _grid.GetGridObject(x, y);
    
                pathNode.gCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }
    
        startNode.gCost = 99999999;
        startNode.CalculateFCost();
    
        PathfindingDebugStepVisual.Instance.ClearSnapshots();
        PathfindingDebugStepVisual.Instance.TakeSnapshot(_grid, startNode, _openList, _closedList);
    
        while (_openList.Count > 0)
        {
            PathNode currentNode = _openList[0];
            
            if (currentNode == endNode)
            {
                // Дійшли до кінцевого вузла
                // Debug.Log($"Breadth-first search: {Time.realtimeSinceStartup - startTime}");
                
                PathfindingDebugStepVisual.Instance.TakeSnapshot(_grid, currentNode, _openList, _closedList);
                PathfindingDebugStepVisual.Instance.TakeSnapshotFinalPath(_grid, CalculatePath(endNode));
                return CalculatePath(endNode);
            }
    
            _openList.Remove(currentNode);
            _closedList.Add(currentNode);
    
            foreach (var neighbourNode in GetNeighbourList(currentNode, _diagonalNeighbours))
            {
                if (_closedList.Contains(neighbourNode) || _openList.Contains(neighbourNode))
                {
                    continue;
                }
    
                if (!neighbourNode.isWalkable)
                {
                    _closedList.Add(neighbourNode);
                    continue;
                }
                
                neighbourNode.cameFromNode = currentNode;
                _openList.Add(neighbourNode);
                
                PathfindingDebugStepVisual.Instance.TakeSnapshot(_grid, currentNode, _openList, _closedList);
            }
        }
    
        // Шлях не було знайдено
        PathfindingDebugStepVisual.Instance.ClearSnapshots();
        return null;
    }
    
    private List<PathNode> BestFirstSearch(int startX, int startY, int endX, int endY)
    {
        // var startTime = Time.realtimeSinceStartup;
    
        PathNode startNode = _grid.GetGridObject(startX, startY);
        PathNode endNode = _grid.GetGridObject(endX, endY);
    
        if (startNode == null || endNode == null)
        {
            return null;
        }
    
        _openList = new List<PathNode> { startNode };
        _closedList = new HashSet<PathNode>();
    
        for (int x = 0; x < _grid.GetWidth(); x++)
        {
            for (int y = 0; y < _grid.GetHeight(); y++)
            {
                PathNode pathNode = _grid.GetGridObject(x, y);
                
                pathNode.gCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }
    
        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);
        startNode.CalculateFCost();
    
        PathfindingDebugStepVisual.Instance.ClearSnapshots();
        PathfindingDebugStepVisual.Instance.TakeSnapshot(_grid, startNode, _openList, _closedList);
    
        while (_openList.Count > 0)
        {
            PathNode currentNode = GetLowestCostNode(_openList, Costs.HCost);
            if (currentNode == endNode)
            {
                // Дійшли до кінечного вузла
                // Debug.Log($"Best-first search: {Time.realtimeSinceStartup - startTime}");
    
                PathfindingDebugStepVisual.Instance.TakeSnapshot(_grid, currentNode, _openList, _closedList);
                PathfindingDebugStepVisual.Instance.TakeSnapshotFinalPath(_grid, CalculatePath(endNode));
                return CalculatePath(endNode);
            }
    
            _openList.Remove(currentNode);
            _closedList.Add(currentNode);
    
            foreach (var neighbourNode in GetNeighbourList(currentNode, _diagonalNeighbours))
            {
                if (_closedList.Contains(neighbourNode))
                {
                    continue;
                }
    
                if (!neighbourNode.isWalkable)
                {
                    _closedList.Add(neighbourNode);
                    continue;
                }
    
                int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistance(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();
    
                    if (!_openList.Contains(neighbourNode))
                    {
                        _openList.Add(neighbourNode);
                    }
                }
    
                PathfindingDebugStepVisual.Instance.TakeSnapshot(_grid, currentNode, _openList, _closedList);
            }
        }
    
        // Шлях не було знайдено
        PathfindingDebugStepVisual.Instance.ClearSnapshots();
        return null;
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        return _algorithmNumber switch
        {
            0 => AStarAlgorithm(startX, startY, endX, endY),
            1 => DijkstraAlgorithm(startX, startY, endX, endY),
            2 => BreadthFirstSearch(startX, startY, endX, endY),
            3 => BestFirstSearch(startX, startY, endX, endY),
            _ => null
        };
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode, bool diagonalNeighbors = true)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        
        // Верх
        if (currentNode.y + 1 < _grid.GetHeight())
        {
            neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));
        }
        
        // Право
        if (currentNode.x + 1 < _grid.GetWidth())
        {
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
        }
        
        // Низ
        if (currentNode.y - 1 >= 0)
        {
            neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        }

        // Ліво
        if (currentNode.x - 1 >= 0)
        {
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
        }

        if (!diagonalNeighbors)
        {
            return neighbourList;
        }

        // Ліво верх
        if (currentNode.x - 1 >= 0 && currentNode.y + 1 < _grid.GetHeight())
        {
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }

        // Право верх
        if (currentNode.x + 1 < _grid.GetWidth() && currentNode.y + 1 < _grid.GetHeight())
        {
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        
        // Право низ
        if (currentNode.x + 1 < _grid.GetWidth() && currentNode.y - 1 >= 0)
        {
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
        }
        
        // Ліво низ
        if (currentNode.x - 1 >= 0 && currentNode.y - 1 >= 0)
        {
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
        }
        
        return neighbourList;
    }

    private PathNode GetNode(int x, int y)
    {
        return _grid.GetGridObject(x, y);
    }

    private static List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode> { endNode };

        PathNode currentNode = endNode.cameFromNode;
        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();
        return path;
    }

    // Метод для отримання октантної відстані (octile distance)
    private static int CalculateDistance(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return MoveDiagonalCost * Mathf.Min(xDistance, yDistance) + MoveStraightCost * remaining;
    }
    
    // Метод для отримання вузла з найменьшою вагою (залежить від параметра Costs cost)
    private static PathNode GetLowestCostNode(List<PathNode> pathNodeList, Costs cost)
    {
        PathNode lowestCostNode = pathNodeList[0];

        foreach (var pathNode in pathNodeList)
        {
            switch (cost)
            {
                case Costs.FCost:
                {
                    if (pathNode.fCost < lowestCostNode.fCost)
                    {
                        lowestCostNode = pathNode;
                    }

                    break;
                }
                case Costs.HCost:
                {
                    if (pathNode.hCost < lowestCostNode.hCost)
                    {
                        lowestCostNode = pathNode;
                    }
                    break;
                }
            }
        }

        return lowestCostNode;
    }

    public static void SetAlgorithmNumber(int number)
    {
        _algorithmNumber = number;
    }

    public static void SetDiagonalNeighbours(bool diagonalNeighbours)
    {
        _diagonalNeighbours = diagonalNeighbours;
    }
    
    private enum Costs
    {
        FCost,
        HCost
    }
}