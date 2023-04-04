// -----------------------------------------------
// Copyright © Jeffrey. All rights reserved.
// CreateTime: 2021/9/2   14:43:38
// -----------------------------------------------

using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 给目标设置参数
    // SetTeamValue
    /*
        "Target"					目标
        "Key"						参数名
	    "Value"				        值
    */
    public class SetTeamValueAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "SetTeamValue";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvKey = kv["Key"];
            KeyValue kvValue = kv["Value"];

            if (kvTarget == null)
            {
                Log.Error("SetValue: 缺少参数:Target");
                return;
            }

            if (kvKey == null)
            {
                Log.Error("SetValue: 缺少参数:Key");
                return;
            }

            if (kvValue == null)
            {
                Log.Error("SetValue: 缺少参数:Value");
                return;
            }

            if (string.IsNullOrEmpty(kvKey.GetString()))
            {
                Log.Error("SetValue: Key为空");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("SetValue: 找不到目标");
                return;
            }

            DFix64 value = DFix64.Floor(BattleData.EvaluateDFix64(kvValue.GetString(), eventData));

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("SetValue: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;

                Log.Info("SetTaamValue: {0} 设置队伍参数 {1} = {2}", unitTarget.LogName, kvKey.GetString(), value.ToString());

                unitTarget.Team[kvKey.GetString()] = value;

                first = first.Next;
            }
        }
    }
}