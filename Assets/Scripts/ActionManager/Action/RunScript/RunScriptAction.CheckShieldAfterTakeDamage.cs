using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public partial class RunScriptAction
    {
        // 检测护盾
        private void CheckShieldAfterTakeDamage(KeyValue kv, EventData eventData)
        {
            if (eventData.Modifier == null)
            {
                return;
            }

            DFix64 finalDamage = (DFix64)eventData.Parms["finalDamage"];
            if (finalDamage == DFix64.Zero)
            {
                return;
            }

            DFix64 totalShield = eventData.Modifier["shield"];
            if (totalShield == DFix64.Zero)
            {
                return;
            }

            DFix64 loseShield = DFix64.Zero;

            if (finalDamage >= totalShield)
            {
                finalDamage -= totalShield;
                loseShield = totalShield;
                totalShield = DFix64.Zero;

                eventData.Parms["finalDamage"] = finalDamage;
                eventData.Modifier["shield"] = totalShield;

                Log.Info("{0} 的护盾 {2}({1}), 吸收 <color=green>{3}</color> 点伤害", eventData.Modifier.Target.LogName, eventData.Ability.LogName, eventData.Modifier.LogName, loseShield.ToString());
                
                eventData.Modifier.Target.RemoveModifier(eventData.Modifier);
            }
            else
            {
                loseShield = finalDamage;
                totalShield -= finalDamage;
                finalDamage = DFix64.Zero;

                eventData.Parms["finalDamage"] = finalDamage;
                eventData.Modifier["shield"] = totalShield;

                Log.Info("{0} 的护盾 {2}(1), 吸收 <color=green>{3}</color> 点伤害", eventData.Modifier.Target.LogName, eventData.Ability.LogName, eventData.Modifier.LogName, loseShield.ToString());
            }
        }
    }
}