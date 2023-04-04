using System;


namespace LiaoZhai.Runtime
{
    // 伤害标志
    [FlagsAttribute]
    public enum DamageFlag
    {
        DAMAGE_FLAG_NONE = 0,
        DAMAGE_FLAG_NO_DIRECTOR_EVENT = 1 << 7,                     // 不响应事件
    }
}
