using System;


namespace LiaoZhai.Runtime
{
    // 技能目标单位队伍
    [FlagsAttribute]
    public enum AbilityUnitTargetTeam
    {
        UNIT_TARGET_TEAM_NONE = 1 << 0,                             // 无
        UNIT_TARGET_TEAM_BOTH = 1 << 1,                             // 所有队伍
        UNIT_TARGET_TEAM_ENEMY = 1 << 2,                            // 敌方队伍
        UNIT_TARGET_TEAM_FRIENDLY = 1 << 3,                         // 友方队伍
    }
}
