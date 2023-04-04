

namespace LiaoZhai.Runtime
{
    // 位置目标
    public class BattlePointTarget : BattleTarget
    {
        private DFixVector3 _pointTarget = DFixVector3.Zero;

        public override BattleTargetType Type
        {
            get
            {
                return BattleTargetType.POINT;
            }
        }
        public override object Target
        {
            get
            {
                return _pointTarget;
            }
        }


        public BattlePointTarget(DFixVector3 pointTarget)
        {
            _pointTarget = pointTarget;
        }
    }
}