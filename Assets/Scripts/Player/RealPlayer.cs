

namespace LiaoZhai.Runtime
{
    // 真实玩家
    public class RealPlayer : BasePlayer
    {
        public override BattlePlayerType Type
        {
            get
            {
                return BattlePlayerType.RealPlayer;
            }
        }


        public RealPlayer(ulong roleId, string name, int level, int teamId)
            : base(roleId, name, level, teamId)
        {

        }
    }
}