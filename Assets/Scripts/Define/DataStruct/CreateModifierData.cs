using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    // Modifier创建信息
    public class CreateModifierData
    {
        private DFix64 _duration = DFix64.Zero;
        private ObscuredBool _isDurationSpecified = false;

        // 持续时间,未设置的话使用Modifier配置的持续时间
        public DFix64 Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
                _isDurationSpecified = true;
            }
        }
        public ObscuredBool IsDurationSpecified
        {
            get
            {
                return _isDurationSpecified;
            }
        }
    }
}
