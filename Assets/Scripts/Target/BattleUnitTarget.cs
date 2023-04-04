

namespace LiaoZhai.Runtime
{
    // 单位目标
    public class BattleUnitTarget : BattleTarget
    {
        private BaseUnit _unitTarget = null;

        public override BattleTargetType Type
        {
            get
            {
                return BattleTargetType.UNIT;
            }
        }
        public override object Target
        {
            get
            {
                return _unitTarget;
            }
        }


        public BattleUnitTarget(BaseUnit unitTarget)
        {
            _unitTarget = unitTarget;
        }
    }
}