using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 随机
    /*
        "Chance"					几率,0-x随机
	    "Condition"					条件,0或1
	
	    "OnSuccess"					成功后的操作
	    {
	
	    }
	
	    "OnFailure"					失败后的操作
	    {
	
	    }
    */
    public class RandomAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "Random";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvChance = kv["Chance"];
            KeyValue kvCondition = kv["Condition"];
            KeyValue kvOnSuccess = kv["OnSuccess"];
            KeyValue kvOnFailure = kv["OnFailure"];

            if (kvChance == null && kvCondition == null)
            {
                Log.Error("Random: 缺少参数:Chance或Condition");
                return;
            }

            bool isSuccess = false;

            if (kvChance != null)
            {
                DFix64 chance = BattleData.EvaluateDFix64(kvChance.GetString(), eventData);
                isSuccess = BattleData.SRandom.RangeDFx(DFix64.Zero, DFix64.One) < chance;
            }
            else if (kvCondition != null)
            {
                isSuccess = BattleData.EvaluateBool(kvCondition.GetString(), eventData);
            }

            if (isSuccess)
            {
                Log.Info("Random: 成功, {0}, {1}", eventData.Modifier != null ? eventData.Modifier.LogName : (eventData.Ability != null ? eventData.Ability.LogName : "null"),
                    kvChance != null ? kvChance.GetString() : (kvCondition != null ? kvCondition.GetString() : string.Empty));

                if (kvOnSuccess != null)
                {
                    BattleData.ExecuteActions(kvOnSuccess, eventData);
                }
            }
            else
            {
                Log.Info("Random: 失败, {0}, {1}", eventData.Modifier != null ? eventData.Modifier.LogName : (eventData.Ability != null ? eventData.Ability.LogName : "null"),
                    kvChance != null ? kvChance.GetString() : (kvCondition != null ? kvCondition.GetString() : string.Empty));

                if (kvOnFailure != null)
                {
                    BattleData.ExecuteActions(kvOnFailure, eventData);
                }
            }
        }
    }
}