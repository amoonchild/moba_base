using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 改变技能剩余冷却时间
    /*
        "Target"						目标
	    "AbilityId"						技能id
	    "TimeBouns"					    时间
    */
    public class ChangeAbilityRemainingCooldownAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "ChangeAbilityCooldown";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvAbilityId = kv["AbilityId"];
            KeyValue kvTime = kv["TimeBouns"];

            if (kvTarget == null)
            {
                Log.Error("ChangeAbilityCooldown: 缺少参数:Target");
                return;
            }

            if (kvAbilityId == null)
            {
                Log.Error("ChangeAbilityCooldown: 缺少参数:AbilityId");
                return;
            }

            if (kvTime == null)
            {
                Log.Error("ChangeAbilityCooldown: 缺少参数:TimeBouns");
                return;
            }

            int abilityId = BattleData.ParseInt(kvAbilityId.GetString());
            if (abilityId == 0)
            {
                Log.Error("ChangeAbilityCooldown: AbilityId为0");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("ChangeAbilityCooldown: 找不到目标");
                return;
            }

            DFix64 timeBouns = BattleData.EvaluateDFix64(kvTime.GetString(), eventData);

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("ChangeAbilityCooldown: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;
                if (unitTarget.IsDeadState)
                {
                    Log.Warning("ChangeAbilityCooldown: 目标已死亡");
                    first = first.Next;
                    continue;
                }

                unitTarget.ChangAbilityCooldown(abilityId, timeBouns);

                first = first.Next;
            }
        }
    }
}