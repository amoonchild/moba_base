using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    // 攻击目标行为
    public class AttackTargetOrder : BaseOrder
    {
        private BaseUnit _target = null;                        // 攻击目标
        private BattleUnitAttribute _bakAtt = null;                   // 攻击开始时的属性
        private ObscuredBool _isCrit = false;                   // 是否暴击
        private Attack _attack = null;                          // 攻击配置
        private DFix64 _interval = DFix64.Zero;                 // 攻击间隔
        private DFix64 _duration = DFix64.Zero;                 // 本次攻击时间
        private DFix64 _animationSpeed = DFix64.Zero;           // 动画速度
        private DFix64 _time = DFix64.Zero;                     // 计时
        private ObscuredInt _stepCount = 0;                     // 已完成的步骤
        private int _phaseEffectId = 0;                         // 前摇特效
        private int _phaseSoundId = 0;                          // 前摇音效

        public override UnitOrderType Type
        {
            get
            {
                return UnitOrderType.UNIT_ORDER_ATTACK_TARGET;
            }
        }
        public BaseUnit Target
        {
            get
            {
                return _target;
            }
        }


        public AttackTargetOrder(BaseUnit source, BaseUnit target)
            : base(source)
        {
            _target = target;
        }

        public override bool IsExecutable()
        {
            if (_source.IsDeadState)
            {
                return false;
            }

            if (_source.IsResticted)
            {
                return false;
            }

            if (_source.IsStunned)
            {
                return false;
            }

            if (_source.IsAttackDisabled)
            {
                return false;
            }

            if (_target == null)
            {
                return false;
            }

            if (_target.IsDeadState)
            {
                return false;
            }

            if (_target.IsUnselectable || _target.IsUnselectableAttack)
            {
                return false;
            }

            if (_target.IsInvincible)
            {
                return false;
            }

            if (_target.IsInvisible)
            {
                return false;
            }

            if (_source.CurrNode.MaxRange(_target.CurrNode) > _source.CurrAtt.AttackNodeRange)
            {
                return false;
            }

            return true;
        }

        public override bool Begin()
        {
            if (!IsExecutable())
            {
                return false;
            }

            _bakAtt = _source.CurrAtt.Clone();

            DFix64 critRange = DFix64.Clamp(_bakAtt.CritOdds / DFix64.Thousand, BattleData.Parm.Parm20, BattleData.Parm.Parm21);
            if (critRange > DFix64.Zero)
            {
                _isCrit = BattleData.SRandom.RangeDFx(DFix64.Zero, DFix64.One) < critRange;
            }
            else
            {
                _isCrit = false;
            }

            _attack = _source.GetRandomAttack(_isCrit);
            if (_attack == null)
            {
                return false;
            }

            if (_attack.BaseAttackInterval <= DFix64.Zero)
            {
                return false;
            }

            if (_bakAtt.AttackSpeed > DFix64.Zero)
            {
                _interval = _attack.BaseAttackInterval * DFix64.Clamp(DFix64.Thousand / _bakAtt.AttackSpeed, BattleData.Parm.Parm16, BattleData.Parm.Parm17);
            }
            else
            {
                _interval = _attack.BaseAttackInterval * BattleData.Parm.Parm17;
            }

            _animationSpeed = _attack.BaseAttackInterval / _interval;
            _duration = _attack.Duration / _animationSpeed;

            _source.IsAttacking = true;
            _source.IsAttackingPS = false;
            _source.IsAttackingPE = false;

            return StartPhase();
        }

        public override void UpdateLogic(DFix64 frameLength)
        {
            if (_isFinished)
            {
                return;
            }

            if (_source.IsAttackingPS)
            {
                if (_target == null || _target.IsDeadState || _target.IsUnselectable || _target.IsUnselectableAttack || _target.IsInvincible || _target.IsInvisible || _source.CurrNode.MaxRange(_target.CurrNode) > _source.CurrAtt.AttackNodeRange)
                {
                    SelfInterrupt();
                    return;
                }
            }

            _time += frameLength;

            if (_target.IsMoving)
            {
                _source.LookToPositionY(_target.LogicPosition);
            }

            if (_stepCount < _attack.Steps.Count)
            {
                for (int i = _stepCount; i < _attack.Steps.Count; i++)
                {
                    AttackStep attackStep = _attack.Steps[i];
                    if (_time >= attackStep.ActivationPoint / _animationSpeed)
                    {
                        _stepCount++;
                        _source.IsAttackingPS = false;

                        if (_stepCount == 1)
                        {
                            _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ATTACK_START, CreateEventData());

                            if (_isFinished)
                            {
                                return;
                            }
                        }

                        if (attackStep.IsProjectile)
                        {
                            CreateAttackProjectileData createData = new CreateAttackProjectileData();
                            createData.Attacker = _source;
                            createData.AttackerAtt = _bakAtt;
                            createData.IsCrit = _isCrit;
                            createData.Target = _target;
                            createData.MoveSpeed = attackStep.ProjectileSpeed;
                            createData.SpawnOriginAttachPoint = attackStep.ProjectileSpawnPoint;
                            createData.HitEffectName = attackStep.LandedEffect;
                            createData.HitSoundKv = attackStep.LandedSoundKv;

                            ProjectileManager.CreateAttackProjectile(attackStep.ProjectileEffect, createData);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(attackStep.LandedEffect))
                            {
                                int particleId = ParticleManager.CreateParticle(attackStep.LandedEffect, false);
                                ParticleManager.SetParticleAttach(particleId, _target, Constant.Battle.UNIT_MODEL_ATTACH_POINT_XIONG, true, false, true, false);
                                if (_target.LogicPosition != _source.LogicPosition)
                                {
                                    ParticleManager.SetParticleControl(particleId, ParticleControlType.LookAt, _target.LogicPosition - _source.LogicPosition, true);
                                }
                            }

                            if (attackStep.LandedSoundKv != null)
                            {
                                BattleData.FireSound(_target, Constant.Battle.UNIT_MODEL_ATTACH_POINT_XIONG, attackStep.LandedSoundKv);
                            }

                            BattleData.ApplyAttackDamage(_source, _bakAtt, _target, _isCrit);

                            if (_isFinished)
                            {
                                return;
                            }
                        }
                    }
                }
            }

            if (_time >= _duration)
            {
                _source.IsAttackingPE = true;
            }

            if (_time >= _interval)
            {
                _isFinished = true;
                _source.IsAttacking = false;
                _source.IsAttackingPE = false;

                _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ATTACK_FINISH, CreateEventData());
            }
        }

        public override void Interrupt()
        {
            if (_isFinished)
            {
                return;
            }

            _isFinished = true;

            if (_source.IsAttacking)
            {
                if (_source.IsAttackingPS)
                {
                    if (_phaseEffectId != 0)
                    {
                        ParticleManager.DestroyParticle(_phaseEffectId);
                    }

                    if (_phaseSoundId != 0)
                    {
                        GameManager.Sound.StopSound(_phaseSoundId);
                    }

                    _source.IsAttackingPS = false;
                    _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
                }
                else if (_source.IsAttackingPE)
                {
                    _source.IsAttackingPE = false;
                    _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
                }
                else
                {

                }

                _source.IsAttacking = false;
                _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ATTACK_FINISH, CreateEventData());
            }
        }

        public override void Quit()
        {
            if (_isFinished)
            {
                return;
            }

            _isFinished = true;

            if (_source.IsAttacking)
            {
                _source.IsAttacking = false;

                if (_source.IsAttackingPS)
                {
                    if (_phaseEffectId != 0)
                    {
                        ParticleManager.DestroyParticle(_phaseEffectId);
                    }

                    if (_phaseSoundId != 0)
                    {
                        GameManager.Sound.StopSound(_phaseSoundId);
                    }

                    _source.IsAttackingPS = false;
                    _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
                }
                else if (_source.IsAttackingPE)
                {
                    _source.IsAttackingPE = false;
                }
                else
                {

                }

                _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ATTACK_FINISH, CreateEventData());
            }
        }

        public override void Clear()
        {
            _target = null;
            _bakAtt = null;
            _isCrit = false;
            _attack = null;
            _interval = DFix64.Zero;
            _duration = DFix64.Zero;
            _animationSpeed = DFix64.Zero;
            _time = DFix64.Zero;
            _stepCount = 0;
            _phaseEffectId = 0;
            _phaseSoundId = 0;

            base.Clear();
        }

        private bool StartPhase()
        {
            _source.LookToPositionY(_target.LogicPosition);
            _source.SyncRenderRotation();

            if (_attack.Steps.Count > 0)
            {
                if (_attack.Steps[0].ActivationPoint > DFix64.Zero)
                {
                    _source.IsAttackingPS = true;
                    _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ATTACK_PHASE_START, CreateEventData());

                    if (_isFinished)
                    {
                        return false;
                    }
                }
            }

            if (_attack.Animation != UnitAnimationType.ACT_NONE)
            {
                _source.PlayAnimation(_attack.Animation, _animationSpeed, true);
            }
            else
            {
                _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
            }

            if (!string.IsNullOrEmpty(_attack.PhaseStartEffect))
            {
                _phaseEffectId = ParticleManager.CreateParticle(_attack.PhaseStartEffect, false);
                ParticleManager.SetParticleControl(_phaseEffectId, ParticleControlType.Position, _source.LogicPosition, true);
                ParticleManager.SetParticleControl(_phaseEffectId, ParticleControlType.Angle, _source.LogicEulerAngles, true);
                ParticleManager.SetParticleControl(_phaseEffectId, ParticleControlType.Speed, new DFixVector3(_animationSpeed, DFix64.Zero, DFix64.Zero), true);
                ParticleManager.SetParticleAttach(_phaseEffectId, _source, string.Empty, true, true, true, true);
            }

            if (_attack.PhaseStartSoundKv != null)
            {
                _phaseSoundId = BattleData.FireSound(_source, Constant.Battle.UNIT_MODEL_ATTACH_POINT_XIONG, _attack.PhaseStartSoundKv);
            }

            if (!_source.IsAttackingPS)
            {
                if (_stepCount < _attack.Steps.Count)
                {
                    for (int i = _stepCount; i < _attack.Steps.Count; i++)
                    {
                        AttackStep attackStep = _attack.Steps[i];
                        if (_time >= attackStep.ActivationPoint / _animationSpeed)
                        {
                            _stepCount++;
                            if (_stepCount == 1)
                            {
                                _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ATTACK_START, CreateEventData());

                                if (_isFinished)
                                {
                                    return false;
                                }
                            }

                            if (attackStep.IsProjectile)
                            {
                                CreateAttackProjectileData createData = new CreateAttackProjectileData();
                                createData.Attacker = _source;
                                createData.AttackerAtt = _bakAtt;
                                createData.IsCrit = _isCrit;
                                createData.Target = _target;
                                createData.MoveSpeed = attackStep.ProjectileSpeed;
                                createData.SpawnOriginAttachPoint = attackStep.ProjectileSpawnPoint;
                                createData.HitEffectName = attackStep.LandedEffect;
                                createData.HitSoundKv = attackStep.LandedSoundKv;

                                ProjectileManager.CreateAttackProjectile(attackStep.ProjectileEffect, createData);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(attackStep.LandedEffect))
                                {
                                    int particleId = ParticleManager.CreateParticle(attackStep.LandedEffect, false);
                                    ParticleManager.SetParticleAttach(particleId, _target, Constant.Battle.UNIT_MODEL_ATTACH_POINT_XIONG, true, false, true, false);
                                    if (_target.LogicPosition != _source.LogicPosition)
                                    {
                                        ParticleManager.SetParticleControl(particleId, ParticleControlType.LookAt, _target.LogicPosition - _source.LogicPosition, true);
                                    }
                                }

                                if (attackStep.LandedSoundKv != null)
                                {
                                    BattleData.FireSound(_target, Constant.Battle.UNIT_MODEL_ATTACH_POINT_XIONG, attackStep.LandedSoundKv);
                                }

                                BattleData.ApplyAttackDamage(_source, _bakAtt, _target, _isCrit);

                                if (_isFinished)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void SelfInterrupt()
        {
            if (_isFinished)
            {
                return;
            }

            _isFinished = true;

            if (_source.IsAttacking)
            {
                if (_source.IsAttackingPS)
                {
                    if (_phaseEffectId != 0)
                    {
                        ParticleManager.DestroyParticle(_phaseEffectId);
                    }

                    if (_phaseSoundId != 0)
                    {
                        GameManager.Sound.StopSound(_phaseSoundId);
                    }

                    _source.IsAttackingPS = false;
                    _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
                }
                else if (_source.IsAttackingPE)
                {
                    _source.IsAttackingPE = false;
                    _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
                }
                else
                {

                }

                _source.IsAttacking = false;
                _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ATTACK_FINISH, CreateEventData());
            }
        }

        private EventData CreateEventData()
        {
            EventData newEvent = BattleData.CreateEventData();
            newEvent.Attacker = _source;
            newEvent.Unit = _target;
            newEvent.Point = _source.LogicPosition;
            newEvent.Node = _source.CurrNode;

            return newEvent;
        }
    }
}