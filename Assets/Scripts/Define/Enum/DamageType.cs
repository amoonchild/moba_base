

namespace LiaoZhai.Runtime
{
    // 伤害类型
    public enum DamageType
    {
        DAMAGE_TYPE_NONE = 0,
        DAMAGE_TYPE_ATTACK_PHYSICAL = 1,                        // 普通攻击伤害
        DAMAGE_TYPE_PHYSICAL = 2,                               // 技能攻击伤害
        DAMAGE_TYPE_MAGICAL = 3,                                // 技能伤害
        DAMAGE_TYPE_HP_REMOVAL = 9,                             // 生命移除直接增加或减少
    }
}
