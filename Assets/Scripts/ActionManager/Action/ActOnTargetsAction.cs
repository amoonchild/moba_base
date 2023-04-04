using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 对多个目标执行多个操作
    /*
        "Target"						目标
        "Action"						操作
    */
    public class ActOnTargetsAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "ActOnTargets";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            if (kvTarget == null)
            {
                Log.Error("ActOnTargets: 缺少参数:Target");
                return;
            }

            KeyValue kvAction = kv["Action"];
            if (kvAction == null)
            {
                Log.Error("ActOnTargets: 缺少参数:Action");
                return;
            }

            if (!kvAction.HasChildren)
            {
                Log.Warning("ActOnTargets: Action为空");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("ActOnTargets: 找不到目标");
                return;
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("ActOnTargets: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                EventData newEventData = eventData.Clone();
                eventData.Target = (BaseUnit)first.Value.Target;
                eventData.Point = eventData.Target.LogicPosition;
                eventData.Node = eventData.Target.CurrNode;

                BattleData.ExecuteActions(kvAction, eventData);

                first = first.Next;
            }
        }
    }
}