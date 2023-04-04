

namespace LiaoZhai.Runtime
{
    public enum BattleTargetType
    {
        None,
        UNIT,
        POINT,
        NODE,
    }

    // 操作目标
    public abstract class BattleTarget
    {
        public abstract BattleTargetType Type
        {
            get;
        }
        public abstract object Target
        {
            get;
        }
    }
}