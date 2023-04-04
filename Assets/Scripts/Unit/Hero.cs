

namespace LiaoZhai.Runtime
{
    public class Hero : BaseUnit
    {
        public override UnitType Type
        {
            get
            {
                return UnitType.UNIT_HERO;
            }
        }


        public Hero(CreateUnitData data)
            : base(data)
        {

        }


        public override void Kill(BaseUnit killer, Ability killerAbility)
        {
            base.Kill(killer, killerAbility);

            if (_isDeadState)
            {
                BattleData.CheckResult(BattleCheckResultType.HeroDead);
            }
        }
    }
}