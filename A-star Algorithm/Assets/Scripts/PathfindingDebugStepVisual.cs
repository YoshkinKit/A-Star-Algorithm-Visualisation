/*
 * В цьому класі знаходяться методи для
 * візуалізації роботи алгоритмів пошуку
 */

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PathfindingDebugStepVisual : MonoBehaviour
{
    public static PathfindingDebugStepVisual Instance { get; private set; }

    [SerializeField] private Transform pfPathfindingDebugStepVisualNode;

    private List<Transform> _visualNodeList;
    private List<GridSnapshotAction> _gridSnapshotActionList;
    private bool _autoShowSnapshots;
    private float _autoShowSnapshotsTimer = 0.01f;
    private float _autoShowSnapshotsTimerMax;
    private Transform[,] _visualNodeArray;

    private void Awake()
    {
        Instance = this;
        _visualNodeList = new List<Transform>();
        _gridSnapshotActionList = new List<GridSnapshotAction>();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextSnapshot();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _autoShowSnapshots = !_autoShowSnapshots;
        }

        if (!_autoShowSnapshots)
        {
            return;
        }

        _autoShowSnapshotsTimer -= Time.deltaTime;
        if (_autoShowSnapshotsTimer > 0f)
        {
            return;
        }

        _autoShowSnapshotsTimer += _autoShowSnapshotsTimerMax;

        ShowNextSnapshot();
        
        if (_gridSnapshotActionList.Count == 0)
        {
            _autoShowSnapshots = false;
        }
    }

    public void Setup(Grid<PathNode> grid)
    {
        _visualNodeArray = new Transform[grid.GetWidth(), grid.GetHeight()];

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Vector3 gridPosition = new Vector3(x, y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * 0.5f;
                Transform visualNode = CreateVisualNode(gridPosition);
                _visualNodeArray[x, y] = visualNode;
                _visualNodeList.Add(visualNode);
            }
        }

        HideNodeVisuals();
    }

    private void ShowNextSnapshot()
    {
        if (_gridSnapshotActionList.Count > 0)
        {
            GridSnapshotAction gridSnapshotAction = _gridSnapshotActionList[0];
            _gridSnapshotActionList.RemoveAt(0);
            gridSnapshotAction.TriggerAction();
        }
    }

    public void ClearSnapshots()
    {
        _gridSnapshotActionList.Clear();
    }

    public void TakeSnapshot(Grid<PathNode> grid, PathNode current, List<PathNode> openList, HashSet<PathNode> closedList)
    {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        gridSnapshotAction.AddAction(HideNodeVisuals);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);

                int gCost = pathNode.gCost;
                int hCost = pathNode.hCost;
                int fCost = pathNode.fCost;
                
                bool isCurrent = pathNode == current;
                bool isInOpenList = openList.Contains(pathNode);
                bool isInClosedList = closedList.Contains(pathNode);
                
                bool isStart = pathNode.isStart;
                bool isEnd = pathNode.isEnd;

                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() =>
                {
                    Transform visualNode = _visualNodeArray[tmpX, tmpY];
                    SetupVisualNode(visualNode, gCost, hCost, fCost);
                    Color backgroundColor = GetBackgroundColor(isStart, isEnd, isInClosedList, isInOpenList, isCurrent);

                    visualNode.Find("sprite").GetComponent<SpriteRenderer>().color = backgroundColor;
                });
            }
        }

        _gridSnapshotActionList.Add(gridSnapshotAction);
    }

    private Color GetBackgroundColor(bool isStart, bool isEnd, bool isInClosedList = false, bool isInOpenList = false,
        bool isCurrent = false, bool isInPath = false)
    {
        Color backgroundColor = Utils.GetColorFromString("636363");

        if (isInClosedList)
        {
            backgroundColor = Utils.GetColorFromString("A62E20");
        }
        else if (isInOpenList)
        {
            backgroundColor = Utils.GetColorFromString("1E3085");
        }

        if (isCurrent || isInPath)
        {
            backgroundColor = Utils.GetColorFromString("018F3E");
        }

        if (isStart)
        {
            backgroundColor = Utils.GetColorFromString("E5C331");
        }
        else if (isEnd)
        {
            backgroundColor = Utils.GetColorFromString("69146B");
        }

        return backgroundColor;
    }

    public void SetStartColor(PathNode pathNode)
    {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();

        gridSnapshotAction.AddAction(() =>
        {
            Transform visualNode = _visualNodeArray[pathNode.x, pathNode.y];
            Color backgroundColor = GetBackgroundColor(pathNode.isStart, false);

            visualNode.Find("sprite").GetComponent<SpriteRenderer>().color = backgroundColor;
        });

        gridSnapshotAction.TriggerAction();
    }

    public void SetEndColor(PathNode pathNode)
    {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();

        gridSnapshotAction.AddAction(() =>
        {
            Transform visualNode = _visualNodeArray[pathNode.x, pathNode.y];
            Color backgroundColor = GetBackgroundColor(false, pathNode.isEnd);

            visualNode.Find("sprite").GetComponent<SpriteRenderer>().color = backgroundColor;
        });

        gridSnapshotAction.TriggerAction();
    }
    
    public void TakeSnapshotFinalPath(Grid<PathNode> grid, List<PathNode> path)
    {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        gridSnapshotAction.AddAction(HideNodeVisuals);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);

                int gCost = pathNode.gCost;
                int hCost = pathNode.hCost;
                int fCost = pathNode.fCost;
                
                bool isInPath = path.Contains(pathNode);

                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() =>
                {
                    Transform visualNode = _visualNodeArray[tmpX, tmpY];
                    SetupVisualNode(visualNode, gCost, hCost, fCost);
                    Color backgroundColor = GetBackgroundColor(pathNode.isStart, pathNode.isEnd, isInPath: isInPath);

                    visualNode.Find("sprite").GetComponent<SpriteRenderer>().color = backgroundColor;
                });
            }
        }

        _gridSnapshotActionList.Add(gridSnapshotAction);
    }

    private void HideNodeVisuals()
    {
        foreach (Transform visualNodeTransform in _visualNodeList)
        {
            SetupVisualNode(visualNodeTransform, 9999, 9999, 9999);
        }
    }

    public void ClearColor(Grid<PathNode> grid)
    {
        HideNodeVisuals();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);

                if (pathNode.isStart || pathNode.isEnd)
                {
                    continue;
                }

                Transform visualNode = _visualNodeArray[pathNode.x, pathNode.y];
                Color backgroundColor = GetBackgroundColor(false, false);

                visualNode.Find("sprite").GetComponent<SpriteRenderer>().color = backgroundColor;
            }
        }
    }

    private Transform CreateVisualNode(Vector3 position)
    {
        Transform visualNodeTransform = Instantiate(pfPathfindingDebugStepVisualNode, position, Quaternion.identity);
        return visualNodeTransform;
    }

    private void SetupVisualNode(Transform visualNodeTransform, int gCost, int hCost, int fCost)
    {
        if (fCost < 1000)
        {
            visualNodeTransform.Find("gCostText").GetComponent<TextMeshPro>().SetText(gCost.ToString());
            visualNodeTransform.Find("hCostText").GetComponent<TextMeshPro>().SetText(hCost.ToString());
            visualNodeTransform.Find("fCostText").GetComponent<TextMeshPro>().SetText(fCost.ToString());
        }
        else
        {
            visualNodeTransform.Find("gCostText").GetComponent<TextMeshPro>().SetText("");
            visualNodeTransform.Find("hCostText").GetComponent<TextMeshPro>().SetText("");
            visualNodeTransform.Find("fCostText").GetComponent<TextMeshPro>().SetText("");
        }
    }

    private class GridSnapshotAction
    {
        private Action _action = () => { };

        public void AddAction(Action action)
        {
            _action += action;
        }

        public void TriggerAction()
        {
            _action();
        }
    }

    public void SetAutoShowSnapshotsTimerMax(float value)
    {
        _autoShowSnapshotsTimer = 0f;
        _autoShowSnapshotsTimerMax = value;
    }
}