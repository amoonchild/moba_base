using KVLib;
using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    public class Ability : BaseObject
    {
        private ObscuredInt _serialId = 0;                                                                          // 流水号id
        private ObscuredInt _abilityId = 0;                                                                         // 技能id
        private string _name = string.Empty;                                                                        // 名字
        private string _nameInIcon = string.Empty;                                                                  // 名字
        private string _icon = string.Empty;                                                                        // 图标资源路径
        private AbilityBehavior _behavior = AbilityBehavior.ABILITY_BEHAVIOR_HIDDEN;                                // 行为
        private MulTargetInfo _unitTargetInfo = null;                                                               // 目标
        private bool _isShowIcon = false;                                                                           // 在界面上显示
        private bool _isShowCooldown = false;                                                                       // 显示技能冷却
        private bool _isShowCastPrompt = false;                                                                     // 显示技能释放提示
        private DFix64 _manaCost = DFix64.Zero;                                                                     // 法力值消耗
        private bool _isFaceToTarget = true;                                                                        // 是否面对目标
        private int _cancelCastNodeRange = -1;                                                                      // 取消施法距离格子(当开始施法并且目标超过该距离缓冲时,取消施法)
        private DFix64 _cancelCastRange = -DFix64.One;                                                              // 取消施法距离(当开始施法并且目标超过该距离缓冲时,取消施法)
        private DFix64 _duration = -DFix64.One;                                                                     // 施法持续时间
        private DFix64 _phaseDuraion = -DFix64.One;                                                                 // 施法前摇时间
        private DFix64 _channelDuration = -DFix64.One;                                                              // 引导时间,从前摇结束后开始计时
        private UnitAnimationType _animation = UnitAnimationType.ACT_NONE;                                          // 施法动作
        private DFix64 _animationDelayTime = -DFix64.One;                                                           // 施法动作延迟
        private DFix64 _firstCooldown = -DFix64.One;                                                                // 首次上场冷却时间
        private DFix64 _cooldown = -DFix64.One;                                                                     // 冷却时间
        private int _sharedCooldownGroup = 0;                                                                       // 共享冷却组
        private Dictionary<string, DFix64> _specialValues = new Dictionary<string, DFix64>();                       // 自定义常量
        private List<KeyValue> _passiveModifierKvs = new List<KeyValue>();                                          // 被动Modifier
        private Dictionary<int, KeyValue> _activeModifierKvs = new Dictionary<int, KeyValue>();                     // 主动Modifier
        private Dictionary<int, KeyValue> _kvActions = new Dictionary<int, KeyValue>();                             // 事件
        private KeyValue _kvPrecache = null;
        private List<Modifier> _passiveModifiers = new List<Modifier>();

        private BaseUnit _caster = null;                                                                            // 技能持有者
        private int _index = 0;
        private bool _isActivated = false;                                                                          // 是否激活
        private bool _isAutoCastState = false;                                                                      // 是否是自动施法状态
        private bool _isInCooldown = false;                                                                         // 是否正在冷却中
        private bool _isInFirstCooldown = false;                                                                    // 是否正在首次上场冷却中
        private DFix64 _currCooldownTime = DFix64.Zero;                                                             // 当前冷却计时
        private DFix64 _remainingCooldownTime = DFix64.Zero;                                                        // 当前冷却计时
        private DFix64 _nextRemainingCooldownTime = DFix64.Zero;                                                    // 当前冷却计时
        private bool _isCasting = false;                                                                            // 是否正在施法中
        private bool _isPhasing = false;                                                                            // 是否正在前摇中
        private bool _isChanneling = false;                                                                         // 是否正在引导中
        private BaseUnit _castUnitTarget = null;                                                                    // 施法目标单位
        private DFixVector3 _castPointTarget = DFixVector3.Zero;                                                    // 施法目标位置
        private BattleNode _castNodeTarget = null;                                                                  // 施法目标位置
        private LinkedList<BaseUnit> _targets = new LinkedList<BaseUnit>();

        public int SerialId
        {
            get
            {
                return _serialId;
            }
        }
        public int AbilityId
        {
            get
            {
                return _abilityId;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public string NameInIcon
        {
            get
            {
                return _nameInIcon;
            }
        }
        public string LogName
        {
            get
            {
                return "<color=#00FFFFFF>" + _name + "</color>";
            }
        }
        public string Icon
        {
            get
            {
                return _icon;
            }
        }
        public AbilityBehavior Behavior
        {
            get
            {
                return _behavior;
            }
        }
        public bool IsHidden
        {
            get
            {
                return (_behavior & AbilityBehavior.ABILITY_BEHAVIOR_HIDDEN) != 0;
            }
        }
        public bool IsPassive
        {
            get
            {
                return (_behavior & AbilityBehavior.ABILITY_BEHAVIOR_PASSIVE) != 0;
            }
        }
        public bool IsNoTarget
        {
            get
            {
                return (_behavior & AbilityBehavior.ABILITY_BEHAVIOR_NO_TARGET) != 0;
            }
        }
        public bool IsUnitTarget
        {
            get
            {
                return (_behavior & AbilityBehavior.ABILITY_BEHAVIOR_UNIT_TARGET) != 0;
            }
        }
        public bool IsPointTarget
        {
            get
            {
                return (_behavior & AbilityBehavior.ABILITY_BEHAVIOR_POINT) != 0;
            }
        }
        public bool IsImmediate
        {
            get
            {
                return (_behavior & AbilityBehavior.ABILITY_BEHAVIOR_IMMEDIATE) != 0;
            }
        }
        public bool IsAutoCast
        {
            get
            {
                return (_behavior & AbilityBehavior.ABILITY_BEHAVIOR_AUTOCAST) != 0;
            }
        }
        public bool IsIgnoreBackswing
        {
            get
            {
                return (_behavior & AbilityBehavior.ABILITY_BEHAVIOR_IGNORE_BACKSWING) != 0;
            }
        }
        public MulTargetInfo UnitTargetInfo
        {
            get
            {
                return _unitTargetInfo;
            }
        }
        public bool IsFaceToTarget
        {
            get
            {
                return _isFaceToTarget;
            }
        }
        public bool IsShowIcon
        {
            get
            {
                return _isShowIcon;
            }
        }
        public bool IsShowCooldown
        {
            get
            {
                return _isShowCooldown;
            }
        }
        public bool IsShowCastPrompt
        {
            get
            {
                return _isShowCastPrompt;
            }
        }
        public DFix64 ManaCost
        {
            get
            {
                return _manaCost;
            }
        }
        public int CancelCastNodeRange
        {
            get
            {
                return _cancelCastNodeRange;
            }
        }
        public DFix64 CancelCastRange
        {
            get
            {
                return _cancelCastRange;
            }
        }
        public DFix64 Duration
        {
            get
            {
                return _duration;
            }
        }
        public DFix64 PhaseDuraion
        {
            get
            {
                return _phaseDuraion;
            }
        }
        public DFix64 ChannelDuration
        {
            get
            {
                return _channelDuration;
            }
        }
        public UnitAnimationType Animation
        {
            get
            {
                return _animation;
            }
        }
        public DFix64 AnimationDelayTime
        {
            get
            {
                return _animationDelayTime;
            }
        }
        public DFix64 FirstCooldown
        {
            get
            {
                return _firstCooldown;
            }
        }
        public DFix64 Cooldown
        {
            get
            {
                return _cooldown;
            }
        }
        public DFix64 CurrCooldown
        {
            get
            {
                return _currCooldownTime;
            }
        }
        public int SharedCooldownGroup
        {
            get
            {
                return _sharedCooldownGroup;
            }
        }
        public Dictionary<string, DFix64> SpecialValues
        {
            get
            {
                return _specialValues;
            }
        }

        public bool IsPrecached
        {
            get;
            private set;
        }

        public BaseUnit Caster
        {
            get
            {
                return _caster;
            }
            private set
            {
                _caster = value;
            }
        }
        public int Index
        {
            get
            {
                return _index;
            }
        }
        public bool IsActivated
        {
            get
            {
                return _isActivated;
            }
            private set
            {
                _isActivated = value;
            }
        }
        public bool IsInCooldown
        {
            get
            {
                return _isInCooldown;
            }
        }
        public bool IsInFirstCooldown
        {
            get
            {
                return _isInFirstCooldown;
            }
        }
        public DFix64 RemainingCooldown
        {
            get
            {
                return _remainingCooldownTime;
            }
        }
        public bool IsAutoCastState
        {
            get
            {
                return _isAutoCastState;
            }
            set
            {
                _isAutoCastState = value;
            }
        }
        public bool IsCasting
        {
            get
            {
                return _isCasting;
            }
            set
            {
                _isCasting = value;
            }
        }
        public bool IsPhasing
        {
            get
            {
                return _isPhasing;
            }
            set
            {
                _isPhasing = value;
            }
        }
        public bool IsChanneling
        {
            get
            {
                return _isChanneling;
            }
            set
            {
                _isChanneling = value;
            }
        }


        public Ability(int serialId, KeyValue kv, BaseUnit caster, int index)
        {
            _serialId = serialId;
            _caster = caster;
            _index = index;

            if (kv != null)
            {
                _unitTargetInfo = new MulTargetInfo();

                foreach (var child in kv.Children)
                {
                    switch (child.Key)
                    {
                        case "Precache": { _kvPrecache = child; } break;
                        case "Id": { _abilityId = BattleData.ParseInt(child.GetString()); } break;
                        case "Name": { _name = child.GetString(); } break;
                        case "Introduction": { _nameInIcon = child.GetString(); } break;
                        case "AbilityIconName": { _icon = child.GetString(); } break;
                        case "AbilityBehavior": { BattleData.TryEvaluateEnums(child.GetString(), out _behavior); } break;
                        case "AbilityUnitTargetTeams": { BattleData.TryEvaluateEnums(child.GetString(), out _unitTargetInfo.UnitTargetTeams); } break;
                        case "AbilityUnitTargetTypes": { BattleData.TryEvaluateEnums(child.GetString(), out _unitTargetInfo.UnitTargetTypes); } break;
                        case "AbilityUnitTargetFlags": { BattleData.TryEvaluateEnums(child.GetString(), out _unitTargetInfo.UnitTargetFlags); } break;
                        case "AbilityUnitTargetExcludedFlags": { BattleData.TryEvaluateEnums(child.GetString(), out _unitTargetInfo.ExcludedUnitTargetFlags); } break;
                        case "AbilityUnitTargetSort": { BattleData.TryEvaluateEnum(child.GetString(), out _unitTargetInfo.UnitTargetSort); } break;
                        case "AbilityUnitTargetTraits": { _unitTargetInfo.UnitTargetTraits = BattleData.EvaluateTrait(child.GetString()); } break;
                        case "AbilityUnitTargetExcludedTraits": { _unitTargetInfo.ExcludedUnitTargetTraits = BattleData.EvaluateTrait(child.GetString()); } break;
                        case "AbilityUnitTargetModifierGroups": { _unitTargetInfo.UnitTargetModifierGroups = BattleData.EvaluateModifierGroup(child.GetString()); } break;
                        case "AbilityUnitTargetExcludedModifierGroups": { _unitTargetInfo.ExcludedUnitTargetModifierGroups = BattleData.EvaluateModifierGroup(child.GetString()); } break;
                        case "AbilityUnitTargetMaxNumber": { _unitTargetInfo.MaxNumber = BattleData.ParseInt(child.GetString()); } break;
                        case "AbilityUnitTargetRandom": { _unitTargetInfo.IsRandom = BattleData.ParseBool01(child.GetString()); } break;
                        case "ShowIcon": { _isShowIcon = BattleData.ParseBool01(child.GetString()); } break;
                        case "ShowCooldown": { _isShowCooldown = BattleData.ParseBool01(child.GetString()); } break;
                        case "ShowPrompt": { _isShowCastPrompt = BattleData.ParseBool01(child.GetString()); } break;
                        case "AbilityCostMana": { _manaCost = BattleData.ParseDFix64(child.GetString()); } break;
                        case "AbilityCastNodeRange": { _unitTargetInfo.NodeRange = BattleData.ParseInt(child.GetString()); } break;
                        case "AbilityCastRange": { _unitTargetInfo.RadiusRange = BattleData.ParseDFix64(child.GetString()); } break;
                        case "AbilityCastNodeRangeBuff": { _cancelCastNodeRange = BattleData.ParseInt(child.GetString()); } break;
                        case "AbilityCastRangeBuff": { _cancelCastRange = BattleData.ParseDFix64(child.GetString()); } break;
                        case "AbilityCastAnimation": { BattleData.TryEvaluateEnum(child.GetString(), out _animation); } break;
                        case "AbilityCastAnimationPoint": { _animationDelayTime = BattleData.ParseDFix64(child.GetString()); } break;
                        case "AbilityDuration": { _duration = BattleData.ParseDFix64(child.GetString()); } break;
                        case "AbilityCastPoint": { _phaseDuraion = BattleData.ParseDFix64(child.GetString()); } break;
                        case "AbilityChannelTime": { _channelDuration = BattleData.ParseDFix64(child.GetString()); } break;
                        case "AbilityFirstCooldown": { _firstCooldown = BattleData.ParseDFix64(child.GetString()); } break;
                        case "AbilityCooldown": { _cooldown = BattleData.ParseDFix64(child.GetString()); } break;
                        case "AbilitySharedCooldown": { _sharedCooldownGroup = BattleData.ParseInt(child.GetString()); } break;
                        case "AbilitySpecial":
                            {
                                if (child.HasChildren)
                                {
                                    foreach (var child2 in child.Children)
                                    {
                                        if (child2.HasChildren)
                                        {
                                            foreach (var child3 in child2.Children)
                                            {
                                                if (_specialValues.ContainsKey(child3.Key))
                                                {
                                                    Log.Error("技能 SerialId:{0} AbilityId:{1} 拥有相同名字的参数 {2}", _serialId, _abilityId, child3.Key);
                                                    continue;
                                                }

                                                DFix64 specialValue = BattleData.ParseDFix64(child3.GetString());
                                                //specialValue = DFix64.Floor(specialValue);
                                                _specialValues.Add(child3.Key, specialValue);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case "Modifiers":
                            {
                                if (child.HasChildren)
                                {
                                    foreach (var child2 in child.Children)
                                    {
                                        KeyValue kvModifierSerialId = child2["SerialId"];
                                        if (kvModifierSerialId == null)
                                        {
                                            Log.Error("技能 SerialId:{0} AbilityId:{1} Modifier没有Id {2}", _serialId, _abilityId, child2.Key);
                                            continue;
                                        }

                                        KeyValue kvPassive = child2["Passive"];
                                        if (kvPassive != null && BattleData.ParseBool01(kvPassive.GetString()))
                                        {
                                            _passiveModifierKvs.Add(child2);
                                        }
                                        else
                                        {
                                            _activeModifierKvs.Add(BattleData.ParseInt(kvModifierSerialId.GetString()), child2);
                                        }
                                    }
                                }
                            }
                            break;
                        case "OnOwnerSpawned": { _kvActions.Add((int)AbilityEventType.ABILITY_EVENT_ON_SPAWN, child); } break;
                        case "OnOwnerDied": { _kvActions.Add((int)AbilityEventType.ABILITY_EVENT_ON_DEATH, child); } break;
                        case "OnAbilityPhaseStart": { _kvActions.Add((int)AbilityEventType.ABILITY_EVENT_ON_ABILITY_PHASE_START, child); } break;
                        case "OnAbilityPhaseInterrupted": { _kvActions.Add((int)AbilityEventType.ABILITY_EVENT_ON_ABILITY_PHASE_INTERRUPTED, child); } break;
                        case "OnAbilityStart": { _kvActions.Add((int)AbilityEventType.ABILITY_EVENT_ON_ABILITY_START, child); } break;
                        case "OnChannelSucceeded": { _kvActions.Add((int)AbilityEventType.ABILITY_EVENT_ON_ABILITY_CHANNEL_SUCCESS, child); } break;
                        case "OnChannelInterrupted": { _kvActions.Add((int)AbilityEventType.ABILITY_EVENT_ON_ABILITY_CHANNEL_INTERRUPTED, child); } break;
                        case "OnChannelFinish": { _kvActions.Add((int)AbilityEventType.ABILITY_EVENT_ON_ABILITY_CHANNEL_FINISH, child); } break;
                        case "OnAbilityFinish": { _kvActions.Add((int)AbilityEventType.ABILITY_EVENT_ON_ABILITY_FINISH, child); } break;
                    }
                }

                InitDefault();
            }
        }

        private void InitDefault()
        {
            if (_unitTargetInfo != null)
            {
                // 技能自带排除标记
                _unitTargetInfo.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE;
                _unitTargetInfo.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE_ABILITY;
                _unitTargetInfo.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_INVINCIBLE;
                _unitTargetInfo.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_INVISIBLE;
            }
        }

        public override void Release()
        {

        }

        public void Precache()
        {
            if (_kvPrecache == null || !_kvPrecache.HasChildren)
            {
                IsPrecached = true;
                return;
            }

            foreach (var child in _kvPrecache.Children)
            {

            }

            IsPrecached = true;
        }

        public void Enable()
        {
            if (!IsActivated)
            {
                IsActivated = true;

                Log.Info("{0} 启用技能 {1}", _caster.LogName, LogName);

                for (int i = 0; i < _passiveModifierKvs.Count; i++)
                {
                    Modifier modifier = ModifierManager.ApplyModifier(_passiveModifierKvs[i], _caster, this, _caster, null);
                    if (modifier != null)
                    {
                        Modifier newModifier = _caster.AddModifier(modifier, false);
                        if (newModifier != null)
                        {
                            _passiveModifiers.Add(newModifier);
                        }
                    }
                }
            }
        }

        public void Disable()
        {
            if (IsActivated)
            {
                IsActivated = false;

                Log.Info("{0} 禁用技能 {1}", _caster.LogName, LogName);

                for (int i = 0; i < _passiveModifiers.Count; i++)
                {
                    _caster.RemoveModifier(_passiveModifiers[i]);
                }
            }
        }

        public override void UpdateLogic(DFix64 frameLength)
        {
            if (IsInCooldown)
            {
                _remainingCooldownTime = _remainingCooldownTime - frameLength;
                if (_remainingCooldownTime <= DFix64.Zero)
                {
                    EndCooldown();
                }
            }
        }

        public bool AutoCast()
        {
            if (!IsActivated || !IsAutoCast || !IsAutoCastState)
            {
                return false;
            }

            if (Caster.IsResticted)
            {
                return false;
            }

            if (!IsPassive && Caster.IsActivatesAbilityDisabled)
            {
                return false;
            }
            else if (IsPassive && Caster.IsPassivesAbilityDisabled)
            {
                return false;
            }

            if (IsIgnoreBackswing)
            {
                if (Caster.IsMoving)
                {
                    return false;
                }
                else if (Caster.IsAttacking && !Caster.IsAttackingPE)
                {
                    return false;
                }
                else if (Caster.IsCasting && !Caster.IsCastingPE)
                {
                    return false;
                }
            }
            else
            {
                if (Caster.IsMoving || Caster.IsAttacking || Caster.IsCasting)
                {
                    return false;
                }
            }

            return CastAbility();
        }

        public bool IsFullyCastable()
        {
            if (!IsActivated || IsPassive || IsInCooldown || (_manaCost > DFix64.Zero && _caster.CurrAtt.Mana < _manaCost))
            {
                return false;
            }

            return true;
        }

        public void StartFirstCooldown()
        {
            if (_firstCooldown > DFix64.Zero)
            {
                _isInCooldown = true;
                _isInFirstCooldown = true;
                _currCooldownTime = PrepareNext(_firstCooldown);
                _remainingCooldownTime = _currCooldownTime;

                BattleAbilityStartCooldownEventArgs ne = GameFramework.ReferencePool.Acquire<BattleAbilityStartCooldownEventArgs>();
                ne.Ability = this;
                ne.IsFirstCooldown = true;
                BattleData.FireEvent(ne);
            }
        }

        public void StartCooldown(bool isStartByGroup = false)
        {
            if (_cooldown > DFix64.Zero)
            {
                _isInCooldown = true;
                _isInFirstCooldown = false;
                _currCooldownTime = PrepareNext(_cooldown);
                if (_nextRemainingCooldownTime > DFix64.Zero)
                {
                    if (_nextRemainingCooldownTime > _remainingCooldownTime)
                    {
                        _remainingCooldownTime = _nextRemainingCooldownTime;
                    }
                    else
                    {
                        _remainingCooldownTime = _nextRemainingCooldownTime;
                    }

                    _nextRemainingCooldownTime = DFix64.Zero;
                }
                else
                {
                    _remainingCooldownTime = _currCooldownTime;
                }

                BattleAbilityStartCooldownEventArgs ne = GameFramework.ReferencePool.Acquire<BattleAbilityStartCooldownEventArgs>();
                ne.Ability = this;
                ne.IsFirstCooldown = false;
                BattleData.FireEvent(ne);

                if (!isStartByGroup)
                {
                    if (_sharedCooldownGroup != 0)
                    {
                        List<Ability> groupAbilities = _caster.FindAbilities(x => x != this && x.SharedCooldownGroup != 0 && x.SharedCooldownGroup == _sharedCooldownGroup);
                        if (groupAbilities.Count > 0)
                        {
                            for (int i = 0; i < groupAbilities.Count; i++)
                            {
                                groupAbilities[i].StartCooldown(true);
                            }
                        }
                    }
                }
            }
        }

        public void EndCooldown()
        {
            if (_isInCooldown)
            {
                BattleAbilityEndCooldownEventArgs ne = GameFramework.ReferencePool.Acquire<BattleAbilityEndCooldownEventArgs>();
                ne.Ability = this;
                ne.IsFirstCooldown = _isInFirstCooldown;

                _isInCooldown = false;
                _isInFirstCooldown = false;
                _remainingCooldownTime = DFix64.Zero;

                BattleData.FireEvent(ne);
            }
        }

        public void AddCooldown(DFix64 timeBouns)
        {
            if (_isInCooldown)
            {
                _remainingCooldownTime = _remainingCooldownTime + timeBouns;
                if (_remainingCooldownTime <= DFix64.Zero)
                {
                    EndCooldown();
                }
            }
            else
            {
                _nextRemainingCooldownTime = timeBouns;
                if (_nextRemainingCooldownTime > DFix64.Zero)
                {
                    StartCooldown(false);
                }
            }
        }

        public bool CastAbility()
        {
            if (!IsFullyCastable())
            {
                return false;
            }

            if (Caster.IsCastAbilityWaiting(this))
            {
                return false;
            }

            if (IsNoTarget)
            {
                _caster.CastNoTargetAbility(this, false);
            }
            else if (IsUnitTarget)
            {
                _castUnitTarget = GetCastTarget();
                if (_castUnitTarget == null)
                {
                    return false;
                }

                _castPointTarget = _castUnitTarget.LogicPosition;
                _castNodeTarget = BattleData.FindNodeByPoint(_castPointTarget);
                _caster.CastUnitTargetAbility(this, _castUnitTarget, false);
            }
            else if (IsPointTarget)
            {
                _castUnitTarget = GetCastTarget();
                if (_castUnitTarget == null)
                {
                    return false;
                }

                _castPointTarget = _castUnitTarget.LogicPosition;
                _castNodeTarget = BattleData.FindNodeByPoint(_castPointTarget);
                _caster.CastPointTargetAbility(this, _castPointTarget, false);
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool PayManaCost()
        {
            if (_caster.CurrAtt.Mana < _manaCost)
            {
                return false;
            }

            _caster.CurrAtt.Mana -= _manaCost;
            return true;
        }

        public Modifier CreateModifier(int modifierSerialId)
        {
            if (_activeModifierKvs.ContainsKey(modifierSerialId))
            {
                return ModifierManager.CreateModifier(_activeModifierKvs[modifierSerialId]);
            }

            return null;
        }

        public Modifier ApplyModifier(BaseUnit caster, BaseUnit target, int modifierSerialId, CreateModifierData createData)
        {
            if (_activeModifierKvs.ContainsKey(modifierSerialId))
            {
                return ModifierManager.ApplyModifier(_activeModifierKvs[modifierSerialId], caster, this, target, createData);
            }

            return null;
        }

        public virtual void OnOwnerSpawned()
        {
            EventData eventData = BattleData.CreateEventData();
            eventData.Unit = _caster;
            eventData.Point = _caster.LogicPosition;
            eventData.Node = _caster.CurrNode;

            HandleEvent(AbilityEventType.ABILITY_EVENT_ON_SPAWN, eventData);
        }
        public virtual void OnOwnerDied()
        {
            EventData eventData = BattleData.CreateEventData();
            eventData.Unit = _caster;
            eventData.Point = _caster.LogicPosition;
            eventData.Node = _caster.CurrNode;

            HandleEvent(AbilityEventType.ABILITY_EVENT_ON_DEATH, eventData);
        }
        public virtual void OnAbilityPhaseStart()
        {
            EventData eventData = BattleData.CreateEventData();

            eventData.Attacker = _caster;
            if (IsUnitTarget)
            {
                eventData.Target = _castUnitTarget;
                eventData.Unit = _castUnitTarget;
                eventData.Point = _castUnitTarget.LogicPosition;
                eventData.Node = _castUnitTarget.CurrNode;
            }
            else if (IsPointTarget)
            {
                eventData.Point = _castPointTarget;
                eventData.Node = _castNodeTarget;
            }
            else
            {

            }

            HandleEvent(AbilityEventType.ABILITY_EVENT_ON_ABILITY_PHASE_START, eventData);
        }
        public virtual void OnAbilityPhaseInterrupted()
        {
            EventData eventData = BattleData.CreateEventData();

            eventData.Attacker = _caster;
            if (IsUnitTarget)
            {
                eventData.Target = _castUnitTarget;
                eventData.Unit = _castUnitTarget;
                eventData.Point = _castUnitTarget.LogicPosition;
                eventData.Node = _castUnitTarget.CurrNode;
            }
            else if (IsPointTarget)
            {
                eventData.Point = _castPointTarget;
                eventData.Node = _castNodeTarget;
            }
            else
            {

            }

            HandleEvent(AbilityEventType.ABILITY_EVENT_ON_ABILITY_PHASE_INTERRUPTED, eventData);
        }
        public virtual void OnAbilityStart()
        {
            EventData eventData = BattleData.CreateEventData();

            eventData.Attacker = _caster;
            if (IsUnitTarget)
            {
                eventData.Target = _castUnitTarget;
                eventData.Unit = _castUnitTarget;
                eventData.Point = _castUnitTarget.LogicPosition;
                eventData.Node = _castUnitTarget.CurrNode;
            }
            else if (IsPointTarget)
            {
                eventData.Point = _castPointTarget;
                eventData.Node = _castNodeTarget;
            }
            else
            {

            }

            HandleEvent(AbilityEventType.ABILITY_EVENT_ON_ABILITY_START, eventData);
        }
        public virtual void OnChannelFinish(bool interrupted)
        {
            EventData eventData = BattleData.CreateEventData();

            eventData.Attacker = _caster;
            if (IsUnitTarget)
            {
                eventData.Target = _castUnitTarget;
                eventData.Unit = _castUnitTarget;
                eventData.Point = _castUnitTarget.LogicPosition;
                eventData.Node = _castUnitTarget.CurrNode;
            }
            else if (IsPointTarget)
            {
                eventData.Point = _castPointTarget;
                eventData.Node = _castNodeTarget;
            }
            else
            {

            }

            if (interrupted)
            {
                HandleEvent(AbilityEventType.ABILITY_EVENT_ON_ABILITY_CHANNEL_INTERRUPTED, eventData);
            }
            else
            {
                HandleEvent(AbilityEventType.ABILITY_EVENT_ON_ABILITY_CHANNEL_SUCCESS, eventData);
            }
        }
        public virtual void OnAbilityFinish()
        {
            EventData eventData = BattleData.CreateEventData();
            eventData.Attacker = _caster;
            if (IsUnitTarget)
            {
                eventData.Target = _castUnitTarget;
                eventData.Unit = _castUnitTarget;
                eventData.Point = _castUnitTarget.LogicPosition;
                eventData.Node = _castUnitTarget.CurrNode;
            }
            else if (IsPointTarget)
            {
                eventData.Point = _castPointTarget;
                eventData.Node = _castNodeTarget;
            }
            else
            {

            }

            HandleEvent(AbilityEventType.ABILITY_EVENT_ON_ABILITY_FINISH, eventData);
        }

        private void HandleEvent(AbilityEventType abilityEventType, EventData eventData)
        {
            if (eventData == null)
            {
                Log.Error("技能 {0} 处理事件失败,eventData为空", LogName);
                return;
            }

            if (_kvActions.ContainsKey((int)abilityEventType))
            {
                eventData.Caster = _caster;
                eventData.Ability = this;

                BattleData.ExecuteActions(_kvActions[(int)abilityEventType], eventData);
            }
        }

        private DFix64 PrepareNext(DFix64 baseCooldown)
        {
            DFix64 ret = baseCooldown * DFix64.Clamp(_caster.CurrAtt.AbilityCDBonus / DFix64.Thousand, BattleData.Parm.Parm45, BattleData.Parm.Parm46);
            ret = DFix64.Max(ret, DFix64.Zero);
            return ret;
        }

        private BaseUnit GetCastTarget()
        {
            BattleData.FindTargets(Caster, Caster, _unitTargetInfo, _targets);

            if (_targets.Count > 0)
            {
                return _targets.First.Value;
            }

            return null;
        }
    }
}