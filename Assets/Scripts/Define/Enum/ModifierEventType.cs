

namespace LiaoZhai.Runtime
{
    // 事件类型
    public enum ModifierEventType
    {
        MODIFIER_EVENT_ON_DEATH,                                    // 死亡
        MODIFIER_EVENT_ON_RESPAWN,                                  // 重生
        MODIFIER_EVENT_ON_KILL_UNIT,                                // 击杀单位
        MODIFIER_EVENT_ON_UNIT_DEATH,                               // 任意单位死亡
        MODIFIER_EVENT_ON_UNIT_DEATH2,                              // 任意单位死亡(延后)

        MODIFIER_EVENT_ON_ATTACK_PHASE_START,                       // 普攻前摇开始
        MODIFIER_EVENT_ON_ATTACK_START,                             // 普攻开始
        MODIFIER_EVENT_ON_ATTACK_FINISH,                            // 普攻结束

        MODIFIER_EVENT_ON_ABILITY_PHASE_START,                      // 技能前摇开始
        MODIFIER_EVENT_ON_ABILITY_PHASE_INTERRUPTED,                // 技能前摇被打断
        MODIFIER_EVENT_ON_ABILITY_START,                            // 技能前摇结束
        MODIFIER_EVENT_ON_ABILITY_CHANNEL_SUCCESS,                  // 技能引导成功
        MODIFIER_EVENT_ON_ABILITY_CHANNEL_INTERRUPTED,              // 技能引导被打断
        MODIFIER_EVENT_ON_ABILITY_CHANNEL_FINISH,                   // 技能引导结束
        MODIFIER_EVENT_ON_ABILITY_FINISH,                           // 技能结束

        MODIFIER_EVENT_ON_BEFORE_TAKE_DAMAGE,                       // 将要受到伤害
        MODIFIER_EVENT_ON_BEFORE_TAKE_ATTACK_DAMAGE,                // 将要受到攻击伤害
        MODIFIER_EVENT_ON_BEFORE_TAKE_ATTACK_NOCRIT_DAMAGE,         // 将要受到攻击暴击伤害
        MODIFIER_EVENT_ON_BEFORE_TAKE_ATTACK_CRIT_DAMAGE,           // 将要受到攻击暴击伤害
        MODIFIER_EVENT_ON_BEFORE_TAKE_ABILITY_DAMAGE,               // 将要受到技能伤害
        MODIFIER_EVENT_ON_BEFORE_TAKE_ABILITY_NOCRIT_DAMAGE,          // 将要受到技能暴击伤害
        MODIFIER_EVENT_ON_BEFORE_TAKE_ABILITY_CRIT_DAMAGE,          // 将要受到技能暴击伤害

        MODIFIER_EVENT_ON_TAKE_DAMAGE,                              // 受到伤害
        MODIFIER_EVENT_ON_TAKE_ATTACK_DAMAGE,                       // 受到攻击伤害
        MODIFIER_EVENT_ON_TAKE_ATTACK_NOCRIT_DAMAGE,                  // 受到攻击暴击伤害
        MODIFIER_EVENT_ON_TAKE_ATTACK_CRIT_DAMAGE,                  // 受到攻击暴击伤害
        MODIFIER_EVENT_ON_TAKE_ABILITY_DAMAGE,                      // 受到技能伤害
        MODIFIER_EVENT_ON_TAKE_ABILITY_NOCRIT_DAMAGE,                 // 受到技能暴击伤害
        MODIFIER_EVENT_ON_TAKE_ABILITY_CRIT_DAMAGE,                 // 受到技能暴击伤害

        MODIFIER_EVENT_ON_BEFORE_DEAL_DAMAGE,                       // 将要造成伤害
        MODIFIER_EVENT_ON_BEFORE_DEAL_ATTACK_DAMAGE,                // 将要造成攻击伤害
        MODIFIER_EVENT_ON_BEFORE_DEAL_ATTACK_NOCRIT_DAMAGE,         // 将要造成攻击伤害
        MODIFIER_EVENT_ON_BEFORE_DEAL_ATTACK_CRIT_DAMAGE,           // 将要造成攻击暴击伤害
        MODIFIER_EVENT_ON_BEFORE_DEAL_ABILITY_DAMAGE,               // 将要造成技能伤害
        MODIFIER_EVENT_ON_BEFORE_DEAL_ABILITY_NOCRIT_DAMAGE,          // 将要造成技能暴击伤害
        MODIFIER_EVENT_ON_BEFORE_DEAL_ABILITY_CRIT_DAMAGE,          // 将要造成技能暴击伤害

        MODIFIER_EVENT_ON_DEAL_DAMAGE,                              // 造成伤害
        MODIFIER_EVENT_ON_DEAL_ATTACK_DAMAGE,                       // 造成攻击伤害
        MODIFIER_EVENT_ON_DEAL_ATTACK_NOCRIT_DAMAGE,                  // 造成攻击暴击伤害
        MODIFIER_EVENT_ON_DEAL_ATTACK_CRIT_DAMAGE,                  // 造成攻击暴击伤害
        MODIFIER_EVENT_ON_DEAL_ABILITY_DAMAGE,                      // 造成技能伤害
        MODIFIER_EVENT_ON_DEAL_ABILITY_NOCRIT_DAMAGE,                 // 造成技能暴击伤害
        MODIFIER_EVENT_ON_DEAL_ABILITY_CRIT_DAMAGE,                 // 造成技能暴击伤害

        MODIFIER_EVENT_ON_MISS,                                     // 伤害未命中
        MODIFIER_EVENT_ON_ATTACK_MISS,                              // 攻击伤害未命中
        MODIFIER_EVENT_ON_ABILITY_MISS,                             // 技能伤害未命中

        MODIFIER_EVENT_ON_DODGE,                                    // 闪避伤害
        MODIFIER_EVENT_ON_ATTACK_DODGE,                             // 闪避攻击伤害
        MODIFIER_EVENT_ON_ABILITY_DODGE,                            // 闪避技能伤害

        MODIFIER_EVENT_ON_BEFORE_TAKE_HEAL,                         // 将要受到治疗
        MODIFIER_EVENT_ON_TAKE_HEAL,                                // 受到治疗

        MODIFIER_EVENT_ON_BEFORE_DEAL_HEAL,                         // 将要造成治疗
        MODIFIER_EVENT_ON_DEAL_HEAL,                                // 造成治疗

        MODIFIER_EVENT_ON_MANA_INCREASE,                            // 法力值增加
        MODIFIER_EVENT_ON_MANA_REDUCE,                              // 法力值减少

        MODIFIER_EVENT_ON_CREATED = 10000,                          // 
        MODIFIER_EVENT_ON_INTERVALTHINK,                            // 
        MODIFIER_EVENT_ON_PURGED,                                   // 
        MODIFIER_EVENT_ON_DESTROY,                                  // 
    }
}
