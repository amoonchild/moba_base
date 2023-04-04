using System;


namespace LiaoZhai.Runtime
{
    // 技能目标标志
    [FlagsAttribute]
    public enum AbilityUnitTargetFlag
    {
        UNIT_TARGET_FLAG_NONE = 0,                                  // 无
        UNIT_TARGET_FLAG_ATTACK_UNIT = 1 << 2,                      // 我的攻击目标
        UNIT_TARGET_FLAG_ATTACKER = 1 << 3,                         // 攻击我的
        UNIT_TARGET_FLAG_DEAD = 1 << 4,                             // 死亡
        UNIT_TARGET_FLAG_MELEE_ONLY = 1 << 5,                       // 近战
        UNIT_TARGET_FLAG_RANGED_ONLY = 1 << 6,                      // 远程
        UNIT_TARGET_FLAG_NOT_ILLUSIONS = 1 << 7,                    // 非幻象(拥有MODIFIER_PROPERTY_IS_ILLUSION状态的)
        UNIT_TARGET_FLAG_NOT_SUMMONED = 1 << 8,                     // 非召唤物(SpawnUnit产生的,拥有MODIFIER_PROPERTY_IS_SUMMONED状态的)
        UNIT_TARGET_FLAG_HAVE_PURGABLE_BUFF = 1 << 9,               // 拥有可驱散的Buff
        UNIT_TARGET_FLAG_HAVE_PURGABLE_DEBUFF = 1 << 10,            // 拥有可驱散的Debuff
        UNIT_TARGET_FLAG_MYILLUSION = 1 << 11,                      // 我召唤的幻象
        UNIT_TARGET_FLAG_MYSUMMONED = 1 << 12,                      // 我召唤的召唤物
        UNIT_TARGET_FLAG_INVISIBLE = 1 << 14,                       // 隐身
        UNIT_TARGET_FLAG_INVINCIBLE = 1 << 15,                      // 无敌
        UNIT_TARGET_FLAG_SELF = 1 << 1,                             // 自身
        UNIT_TARGET_FLAG_CENTER = 1 << 13,                          // 目标中心(多目标有效)
        UNIT_TARGET_FLAG_UNSELECTABLE = 1 << 16,                    // 不可选的
        UNIT_TARGET_FLAG_UNSELECTABLE_ATTACK = 1 << 18,             // 不可选的(普攻)
        UNIT_TARGET_FLAG_UNSELECTABLE_ABILITY = 1 << 19,            // 不可选的(技能)
    }
}