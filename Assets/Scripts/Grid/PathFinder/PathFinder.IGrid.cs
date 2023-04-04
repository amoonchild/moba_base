using System.Collections.Generic;


namespace PathFinder
{
    /// <summary>
	/// map
	/// </summary>
	/// <typeparam name="T">node type</typeparam>
    public interface IGrid<T> where T : INode
    {
        int Width
        {
            get;
        }

        int Height
        {
            get;
        }

        int MaxSize
        {
            get;
        }

        T this[int x, int y]
        {
            get;
        }

        bool IsNodeInGrid(int x, int y);
        bool IsNodeWalkable(int x, int y);

        void SetNodeWalkable(int x, int y, bool isWalkable);
        void SetAllNodesWalkable(bool isWalkable);

        void GetNeighbours(int x, int y, List<T> result);
        void GetNeighbours(int x, int y, DistanceType distanceType, List<T> result);
        void GetNeighbours(int x, int y, DistanceType distanceType, int range, List<T> result);
        void GetNeighbours(int x, int y, DistanceType distanceType, int range, List<T> result, HashSet<int> excludedNodes);

        void GetWalkableNeighbours(int x, int y, List<T> result);
        void GetWalkableNeighbours(int x, int y, DistanceType distanceType, List<T> result);
        void GetWalkableNeighbours(int x, int y, DistanceType distanceType, int range, List<T> result, HashSet<int> excludedNodes);
    }
}