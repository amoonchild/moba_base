

namespace LiaoZhai.Runtime
{
    // 动作类型
    public enum UnitAnimationType
    {
        ACT_NONE = 0,                                               // 无
        ACT_ATTACK1 = 1,                                            // 攻击1
        ACT_ATTACK2 = 2,                                            // 攻击2
        ACT_ATTACK3 = 3,                                            // 攻击3
        ACT_CAST_ABILITY_1 = 4,                                     // 技能1
        ACT_CAST_ABILITY_2 = 5,                                     // 技能2
        ACT_CAST_ABILITY_3 = 6,                                     // 技能3
        ACT_RUN = 7,                                                // 移动
        ACT_DEAD = 8,                                               // 死亡
        ACT_IDLE = 9,                                               // 待机
    }
}
