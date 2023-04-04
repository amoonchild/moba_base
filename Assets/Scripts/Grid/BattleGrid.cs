using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LiaoZhai.Runtime
{
    public class BattleGrid : IGrid<BattleNode>
    {
        public class PathState
        {
            public int stateIndex = 0;
            public List<BattleNode> pathNodes = new List<BattleNode>();
        }

        private DFixVector3 _centerPosition = DFixVector3.Zero;
        private DFixVector3 _startPosition = DFixVector3.Zero;
        private DFix64 _nodeSize = DFix64.Zero;
        private BattleNode[,] _nodes = null;                        // 
        private int xMax = 0;
        private int yMax = 0;
        private PathFinder<BattleNode> _aStarPathFinder = null;

        public int Width
        {
            get
            {
                return _nodes.GetLength(1);
            }
        }
        public int Height
        {
            get
            {
                return _nodes.GetLength(0);
            }
        }
        public int MaxSize
        {
            get
            {
                return Width > Height ? Width : Height;
            }
        }
        public DFix64 NodeSize
        {
            get { return _nodeSize; }
        }
        public DFixVector3 CenterPosition
        {
            get
            {
                return _centerPosition;
            }
        }
        public BattleNode this[int x, int y]
        {
            get
            {
                if (!IsNodeInGrid(x, y))
                {
                    return null;
                }

                return _nodes[x, y];
            }
        }
        public int nodeStateIndex = 0;


        public BattleGrid(int column, int row, DFixVector3 centerPosition, DFix64 nodeSize)
        {
            float gridSize = (float)nodeSize * 100f;

            _centerPosition = centerPosition;

            _nodeSize = nodeSize;
            _startPosition = new DFixVector3(_centerPosition.x - _nodeSize * (DFix64)column / (DFix64)2f + _nodeSize / (DFix64)2f, _centerPosition.y, _centerPosition.z + _nodeSize * (DFix64)row / (DFix64)2f - _nodeSize / (DFix64)2f);

            _nodes = new BattleNode[row, column];
            xMax = row;
            yMax = column;

            for (int i = 0; i < row * column; i++)
            {
                int columnN = i % column;
                int rowN = i / column;

                _nodes[rowN, columnN] = new BattleNode(this, i, rowN, columnN, new DFixVector3(_startPosition.x + _nodeSize * (DFix64)columnN, _startPosition.y, _startPosition.z - _nodeSize * (DFix64)rowN));
                _nodes[rowN, columnN].IsWalkable = true;
            }

            _aStarPathFinder = new PathFinder<BattleNode>();
        }

        public void InitArea(bool isShowGround)
        {
            _isShowAreaGround = isShowGround;

            _quyuObj = GameManager.PrefabPool.Spawn<PrefabPoolObject>(Constant.InstanceObject.BATTLE_AREA, null);
            if (_quyuObj != null)
            {
                _quyuObj.gameObject.SetActive(true);
                _quyuObj.transform.SetLocalPositionXYZ(Vector3.zero);
                _quyuObj.transform.SetLocalEulerAngleXYZ(Vector3.zero);
                _quyuObj.transform.SetLocalScaleXYZ(Vector3.one);

                Transform quyu = _quyuObj.transform;

                _selfLineupAreaEffect = quyu.Find("quyu");
                _dragStartEffect = quyu.Find("xuanzhong1");
                _dragEndEffect = quyu.Find("xuanzhong2");
                _dragArrowEffect = quyu.Find("jiantou");
            }

            HideSelfLineupArea();
            HideDragStartEffect();
            HideDragEndEffect();
            HideDragArrowEffect();
        }

        public void Destroy()
        {
            if (_quyuObj != null)
            {
                GameManager.PrefabPool.Unspawn(_quyuObj);
                _quyuObj = null;
            }
        }

        public void Release()
        {
            //GameObject.DestroyImmediate(_gridObj);
            _gridObj = null;
            _nodes = null;
            _cachedPaths.Clear();
        }

        public bool IsNodeInGrid(int x, int y)
        {
            if (x < 0 || x >= xMax || y < 0 || y >= yMax)
            {
                return false;
            }

            return true;
        }
        public bool IsNodeWalkable(int x, int y)
        {
            if (!IsNodeInGrid(x, y))
            {
                return false;
            }

            return _nodes[x, y].IsWalkable;
        }

        public BattleNode FindNode(DFixVector3 position)
        {
            DFixVector3 offset = position - _startPosition;

            offset.x = offset.x / _nodeSize;
            offset.z = DFix64.Abs(offset.z / _nodeSize);

            int x = -1;
            int y = -1;

            if (offset.z - DFix64.Floor(offset.z) >= DFix64.Half)
            {
                x = (int)DFix64.Ceiling(offset.z);
            }
            else
            {
                x = (int)DFix64.Floor(offset.z);
            }

            if (offset.x - DFix64.Floor(offset.x) >= DFix64.Half)
            {
                y = (int)DFix64.Ceiling(offset.x);
            }
            else
            {
                y = (int)DFix64.Floor(offset.x);
            }

            if (!IsNodeInGrid(x, y))
            {
                return null;
            }

            return this[x, y] as BattleNode;
        }

        public void SetNodeWalkable(int x, int y, bool walkable)
        {
            if (!IsNodeInGrid(x, y))
            {
                return;
            }

            _nodes[x, y].IsWalkable = walkable;
        }
        public void SetAllNodesWalkable(bool walkable)
        {
            for (int x = 0; x < _nodes.GetLength(0); x++)
            {
                for (int y = 0; y < _nodes.GetLength(1); y++)
                {
                    _nodes[x, y].IsWalkable = walkable;
                }
            }
        }

        public void GetNeighbours(int x, int y, List<BattleNode> result)
        {
            GetNeighbours(x, y, DistanceType.Euclidean, 1, result);
        }
        public void GetNeighbours(int x, int y, DistanceType distanceType, List<BattleNode> result)
        {
            GetNeighbours(x, y, DistanceType.Euclidean, 1, result);
        }
        public void GetNeighbours(int x, int y, DistanceType distanceType, int range, List<BattleNode> result)
        {
            result.Clear();

            switch (distanceType)
            {
                case DistanceType.Manhattan:
                    {
                        for (int searchX = -range; searchX <= range; searchX++)
                        {
                            BattleNode node = this[x + searchX, y];
                            if (node != null)
                            {
                                result.Add(node);
                            }
                        }

                        for (int searchY = -range; searchY <= range; searchY++)
                        {
                            BattleNode node = this[x, y + searchY];
                            if (node != null)
                            {
                                result.Add(node);
                            }
                        }
                    }
                    break;
                case DistanceType.Euclidean:
                    {
                        for (int searchX = -range; searchX <= range; searchX++)
                        {
                            for (int searchY = -range; searchY <= range; searchY++)
                            {
                                if (searchX == 0 && searchY == 0)
                                {
                                    continue;
                                }

                                BattleNode node = this[x + searchX, y + searchY];
                                if (node != null)
                                {
                                    result.Add(node);
                                }
                            }
                        }
                    }
                    break;
            }
        }
        public void GetNeighbourUnits(int x, int y, DistanceType distanceType, int range, List<BaseUnit> result)
        {
            result.Clear();

            switch (distanceType)
            {
                case DistanceType.Manhattan:
                    {
                        for (int searchX = -range; searchX <= range; searchX++)
                        {
                            BattleNode node = this[x + searchX, y];
                            if (node != null && node.Unit != null)
                            {
                                result.Add(node.Unit);
                            }
                        }

                        for (int searchY = -range; searchY <= range; searchY++)
                        {
                            BattleNode node = this[x, y + searchY];
                            if (node != null && node.Unit != null)
                            {
                                result.Add(node.Unit);
                            }
                        }
                    }
                    break;
                case DistanceType.Euclidean:
                    {
                        for (int searchX = -range; searchX <= range; searchX++)
                        {
                            for (int searchY = -range; searchY <= range; searchY++)
                            {
                                if (searchX == 0 && searchY == 0)
                                {
                                    continue;
                                }

                                BattleNode node = this[x + searchX, y + searchY];
                                if (node != null && node.Unit != null)
                                {
                                    result.Add(node.Unit);
                                }
                            }
                        }
                    }
                    break;
            }
        }
        public void GetNeighbours(int x, int y, DistanceType distanceType, int range, List<BattleNode> result, HashSet<int> excludedNodes)
        {
            result.Clear();

            switch (distanceType)
            {
                case DistanceType.Manhattan:
                    {
                        for (int searchX = -range; searchX <= range; searchX++)
                        {
                            BattleNode node = this[x + searchX, y];
                            if (node == null || (excludedNodes != null && excludedNodes.Contains(node.Index)))
                            {
                                continue;
                            }

                            result.Add(node);
                        }

                        for (int searchY = -range; searchY <= range; searchY++)
                        {
                            BattleNode node = this[x, y + searchY];
                            if (node == null || (excludedNodes != null && excludedNodes.Contains(node.Index)))
                            {
                                continue;
                            }

                            result.Add(node);
                        }
                    }
                    break;
                case DistanceType.Euclidean:
                    {
                        for (int searchX = -range; searchX <= range; searchX++)
                        {
                            for (int searchY = -range; searchY <= range; searchY++)
                            {
                                if (searchX == 0 && searchY == 0)
                                {
                                    continue;
                                }

                                BattleNode node = this[x + searchX, y + searchY];
                                if (node == null || (excludedNodes != null && excludedNodes.Contains(node.Index)))
                                {
                                    continue;
                                }

                                result.Add(node);
                            }
                        }
                    }
                    break;
            }
        }
        public void GetNeighbourEmptyItems(int x, int y, DistanceType distanceType, int range, List<BattleNode> result, HashSet<int> excludedNodes)
        {
            result.Clear();

            switch (distanceType)
            {
                case DistanceType.Manhattan:
                    {
                        for (int searchX = -range; searchX <= range; searchX++)
                        {
                            BattleNode node = this[x + searchX, y];
                            if (node == null || node.DropItem != null || (excludedNodes != null && excludedNodes.Contains(node.Index)))
                            {
                                continue;
                            }

                            result.Add(node);
                        }

                        for (int searchY = -range; searchY <= range; searchY++)
                        {
                            BattleNode node = this[x, y + searchY];
                            if (node == null || node.DropItem != null || (excludedNodes != null && excludedNodes.Contains(node.Index)))
                            {
                                continue;
                            }

                            result.Add(node);
                        }
                    }
                    break;
                case DistanceType.Euclidean:
                    {
                        for (int searchX = -range; searchX <= range; searchX++)
                        {
                            for (int searchY = -range; searchY <= range; searchY++)
                            {
                                if (searchX == 0 && searchY == 0)
                                {
                                    continue;
                                }

                                BattleNode node = this[x + searchX, y + searchY];
                                if (node == null || node.DropItem != null || (excludedNodes != null && excludedNodes.Contains(node.Index)))
                                {
                                    continue;
                                }

                                result.Add(node);
                            }
                        }
                    }
                    break;
            }
        }
        public void GetNeighboursRange(int x, int y, DistanceType distanceType, int startRange, int endRange, List<BattleNode> result)
        {
            result.Clear();

            switch (distanceType)
            {
                case DistanceType.Manhattan:
                    {
                        for (int searchX = -endRange; searchX <= endRange; searchX++)
                        {
                            if (searchX > -startRange && searchX < startRange)
                            {
                                continue;
                            }

                            if (searchX == x)
                            {
                                continue;
                            }

                            BattleNode node = this[x + searchX, y];
                            if (node != null)
                            {
                                result.Add(node);
                            }
                        }

                        for (int searchY = -endRange; searchY <= endRange; searchY++)
                        {
                            if (searchY > -startRange && searchY < startRange)
                            {
                                continue;
                            }

                            if (searchY == y)
                            {
                                continue;
                            }

                            BattleNode node = this[x, y + searchY];
                            if (node != null)
                            {
                                result.Add(node);
                            }
                        }
                    }
                    break;
                case DistanceType.Euclidean:
                    {
                        for (int searchX = -endRange; searchX <= endRange; searchX++)
                        {
                            for (int searchY = -endRange; searchY <= endRange; searchY++)
                            {
                                if (searchX > -startRange && searchX < startRange && searchY > -startRange && searchY < startRange)
                                {
                                    continue;
                                }

                                if (searchX == 0 && searchY == 0)
                                {
                                    continue;
                                }

                                BattleNode node = this[x + searchX, y + searchY];
                                if (node != null)
                                {
                                    result.Add(node);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        public void GetWalkableNeighbours(int x, int y, List<BattleNode> result)
        {
            GetWalkableNeighbours(x, y, DistanceType.Euclidean, 1, result, null);
        }
        public void GetWalkableNeighbours(int x, int y, DistanceType distanceType, List<BattleNode> result)
        {
            GetWalkableNeighbours(x, y, distanceType, 1, result, null);
        }
        public void GetWalkableNeighbours(int x, int y, DistanceType distanceType, int range, List<BattleNode> result, HashSet<int> excludedNodes)
        {
            result.Clear();

            switch (distanceType)
            {
                case DistanceType.Manhattan:
                    {
                        for (int searchX = -range; searchX <= range; searchX++)
                        {
                            if (searchX == 0)
                            {
                                continue;
                            }

                            BattleNode node = this[x + searchX, y];
                            if (node == null || !node.IsWalkable || (excludedNodes != null && excludedNodes.Contains(node.Index)))
                            {
                                continue;
                            }

                            result.Add(node);
                        }

                        for (int searchY = -range; searchY <= range; searchY++)
                        {
                            if (searchY == 0)
                            {
                                continue;
                            }

                            BattleNode node = this[x, y + searchY];
                            if (node == null || !node.IsWalkable || (excludedNodes != null && excludedNodes.Contains(node.Index)))
                            {
                                continue;
                            }

                            result.Add(node);
                        }
                    }
                    break;
                case DistanceType.Euclidean:
                    {
                        for (int searchX = -range; searchX <= range; searchX++)
                        {
                            if (x + searchX < 0 || x + searchX >= xMax)
                            {
                                continue;
                            }

                            for (int searchY = -range; searchY <= range; searchY++)
                            {
                                if (searchX == 0 && searchY == 0)
                                {
                                    continue;
                                }

                                if (y + searchY < 0 || y + searchY >= yMax)
                                {
                                    continue;
                                }

                                BattleNode node = this[x + searchX, y + searchY];
                                if (node == null || !node.IsWalkable || (excludedNodes != null && excludedNodes.Contains(node.Index)))
                                {
                                    continue;
                                }

                                result.Add(node);
                            }
                        }
                    }
                    break;
            }
        }

        public void buildJumpPoints()
        {
            //_jpsGrid.buildPrimaryJumpPoints();
            //_jpsGrid.buildStraightJumpPoints();
            //_jpsGrid.buildDiagonalJumpPoints();
        }

        private Dictionary<int, Dictionary<int, PathState>> _cachedPaths = new Dictionary<int, Dictionary<int, PathState>>();

        public List<BattleNode> FindAPath(int startX, int startY, int endX, int endY, DistanceType distance = DistanceType.Manhattan)
        {
            return FindAPath(this[startX, startY], this[endX, endY], distance);
        }
        public List<BattleNode> FindAPath(BattleNode startNode, BattleNode endNode, DistanceType distance = DistanceType.Manhattan)
        {
            if (startNode == null || endNode == null)
            {
                return null;
            }

            PathState pathState = null;

            if (_cachedPaths.ContainsKey(startNode.Index))
            {
                Dictionary<int, PathState> endPath = _cachedPaths[startNode.Index];
                if (endPath.ContainsKey(endNode.Index))
                {
                    pathState = endPath[endNode.Index];
                    if (pathState.stateIndex == nodeStateIndex)
                    {
                        return pathState.pathNodes;
                    }
                    else
                    {
                        //int findIndex = 0;
                        //for (; findIndex < pathState.pathNodes.Count; findIndex++)
                        //{
                        //    if (!pathState.pathNodes[findIndex].IsWalkable)
                        //    {
                        //        break;
                        //    }
                        //}

                        //if (findIndex == pathState.pathNodes.Count)
                        //{
                        //    pathState.stateIndex = nodeStateIndex;
                        //    return pathState.pathNodes;
                        //}

                        pathState.stateIndex = nodeStateIndex;
                        pathState.pathNodes = _aStarPathFinder.FindPath(this, startNode, endNode, distance);
                    }
                }
                else
                {
                    pathState = new PathState();
                    pathState.stateIndex = nodeStateIndex;
                    pathState.pathNodes = _aStarPathFinder.FindPath(this, startNode, endNode, distance);

                    endPath.Add(endNode.Index, pathState);
                }
            }
            else
            {
                pathState = new PathState();
                pathState.stateIndex = nodeStateIndex;
                pathState.pathNodes = _aStarPathFinder.FindPath(this, startNode, endNode, distance);

                Dictionary<int, PathState> endPath = new Dictionary<int, PathState>();
                endPath.Add(endNode.Index, pathState);
                _cachedPaths.Add(startNode.Index, endPath);
            }

            return pathState.pathNodes;
        }
    }
}