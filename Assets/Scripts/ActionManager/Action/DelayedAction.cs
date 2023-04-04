using KVLib;


namespace LiaoZhai.Runtime
{
    // 延迟操作
    /*
        "Target"						目标
        "Action"						操作
    */
    public class DelayedAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "DelayedAction";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvAction = kv["Action"];
            KeyValue kvDelay = kv["Delay"];

            if (kvAction == null)
            {
                Log.Error("DelayedAction: 缺少参数:Action");
                return;
            }

            if (kvDelay == null)
            {
                Log.Error("DelayedAction: 缺少参数:Delay");
                return;
            }

            if (!kvAction.HasChildren)
            {
                Log.Error("DelayedAction: Action为空");
                return;
            }

            DFix64 delay = DFix64.Zero;
            if (kvDelay != null)
            {
                delay = BattleData.EvaluateDFix64(kvDelay.GetString(), eventData);
            }

            if (delay > DFix64.Zero)
            {
                Log.Info("DelayedAction: 添加延迟操作, 延迟:{0}秒", delay.ToString());

                BattleData.AddDelayAction(delay, kvAction, eventData);
            }
            else
            {
                Log.Info("DelayedAction: start immediate");

                BattleData.ExecuteActions(kvAction, eventData);
            }
        }
    }
}