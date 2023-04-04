using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 伤害
    /*
        "Target"						目标
	    "Type"							类型
	    "Flags"							标志
	    "Damage"						伤害值
	    "DefenseIgnore"					忽略防御参数
	    "CleaveDamage"					多个目标时平摊伤害
    */
    public class DamageAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "Damage";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            if (kvTarget == null)
            {
                Log.Error("Damage: 缺少参数:Target");
                return;
            }

            KeyValue kvType = kv["Type"];
            if (kvType == null)
            {
                Log.Error("Damage: 缺少参数:Type");
                return;
            }

            KeyValue kvDamage = kv["Damage"];
            if (kvDamage == null)
            {
                Log.Error("Damage: 缺少参数:Damage");
                return;
            }

            DamageType damageType;
            if (!BattleData.TryEvaluateEnum(kvType.GetString(), out damageType))
            {
                Log.Error("Damage: 解析Type失败 {0}", kvType.GetString());
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("Damage: 找不到目标");
                return;
            }

            DFix64 damage = DFix64.Floor(BattleData.EvaluateDFix64(kvDamage.GetString(), eventData));

            DFix64 defenseIgnore = DFix64.Zero;
            KeyValue kvDefenseIgnore = kv["DefenseIgnore"];
            if (kvDefenseIgnore != null)
            {
                defenseIgnore = DFix64.Floor(BattleData.EvaluateDFix64(kvDefenseIgnore.GetString(), eventData));
            }

            DamageFlag damageFlags = DamageFlag.DAMAGE_FLAG_NONE;
            KeyValue kvFlags = kv["Flags"];
            if (kvFlags != null)
            {
                if (!BattleData.TryEvaluateEnums(kvFlags.GetString(), out damageFlags))
                {
                    Log.Error("Damage: 解析Flags失败 {0}", kvFlags.GetString());
                }
            }

            bool isCleaveDamage = false;
            KeyValue kvCleaveDamage = kv["CleaveDamage"];
            if (kvCleaveDamage != null)
            {
                isCleaveDamage = BattleData.ParseBool01(kvCleaveDamage.GetString());
            }

            bool isCrit = false;

            KeyValue kvCritOdds = kv["CritOdds"];
            if (kvCritOdds != null)
            {
                DFix64 critOdds = BattleData.EvaluateDFix64(kvCritOdds.GetString(), eventData);
                DFix64 critRange = DFix64.Clamp((eventData.Caster.CurrAtt.CritOdds + critOdds) / DFix64.Thousand, BattleData.Parm.Parm20, BattleData.Parm.Parm21);
                Log.Info("暴击修正 {0}  range:0-{1}", critOdds.ToString(), critRange.ToString());
                if (critRange > DFix64.Zero)
                {
                    isCrit = BattleData.SRandom.RangeDFx(DFix64.Zero, DFix64.One) < critRange;
                }
            }
            else
            {
                DFix64 critRange = DFix64.Clamp(eventData.Caster.CurrAtt.CritOdds / DFix64.Thousand, BattleData.Parm.Parm20, BattleData.Parm.Parm21);
                if (critRange > DFix64.Zero)
                {
                    isCrit = BattleData.SRandom.RangeDFx(DFix64.Zero, DFix64.One) < critRange;
                }
            }

            DFix64 hitBuff = DFix64.Zero;
            KeyValue kvHitOdds = kv["HitOdds"];
            if (kvHitOdds != null)
            {
                hitBuff = BattleData.ParseDFix64(kvHitOdds.GetString());
            }

            if (isCleaveDamage)
            {
                if (targets.Count > 1)
                {
                    damage = DFix64.Floor(damage / (DFix64)targets.Count);
                }

                LinkedListNode<BattleTarget> first = targets.First;
                while (first != null)
                {
                    if (first.Value.Type != BattleTargetType.UNIT)
                    {
                        Log.Error("Damage: 目标类型不是UNIT");
                        first = first.Next;
                        continue;
                    }

                    BaseUnit unitTarget = (BaseUnit)first.Value.Target;
                    BattleData.ApplyAbilityDamage(unitTarget, eventData.Caster, eventData.Ability, eventData.Modifier, damage, damageType, damageFlags, isCrit, defenseIgnore, hitBuff);

                    first = first.Next;
                }
            }
            else
            {
                LinkedListNode<BattleTarget> first = targets.First;
                while (first != null)
                {
                    if (first.Value.Type != BattleTargetType.UNIT)
                    {
                        Log.Error("Damage: 目标类型不是UNIT");
                        first = first.Next;
                        continue;
                    }

                    BaseUnit unitTarget = (BaseUnit)first.Value.Target;
                    BattleData.ApplyAbilityDamage(unitTarget, eventData.Caster, eventData.Ability, eventData.Modifier, damage, damageType, damageFlags, isCrit, defenseIgnore, hitBuff);

                    first = first.Next;
                }
            }
        }
    }
}