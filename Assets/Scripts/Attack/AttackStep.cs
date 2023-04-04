using KVLib;


namespace LiaoZhai.Runtime
{
    public class AttackStep
    {
        private DFix64 _activationPoint = DFix64.Zero;                                // 生效时间
        private string _launchedEffect = string.Empty;                              // 自身特效
        private KeyValue _launchedSoundKv = null;                                   // 自身音效
        private string _landedEffect = string.Empty;                                // 命中特效
        private KeyValue _landedSoundKv = null;                                     // 命中音效
        private DFix64 _projectileSpeed = DFix64.Zero;                                // 弹道速度
        private string _projectileEffect = string.Empty;                            // 弹道投掷物
        private string _projectileSpawnPoint = string.Empty;                        // 弹道起始挂点

        public DFix64 ActivationPoint
        {
            get
            {
                return _activationPoint;
            }
        }
        public string LaunchedEffect
        {
            get
            {
                return _launchedEffect;
            }
        }
        public KeyValue LaunchedSoundKv
        {
            get
            {
                return _launchedSoundKv;
            }
        }
        public string LandedEffect
        {
            get
            {
                return _landedEffect;
            }
        }
        public KeyValue LandedSoundKv
        {
            get
            {
                return _landedSoundKv;
            }
        }
        public bool IsProjectile
        {
            get
            {
                return _projectileSpeed > DFix64.Zero;
            }
        }
        public DFix64 ProjectileSpeed
        {
            get
            {
                return _projectileSpeed;
            }
        }
        public string ProjectileEffect
        {
            get
            {
                return _projectileEffect;
            }
        }
        public string ProjectileSpawnPoint
        {
            get
            {
                return _projectileSpawnPoint;
            }
        }

        public AttackStep(KeyValue kv)
        {
            KeyValue kvTemp = kv["AttackAnimationPoint"];

            _activationPoint = kvTemp != null ? BattleData.ParseDFix64(kvTemp.GetString()) : DFix64.Zero;

            kvTemp = kv["AttackLaunchedEffect"];
            _launchedEffect = kvTemp != null ? kvTemp.GetString() : string.Empty;

            kvTemp = kv["AttackLandedEffect"];
            _landedEffect = kvTemp != null ? kvTemp.GetString() : string.Empty;

            _launchedSoundKv = kv["AttackLaunchedSound"];

            _landedSoundKv = kv["AttackLandedSound"];

            kvTemp = kv["ProjectileSpeed"];
            if (kvTemp != null)
            {
                _projectileSpeed = BattleData.ParseDFix64(kvTemp.GetString());

                kvTemp = kv["ProjectileEffect"];
                _projectileEffect = kvTemp != null ? kvTemp.GetString() : string.Empty;

                kvTemp = kv["ProjectileSpawnPoint"];
                _projectileSpawnPoint = kvTemp != null ? kvTemp.GetString() : Constant.Battle.UNIT_MODEL_ATTACH_POINT_DANDAO;
            }
        }
    }
}