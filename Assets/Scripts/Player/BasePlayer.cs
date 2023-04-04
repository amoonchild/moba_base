using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 玩家
    public abstract class BasePlayer
    {
        protected ulong _roleId = 0UL;                      // 玩家id
        protected string _name = string.Empty;              // 名字
        protected int _level = 0;                           // 等级
        protected int _teamId = 0;                          // 队伍Id
        protected Boss _boss = null;                        // 首领
        protected int _teamTotalNumber = 0;
        protected int _teamDeathNumber = 0;
        protected int _teamKillNumber = 0;
        protected Dictionary<string, DFix64> _parms = new Dictionary<string, DFix64>();

        public abstract BattlePlayerType Type
        {
            get;
        }
        public ulong RoleId
        {
            get
            {
                return _roleId;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public int Level
        {
            get
            {
                return _level;
            }
        }
        public int TeamId
        {
            get
            {
                return _teamId;
            }
        }
        public Boss Boss
        {
            get
            {
                return _boss;
            }
            set
            {
                _boss = value;
            }
        }
        public int TeamTotalNumber
        {
            get { return _teamTotalNumber; }
            set { _teamTotalNumber = value; }
        }
        public int TeamDeathNumber
        {
            get { return _teamDeathNumber; }
            set { _teamDeathNumber = value; }
        }
        public int TeamKillNumber
        {
            get { return _teamKillNumber; }
            set { _teamKillNumber = value; }
        }

        public DFix64 this[string key]
        {
            get
            {
                if (!_parms.ContainsKey(key))
                {
                    return DFix64.Zero;
                }

                return _parms[key];
            }
            set
            {
                if (!_parms.ContainsKey(key))
                {
                    _parms.Add(key, value);
                }
                else
                {
                    _parms[key] = value;
                }
            }
        }


        public BasePlayer(ulong roleId, string name, int level, int teamId)
        {
            _roleId = roleId;
            _name = name;
            _level = level;
            _teamId = teamId;
        }
    }
}