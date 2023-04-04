// -----------------------------------------------
// Copyright © Jeffrey. All rights reserved.
// CreateTime: 2021/7/15   18:16:19
// -----------------------------------------------

using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public partial class RunScriptAction
    {
        // 调整伤害
        private void ChangeFinalHeal(KeyValue kv, EventData eventData)
        {
            if (eventData.Modifier == null)
            {
                return;
            }

            KeyValue kvFinalHeal = kv["FinalHeal"];
            if (kvFinalHeal == null)
            {
                return;
            }

            if (!eventData.Parms.ContainsKey("finalHeal"))
            {
                return;
            }

            DFix64 finalDamage = eventData.Parms["finalHeal"];
            eventData.Parms["finalHeal"] = DFix64.Floor(BattleData.EvaluateDFix64(kvFinalHeal.GetString(), eventData));

            Log.Info("治疗修正 {0} -> {1}", finalDamage.ToString(), eventData.Parms["finalHeal"].ToString());
        }
    }
}