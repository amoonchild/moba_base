// -----------------------------------------------
// Copyright © Jeffrey. All rights reserved.
// CreateTime: 2021/7/6   12:26:52
// -----------------------------------------------
using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    public class ResetUnitAttributeData
    {
        public ObscuredULong player_id = 0UL;
        public ObscuredInt unit_index = 0;
        public ObscuredInt unit_id = 0;
        public BattleUnitAttribute Att = null;
        public ObscuredInt[] skill_id = null;
    }
}
