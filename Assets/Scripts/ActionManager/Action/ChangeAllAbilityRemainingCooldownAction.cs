// -----------------------------------------------
// Copyright © Jeffrey. All rights reserved.
// CreateTime: 2021/7/16   0:12:56
// -----------------------------------------------

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
    public class ChangeAllAbilityRemainingCooldownAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "ChangeAllAbilityCooldown";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvTime = kv["TimeBouns"];

            if (kvTarget == null)
            {
                Log.Error("ChangeAbilityCooldown: 缺少参数:Target");
                return;
            }

            if (kvTime == null)
            {
                Log.Error("ChangeAbilityCooldown: 缺少参数:TimeBouns");
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

                unitTarget.ChangeAllAbilityCooldown(timeBouns);

                first = first.Next;
            }
        }
    }
}