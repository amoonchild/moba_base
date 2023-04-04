using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public class BattleInitData
    {
        private static BattleType _battleType = BattleType.Undefined;
        private static ulong _battleTargetId = 0UL;
        private static int _battleNodeId = 0;
        private static List<CreatePlayerData> _initPlayerInfos = new List<CreatePlayerData>();
        private static List<CreateUnitData> _initEnemyUnitInfos = new List<CreateUnitData>();
        private static int _state = 0;

        public static BattleType BattleType
        {
            get
            {
                return _battleType;
            }
            set
            {
                _battleType = value;
            }
        }
        public static ulong BattleTargetId
        {
            get
            {
                return _battleTargetId;
            }
            set
            {
                _battleTargetId = value;
            }
        }
        public static int BattleNodeId
        {
            get
            {
                return _battleNodeId;
            }
            set
            {
                _battleNodeId = value;
            }
        }
        public static List<CreatePlayerData> InitPlayerInfos
        {
            get
            {
                return _initPlayerInfos;
            }
        }
        public static List<CreateUnitData> InitEnemyUnitInfos
        {
            get
            {
                return _initEnemyUnitInfos;
            }
        }
        public static int State
        {
            get { return _state; }
            set { _state = value; }
        }


        public static void Clear()
        {
            _battleType = BattleType.Undefined;
            _battleTargetId = 0UL;
            _battleNodeId = 0;
            _initPlayerInfos.Clear();
            _initEnemyUnitInfos.Clear();
            _state = 0;
        }
    }
}