

namespace LiaoZhai.Runtime
{
    // Modifier状态
    public enum ModifierStateType
    {
        MODIFIER_STATE_NONE = 0,                                    //
        MODIFIER_STATE_MOVE_DISABLED = 1,                           // 不能移动
        MODIFIER_STATE_ATTACK_DISABLED,                             // 禁用普攻状态
        MODIFIER_STATE_ACTIVATES_DISABLED,                          // 禁用主动技能状态
        MODIFIER_STATE_PASSIVES_DISABLED,                           // 禁用被动技能状态
        MODIFIER_STATE_STUNNED,                                     // 眩晕状态
        MODIFIER_STATE_BUFF_IMMUNE,                                 // 免疫Buff状态
        MODIFIER_STATE_DEBUFF_IMMUNE,                               // 免疫Debuff状态
        MODIFIER_STATE_NEBUFF_IMMUNE,                               // 免疫中立Buff状态
        MODIFIER_STATE_ALLBUFF_IMMUNE,                              // 免疫所有Buff状态

        MODIFIER_STATE_NO_HEALTH_BAR,                               // 隐藏生命条
        MODIFIER_STATE_INVISIBLE,                                   // 模型不可见
        MODIFIER_STATE_COMMAND_RESTRICTED,                          // 无法执行任何指令
        MODIFIER_STATE_INVINCIBLE,                                  // 无敌的
        MODIFIER_STATE_UNSELECTABLE,                                // 无法被选取为目标
        MODIFIER_STATE_UNSELECTABLE_ATTACK,                         // 无法被选取为普攻目标
        MODIFIER_STATE_UNSELECTABLE_ABILITY,                        // 无法被选取为技能目标
    }
}