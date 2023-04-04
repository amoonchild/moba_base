

namespace LiaoZhai.Runtime
{
    // 单位命令类型
    public enum UnitOrderType
    {
        UNIT_ORDER_NONE = 0,                                        // 无
        UNIT_ORDER_MOVE_TO_NODE = 30,                               // 移动到指定格子
        UNIT_ORDER_ATTACK_TARGET = 4,                               // 攻击目标
        UNIT_ORDER_CAST_POSITION = 5,                               // 释放技能,对某个坐标
        UNIT_ORDER_CAST_TARGET = 6,                                 // 释放技能,对某个单位
        UNIT_ORDER_CAST_NODE = 7,                                   // 释放技能,对某个格子
        UNIT_ORDER_CAST_NO_TARGET = 8,                              // 释放技能,无目标
        UNIT_ORDER_STOP = 21,                                       // 停止
    }
}
