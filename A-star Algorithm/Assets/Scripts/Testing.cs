/*
 * В цьому класі знаходиться основна
 * логіка додатку. В ньому обробюються
 * введення користувача
 */

using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private PathfindingVisual pathfindingVisual;
    [SerializeField] private PathfindingDebugStepVisual pathfindingDebugStepVisual;
    [SerializeField] private Camera cam;

    private static bool _anyStartNode;
    private static bool _anyEndNode;

    private LineRenderer _lineRenderer;
    private Pathfinding _pathfinding;
    private PathNode _startNode;
    private PathNode _endNode;

    private void Start()
    {
        _pathfinding = new Pathfinding(20, 15);
        Grid<PathNode> grid = _pathfinding.GetGrid();

        cam.transform.position = new Vector3((grid.GetWidth() / 2f + 3f) * grid.GetCellSize(),
            grid.GetHeight() / 2f * grid.GetCellSize(), -10);

        cam.GetComponent<Camera>().orthographicSize = grid.GetHeight() * grid.GetCellSize() / 2;

        pathfindingDebugStepVisual.Setup(_pathfinding.GetGrid());
        pathfindingVisual.SetGrid(_pathfinding.GetGrid());

        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();

            _pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            PathNode pathNode = _pathfinding.GetGrid().GetGridObject(mouseWorldPosition);

            if (pathNode == _startNode || pathNode == _endNode)
            {
                return;
            }
            
            _pathfinding.GetGrid().GetGridObject(x, y)?.SetIsWalkable();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
            PathNode pathNode = _pathfinding.GetGrid().GetGridObject(mouseWorldPosition);

            if (pathNode is not { isWalkable: true } || pathNode == _endNode)
            {
                return;
            }

            if (!_anyStartNode)
            {
                _startNode = pathNode;
                _startNode.isStart = true;
                _anyStartNode = true;
            }
            else
            {
                if (_startNode == pathNode)
                {
                    _startNode.isStart = false;
                    _startNode = null;
                    _anyStartNode = false;
                }
            }

            pathfindingDebugStepVisual.SetStartColor(pathNode);
        }

        if (Input.GetMouseButtonDown(2))
        {
            Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
            PathNode pathNode = _pathfinding.GetGrid().GetGridObject(mouseWorldPosition);

            if (pathNode is not { isWalkable: true } || pathNode == _startNode)
            {
                return;
            }

            if (!_anyEndNode)
            {
                _endNode = pathNode;
                _endNode.isEnd = true;
                _anyEndNode = true;
            }
            else
            {
                if (_endNode == pathNode)
                {
                    _endNode.isEnd = false;
                    _endNode = null;
                    _anyEndNode = false;
                }
            }

            pathfindingDebugStepVisual.SetEndColor(pathNode);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_anyStartNode && _anyEndNode)
            {
                List<PathNode> path = _pathfinding.FindPath(_startNode.x, _startNode.y, _endNode.x, _endNode.y);

                if (path == null)
                {
                    return;
                }

                DrawLine(path);
            }
        }
    }

    private void DrawLine(List<PathNode> path)
    {
        _lineRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            _lineRenderer.SetPosition(i, new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f + Vector3.back);
        }
    }
    
    public void ClearWalls()
    {
        Grid<PathNode> grid = _pathfinding.GetGrid();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);

                pathNode.isWalkable = true;
                grid.TriggerGridObjectChanged(x, y);
            }
        }
    }

    public void ClearPath()
    {
        pathfindingDebugStepVisual.ClearColor(_pathfinding.GetGrid());
        ClearLine();
    }
    
    private void ClearLine()
    {
        _lineRenderer.positionCount = 0;
    }
}