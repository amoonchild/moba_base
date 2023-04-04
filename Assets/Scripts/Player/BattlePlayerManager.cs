// -----------------------------------------------
// Copyright © Jeffrey. All rights reserved.
// CreateTime: 2021/7/7   13:21:14
// -----------------------------------------------

using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public class BattlePlayerManager
    {
        private static Dictionary<ulong, BasePlayer> _players = null;
        private static BasePlayer _selfPlayer = null;
        private static BasePlayer _enemyPlayer = null;

        public static BasePlayer SelfPlayer
        {
            get
            {
                return _selfPlayer;
            }
        }
        public static BasePlayer EnemyPlayer
        {
            get
            {
                return _enemyPlayer;
            }
        }


        public static void Init()
        {
            _players = new Dictionary<ulong, BasePlayer>();
        }

        public static void Destroy()
        {
            _players.Clear();
        }

        public static void Release()
        {
            _selfPlayer = null;
            _enemyPlayer = null;
            _players.Clear();
            _players = null;
        }

        public static BasePlayer CreatePlayer(CreatePlayerData initPlayerData)
        {
            if (!_players.ContainsKey(initPlayerData.RoleId))
            {
                RealPlayer player = new RealPlayer(initPlayerData.RoleId, initPlayerData.Name, initPlayerData.Level, initPlayerData.TeamId);
                _players.Add(player.RoleId, player);

                if (player.RoleId == GameManager.GlobalData.SelfPlayer.RoleId)
                {
                    _selfPlayer = player;
                }
                else
                {
                    _enemyPlayer = player;
                }

                return player;
            }

            return null;
        }
    }
}