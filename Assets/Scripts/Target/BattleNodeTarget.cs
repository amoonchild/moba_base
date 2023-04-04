

namespace LiaoZhai.Runtime
{
    // 位置目标
    public class BattleNodeTarget : BattleTarget
    {
        private BattleNode _battleNode = null;

        public override BattleTargetType Type
        {
            get
            {
                return BattleTargetType.NODE;
            }
        }
        public override object Target
        {
            get
            {
                return _battleNode;
            }
        }


        public BattleNodeTarget(BattleNode battleNode)
        {
            _battleNode = battleNode;
        }
    }
}