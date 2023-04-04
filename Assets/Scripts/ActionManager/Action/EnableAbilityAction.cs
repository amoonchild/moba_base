using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 启用技能
    /*
        "Target"						目标
	    "AbilityId"						技能id
    */
    public class EnableAbilityAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "EnableAbility";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvAbilityId = kv["AbilityId"];

            if (kvTarget == null)
            {
                Log.Error("EnableAbility: 缺少参数:Target");
                return;
            }

            if (kvAbilityId == null)
            {
                Log.Error("EnableAbility: 缺少参数:AbilityId");
                return;
            }

            int abilityId = BattleData.ParseInt(kvAbilityId.GetString());
            if (abilityId == 0)
            {
                Log.Error("EnableAbility: AbilityId为0");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("EnableAbility: 找不到目标");
                return;
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("EnableAbility: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;
                if (unitTarget.IsDeadState)
                {
                    Log.Warning("EnableAbility: 目标已死亡");
                    first = first.Next;
                    continue;
                }

                Ability ability = unitTarget.FindAbilityByAbilityId(abilityId);
                if (ability == null)
                {
                    Log.Error("EnableAbility: 目标没有技能 {0}", abilityId.ToString());
                    first = first.Next;
                    continue;
                }

                if (ability.IsActivated)
                {
                    Log.Warning("EnableAbility: 目标技能 {0} 已被启用", abilityId.ToString());
                    first = first.Next;
                    continue;
                }

                Log.Info("EnableAbility: {0} 的技能 {1} 被启用", unitTarget.LogName, ability.LogName);

                ability.Enable();

                first = first.Next;
            }
        }
    }
}