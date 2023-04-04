using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public partial class RunScriptAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "RunScript";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvFunction = kv["Function"];
            if (kvFunction != null)
            {
                switch (kvFunction.GetString())
                {
                    case "KillIllusion": { KillIllusion(kv, eventData); } break;
                    case "KillSummoned": { KillSummoned(kv, eventData); } break;
                    case "KillThinker": { KillThinker(kv, eventData); } break;
                    case "StartShield": { StartShield(kv, eventData); } break;
                    case "EndShield": { EndShield(kv, eventData); } break;
                    case "CheckShield":
                        {
                            CheckShieldAfterTakeDamage(kv, eventData);
                        }
                        break;
                    case "ChangeFinalDamage": { ChangeFinalDamage(kv, eventData); } break;
                    case "ChangeFinalHeal": { ChangeFinalHeal(kv, eventData); } break;
                    case "DropItem": { DropItem(kv, eventData); } break;
                    default: break;
                }
            }
        }
    }
}