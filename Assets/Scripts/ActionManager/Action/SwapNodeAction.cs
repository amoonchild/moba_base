using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 交换位置
    /*
        "Target"					目标1
	    "SwapTarget"				目标2
    */
    public class SwapNodeAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "SwapNode";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvSwapTarget = kv["SwapTarget"];

            if (kvTarget == null)
            {
                Log.Error("SwapNode: 缺少参数:Target");
                return;
            }

            if (kvSwapTarget == null)
            {
                Log.Error("SwapNode: 缺少参数:SwapTarget");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("SwapNode: 找不到目标");
                return;
            }

            if (targets.Count > 1)
            {
                Log.Error("SwapNode: 目标数量大于1");
                return;
            }

            if (targets.First.Value.Type != BattleTargetType.UNIT)
            {
                Log.Error("SwapNode: 目标类型不是UNIT");
                return;
            }

            BaseUnit fromTarget = (BaseUnit)targets.First.Value.Target;

            LinkedList<BattleTarget> swapTargets = FindTargets(kvSwapTarget, eventData);
            if (swapTargets.Count == 0)
            {
                Log.Warning("SwapNode: 找不到swap目标");
                return;
            }

            if (swapTargets.Count > 1)
            {
                Log.Error("SwapNode: swap目标数量不能大于1");
                return;
            }

            if (swapTargets.First.Value.Type != BattleTargetType.UNIT)
            {
                Log.Error("SwapNode: swap目标类型不是UNIT");
                return;
            }

            BaseUnit toTarget = (BaseUnit)swapTargets.First.Value.Target;

            Log.Info("SwapNode: {0} 和 {1} 交换位置", fromTarget.LogName, toTarget.LogName);

            fromTarget.StopMoveOrder();
            if (fromTarget != eventData.Caster)
            {
                fromTarget.InterruptOrder();
            }

            toTarget.StopMoveOrder();
            if (toTarget != eventData.Caster)
            {
                toTarget.InterruptOrder();
            }

            BattleNode fromNode = fromTarget.CurrNode;
            BattleNode toNode = toTarget.CurrNode;

            fromTarget.SetNode(toNode);
            fromTarget.ResetToNodePosition();
            fromTarget.SyncRenderPosition();

            toTarget.SetNode(fromNode);
            toTarget.ResetToNodePosition();
            toTarget.SyncRenderPosition();
        }
    }
}