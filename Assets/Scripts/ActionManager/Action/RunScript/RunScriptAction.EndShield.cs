using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public partial class RunScriptAction
    {
        // 关闭护盾
        private void EndShield(KeyValue kv, EventData eventData)
        {
            if (eventData.Modifier == null)
            {
                return;
            }

            //if (eventData.Modifier.Target[BattleCustomValueKey.shield_modifier] == null)
            //{
            //    return;
            //}

            //int shieldModiferIndex = (int)eventData.Modifier.Target[BattleCustomValueKey.shield_modifier];
            //if (shieldModiferIndex != eventData.Modifier.ObjectId)
            //{
            //    return;
            //}

            //eventData.Modifier.Target.RemoveCustomValue(BattleCustomValueKey.shield_modifier);
            //eventData.Modifier.Target.RemoveCustomValue(BattleCustomValueKey.shield_value);

            eventData.Modifier.Target.RemoveModifier(eventData.Modifier);
        }
    }
}