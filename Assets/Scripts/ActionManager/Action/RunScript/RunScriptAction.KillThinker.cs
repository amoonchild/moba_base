using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public partial class RunScriptAction
    {
        // 杀死定时器
        private void KillThinker(KeyValue kv, EventData eventData)
        {
            if (eventData.Target == null || eventData.Target.IsDeadState)
            {
                return;
            }

            eventData.Target.Kill(null, null);

            if (eventData.Target.IsDeadState)
            {
                eventData.Target.SetHideState(true);
            }

            return;

            LinkedList<BattleTarget> targets = FindTargets(kv["Target"], eventData);
            if (targets.Count == 0)
            {
                return;
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;
                if (unitTarget.IsDeadState)
                {
                    first = first.Next;
                    continue;
                }

                unitTarget.Kill(null, null);

                if (unitTarget.IsDeadState)
                {
                    unitTarget.SetHideState(true);
                }

                first = first.Next;
            }
        }
    }
}