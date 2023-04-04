

namespace LiaoZhai.Runtime
{
    // 事件类型
    public enum AbilityEventType
    {
        ABILITY_EVENT_ON_SPAWN,                                    // 出生
        ABILITY_EVENT_ON_DEATH,                                    // 死亡
        ABILITY_EVENT_ON_ABILITY_PHASE_START,                      // 技能前摇开始
        ABILITY_EVENT_ON_ABILITY_PHASE_INTERRUPTED,                // 技能前摇被打断
        ABILITY_EVENT_ON_ABILITY_START,                            // 技能前摇结束
        ABILITY_EVENT_ON_ABILITY_CHANNEL_SUCCESS,                  // 技能引导成功
        ABILITY_EVENT_ON_ABILITY_CHANNEL_INTERRUPTED,              // 技能引导被打断
        ABILITY_EVENT_ON_ABILITY_CHANNEL_FINISH,                   // 技能引导结束
        ABILITY_EVENT_ON_ABILITY_FINISH,                           // 技能结束
    }
}
