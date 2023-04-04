

namespace LiaoZhai.Runtime
{
    public class CastNoTargetAbilityOrder : CastAbilityOrder
    {
        public override UnitOrderType Type
        {
            get
            {
                return UnitOrderType.UNIT_ORDER_CAST_NO_TARGET;
            }
        }


        public CastNoTargetAbilityOrder(BaseUnit source, Ability ability)
            : base(source, ability)
        {

        }

        public override bool Begin()
        {
            if (!IsExecutable())
            {
                return false;
            }

            Log.Info("{0} 施放了技能 {1}", _source.LogName, _ability.LogName);

            _ability.PayManaCost();
            _ability.StartCooldown(false);

            BattleAbilityCastSuccessEventArgs ne = GameFramework.ReferencePool.Acquire<BattleAbilityCastSuccessEventArgs>();
            ne.Ability = _ability;
            BattleData.FireEvent(ne);

            EventData newEvent = CreateEventData();
            newEvent.Parms.Add("finalMana", _ability.ManaCost);
            newEvent.Parms.Add("realMana", _ability.ManaCost);
            newEvent.Parms.Add("excessMana", DFix64.Zero);

            _ability.Caster.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_MANA_REDUCE, newEvent);

            if (_isFinished)
            {
                return false;
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

        private EventData CreateEventData()
        {
            EventData newEvent = BattleData.CreateEventData();
            newEvent.Attacker = _source;
            newEvent.Point = _source.LogicPosition;
            newEvent.Node = _source.CurrNode;

            return newEvent;
        }
    }
}