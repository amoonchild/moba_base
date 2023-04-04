using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    public class CreateUnitData
    {
        public ObscuredULong player_id = 0UL;                                   // 玩家id
        public ObscuredInt bag_index = Constant.Game.EmptyBagIndex;             // 背包索引
        public ObscuredInt unit_index = 0;                                      // 单位索引
        public ObscuredInt player_camp = 0;                                     // 阵营
        public ObscuredInt level = 0;                                           // 等级
        public UnitType unit_type = UnitType.UNIT_NONE;                         // 单位类型
        public ObscuredInt card_id = 0;                                         // 单位id
        public ObscuredInt skin_id = 0;                                         // 皮肤id
        public ObscuredInt unit_in_x = 0;                                       // 初始格子row
        public ObscuredInt unit_in_y = 0;                                       // 初始格子column
        public DFix64 model_radii = DFix64.One;                                 // 初始缩放
        public ObscuredInt[] skill_id = null;                                   // 初始技能列表
        public DFix64 DelaySpawnTime = DFix64.Zero;                             // 上场延迟

        public string name = string.Empty;                                      // 名字
        public ObscuredULong score = 0;                                         // 战力
    }
}