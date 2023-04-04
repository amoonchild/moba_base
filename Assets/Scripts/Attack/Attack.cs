using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public class Attack
    {
        private int _index = 0;                                                     // 攻击索引
        private DFix64 _baseInterval = DFix64.Zero;                                   // 基础攻击间隔
        private UnitAnimationType _animation = UnitAnimationType.ACT_NONE;          // 动作
        private DFix64 _duration = DFix64.Zero;                                       // 攻击时长
        private string _phaseStartEffect = string.Empty;                            // 前摇特效
        private KeyValue _phaseStartSoundKv = null;                                 // 前摇开始音效
        private List<AttackStep> _steps = new List<AttackStep>();                   // 步骤

        public int Index
        {
            get
            {
                return _index;
            }
        }
        public UnitAnimationType Animation
        {
            get
            {
                return _animation;
            }
        }
        public DFix64 BaseAttackInterval
        {
            get
            {
                return _baseInterval;
            }
        }
        public DFix64 Duration
        {
            get
            {
                return _duration;
            }
        }
        public string PhaseStartEffect
        {
            get
            {
                return _phaseStartEffect;
            }
        }
        public KeyValue PhaseStartSoundKv
        {
            get
            {
                return _phaseStartSoundKv;
            }
        }
        public List<AttackStep> Steps
        {
            get
            {
                return _steps;
            }
        }


        public Attack(int index, KeyValue kv)
        {
            _index = index;

            KeyValue kvTemp = kv["BaseAttackInterval"];
            if (kvTemp != null)
            {
                _baseInterval = BattleData.ParseDFix64(kvTemp.GetString());
            }

            kvTemp = kv["AttackAnimation"];
            if (kvTemp != null)
            {
                _animation = BattleData.EvaluateEnum<UnitAnimationType>(kvTemp.GetString(), UnitAnimationType.ACT_NONE);
            }

            kvTemp = kv["AttackAnimationDuration"];
            if (kvTemp != null)
            {
                _duration = BattleData.ParseDFix64(kvTemp.GetString());
            }

            kvTemp = kv["AttackPhaseStartEffect"];
            if (kvTemp != null)
            {
                _phaseStartEffect = kvTemp.GetString();
            }

            _phaseStartSoundKv = kv["AttackPhaseStartSound"];

            kvTemp = kv["Step"];
            if (kvTemp != null && kvTemp.HasChildren)
            {
                foreach (var child in kvTemp.Children)
                {
                    AttackStep attackStep = new AttackStep(child);
                    _steps.Add(attackStep);
                }
            }
        }
    }
}