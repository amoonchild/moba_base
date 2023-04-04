using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 改变攻击目标
    /*
        "Target"						目标
        "AttackTo"						攻击目标
        "Immediate"                     立刻执行,打断当前行为
    */
    public class ChangeAttackTargetAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "ChangeAttackTarget";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvAttackTo = kv["AttackTo"];
            KeyValue kvImmediate = kv["Immediate"];

            if (kvTarget == null)
            {
                Log.Error("ChangeAttackTarget: 缺少参数:Target");
                return;
            }

            if (kvAttackTo == null)
            {
                Log.Error("ChangeAttackTarget: 缺少参数:AttackTo");
                return;
            }

            LinkedList<BattleTarget> attackToTargets = FindTargets(kvAttackTo, eventData);
            if (attackToTargets == null)
            {
                Log.Warning("ChangeAttackTarget: 找不到attackTo目标");
                return;
            }

            //if (attackToTargets.Count > 1)
            //{
            //    Log.Error("ChangeAttackTarget: attackTo目标数量大于1");
            //    return;
            //}

            if (attackToTargets.First.Value.Type != BattleTargetType.UNIT)
            {
                Log.Error("ChangeAttackTarget: attackTo目标类型不是UNIT");
                return;
            }

            BaseUnit attackToUnitTarget = (BaseUnit)attackToTargets.First.Value.Target;
            if (attackToUnitTarget.IsDeadState)
            {
                Log.Warning("ChangeAttackTarget: attackTo目标已死亡");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("ChangeAttackTarget: 找不到目标");
                return;
            }

            bool isImmediate = kvImmediate != null ? BattleData.ParseBool01(kvImmediate.GetString()) : false;
            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("ChangeAttackTarget: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;
                if (unitTarget.IsDeadState || attackToUnitTarget.IsDeadState)
                {
                    Log.Warning("ChangeAttackTarget: 目标或attackTo目标已死亡");
                    first = first.Next;
                    continue;
                }

                Log.Info("ChangeAttackTarget: {0} 改变攻击目标为 {1}", unitTarget.LogName, attackToUnitTarget.LogName);

                unitTarget.FroceAttackTarget(attackToUnitTarget, isImmediate);

                first = first.Next;
            }
        }
    }
}