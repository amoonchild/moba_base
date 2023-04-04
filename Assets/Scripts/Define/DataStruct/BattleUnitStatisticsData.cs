using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    public class BattleUnitStatisticsData
    {
        public DFix64 AttackDamageOut = DFix64.Zero;
        public DFix64 AbilityDamageOut = DFix64.Zero;
        public DFix64 AttackDamageIn = DFix64.Zero;
        public DFix64 AbilityDamageIn = DFix64.Zero;
        public DFix64 HealOut = DFix64.Zero;
        public DFix64 HealIn = DFix64.Zero;
        public ObscuredInt KillUnitCount = 0;
        public ObscuredInt KillIllusionCount = 0;
        public ObscuredInt KillSummonedCount = 0;
        public DFix64 TotalDamageOut
        {
            get
            {
                return AttackDamageOut + AbilityDamageOut;
            }
        }
        public DFix64 TotalDOPS
        {
            get
            {
                if (BattleData.LogicTime == DFix64.Zero)
                {
                    return DFix64.Zero;
                }

                return TotalDamageOut / BattleData.LogicTime;
            }
        }
        public DFix64 TotalDamageIn
        {
            get
            {
                return AttackDamageIn + AbilityDamageIn;
            }
        }


        public void Clear()
        {
            AttackDamageOut = DFix64.Zero;
            AbilityDamageOut = DFix64.Zero;
            AttackDamageIn = DFix64.Zero;
            AbilityDamageIn = DFix64.Zero;
            HealOut = DFix64.Zero;
            HealIn = DFix64.Zero;
            KillUnitCount = 0;
            KillIllusionCount = 0;
            KillSummonedCount = 0;
        }

        public BattleUnitStatisticsData Clone()
        {
            BattleUnitStatisticsData statisticsData = new BattleUnitStatisticsData();

            statisticsData.AttackDamageOut = AttackDamageOut;
            statisticsData.AbilityDamageOut = AbilityDamageOut;
            statisticsData.AttackDamageIn = AttackDamageIn;
            statisticsData.AbilityDamageIn = AbilityDamageIn;
            statisticsData.HealOut = HealOut;
            statisticsData.HealOut = HealIn;
            statisticsData.KillUnitCount = KillUnitCount;
            statisticsData.KillIllusionCount = KillIllusionCount;
            statisticsData.KillSummonedCount = KillSummonedCount;

            return statisticsData;
        }
    }
}