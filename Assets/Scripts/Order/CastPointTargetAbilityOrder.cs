

namespace LiaoZhai.Runtime
{
    public class CastPointTargetAbilityOrder : CastAbilityOrder
    {
        private DFixVector3 _point = DFixVector3.Zero;
        private BattleNode _node = null;

        public override UnitOrderType Type
        {
            get
            {
                return UnitOrderType.UNIT_ORDER_CAST_TARGET;
            }
        }


        public CastPointTargetAbilityOrder(BaseUnit source, Ability ability, DFixVector3 point)
            : base(source, ability)
        {
            _point = point;
            _node = BattleData.FindNodeByPoint(_point);
        }

        public override bool IsExecutable()
        {
            if (!base.IsExecutable())
            {
                return false;
            }

            if (_ability.UnitTargetInfo.NodeRange >= 0)
            {
                if (_node == null)
                {
                    return false;
                }

                if (_source.CurrNode.MaxRange(_node) > _ability.UnitTargetInfo.NodeRange)
                {
                    return false;
                }
            }
            else if (_ability.UnitTargetInfo.RadiusRange >= DFix64.Zero)
            {
                if (DFixVector3.Distance(_source.LogicPosition, _point) > _ability.UnitTargetInfo.RadiusRange)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Begin()
        {
            if (!IsExecutable())
            {
                return false;
            }

            Log.Info("{0} 对 {1} 施放了技能 {2}", _source.LogName, _point.ToString(), _ability.LogName);

            _ability.PayManaCost();
            _ability.StartCooldown(false);

            BattleAbilityCastSuccessEventArgs ne = GameFramework.ReferencePool.Acquire<BattleAbilityCastSuccessEventArgs>();
            ne.Ability = _ability;
            BattleData.FireEvent(ne);

            EventData eventData = CreateEventData();
            eventData.Parms.Add("finalMana", _ability.ManaCost);
            eventData.Parms.Add("realMana", _ability.ManaCost);
            eventData.Parms.Add("excessMana", DFix64.Zero);
            _ability.Caster.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_MANA_REDUCE, eventData);

            if (_isFinished)
            {
                return false;
            }

            if (_ability.IsFaceToTarget)
            {
                _source.LookToPositionY(_point);
                _source.SyncRenderRotation();
            }

            if (_ability.Animation != UnitAnimationType.ACT_NONE)
            {
                if (_ability.AnimationDelayTime == -DFix64.One)
                {
                    _isPlayedAnimation = true;
                    _source.PlayAnimation(_ability.Animation, DFix64.One, true);
                }
                else
                {
                    _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
                }
            }
            else
            {
                _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
            }

            if (_ability.Duration > DFix64.Zero)
            {
                _ability.IsCasting = true;
                _source.IsCasting = true;

                if (_ability.PhaseDuraion > DFix64.Zero)
                {
                    _ability.IsPhasing = true;
                    _source.IsCastingPE = true;

                    _ability.OnAbilityPhaseStart();
                    _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_PHASE_START, CreateEventData());
                }
                else if (_ability.ChannelDuration > DFix64.Zero)
                {
                    _ability.IsChanneling = true;

                    _ability.OnAbilityStart();
                    _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_START, CreateEventData());
                }
                else
                {
                    _ability.OnAbilityStart();
                    _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_START, CreateEventData());
                }

                if (_isFinished)
                {
                    return false;
                }
            }
            else
            {
                _ability.IsCasting = true;
                _source.IsCasting = true;

                _ability.OnAbilityStart();
                _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_START, CreateEventData());

                if (_isFinished)
                {
                    return false;
                }

                _isFinished = true;

                _source.IsCasting = false;
                _ability.IsCasting = false;

                _ability.OnAbilityFinish();
                _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_FINISH, CreateEventData());

                return false;
            }

            return true;
        }

        public override void UpdateLogic(DFix64 frameLength)
        {
            if (_isFinished)
            {
                return;
            }

            _time += frameLength;

            if (_ability.Animation != UnitAnimationType.ACT_NONE)
            {
                if (!_isPlayedAnimation)
                {
                    if (_time >= _ability.AnimationDelayTime)
                    {
                        _isPlayedAnimation = true;
                        _source.PlayAnimation(_ability.Animation, DFix64.One, _time - _ability.AnimationDelayTime, false);
                    }
                }
            }

            if (_ability.IsPhasing)
            {
                if (_time >= _ability.PhaseDuraion)
                {
                    if (_ability.ChannelDuration > DFix64.Zero)
                    {
                        _ability.IsPhasing = false;
                        _source.IsCastingPS = false;

                        _ability.IsChanneling = true;

                        _ability.OnAbilityStart();
                        _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_START, CreateEventData());

                        if (_isFinished)
                        {
                            return;
                        }
                    }
                    else
                    {
                        _ability.IsPhasing = false;
                        _source.IsCastingPS = false;

                        _ability.OnAbilityStart();
                        _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_START, CreateEventData());

                        if (_isFinished)
                        {
                            return;
                        }

                        _source.IsCastingPE = true;
                    }
                }
                else
                {
                    if (_ability.CancelCastNodeRange != -1)
                    {
                        if (_source.CurrNode.MaxRange(_node) > _ability.CancelCastNodeRange)
                        {
                            SelfInterrupt();
                            return;
                        }
                    }
                    else if (_ability.CancelCastRange != -DFix64.One)
                    {
                        if (DFixVector3.Distance(_source.LogicPosition, _point) > _ability.CancelCastRange)
                        {
                            SelfInterrupt();
                            return;
                        }
                    }
                }
            }

            if (_ability.IsChanneling)
            {
                if (_time >= _ability.PhaseDuraion + _ability.ChannelDuration)
                {
                    _ability.IsChanneling = false;
                    _source.IsCastingPE = true;

                    _ability.OnChannelFinish(false);
                    _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_CHANNEL_SUCCESS, CreateEventData());
                    _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_CHANNEL_FINISH, CreateEventData());

                    if (_isFinished)
                    {
                        return;
                    }
                }
                else
                {
                    if (_ability.CancelCastNodeRange != -1)
                    {
                        if (_source.CurrNode.MaxRange(_node) > _ability.CancelCastNodeRange)
                        {
                            SelfInterrupt();
                            return;
                        }
                    }
                    else if (_ability.CancelCastRange != -DFix64.One)
                    {
                        if (DFixVector3.Distance(_source.LogicPosition, _point) > _ability.CancelCastRange)
                        {
                            SelfInterrupt();
                            return;
                        }
                    }
                }
            }

            if (_time >= _ability.Duration)
            {
                _isFinished = true;

                _ability.IsCasting = false;
                _source.IsCasting = false;
                _source.IsCastingPS = false;
                _source.IsCastingPE = false;

                _ability.OnAbilityFinish();
                _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_FINISH, CreateEventData());
            }
        }

        public override void Interrupt()
        {
            if (_isFinished)
            {
                return;
            }

            _isFinished = true;

            if (_ability.IsCasting)
            {
                _ability.IsCasting = false;
                _source.IsCasting = false;
                _source.IsCastingPS = false;
                _source.IsCastingPE = false;

                if (_isPlayedAnimation)
                {
                    _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
                }

                if (_ability.IsPhasing)
                {
                    _ability.IsPhasing = false;
                    _ability.OnAbilityPhaseInterrupted();
                    _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_PHASE_INTERRUPTED, CreateEventData());
                }
                else if (_ability.IsChanneling)
                {
                    _ability.IsChanneling = false;
                    _ability.OnChannelFinish(true);
                    _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_CHANNEL_INTERRUPTED, CreateEventData());
                }

                _ability.OnAbilityFinish();
                _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_FINISH, CreateEventData());
            }
        }

        public override void Quit()
        {
            if (_isFinished)
            {
                return;
            }

            _isFinished = true;

            if (_ability.IsCasting)
            {
                _ability.IsCasting = false;
                _source.IsCasting = false;
                _source.IsCastingPS = false;
                _source.IsCastingPE = false;

                if (_isPlayedAnimation)
                {
                    _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
                }

                if (_ability.IsPhasing)
                {
                    _ability.IsPhasing = false;
                }
                else if (_ability.IsChanneling)
                {
                    _ability.IsChanneling = false;
                    _ability.OnChannelFinish(false);
                    _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_CHANNEL_FINISH, CreateEventData());
                }

                _ability.OnAbilityFinish();
                _source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_FINISH, CreateEventData());
            }
        }

        private void SelfInterrupt()
        {
            if (_isFinished)
            {
                return;
            }

            _isFinished = true;

            if (_ability.IsCasting)
            {
                _ability.IsCasting = false;
                _source.IsCasting = false;
                _source.IsCastingPS = false;
                _source.IsCastingPE = false;

                if (_isPlayedAnimation)
                {
                    _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
                }

                if (_ability.IsPhasing)
                {
                    _ability.IsPhasing = false;
                    _ability.OnAbilityPhaseInterrupted();
                    //_source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_PHASE_INTERRUPTED, CreateEventData());
                }
                else if (_ability.IsChanneling)
                {
                    _ability.IsChanneling = false;
                    _ability.OnChannelFinish(true);
                    //_source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_CHANNEL_INTERRUPTED, CreateEventData());
                }

                _ability.OnAbilityFinish();
                //_source.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_FINISH, CreateEventData());
            }
        }

        private EventData CreateEventData()
        {
            EventData newEvent = BattleData.CreateEventData();
            newEvent.Attacker = _source;
            newEvent.Point = _point;
            newEvent.Node = _node;

            return newEvent;
        }
    }
}
