using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 禁用技能
    /*
        "Target"						目标
	    "AbilityId"						技能id
    */
    public class DisableAbilityAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "DisableAbility";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvAbilityId = kv["AbilityId"];

            if (kvTarget == null)
            {
                Log.Error("DisableAbility: 缺少参数:Target");
                return;
            }

            if (kvAbilityId == null)
            {
                Log.Error("DisableAbility: 缺少参数:AbilityId");
                return;
            }

            int abilityId = BattleData.ParseInt(kvAbilityId.GetString());
            if (abilityId == 0)
            {
                Log.Error("DisableAbility: AbilityId为0");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("DisableAbility: 找不到目标");
                return;
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("DisableAbility: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;
                if (unitTarget.IsDeadState)
                {
                    Log.Warning("DisableAbility: 目标已死亡");
                    first = first.Next;
                    continue;
                }

                Ability ability = unitTarget.FindAbilityByAbilityId(abilityId);
                if (ability == null)
                {
                    Log.Error("DisableAbility: 目标没有技能 {0}", abilityId.ToString());
                    first = first.Next;
                    continue;
                }

                if (!ability.IsActivated)
                {
                    Log.Warning("DisableAbility: 目标技能 {0} 已被禁用", abilityId.ToString());
                    first = first.Next;
                    continue;
                }

                Log.Info("DisableAbility: {0} 的技能 {1} 被禁用", unitTarget.LogName, ability.LogName);

                ability.Disable();

                first = first.Next;
            }
        }
    }
}