using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public partial class RunScriptAction
    {
        // 调整伤害
        private void ChangeFinalDamage(KeyValue kv, EventData eventData)
        {
            if (eventData.Modifier == null)
            {
                return;
            }

            KeyValue kvFinalDamage = kv["FinalDamage"];
            if (kvFinalDamage == null)
            {
                return;
            }

            if (!eventData.Parms.ContainsKey("finalDamage"))
            {
                return;
            }

            DFix64 finalDamage = eventData.Parms["finalDamage"];
            eventData.Parms["finalDamage"] = DFix64.Floor(BattleData.EvaluateDFix64(kvFinalDamage.GetString(), eventData));

            Log.Info("伤害修正 {0} -> {1}", finalDamage.ToString(), eventData.Parms["finalDamage"].ToString());
        }
    }
}