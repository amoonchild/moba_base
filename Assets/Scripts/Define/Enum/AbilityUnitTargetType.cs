using System;


namespace LiaoZhai.Runtime
{
    // 技能目标类型
    [FlagsAttribute]
    public enum AbilityUnitTargetType
    {
        UNIT_TARGET_NONE = 1 << 0,                                  // 无
        UNIT_TARGET_ALL = 1 << 1,                                   // 所有单位
        UNIT_TARGET_BOSS = 1 << 3,                                  // 首领
        UNIT_TARGET_HERO = 1 << 4,                                  // 英雄
    }
}
