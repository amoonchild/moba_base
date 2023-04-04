

namespace LiaoZhai.Runtime
{
    // 同组Modifier叠加类型
    public enum ModifierGroupStackType
    {
        MODIFIER_GROUP_STACK_IGNORE = 0,                            // 不影响其他Modifier
        MODIFIER_GROUP_STACK_CLEAR_OTHER = 1,                       // 清除其他Modifier
        MODIFIER_GROUP_STACK_REFRESH_OTHER_DURATION = 2,            // 刷新其他Modifier时间
        MODIFIER_GROUP_STACK_ABSORB_OTHER_DURATION_AND_CLEAR = 3,   // 获得其他Modifier剩余时间,清除其他Modifier
    }
}