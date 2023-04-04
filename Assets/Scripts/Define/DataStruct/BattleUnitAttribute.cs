using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    public class BattleUnitAttribute
    {
        private Dictionary<ObscuredInt, DFix64> _traitDamageOutBouns = new Dictionary<ObscuredInt, DFix64>();

        public DFix64 Attack = DFix64.Zero;
        public DFix64 Defense = DFix64.Zero;
        public DFix64 DefenseRatio = BattleData.Parm.Parm47;
        public DFix64 MaxHp = DFix64.Zero;
        public DFix64 Hp = DFix64.Zero;
        public DFix64 HpPct
        {
            get
            {
                if (MaxHp > DFix64.Zero)
                {
                    return Hp / MaxHp;
                }

                return DFix64.Zero;
            }
        }
        public DFix64 AttackSpeed = DFix64.Zero;
        public DFix64 MoveSpeed = DFix64.Zero;
        public DFix64 RotateSpeed = DFix64.Zero;
        public DFix64 CritOdds = DFix64.Zero;
        public DFix64 CritDamage = DFix64.Zero;
        public DFix64 CritResistance = DFix64.Zero;
        public DFix64 DodgeOdds = DFix64.Zero;
        public DFix64 HitOdds = DFix64.Zero;
        public DFix64 AbilityCDBonus = DFix64.Zero;
        public DFix64 PhysicalArmorBreak = DFix64.Zero;
        public DFix64 HpSteal = DFix64.Zero;
        public int AttackNodeRange = 0;
        public DFix64 DamageOutBonus = DFix64.Zero;
        public DFix64 DamageInBonus = DFix64.Zero;
        public DFix64 AttackDamageOutBonus = DFix64.Zero;
        public DFix64 AttackDamageInBonus = DFix64.Zero;
        public DFix64 AbilityDamageOutBonus = DFix64.Zero;
        public DFix64 AbilityDamageInBonus = DFix64.Zero;
        public DFix64 HealOutBonus = DFix64.Zero;
        public DFix64 HealInBonus = DFix64.Zero;
        public DFix64 FinalDamageOutBouns = DFix64.Zero;
        public DFix64 FinalDamageInBouns = DFix64.Zero;
        public DFix64 Mana = DFix64.Zero;
        public DFix64 MaxMana = DFix64.Zero;
        public DFix64 ManaRegen = DFix64.Zero;
        public DFix64 Scale = DFix64.One;
        public DFix64 IsIllusion = DFix64.Zero;
        public DFix64 IsSummoned = DFix64.Zero;
        public Dictionary<ObscuredInt, DFix64> TraitDamageOutBouns
        {
            get
            {
                return _traitDamageOutBouns;
            }
        }


        public BattleUnitAttribute CopyTo(BattleUnitAttribute unitAttribute)
        {
            unitAttribute.Attack = Attack;
            unitAttribute.Defense = Defense;
            unitAttribute.DefenseRatio = DefenseRatio;
            unitAttribute.MaxHp = MaxHp;
            unitAttribute.Hp = Hp;
            unitAttribute.AttackSpeed = AttackSpeed;
            unitAttribute.MoveSpeed = MoveSpeed;
            unitAttribute.RotateSpeed = RotateSpeed;
            unitAttribute.CritOdds = CritOdds;
            unitAttribute.CritDamage = CritDamage;
            unitAttribute.CritResistance = CritResistance;
            unitAttribute.DodgeOdds = DodgeOdds;
            unitAttribute.HitOdds = HitOdds;
            unitAttribute.AbilityCDBonus = AbilityCDBonus;
            unitAttribute.PhysicalArmorBreak = PhysicalArmorBreak;
            unitAttribute.HpSteal = HpSteal;
            unitAttribute.AttackNodeRange = AttackNodeRange;
            unitAttribute.DamageOutBonus = DamageOutBonus;
            unitAttribute.DamageInBonus = DamageInBonus;
            unitAttribute.AttackDamageOutBonus = AttackDamageOutBonus;
            unitAttribute.AttackDamageInBonus = AttackDamageInBonus;
            unitAttribute.AbilityDamageOutBonus = AbilityDamageOutBonus;
            unitAttribute.AbilityDamageInBonus = AbilityDamageInBonus;
            unitAttribute.HealOutBonus = HealOutBonus;
            unitAttribute.HealInBonus = HealInBonus;
            unitAttribute.FinalDamageOutBouns = FinalDamageOutBouns;
            unitAttribute.FinalDamageInBouns = FinalDamageInBouns;
            unitAttribute.Mana = Mana;
            unitAttribute.MaxMana = MaxMana;
            unitAttribute.ManaRegen = ManaRegen;
            unitAttribute.Scale = Scale;

            unitAttribute.TraitDamageOutBouns.Clear();
            foreach (var traitDamageOutBoun in _traitDamageOutBouns)
            {
                unitAttribute.TraitDamageOutBouns.Add(traitDamageOutBoun.Key, traitDamageOutBoun.Value);
            }

            return unitAttribute;
        }
        public BattleUnitAttribute Clone()
        {
            BattleUnitAttribute unitAttribute = new BattleUnitAttribute();

            unitAttribute.Attack = Attack;
            unitAttribute.Defense = Defense;
            unitAttribute.DefenseRatio = DefenseRatio;
            unitAttribute.MaxHp = MaxHp;
            unitAttribute.Hp = Hp;
            unitAttribute.AttackSpeed = AttackSpeed;
            unitAttribute.MoveSpeed = MoveSpeed;
            unitAttribute.RotateSpeed = RotateSpeed;
            unitAttribute.CritOdds = CritOdds;
            unitAttribute.CritDamage = CritDamage;
            unitAttribute.CritResistance = CritResistance;
            unitAttribute.DodgeOdds = DodgeOdds;
            unitAttribute.HitOdds = HitOdds;
            unitAttribute.AbilityCDBonus = AbilityCDBonus;
            unitAttribute.PhysicalArmorBreak = PhysicalArmorBreak;
            unitAttribute.HpSteal = HpSteal;
            unitAttribute.AttackNodeRange = AttackNodeRange;
            unitAttribute.DamageOutBonus = DamageOutBonus;
            unitAttribute.DamageInBonus = DamageInBonus;
            unitAttribute.AttackDamageOutBonus = AttackDamageOutBonus;
            unitAttribute.AttackDamageInBonus = AttackDamageInBonus;
            unitAttribute.AbilityDamageOutBonus = AbilityDamageOutBonus;
            unitAttribute.AbilityDamageInBonus = AbilityDamageInBonus;
            unitAttribute.HealOutBonus = HealOutBonus;
            unitAttribute.HealInBonus = HealInBonus;
            unitAttribute.FinalDamageOutBouns = FinalDamageOutBouns;
            unitAttribute.FinalDamageInBouns = FinalDamageInBouns;
            unitAttribute.Mana = Mana;
            unitAttribute.MaxMana = MaxMana;
            unitAttribute.ManaRegen = ManaRegen;
            unitAttribute.Scale = Scale;

            foreach (var traitDamageOutBoun in _traitDamageOutBouns)
            {
                unitAttribute.TraitDamageOutBouns.Add(traitDamageOutBoun.Key, traitDamageOutBoun.Value);
            }

            return unitAttribute;
        }
        public bool IsSame(BattleUnitAttribute target)
        {
            if (target.Attack != Attack) { return false; }
            if (target.Defense != Defense) { return false; }
            if (target.DefenseRatio != DefenseRatio) { return false; }
            if (target.AttackSpeed != AttackSpeed) { return false; }
            if (target.MoveSpeed != MoveSpeed) { return false; }
            if (target.RotateSpeed != RotateSpeed) { return false; }
            if (target.CritOdds != CritOdds) { return false; }
            if (target.CritDamage != CritDamage) { return false; }
            if (target.CritResistance != CritResistance) { return false; }
            if (target.DodgeOdds != DodgeOdds) { return false; }
            if (target.HitOdds != HitOdds) { return false; }
            if (target.AbilityCDBonus != AbilityCDBonus) { return false; }
            if (target.PhysicalArmorBreak != PhysicalArmorBreak) { return false; }
            if (target.HpSteal != HpSteal) { return false; }
            if (target.AttackNodeRange != AttackNodeRange) { return false; }
            if (target.DamageOutBonus != DamageOutBonus) { return false; }
            if (target.DamageInBonus != DamageInBonus) { return false; }
            if (target.AttackDamageOutBonus != AttackDamageOutBonus) { return false; }
            if (target.AttackDamageInBonus != AttackDamageInBonus) { return false; }
            if (target.AbilityDamageOutBonus != AbilityDamageOutBonus) { return false; }
            if (target.AbilityDamageInBonus != AbilityDamageInBonus) { return false; }
            if (target.HealOutBonus != HealOutBonus) { return false; }
            if (target.HealInBonus != HealInBonus) { return false; }
            if (target.FinalDamageOutBouns != FinalDamageOutBouns) { return false; }
            if (target.FinalDamageInBouns != FinalDamageInBouns) { return false; }
            if (target.ManaRegen != ManaRegen) { return false; }

            return true;
        }
    }
}