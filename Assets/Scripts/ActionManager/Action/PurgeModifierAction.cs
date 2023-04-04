using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 驱散Modifier
    /*
        "Target"					目标
	    "ModifierGroup"				Modifier group
	    "Buff"						驱散增益
	    "Debuff"					驱散减益
	    "Nebuff"					驱散中立buff
	    "Level"						驱散等级,小于该等级的会被驱散
	    "MaxCount"					最大驱散数量,大于1随机驱散 
    */
    public class PurgeModifierAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "PurgeModifier";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvModifierGroup = kv["ModifierGroup"];
            KeyValue kvBuff = kv["Buff"];
            KeyValue kvDebuff = kv["Debuff"];
            KeyValue kvNebuff = kv["Nebuff"];
            KeyValue kvLevel = kv["Level"];
            KeyValue kvMaxCount = kv["MaxCount"];

            if (kvTarget == null)
            {
                Log.Error("PurgeModifier: 缺少参数:Target");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("PurgeModifier: 找不到目标");
                return;
            }

            int modifierGroup = kvModifierGroup != null ? BattleData.ParseInt(kvModifierGroup.GetString()) : 0;
            bool buff = kvBuff != null ? BattleData.ParseBool01(kvBuff.GetString()) : false;
            bool debuff = kvDebuff != null ? BattleData.ParseBool01(kvDebuff.GetString()) : false;
            bool nebuff = kvNebuff != null ? BattleData.ParseBool01(kvNebuff.GetString()) : false;
            int level = kvLevel != null ? BattleData.ParseInt(kvLevel.GetString()) : -1;
            int maxCount = kvMaxCount != null ? BattleData.ParseInt(kvMaxCount.GetString()) : -1;

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("PurgeModifier: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;

                if (unitTarget.IsDeadState)
                {
                    Log.Warning("PurgeModifier: 目标已死亡");
                    first = first.Next;
                    continue;
                }

                unitTarget.PurgeModifier(modifierGroup, buff, debuff, nebuff, level, maxCount);

                first = first.Next;
            }
        }
    }
}