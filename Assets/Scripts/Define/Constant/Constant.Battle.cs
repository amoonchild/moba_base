using System.Collections.Generic;
using UnityEngine;


namespace LiaoZhai.Runtime
{
    public static partial class Constant
    {
        public static class Battle
        {
            public const int LAYER_DEFAULT = 0;
            public const int LAYER_CULL = 2;

            public const int UNIT_KV_LIBRARY_ID = 1;
            public const int SKIN_KV_LIBRARY_ID = 2;
            public const int MODEL_KV_LIBRARY_ID = 3;
            public const int ABILITY_KV_LIBRARY_ID = 4;
            public const int BUILTIN_KV_LIBRARY_ID = 5;


            public static Dictionary<UnitAnimationType, string> UnitActNames = new Dictionary<UnitAnimationType, string>                    // 单位动作名列表
            {
                { UnitAnimationType.ACT_ATTACK1,"攻击1"},
                { UnitAnimationType.ACT_ATTACK2,"攻击2"},
                { UnitAnimationType.ACT_ATTACK3,"攻击3"},
                { UnitAnimationType.ACT_CAST_ABILITY_1,"施法1"},
                { UnitAnimationType.ACT_CAST_ABILITY_2,"施法2"},
                { UnitAnimationType.ACT_CAST_ABILITY_3,"施法3"},
                { UnitAnimationType.ACT_RUN,"走跑"},
                { UnitAnimationType.ACT_DEAD,"死亡"},
                { UnitAnimationType.ACT_IDLE,"战斗待机"},
            };

            public static int GROUND_LAYER = LayerMask.NameToLayer("Battle Ground");                                                // 战斗地面Layer
            public static int GRID_LAYER = LayerMask.NameToLayer("Battle Area");                                                    // 战斗格子Layer

            public static string SELF_TEAM_DEATH_EFFECT_ASSET = "Assets/AssetBundle/Effect/Common/siwang_you.prefab";
            public static string ENEMY_TEAM_DEATH_EFFECT_ASSET = "Assets/AssetBundle/Effect/Common/siwang_di.prefab";
            public static string FRIENDLY_SPAWN_EFFECT_ASSET = "Assets/AssetBundle/Effect/Common/zhaohuan_1.prefab";
            public static string ENEMY_SPAWN_EFFECT_ASSET = "Assets/AssetBundle/Effect/Common/zhaohuan_2.prefab";

            public const string UNIT_MODEL_ATTACH_POINT_BLOODBAR = "XUETIAO_GD";            // 血条挂点
            public const string UNIT_MODEL_ATTACH_POINT_XIONG = "XIONG_GD";                 // 胸挂点
            public const string UNIT_MODEL_ATTACH_POINT_DANDAO = "DANDAO_GD";               // 弹道挂点
            public const string UNIT_MODEL_ATTACH_POINT_GUANGHUAN = "GUANGHUAN_GD";         // 光环挂点

            public static int MAX_LINEUP_UNIT_NUMBER = 6;                                   // 最大可上阵人数

            public static DFix64 BASE_MOVE_SPEED = (DFix64)180f;                              // 基础移动速度

            public static int BUILTIN_MODIFIER_ILLUSION = 105;                              // 内置ModifierId 幻象
            public static int BUILTIN_MODIFIER_ILLUSION_Kill = 106;                         // 内置ModifierId 删除幻象
            public static int BUILTIN_MODIFIER_SUMMONED = 107;                              // 内置ModifierId 召唤物
            public static int BUILTIN_MODIFIER_SUMMONED_Kill = 108;                         // 内置ModifierId 删除召唤物
            public static int BUILTIN_MODIFIER_THINKER = 109;                               // 内置ModifierId 定时器
            public static int BUILTIN_MODIFIER_DEATH = 110;                                 // 内置ModifierId 死亡
            public static int BUILTIN_MODIFIER_BATTLE_END = 111;                            // 内置ModifierId 战斗结束
            public static int BUILTIN_MODIFIER_THINKER_Kill = 112;                          // 内置ModifierId 删除定时器
            public static int BUILTIN_MODIFIER_BATTLE_LINEUP = 113;                         // 布阵
            public static int BUILTIN_MODIFIER_BATTLE_RESULT = 114;                         // 等待结算

        }
    }
}