using System;


namespace LiaoZhai.Runtime
{
    // 技能行为
    [FlagsAttribute]
    public enum AbilityBehavior
    {
        ABILITY_BEHAVIOR_NONE = 0,
        ABILITY_BEHAVIOR_HIDDEN = 1 << 0,                           // 这个技能是单位所拥有的技能，但是不会在HUD上显示。
        ABILITY_BEHAVIOR_PASSIVE = 1 << 1,                          // 这个技能是一个被动技能，不能被使用
        ABILITY_BEHAVIOR_NO_TARGET = 1 << 2,                        // 不需要指定目标就能释放的技能，当按下技能按钮的时候，这个技能就会被释放。
        ABILITY_BEHAVIOR_UNIT_TARGET = 1 << 3,                      // 技能需要指定一个目标来释放。
        ABILITY_BEHAVIOR_POINT = 1 << 4,                            // 技能将会在指定的位置释放（如果鼠标指向了一个单位，会在单位所在的位置释放）。
        ABILITY_BEHAVIOR_IMMEDIATE = 1 << 9,                        // 这个技能将会被立即释放，不会进入操作序列。
        ABILITY_BEHAVIOR_AUTOCAST = 1 << 10,                        // 这个技能可以被自动释放。
        ABILITY_BEHAVIOR_IGNORE_BACKSWING = 1 << 16,                // 这个技能将会无视施法后摇。
    }
}