using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public partial class RunScriptAction
    {
        // 杀死幻象
        private void KillIllusion(KeyValue kv, EventData eventData)
        {
            if(eventData.Target == null || eventData.Target.IsDeadState)
            {
                return;
            }

            eventData.Target.Kill(null, null);

            if (eventData.Target.IsDeadState)
            {
                eventData.Target.SetHideState(true);

                EventData newEvent = BattleData.CreateEventData();
                newEvent.Unit = eventData.Target;
                newEvent.Point = eventData.Target.LogicPosition;
                newEvent.Node = eventData.Target.CurrNode;

                eventData.Target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEATH, newEvent);

                BattleData.SendFulltimeHandle(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH, newEvent);
                BattleData.SendFulltimeHandle(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH2, newEvent);
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

                    EventData newEvent = BattleData.CreateEventData();
                    newEvent.Unit = unitTarget;
                    newEvent.Point = unitTarget.LogicPosition;
                    newEvent.Node = unitTarget.CurrNode;

                    unitTarget.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEATH, newEvent);

                    BattleData.SendFulltimeHandle(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH, newEvent);
                    BattleData.SendFulltimeHandle(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH2, newEvent);
                }

                first = first.Next;
            }
        }
    }
}