using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 瞬移
    /*
        "Target"						移动单位
	    "BlinkToTarget"					目标
    */
    public class BlinkAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "Blink";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvBlinkToTarget = kv["BlinkToTarget"];

            if (kvTarget == null)
            {
                Log.Error("BlinkAction: 缺少参数:Target");
                return;
            }

            if (kvBlinkToTarget == null)
            {
                Log.Error("BlinkAction: 缺少参数:BlinkToTarget");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("BlinkAction: 找不到目标");
                return;
            }

            if (targets.Count > 1)
            {
                Log.Error("BlinkAction: 目标数量大于1");
                return;
            }

            if (targets.First.Value.Type != BattleTargetType.UNIT)
            {
                Log.Error("BlinkAction: 目标类型不是UNIT");
                return;
            }

            BaseUnit sourceUnit = (BaseUnit)targets.First.Value.Target;

            LinkedList<BattleTarget> blinkToTargets = FindTargets(kvBlinkToTarget, eventData);
            if (blinkToTargets == null)
            {
                Log.Warning("BlinkAction: 找不到blinkTo目标");
                return;
            }

            if (blinkToTargets.Count > 1)
            {
                Log.Warning("BlinkAction: 找不到blinkTo目标数量大于1");
                return;
            }

            BattleNode blinkToNode = null;
            if (blinkToTargets.First.Value.Type == BattleTargetType.UNIT)
            {
                BaseUnit unitBlinkToTarget = (BaseUnit)blinkToTargets.First.Value.Target;
                blinkToNode = BattleData.FindWalkableNeighbourNearest(unitBlinkToTarget.CurrNode);

                if (blinkToNode != null)
                {
                    sourceUnit.StopMoveOrder();
                    sourceUnit.SetNode(blinkToNode);
                    sourceUnit.ResetToNodePosition();
                    sourceUnit.SyncRenderPosition();

                    sourceUnit.LookToPositionY(unitBlinkToTarget.LogicPosition);
                    sourceUnit.SyncRenderRotation();

                    Log.Info("BlinkAction: {0} 瞬移到格子 x:{1},y:{2}", sourceUnit.LogName, blinkToNode.X.ToString(), blinkToNode.Y.ToString());
                }
            }
            else if (blinkToTargets.First.Value.Type == BattleTargetType.POINT)
            {
                DFixVector3 findPos = (DFixVector3)blinkToTargets.First.Value.Target;
                blinkToNode = BattleData.FindWalkableNeighbourNearest(findPos);
                if (blinkToNode != null)
                {
                    sourceUnit.StopMoveOrder();
                    sourceUnit.SetNode(blinkToNode);
                    sourceUnit.ResetToNodePosition();
                    sourceUnit.SyncRenderPosition();

                    sourceUnit.LookToPositionY(findPos);
                    sourceUnit.SyncRenderRotation();

                    Log.Info("BlinkAction: {0} 瞬移到格子 x:{1},y:{2}", sourceUnit.LogName, blinkToNode.X.ToString(), blinkToNode.Y.ToString());
                }
            }
            else if (blinkToTargets.First.Value.Type == BattleTargetType.NODE)
            {
                BattleNode findNode = (BattleNode)blinkToTargets.First.Value.Target;
                blinkToNode = BattleData.FindWalkableNeighbourNearest(findNode);
                if (blinkToNode != null)
                {
                    sourceUnit.StopMoveOrder();
                    sourceUnit.SetNode(blinkToNode);
                    sourceUnit.ResetToNodePosition();
                    sourceUnit.SyncRenderPosition();

                    sourceUnit.LookToPositionY(findNode.WorldPosition);
                    sourceUnit.SyncRenderRotation();

                    Log.Info("BlinkAction: {0} 瞬移到格子 x:{1},y:{2}", sourceUnit.LogName, blinkToNode.X.ToString(), blinkToNode.Y.ToString());
                }
            }
        }
    }
}