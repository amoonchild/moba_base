using System;
using System.Collections.Generic;


namespace PathFinder
{
    public class PathFinder<T> where T : INode
    {
        private HashSet<int> _closedSet = new HashSet<int>();
        private List<T> _neighbours = new List<T>();


        private int GetDistance(T nodeA, T nodeB)
        {
            int dstX = Math.Abs(nodeA.X - nodeB.X);
            int dstY = Math.Abs(nodeA.Y - nodeB.Y);
            return (dstX > dstY) ?
                14 * dstY + 10 * (dstX - dstY) :
                14 * dstX + 10 * (dstY - dstX);
        }

        private void RetracePath(T startNode, T endNode, List<T> result)
        {
            result.Clear();
            T currentNode = endNode;

            while ((INode)currentNode != (INode)startNode)
            {
                result.Add(currentNode);
                currentNode = (T)currentNode.ParentNode;
            }

            result.Reverse();
        }

        public List<T> FindPath(IGrid<T> grid, int startX, int startY, int endX, int endY, DistanceType distance = DistanceType.Manhattan)
        {
            T startNode = grid[startX, startY];
            if (startNode == null)
            {
                return null;
            }
            T targetNode = grid[endX, endY];
            if (targetNode == null)
            {
                return null;
            }

            return FindPath(grid, startNode, targetNode, distance);
        }

        public List<T> FindPath(IGrid<T> grid, T startNode, T endNode, DistanceType distance = DistanceType.Manhattan)
        {
            List<T> openSet = new List<T>();
            FindPath(grid, startNode, endNode, openSet, distance);
            return openSet;
        }

        public void FindPath(IGrid<T> grid, int startX, int startY, int endX, int endY, List<T> result, DistanceType distance = DistanceType.Manhattan)
        {
            T startNode = grid[startX, startY];
            if (startNode == null)
            {
                return;
            }
            T targetNode = grid[endX, endY];
            if (targetNode == null)
            {
                return;
            }

            FindPath(grid, startNode, targetNode, result, distance);
        }
        public void FindPath(IGrid<T> grid, T startNode, T endNode, List<T> result, DistanceType distance = DistanceType.Manhattan)
        {
            result.Clear();

            if (startNode == null || endNode == null)
            {
                return;
            }

            _closedSet.Clear();

            result.Add(startNode);

            while (result.Count > 0)
            {
                T currentNode = result[0];
                for (int i = 1; i < result.Count; i++)
                {
                    T checkNode = result[i];
                    if (checkNode.F < currentNode.F || (checkNode.F == currentNode.F && checkNode.H < currentNode.H))
                    {
                        currentNode = checkNode;
                    }
                }

                result.Remove(currentNode);
                _closedSet.Add(currentNode.Index);

                if ((INode)currentNode == (INode)endNode)
                {
                    RetracePath(startNode, endNode, result);
                    return;
                }

                grid.GetNeighbours(currentNode.X, currentNode.Y, distance, 1, _neighbours, _closedSet);
                foreach (T neighbour in _neighbours)
                {
                    if (((INode)neighbour != (INode)startNode && (INode)neighbour != (INode)endNode && !neighbour.IsWalkable))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.G + GetDistance(currentNode, neighbour);
                    bool inOpenSet = result.Contains(neighbour);
                    if (newMovementCostToNeighbour < neighbour.G || !inOpenSet)
                    {
                        neighbour.G = newMovementCostToNeighbour;
                        neighbour.H = GetDistance(neighbour, endNode);
                        neighbour.ParentNode = currentNode;

                        if (!inOpenSet)
                        {
                            result.Add(neighbour);
                        }
                    }
                }
            }

            result.Clear();
        }
    }
}