

namespace LiaoZhai.Runtime
{
    // 目标排序
    public enum AbilityUnitTargetSort
    {
        UNIT_TARGET_SORT_NONE = 0,                                  // 无
        UNIT_TARGET_SORT_NEAREST = 1,                               // 距离最近的
        UNIT_TARGET_SORT_MOST_CURR_ATTACK = 2,                      // 当前攻击最高的
        UNIT_TARGET_SORT_MOST_INIT_ATTACK = 3,                      // 初始攻击最高的
        UNIT_TARGET_SORT_MOST_CURR_LEVEL = 4,                       // 等级最高的
        UNIT_TARGET_SORT_LEAST_CURR_HP = 5,                         // 当前生命值最低的
        UNIT_TARGET_SORT_MOST_CURR_HP = 6,                          // 当前生命值最高的
        UNIT_TARGET_SORT_FARTHEST = 7,                              // 距离最远的
        UNIT_TARGET_SORT_TOTAL_DAMAGE_OUT_MAX = 9,                  // 造成总伤害最高的
        UNIT_TARGET_SORT_TOTAL_DAMAGE_OUT_MIN = 10,                 // 造成总伤害最低的
        UNIT_TARGET_SORT_TOTAL_DAMAGE_IN_MAX = 11,                  // 承受总伤害最高的
        UNIT_TARGET_SORT_TOTAL_DAMAGE_IN_MIN = 12,                  // 承受总伤害最低的
        UNIT_TARGET_SORT_LEAST_CURR_HPPCT = 13,                     // 当前生命值百分比最低的
        UNIT_TARGET_SORT_MOST_CURR_DEFENSE = 14,                    // 当前防御最高的
        UNIT_TARGET_SORT_MOST_INIT_DEFENSE = 15,                    // 初始防御最高的
        UNIT_TARGET_SORT_MOST_POWER = 16,                           // 战力最高
        UNIT_TARGET_SORT_LEAST_POWER = 17,                          // 战力最低
    }
}