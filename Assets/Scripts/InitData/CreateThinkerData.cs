

namespace LiaoZhai.Runtime
{
    public class CreateThinkerData : CreateUnitData
    {
        public BaseUnit Caster { get; set; }
        public BattleNode Node { get; set; }
        public DFixVector3 Point{ get; set; }
        public DFixVector3 EulerAngles { get; set; }


        public CreateThinkerData()
        {
            unit_type = UnitType.UNIT_THINKER;
        }
    }
}
