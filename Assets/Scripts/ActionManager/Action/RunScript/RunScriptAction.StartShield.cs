using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public partial class RunScriptAction
    {
        // 开启护盾
        private void StartShield(KeyValue kv, EventData eventData)
        {
            KeyValue kvShieldValue = kv["Shield"];
            if (kvShieldValue == null)
            {
                return;
            }

            if (!eventData.Modifier.Target.IsAlive)
            {
                //Log.Error("生命值为0的单位不能添加护盾 {0}", eventData.Modifier.Target.LogName);
                return;
            }

            DFix64 shield = BattleData.EvaluateDFix64(kvShieldValue.GetString(), eventData);
            shield = DFix64.Floor(shield);

            eventData.Modifier["shield"] = shield;

            Log.Info("{0} 获得来自 {1} 的 <color=green>{2}</color> 点护盾", eventData.Modifier.Target.LogName, eventData.Ability.LogName, shield.ToString());
        }
    }
}