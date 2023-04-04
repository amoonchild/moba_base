// -----------------------------------------------
// Copyright © Jeffrey. All rights reserved.
// CreateTime: 2021/7/6   19:32:18
// -----------------------------------------------



namespace LiaoZhai.Runtime
{
    public class CreateBossData : CreateUnitData
    {
        public int OccId = 0;


        public CreateBossData()
        {
            unit_type = UnitType.UNIT_BOSS;
        }
    }
}