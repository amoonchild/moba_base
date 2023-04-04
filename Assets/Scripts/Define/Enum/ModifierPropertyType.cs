

namespace LiaoZhai.Runtime
{
    // Modifier属性类型
    public enum ModifierPropertyType
    {
        MODIFIER_PROPERTY_NONE,
        MODIFIER_PROPERTY_ATTACK_BONUS,                             // 增加攻击数值
        MODIFIER_PROPERTY_DEFENSE_BONUS,                            // 增加防御数值
        MODIFIER_PROPERTY_DEFENSE_RATIO_BONUS,                      // 增加防御基数
        MODIFIER_PROPERTY_ATTACK_SPEED_BONUS,                       // 增加攻击速度数值
        MODIFIER_PROPERTY_MOVE_SPEED_BONUS,                         // 增加移动速度数值
        MODIFIER_PROPERTY_CRIT_DAMAGE_BONUS,                        // 增加暴击伤害数值
        MODIFIER_PROPERTY_CRIT_RESISTANCE_BONUS,                    // 增加暴击抗性
        MODIFIER_PROPERTY_CRIT_ODDS_BONUS,                          // 增加暴击率数值
        MODIFIER_PROPERTY_DAMAGE_OUT_BONUS,                         // 增加伤害输出数值
        MODIFIER_PROPERTY_DAMAGE_IN_BONUS,                          // 增加伤害减免数值
        MODIFIER_PROPERTY_DODGE_ODDS_BONUS,                         // 增加闪避率数值
        MODIFIER_PROPERTY_HIT_ODDS_BONUS,                           // 增加命中率数值
        MODIFIER_PROPERTY_ABILITY_OUT_BONUS,                        // 增加技能输出数值
        MODIFIER_PROPERTY_HEAL_OUT_BONUS,                           // 增加治疗输出数值
        MODIFIER_PROPERTY_PHYSICAL_ARMOR_BREAK_BONUS,               // 增加破甲数值
        MODIFIER_PROPERTY_LIFE_STEAL_BONUS,                         // 增加吸血数值
        MODIFIER_PROPERTY_ATTACK_OUT_BONUS,                         // 增加攻击输出数值
        MODIFIER_PROPERTY_ATTACK_IN_BONUS,                          // 增加攻击减免数值
        MODIFIER_PROPERTY_ABILITY_IN_BONUS,                         // 增加技能减免数值
        MODIFIER_PROPERTY_HEAL_IN_BONUS,                            // 增加受到治疗加成数值
        MODIFIER_PROPERTY_FINAL_DAMAGE_OUT_BONUS,                   // 造成伤害时改变伤害值
        MODIFIER_PROPERTY_FINAL_DAMAGE_IN_BONUS,					// 受到伤害时改变伤害值
        MODIFIER_PROPERTY_COOLDOWN_BONUS,                           // 增加技能冷却时间
        MODIFIER_PROPERTY_MAXHP_BONUS,                              // 增加最大生命值数值
        MODIFIER_PROPERTY_ATTACKRANGE_BONUS,                        // 增加攻击距离数值
        MODIFIER_PROPERTY_MODEL_RADII_BONUS,                        // 增加模型缩放

        MODIFIER_PROPERTY_IS_ILLUSION,                              // 是某个单位的幻象
        MODIFIER_PROPERTY_IS_SUMMONED,                              // 是某个单位的召唤物
    }
}