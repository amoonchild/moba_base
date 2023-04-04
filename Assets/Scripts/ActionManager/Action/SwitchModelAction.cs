using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 切换模型
    /*
        "Target"						目标
	    "ModelIndex"				    模型索引
    */
    public class SwitchModelAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "SwitchModel";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvModelIndex = kv["ModelIndex"];


            if (kvTarget == null)
            {
                Log.Error("SwitchModel: 缺少参数:Target");
                return;
            }

            if (kvModelIndex == null)
            {
                Log.Error("SwitchModel: 缺少参数:ModelIndex");
                return;
            }

            int modelIndex = BattleData.ParseInt(kvModelIndex.GetString());
            if (modelIndex < 0)
            {
                Log.Error("SwitchModel: ModelIndex小于0");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("SwitchModel: 找不到目标");
                return;
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("SwitchModel: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;

                Log.Info("SwitchModel: {0} 切换到第 {1} 组模型", unitTarget.LogName, modelIndex);

                unitTarget.ChangeModelByIndex(modelIndex);

                first = first.Next;
            }
        }
    }
}