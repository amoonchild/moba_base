using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 添加Modifier
    /*
        "Target"						目标
	    "ModifierSerialId"				Modifier序列号
	    "Duration"						Modifier持续时间 
    */
    public class ApplyModifierAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "ApplyModifier";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            if (kvTarget == null)
            {
                Log.Error("ApplyModifier: 缺少参数:Target");
                return;
            }

            KeyValue kvModifierSerialId = kv["ModifierSerialId"];
            if (kvModifierSerialId == null)
            {
                Log.Error("ApplyModifier: 缺少参数:ModifierSerialId");
                return;
            }

            int modifierSerialId = BattleData.ParseInt(kvModifierSerialId.GetString());
            if (modifierSerialId == 0)
            {
                Log.Error("ApplyModifier: ModifierSerialId为0");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("ApplyModifier: 找不到目标");
                return;
            }

            CreateModifierData createModifierData = new CreateModifierData();

            KeyValue kvDuration = kv["Duration"];
            if (kvDuration != null)
            {
                createModifierData.Duration = DFix64.Max(BattleData.EvaluateDFix64(kvDuration.GetString(), eventData), -DFix64.One);
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("ApplyModifier: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;
                //if (unitTarget.IsDeadState || unitTarget.IsInvincible)
                //{
                //    first = first.Next;
                //    continue;
                //}

                unitTarget.ApplyModifier(eventData.Caster, eventData.Ability, modifierSerialId, createModifierData, true);

                first = first.Next;
            }
        }
    }
}