using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 治疗
    /*
        "Target"						目标
	    "Type"							类型
	    "Flags"							标志
	    "HealAmount"					治疗量
    */
    public class HealAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "Heal";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvType = kv["Type"];
            KeyValue kvHealAmount = kv["HealAmount"];
            KeyValue kvFlags = kv["Flags"];

            if (kvTarget == null)
            {
                Log.Error("Heal: 缺少参数:Target");
                return;
            }

            if (kvType == null)
            {
                Log.Error("Heal: 缺少参数:Type");
                return;
            }

            if (kvHealAmount == null)
            {
                Log.Error("Heal: 缺少参数:HealAmount");
                return;
            }

            HealType healType;
            if (!BattleData.TryEvaluateEnum(kvType.GetString(), out healType))
            {
                Log.Error("Heal: 解析Type失败 {0}", kvType.GetString());
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("Heal: 找不到目标");
                return;
            }

            DamageFlag damageFlags = DamageFlag.DAMAGE_FLAG_NONE;
            if (kvFlags != null)
            {
                if (!BattleData.TryEvaluateEnums(kvFlags.GetString(), out damageFlags))
                {
                    Log.Error("Heal: 解析Flags失败 {0}", kvFlags.GetString());
                }
            }

            DFix64 healAmount = DFix64.Floor(BattleData.EvaluateDFix64(kvHealAmount.GetString(), eventData));

            //Log.Debug("Heal: ApplyAbilityHeal, from:{0}, ability:{1}, healAmount:{2}, type:{3}, flags:{4}",
            //        eventData.Caster.LogName, eventData.Ability.LogName, healAmount.ToString(), healType.ToString(), damageFlags.ToString());

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("Heal: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;

                if (unitTarget.IsDeadState || unitTarget.IsInvincible)
                {
                    first = first.Next;
                    continue;
                }

                BattleData.ApplyAbilityHeal(eventData.Caster, eventData.Ability, eventData.Modifier, healAmount, healType, damageFlags, unitTarget);

                first = first.Next;
            }
        }
    }
}