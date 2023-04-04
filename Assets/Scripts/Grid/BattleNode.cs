using System;


namespace LiaoZhai.Runtime
{
    public class BattleNode : INode
    {
        private int _index = 0;
        private int _x = 0;
        private int _y = 0;
        private DFixVector3 _worldPosition = DFixVector3.Zero;
        private bool _isWalkable = true;
        private bool _isSelfNode = false;
        private BaseUnit _unit = null;
        private BattleGrid _grid = null;
        private Particle _particle = null;

        public int Index
        {
            get
            {
                return _index;
            }
        }
        public int X
        {
            get
            {
                return _x;
            }
        }
        public int Y
        {
            get
            {
                return _y;
            }
        }
        public DFixVector3 WorldPosition
        {
            get
            {
                return _worldPosition;
            }
        }
        public bool IsWalkable
        {
            get
            {
                return _isWalkable;
            }
            set
            {
                _isWalkable = value;
                _grid.nodeStateIndex++;
            }
        }
        public bool IsSelfNode
        {
            get
            {
                return _isSelfNode;
            }
            set
            {
                _isSelfNode = value;
            }
        }
        public BaseUnit Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                if (_unit != value)
                {
                    if(_unit != null)
                    {
                        if(value == null)
                        {
                            _grid.nodeStateIndex++;
                        }
                    }
                    else
                    {
                        if(value != null)
                        {
                            _grid.nodeStateIndex++;
                        }
                    }

                    _unit = value;
                    _isWalkable = _unit == null;
                }
            }
        }
        public Particle DropItem
        {
            get
            {
                return _particle;
            }
            set
            {
                _particle = value;
            }
        }

        public int H
        {
            get;
            set;
        }
        public int G
        {
            get;
            set;
        }
        public int F
        {
            get
            {
                return G + H;
            }
        }
        public INode ParentNode
        {
            get;
            set;
        }


        public BattleNode(BattleGrid grid, int index, int x, int y, DFixVector3 worldPosition)
        {
            _grid = grid;
            _index = index;
            _x = x;
            _y = y;
            _worldPosition = worldPosition;
        }

        public DFix64 Distance(BattleNode target)
        {
            return DFixVector3.Distance(WorldPosition, target.WorldPosition);
        }

        public int MinRange(BattleNode target)
        {
            int xRange = BattleData.Abs(_x - target.X);
            int yRange = BattleData.Abs(_y - target.Y);

            return xRange < yRange ? xRange : yRange;
        }

        public int MaxRange(BattleNode target)
        {
            if (target == null)
            {
                return 0;
            }

            int xRange = BattleData.Abs(_x - target.X);
            int yRange = BattleData.Abs(_y - target.Y);

            return xRange > yRange ? xRange : yRange;
        }

        public int GetDistance(BattleNode nodeB)
        {
            int dstX = BattleData.Abs(X - nodeB.X);
            int dstY = BattleData.Abs(Y - nodeB.Y);
            return (dstX > dstY) ?
                14 * dstY + 10 * (dstX - dstY) :
                14 * dstX + 10 * (dstY - dstX);
        }
    }
}