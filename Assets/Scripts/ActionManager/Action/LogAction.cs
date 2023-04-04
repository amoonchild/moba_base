using KVLib;


namespace LiaoZhai.Runtime
{
    public class LogAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "Log";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            return;

            KeyValue kvLevel = null;
            KeyValue kvFormat = null;
            KeyValue kvMsg = null;
            KeyValue kvExpression = null;

            foreach (var child in kv.Children)
            {
                switch (child.Key)
                {
                    case "Level": { kvLevel = child; } break;
                    case "Format": { kvFormat = child; } break;
                    case "Msg": { kvMsg = child; } break;
                    case "Expression": { kvExpression = child; } break;
                }
            }

            if (kvLevel == null)
            {
                Log.Error("Log: 缺少参数:Level");
                return;
            }

            if (kvMsg == null)
            {
                Log.Error("Log: 缺少参数:Msg");
                return;
            }

            int level = BattleData.ParseInt(kvLevel.GetString());
            if (level < 1 || level > 3)
            {
                Log.Error("Log: Level不在1-3之间");
                return;
            }

            switch (level)
            {
                case 1:
                    {
                        if (kvFormat != null)
                        {
                            Log.Info(kvFormat.GetString(), kvMsg.GetString());
                        }
                        else
                        {
                            Log.Info(kvMsg.GetString());
                        }
                    }
                    break;
                case 2:
                    {
                        if (kvFormat != null)
                        {
                            Log.Warning(kvFormat.GetString(), kvMsg.GetString());
                        }
                        else
                        {
                            Log.Warning(kvMsg.GetString());
                        }
                    }
                    break;
                case 3:
                    {
                        if (kvFormat != null)
                        {
                            Log.Error(kvFormat.GetString(), kvMsg.GetString());
                        }
                        else
                        {
                            Log.Error(kvMsg.GetString());
                        }
                    }
                    break;
            }
        }
    }
}