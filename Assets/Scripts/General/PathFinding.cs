using System.Collections.Generic;
using System.IO;
using Pathfinding;
using UnityEngine;
using Path = System.IO.Path;

namespace General
{
    public class PathFinding
    {

        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private Grid<PathNode> _grid;
        private List<PathNode> _openList;
        private List<PathNode> _closedList;
        
        public static PathFinding Instance {private set; get;}

        public PathFinding(int width, int height)
        {
            _grid = new Grid<PathNode>(width, height, 2, Vector3.zero,
                (g, x, y) => new PathNode(g, x, y), true);
            Instance = this;
        }

        public Grid<PathNode> GetGrid()
        {
            return _grid;
        }

        public List<Vector3> FindPath(Vector3 worldStartPos, Vector3 worldEndPos)
        {
            _grid.GetXY(worldStartPos, out int sX, out int sY);
            _grid.GetXY(worldEndPos, out int eX, out int eY);

            List<PathNode> path = FindPath(new Vector2Int(sX, sY), new Vector2Int(eX, eY));
            if (path == null) return null;
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode node in path)
            {
                vectorPath.Add(new Vector3(node.x, node.y) * _grid.GetCellSize() + Vector3.one * _grid.GetCellSize() / 2);
            }
            return vectorPath;

        }

        public List<PathNode> FindPath(Vector2Int start, Vector2Int end)
        {
            PathNode startNode = _grid.GetGridObject(start.x, start.y);
            PathNode endNode = _grid.GetGridObject(end.x, end.y);

            _openList = new List<PathNode> { startNode };
            _closedList = new List<PathNode>();

            for (int x = 0; x < _grid.GetWidth(); x++)
            {
                for (int y = 0; y < _grid.GetHeight(); y++)
                {
                    PathNode pathNode = _grid.GetGridObject(x, y);
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.PreviousNode = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistance(startNode, endNode);
            startNode.CalculateFCost();

            while (_openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCost(_openList);

                if (currentNode == endNode) return CalculatePath(endNode);

                _openList.Remove(currentNode);
                _closedList.Add(currentNode);

                foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                {
                    if (_closedList.Contains(neighbourNode)) continue;
                    if (!neighbourNode.isWalkable)
                    {
                        _closedList.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.PreviousNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistance(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();
                        
                        if (!_openList.Contains(neighbourNode))
                            _openList.Add(neighbourNode);
                    }
                }

            }

            return null;
        }

        private List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);

            PathNode currentNode = endNode;
            while (currentNode.PreviousNode != null)
            {
                currentNode = currentNode.PreviousNode;
                path.Add(currentNode);
            }

            path.Reverse();
            return path;
        }

        private List<PathNode> GetNeighbourList(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();
            if (currentNode.x - 1 >= 0)
            {
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
                if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
                if (currentNode.y + 1 < _grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
            }
            
            if (currentNode.x + 1 < _grid.GetWidth())
            {
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
                if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
                if (currentNode.y + 1 < _grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
            }
            
            
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
            if (currentNode.y + 1 < _grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

            return neighbourList;
        }

        private PathNode GetNode(int x, int y)
        {
            return _grid.GetGridObject(x, y);
        }

        

        private PathNode GetLowestFCost(List<PathNode> pathNodes)
        {
            PathNode lowest = pathNodes[0];
            foreach (PathNode node in pathNodes)
            {
                if (node.fCost < lowest.fCost)
                {
                    lowest = node;
                }
            }

            return lowest;
        }

        private int CalculateDistance(PathNode a, PathNode b)
        {
            int xDis = Mathf.Abs(a.x - b.x);
            int yDis = Mathf.Abs(a.y - b.y);

            int remaining = Mathf.Abs(xDis - yDis);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDis, yDis) + MOVE_STRAIGHT_COST * remaining;
        }
    }
}