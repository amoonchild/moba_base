using KVLib;


namespace LiaoZhai.Runtime
{
    public partial class BattleActionManager
    {
        public class DelayActionTimer : BaseObject
        {
            private DFix64 _delayTime = DFix64.Zero;
            private KeyValue _keyValue = null;
            private EventData _eventData = null;
            private bool _isExecuted = false;

            public bool IsExecuted
            {
                get { return _isExecuted; }
            }


            public DelayActionTimer(DFix64 delayTime, KeyValue keyValue, EventData eventData)
                : base()
            {
                _delayTime = delayTime;
                _keyValue = keyValue;
                _eventData = eventData;
            }

            public override void UpdateLogic(DFix64 frameLength)
            {
                _delayTime -= frameLength;

                if (_delayTime <= DFix64.Zero)
                {
                    _isExecuted = true;
                    _eventData.IsOld = true;
                    if (_eventData.Modifier != null)
                    {
                        _eventData.Modifier.DelayCount--;
                    }

                    BattleData.ExecuteActions(_keyValue, _eventData);
                }
            }
        }
    }
}
