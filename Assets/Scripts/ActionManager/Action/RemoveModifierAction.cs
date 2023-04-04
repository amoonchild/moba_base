using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 移除Modifier
    /*
        "Target"						目标
	    "ModifierSerialId"				Modifier序列号
    */
    public class RemoveModifierAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "RemoveModifier";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvModifierSerialId = kv["ModifierSerialId"];

            if (kvTarget == null)
            {
                Log.Error("RemoveModifier: 缺少参数:Target");
                return;
            }

            if (kvModifierSerialId == null)
            {
                Log.Error("RemoveModifier: 缺少参数:ModifierSerialId");
                return;
            }

            int modifierSerialId = BattleData.ParseInt(kvModifierSerialId.GetString());
            if (modifierSerialId == 0)
            {
                Log.Error("RemoveModifier: ModifierSerialId为0");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("RemoveModifier: 找不到目标");
                return;
            }

            int number = -1;
            KeyValue kvNumber = kv["Number"];
            if (kvNumber != null)
            {
                number = (int)BattleData.EvaluateLong(kvNumber.GetString(), eventData);
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("RemoveModifier: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;

                //if (unitTarget.IsDeadState)
                //{
                //    Log.Warning("RemoveModifier: 目标已死亡");
                //    first = first.Next;
                //    continue;
                //}

                unitTarget.RemoveModifier(modifierSerialId, number);

                first = first.Next;
            }
        }
    }
}