using KVLib;
using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    // Modifier
    public class Modifier : BaseObject
    {
        // 参数
        protected KeyValue _keyValue = null;                                                                                                // 配置文件
        protected int _serialId = 0;                                                                                                        // 序列号
        protected string _name = string.Empty;                                                                                              // 名字
        protected KeyValue _kvPrecache = null;                                                                                              // 预加载配置
        protected bool _isPassive = false;                                                                                                  // 是否为被动,被动Modifier会在技能激活时自动添加给单位,技能禁用时删除,不可被删除或驱散
        protected bool _isRemoveOnDeath = true;                                                                                             // 死亡时删除
        protected bool _isBuff = false;                                                                                                     // 是否为Buff
        protected bool _isDebuff = false;                                                                                                   // 是否为Debuff
        protected bool _isNebuff = false;                                                                                                   // 是否为中立Buff
        protected bool _isHidden = false;                                                                                                   // 是否隐藏,不会在UI上显示所有相关内容
        protected bool _isShowPrompt = false;                                                                                               // 是否在获得时弹出提示
        protected int _priority = -1;                                                                                                       // 优先级,优先级越高更新顺序越靠前
        protected bool _isPurgable = false;                                                                                                 // 是否可被驱散
        protected int _purgableLevel = -1;                                                                                                  // 驱散需求等级
        protected UnitAnimationType _overrideAnimation = UnitAnimationType.ACT_NONE;                                                        // 覆盖单位动作
        protected int _group = 0;                                                                                                           // 组
        protected ModifierGroupStackType _groupStackType = ModifierGroupStackType.MODIFIER_GROUP_STACK_IGNORE;                              // 同组叠加类型
        protected int _groupStackCount = -1;                                                                                                // 同组叠加上限
        protected int _auraModifierSerialId = 0;                                                                                            // 光环Modifier序列号
        private MulTargetInfo _auraUnitTargetInfo = null;                                                                                   // 光环目标
        protected bool _isAuraApplyToCaster = true;                                                                                         // 光环是否添加施法者
        protected Dictionary<int, KeyValue> _propertyKvs = null;                                                                            // Modifier属性配置
        protected Dictionary<int, ModifierStateValue> _states = null;                                                                       // Modifier状态
        protected int[] _immuneModifierGroups = null;                                                                                       // 免疫Modifier组
        protected Dictionary<int, KeyValue> _kvActions = null;                                                                              // 事件

        protected bool _isRecycle = false;                                                                                                  // 是否已回收
        protected BaseUnit _caster = null;                                                                                                  // 施法者
        protected Ability _ability = null;                                                                                                  // 施法者技能
        protected BaseUnit _target = null;                                                                                                  // 目标
        protected DFix64 _duration = -DFix64.One;                                                                                           // 持续时间,-1为永久
        protected DFix64 _thinkInterval = -DFix64.One;                                                                                      // 定时器间隔
        protected bool _isNeedUpdateTime = false;                                                                                           // 是否需要计时
        protected bool _isNeedUpdateThink = false;                                                                                          // 是否需要更新定时器
        protected DFix64 _time = DFix64.Zero;                                                                                               // 计时
        protected DFix64 _nextThinkTime = DFix64.Zero;                                                                                      // 下一次定时器时间
        protected Dictionary<int, DFix64> _properties = null;                                                                               // Modifier属性
        protected bool _isRemoved = false;                                                                                                  // 是否已失效
        protected Dictionary<string, int> _attachedParticles = null;                                                                        // 绑定特效id
        protected Dictionary<string, int> _attachedSoundIds = null;                                                                         // 绑定音效id
        protected Modifier _auraModifier = null;
        protected LinkedList<BaseUnit> _auraUnitTargets = null;                                                                             // 光环目标
        protected Dictionary<BaseUnit, Modifier> _auraUnitTargetModifiers = null;                                                           // 光环目标Modifier
        protected Dictionary<string, DFix64> _values = null;                                                                                // 自定义参数列表
        protected int _delayCount = 0;
        protected bool _isAura = false;

        public KeyValue Kv
        {
            get
            {
                return _keyValue;
            }
        }
        public int SerialId
        {
            get
            {
                return _serialId;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public string LogName
        {
            get
            {
                return "<color=#FF8000FF>" + _name + "</color>";
            }
        }
        public bool IsPassive
        {
            get
            {
                return _isPassive;
            }
        }
        public bool IsRemoveOnDeath
        {
            get
            {
                return _isRemoveOnDeath;
            }
        }
        public bool IsHidden
        {
            get
            {
                return _isHidden;
            }
        }
        public bool IsBuff
        {
            get
            {
                return _isBuff;
            }
        }
        public bool IsDebuff
        {
            get
            {
                return _isDebuff;
            }
        }
        public bool IsNebuff
        {
            get
            {
                return _isNebuff;
            }
        }
        public bool IsShowPrompt
        {
            get
            {
                return _isShowPrompt;
            }
        }
        public int Priority
        {
            get
            {
                return _priority;
            }
        }
        public bool IsPurgable
        {
            get
            {
                return _isPurgable;
            }
        }
        public int PurgableLevel
        {
            get
            {
                return _purgableLevel;
            }
        }
        public UnitAnimationType OverrideAnimation
        {
            get
            {
                return _overrideAnimation;
            }
        }
        public int Group
        {
            get
            {
                return _group;
            }
        }
        public ModifierGroupStackType GroupStackType
        {
            get
            {
                return _groupStackType;
            }
        }
        public int GroupStackCount
        {
            get
            {
                return _groupStackCount;
            }
        }
        public DFix64 Duration
        {
            get
            {
                return _duration;
            }
        }
        public DFix64 ThinkInterval
        {
            get
            {
                return _thinkInterval;
            }
        }
        public int AuraModifierSerialId
        {
            get
            {
                return _auraModifierSerialId;
            }
        }
        public MulTargetInfo AuraUnitTargetInfo
        {
            get
            {
                return _auraUnitTargetInfo;
            }
        }
        public bool IsAuraApplyToCaster
        {
            get
            {
                return _isAuraApplyToCaster;
            }
        }
        public int[] ImmuneModifierGroups
        {
            get
            {
                return _immuneModifierGroups;
            }
        }
        public BaseUnit Caster
        {
            get
            {
                return _caster;
            }
        }
        public Ability Ability
        {
            get
            {
                return _ability;
            }
        }
        public BaseUnit Target
        {
            get
            {
                return _target;
            }
        }
        public bool IsRemoved
        {
            get
            {
                return _isRemoved;
            }
        }
        public DFix64 Time
        {
            get
            {
                return _time;
            }
        }
        public DFix64 RemainingTime
        {
            get
            {
                return _duration == DFix64.One ? DFix64.Zero : DFix64.Max(_duration - _time, DFix64.Zero);
            }
        }
        public bool IsPrecached
        {
            get;
            protected set;
        }
        public Dictionary<int, DFix64> Properties
        {
            get
            {
                return _properties;
            }
        }
        public Dictionary<int, ModifierStateValue> ModifierStates
        {
            get
            {
                return _states;
            }
        }
        public int DelayCount
        {
            get
            {
                return _delayCount;
            }
            set
            {
                _delayCount = value;
            }
        }
        public bool IsAura
        {
            get
            {
                return _isAura;
            }
            set
            {
                _isAura = value;
            }
        }


        public DFix64 this[string key]
        {
            get
            {
                if (_values == null)
                {
                    return DFix64.Zero;
                }

                if (!_values.ContainsKey(key))
                {
                    return DFix64.Zero;
                }

                return _values[key];
            }
            set
            {
                if (_values == null)
                {
                    _values = new Dictionary<string, DFix64>();
                    _values.Add(key, value);
                }
                else if (_values.ContainsKey(key))
                {
                    _values[key] = value;
                }
                else
                {
                    _values.Add(key, value);
                }
            }
        }


        public Modifier(KeyValue keyValue)
        {
            _keyValue = keyValue;

            InitParm();
        }

        private void InitParm()
        {
            if (_keyValue == null)
            {
                return;
            }

            _kvActions = new Dictionary<int, KeyValue>();
            _auraUnitTargetInfo = new MulTargetInfo();

            foreach (var child in _keyValue.Children)
            {
                switch (child.Key)
                {
                    case "SerialId": { _serialId = BattleData.ParseInt(child.GetString()); } break;
                    case "Name": { _name = child.GetString(); } break;
                    case "Precache": { _kvPrecache = child; } break;
                    case "Passive": { _isPassive = BattleData.ParseBool01(child.GetString()); } break;
                    case "RemoveOnDeath": { _isRemoveOnDeath = BattleData.ParseBool01(child.GetString()); } break;
                    case "IsHidden": { _isHidden = BattleData.ParseBool01(child.GetString()); } break;
                    case "IsBuff": { _isBuff = BattleData.ParseBool01(child.GetString()); } break;
                    case "IsDebuff": { _isDebuff = BattleData.ParseBool01(child.GetString()); } break;
                    case "IsNebuff": { _isNebuff = BattleData.ParseBool01(child.GetString()); } break;
                    case "IsShowPrompt": { _isShowPrompt = BattleData.ParseBool01(child.GetString()); } break;
                    case "IsPurgable": { _isPurgable = BattleData.ParseBool01(child.GetString()); } break;
                    case "PurgableLevel": { _purgableLevel = BattleData.ParseInt(child.GetString()); } break;
                    case "ModifierGroup": { _group = BattleData.ParseInt(child.GetString()); } break;
                    case "ModifierGroupStackType": { BattleData.TryEvaluateEnum(child.GetString(), out _groupStackType); } break;
                    case "ModifierGroupStackCount": { _groupStackCount = BattleData.ParseInt(child.GetString()); } break;
                    case "OverrideAnimation": { BattleData.TryEvaluateEnum(child.GetString(), out _overrideAnimation); } break;
                    case "Priority": { _priority = BattleData.ParseInt(child.GetString()); } break;
                    case "AuraSerialId": { _auraModifierSerialId = BattleData.ParseInt(child.GetString()); } break;
                    case "AuraNodeRange": { _auraUnitTargetInfo.NodeRange = BattleData.ParseInt(child.GetString()); } break;
                    case "AuraRadius": { _auraUnitTargetInfo.RadiusRange = BattleData.ParseDFix64(child.GetString()); } break;
                    case "AuraTargetTeams": { BattleData.TryEvaluateEnums(child.GetString(), out _auraUnitTargetInfo.UnitTargetTeams); } break;
                    case "AuraTargetTypes": { BattleData.TryEvaluateEnums(child.GetString(), out _auraUnitTargetInfo.UnitTargetTypes); } break;
                    case "AuraTargetFlags": { BattleData.TryEvaluateEnums(child.GetString(), out _auraUnitTargetInfo.UnitTargetFlags); } break;
                    case "AuraTargetExcludedFlags": { BattleData.TryEvaluateEnums(child.GetString(), out _auraUnitTargetInfo.ExcludedUnitTargetFlags); } break;
                    case "AuraTargetSort": { BattleData.TryEvaluateEnum(child.GetString(), out _auraUnitTargetInfo.UnitTargetSort); } break;
                    case "AuraTargetTraits": { _auraUnitTargetInfo.UnitTargetTraits = BattleData.EvaluateTrait(child.GetString()); } break;
                    case "AuraTargetExcludedTraits": { _auraUnitTargetInfo.ExcludedUnitTargetTraits = BattleData.EvaluateTrait(child.GetString()); } break;
                    case "AuraTargetModifierGroups": { _auraUnitTargetInfo.UnitTargetModifierGroups = BattleData.EvaluateModifierGroup(child.GetString()); } break;
                    case "AuraTargetExcludedModifierGroups": { _auraUnitTargetInfo.ExcludedUnitTargetModifierGroups = BattleData.EvaluateModifierGroup(child.GetString()); } break;
                    case "AuraTargetMaxNumber": { _auraUnitTargetInfo.MaxNumber = BattleData.ParseInt(child.GetString()); } break;
                    case "AuraApplyToCaster": { _isAuraApplyToCaster = BattleData.ParseBool01(child.GetString()); } break;
                    case "Properties":
                        {
                            if (child.HasChildren)
                            {
                                _propertyKvs = new Dictionary<int, KeyValue>();

                                foreach (KeyValue child2 in child.Children)
                                {
                                    ModifierPropertyType modifierPropertyType;
                                    if (!BattleData.TryEvaluateEnum(child2.Key, out modifierPropertyType))
                                    {
                                        Log.Fatal("Modifier {0} 解析属性失败:{1}", LogName, child2.Key);
                                        continue;
                                    }

                                    if (_propertyKvs.ContainsKey((int)modifierPropertyType))
                                    {
                                        Log.Fatal("Modifier {0} 拥有相同类型的属性 {1}", LogName, child2.Key);
                                        continue;
                                    }

                                    _propertyKvs.Add((int)modifierPropertyType, child2);
                                }
                            }
                        }
                        break;
                    case "ImmuneModifierGroups":
                        {
                            if (child.HasChildren)
                            {
                                _immuneModifierGroups = BattleData.EvaluateModifierGroup(child.GetString());
                            }
                        }
                        break;
                    case "States":
                        {
                            if (child.HasChildren)
                            {
                                _states = new Dictionary<int, ModifierStateValue>();

                                foreach (KeyValue child2 in child.Children)
                                {
                                    ModifierStateType modifierStateType;
                                    if (!BattleData.TryEvaluateEnum(child2.Key, out modifierStateType))
                                    {
                                        Log.Fatal("Modifier {0} 解析状态失败:{1}", LogName, child2.Key);
                                        continue;
                                    }

                                    if (_states.ContainsKey((int)modifierStateType))
                                    {
                                        Log.Fatal("Modifier {0} 拥有相同类型的状态 {1}", LogName, child2.Key);
                                        continue;
                                    }

                                    ModifierStateValue modifierStateValue;
                                    if (!BattleData.TryEvaluateEnum(child2.GetString(), out modifierStateValue))
                                    {
                                        Log.Fatal("Modifier {0} 解析状态失败:{1}", LogName, child2.GetString());
                                        continue;
                                    }

                                    _states.Add((int)modifierStateType, modifierStateValue);
                                }
                            }
                        }
                        break;
                    case "OnCreated": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_CREATED, child); } break;
                    case "OnIntervalThink": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_INTERVALTHINK, child); } break;
                    case "OnPurged": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_PURGED, child); } break;
                    case "OnDestroy": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_DESTROY, child); } break;
                    case "OnDeath": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_DEATH, child); } break;
                    case "OnRespawn": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_RESPAWN, child); } break;
                    case "OnKilledUnit": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_KILL_UNIT, child); } break;
                    case "OnUnitDeath": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH, child); } break;
                    case "OnUnitDeath2": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH2, child); } break;
                    case "OnAttackPhaseStart": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ATTACK_PHASE_START, child); } break;
                    case "OnAttackStart": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ATTACK_START, child); } break;
                    case "OnAttackFinish": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ATTACK_FINISH, child); } break;
                    case "OnAbilityPhaseStart": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ABILITY_PHASE_START, child); } break;
                    case "OnAbilityPhaseInterrupted": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ABILITY_PHASE_INTERRUPTED, child); } break;
                    case "OnAbilityStart": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ABILITY_START, child); } break;
                    case "OnChannelSuccess": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ABILITY_CHANNEL_SUCCESS, child); } break;
                    case "OnChannelInterrupted": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ABILITY_CHANNEL_INTERRUPTED, child); } break;
                    case "OnChannelFinish": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ABILITY_CHANNEL_FINISH, child); } break;
                    case "OnAbilityFinish": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ABILITY_FINISH, child); } break;
                    case "OnBeforeTakeDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_DAMAGE, child); } break;
                    case "OnBeforeTakeAttackDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_ATTACK_DAMAGE, child); } break;
                    case "OnBeforeTakeAttackNoCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_ATTACK_NOCRIT_DAMAGE, child); } break;
                    case "OnBeforeTakeAttackCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_ATTACK_CRIT_DAMAGE, child); } break;
                    case "OnBeforeTakeAbilityDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_ABILITY_DAMAGE, child); } break;
                    case "OnBeforeTakeAbilityNoCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_ABILITY_NOCRIT_DAMAGE, child); } break;
                    case "OnBeforeTakeAbilityCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_ABILITY_CRIT_DAMAGE, child); } break;
                    case "OnTakeDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_TAKE_DAMAGE, child); } break;
                    case "OnTakeAttackDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_TAKE_ATTACK_DAMAGE, child); } break;
                    case "OnTakeAttackNoCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_TAKE_ATTACK_NOCRIT_DAMAGE, child); } break;
                    case "OnTakeAttackCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_TAKE_ATTACK_CRIT_DAMAGE, child); } break;
                    case "OnTakeAbilityDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_TAKE_ABILITY_DAMAGE, child); } break;
                    case "OnTakeAbilityNoCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_TAKE_ABILITY_NOCRIT_DAMAGE, child); } break;
                    case "OnTakeAbilityCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_TAKE_ABILITY_CRIT_DAMAGE, child); } break;
                    case "OnBeforeDealDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_DAMAGE, child); } break;
                    case "OnBeforeDealAttackDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_ATTACK_DAMAGE, child); } break;
                    case "OnBeforeDealAttackNoCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_ATTACK_NOCRIT_DAMAGE, child); } break;
                    case "OnBeforeDealAttackCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_ATTACK_CRIT_DAMAGE, child); } break;
                    case "OnBeforeDealAbilityDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_ABILITY_DAMAGE, child); } break;
                    case "OnBeforeDealAbilityNoCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_ABILITY_NOCRIT_DAMAGE, child); } break;
                    case "OnBeforeDealAbilityCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_ABILITY_CRIT_DAMAGE, child); } break;
                    case "OnDealDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_DEAL_DAMAGE, child); } break;
                    case "OnDealAttackDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_DEAL_ATTACK_DAMAGE, child); } break;
                    case "OnDealAttackNoCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_DEAL_ATTACK_NOCRIT_DAMAGE, child); } break;
                    case "OnDealAttackCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_DEAL_ATTACK_CRIT_DAMAGE, child); } break;
                    case "OnDealAbilityDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_DEAL_ABILITY_DAMAGE, child); } break;
                    case "OnDealAbilityNoCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_DEAL_ABILITY_NOCRIT_DAMAGE, child); } break;
                    case "OnDealAbilityCritDamage": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_DEAL_ABILITY_CRIT_DAMAGE, child); } break;
                    case "OnMiss": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_MISS, child); } break;
                    case "OnAttackMiss": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ATTACK_MISS, child); } break;
                    case "OnAbilityMiss": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ABILITY_MISS, child); } break;
                    case "OnDodge": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_DODGE, child); } break;
                    case "OnDodgeAttack": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ATTACK_DODGE, child); } break;
                    case "OnDodgeAbility": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_ABILITY_DODGE, child); } break;
                    case "OnBeforeTakeHeal": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_HEAL, child); } break;
                    case "OnTakeHeal": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_TAKE_HEAL, child); } break;
                    case "OnBeforeDealHeal": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_HEAL, child); } break;
                    case "OnDealHeal": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_DEAL_HEAL, child); } break;
                    case "OnManaIncrease": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_MANA_INCREASE, child); } break;
                    case "OnManaReduce": { _kvActions.Add((int)ModifierEventType.MODIFIER_EVENT_ON_MANA_REDUCE, child); } break;
                }
            }

            // Modifier自带排除标志
            _auraUnitTargetInfo.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE;
            _auraUnitTargetInfo.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_INVINCIBLE;
        }

        private bool CheckAuraTarget(BaseUnit target)
        {
            if (_auraModifier == null)
            {
                return false;
            }

            if (_auraModifier.IsBuff && (target.IsBuffImmune || target.IsAllModifierImmune))
            {
                return false;
            }
            else if (_auraModifier.IsDebuff && (target.IsDebuffImmune || target.IsAllModifierImmune))
            {
                return false;
            }
            else if (_auraModifier.IsNebuff && (target.IsNebuffImmune || target.IsAllModifierImmune))
            {
                return false;
            }

            if (_auraModifier.Group != 0)
            {
                if (target.IsImmuneModifierGroup(_auraModifier.Group))
                {
                    return false;
                }

                if (_auraModifier.GroupStackCount != -1 && target.GetModifierGroupCount(_auraModifier.Group) >= _auraModifier.GroupStackCount)
                {
                    return false;
                }
            }

            return true;
        }

        public override void Release()
        {
            if (_attachedParticles != null)
            {
                foreach (KeyValuePair<string, int> item in _attachedParticles)
                {
                    ParticleManager.DestroyParticle(item.Value, ObjectId);
                }

                _attachedParticles.Clear();
                _attachedParticles = null;
            }

            if (_attachedSoundIds != null && _attachedSoundIds.Count > 0)
            {
                foreach (KeyValuePair<string, int> item in _attachedSoundIds)
                {
                    GameManager.Sound.StopSound(item.Value, 0f);
                }

                _attachedSoundIds.Clear();
                _attachedSoundIds = null;
            }

            if (_kvActions != null)
            {
                if (_kvActions.ContainsKey((int)ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH))
                {
                    BattleData.RemoveFulltimeHandler(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH, this);
                }

                if (_kvActions.ContainsKey((int)ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH2))
                {
                    BattleData.RemoveFulltimeHandler(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH2, this);
                }
            }

            if (_auraModifierSerialId != 0)
            {
                if (_auraUnitTargetModifiers.Count > 0)
                {
                    foreach (KeyValuePair<BaseUnit, Modifier> modifier in _auraUnitTargetModifiers)
                    {
                        if (modifier.Value != null)
                        {
                            modifier.Value.IsAura = false;
                            modifier.Key.RemoveModifier(modifier.Value);
                        }
                    }

                    _auraUnitTargetModifiers.Clear();
                }

                _auraUnitTargets.Clear();
            }

            _auraUnitTargetInfo = null;

            _caster = null;
            _ability = null;
            _target = null;
            _duration = -DFix64.One;
            _thinkInterval = -DFix64.One;
            _isNeedUpdateTime = false;
            _isNeedUpdateThink = false;
            _time = DFix64.Zero;
            _nextThinkTime = -DFix64.One;
            if (_properties != null)
            {
                _properties.Clear();
                _properties = null;
            }
            if (_attachedParticles != null)
            {
                _attachedParticles.Clear();
                _attachedParticles = null;
            }
            if (_attachedSoundIds != null)
            {
                _attachedSoundIds.Clear();
                _attachedSoundIds = null;
            }
            if (_auraUnitTargets != null)
            {
                _auraUnitTargets.Clear();
                _auraUnitTargets = null;
            }
            if (_auraUnitTargetModifiers != null)
            {
                _auraUnitTargetModifiers.Clear();
                _auraUnitTargetModifiers = null;
            }
            if (_values != null)
            {
                _values.Clear();
                _values = null;
            }

            base.Release();
        }

        public void SetData(BaseUnit caster, Ability ability, BaseUnit target, CreateModifierData createModifierData)
        {
            _isRecycle = false;
            _isRemoved = false;

            _caster = caster;
            _ability = ability;
            _target = target;

            EventData eventData = BattleData.CreateEventData();
            eventData.Caster = _caster;
            eventData.Ability = _ability;
            eventData.Target = _target;

            if (_keyValue != null)
            {
                if (_keyValue["Duration"] != null)
                {
                    _duration = BattleData.EvaluateDFix64(_keyValue["Duration"].GetString(), eventData);
                }
                else
                {
                    _duration = -DFix64.One;
                }

                if (_keyValue["ThinkInterval"] != null)
                {
                    _thinkInterval = BattleData.EvaluateDFix64(_keyValue["ThinkInterval"].GetString(), eventData);
                    _nextThinkTime = _thinkInterval;
                }
                else
                {
                    _thinkInterval = -DFix64.One;
                    _nextThinkTime = -DFix64.One;
                }

                if (_kvActions != null)
                {
                    if (_kvActions.ContainsKey((int)ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH))
                    {
                        BattleData.AddFulltimeHandler(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH, this);
                    }

                    if (_kvActions.ContainsKey((int)ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH2))
                    {
                        BattleData.AddFulltimeHandler(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH2, this);
                    }
                }
            }

            if (createModifierData != null)
            {
                if (createModifierData.IsDurationSpecified)
                {
                    _duration = createModifierData.Duration;
                }
            }

            if (_duration != -DFix64.One)
            {
                _isNeedUpdateTime = true;
            }

            if (_thinkInterval > DFix64.Zero)
            {
                _isNeedUpdateThink = true;
            }

            if (_propertyKvs != null)
            {
                _properties = new Dictionary<int, DFix64>();
                foreach (KeyValuePair<int, KeyValue> item in _propertyKvs)
                {
                    DFix64 pValue = BattleData.EvaluateDFix64(item.Value.GetString(), eventData);
                    pValue = DFix64.Floor(pValue);
                    _properties.Add(item.Key, pValue);
                }
            }

            if (_auraModifierSerialId != 0)
            {
                if (_auraModifier != null)
                {
                    _auraModifier.Remove();
                    _auraModifier = null;
                }

                _auraUnitTargets = new LinkedList<BaseUnit>();
                _auraUnitTargetModifiers = new Dictionary<BaseUnit, Modifier>();

                if (_ability != null)
                {
                    _auraModifier = _ability.CreateModifier(_auraModifierSerialId);
                }
                else
                {
                    _auraModifier = BattleKvLibraryManager.CreateBuiltinModifier(_auraModifierSerialId);
                }
            }
        }

        public void SetRecycleState()
        {
            _isRemoved = true;
        }

        public void Recycle()
        {
            if (_isRecycle)
            {
                return;
            }

            _isRecycle = true;

            _caster = null;
            _ability = null;
            _target = null;
            _duration = -DFix64.One;
            _thinkInterval = -DFix64.One;
            _isNeedUpdateTime = false;
            _isNeedUpdateThink = false;
            _time = DFix64.Zero;
            _nextThinkTime = -DFix64.One;
            if (_properties != null)
            {
                _properties.Clear();
                _properties = null;
            }
            if (_attachedParticles != null)
            {
                _attachedParticles.Clear();
                _attachedParticles = null;
            }
            if (_attachedSoundIds != null)
            {
                _attachedSoundIds.Clear();
                _attachedSoundIds = null;
            }
            if (_auraUnitTargets != null)
            {
                _auraUnitTargets.Clear();
                _auraUnitTargets = null;
            }
            if (_auraUnitTargetModifiers != null)
            {
                _auraUnitTargetModifiers.Clear();
                _auraUnitTargetModifiers = null;
            }
            if (_values != null)
            {
                _values.Clear();
                _values = null;
            }
        }

        public virtual void Remove()
        {
            if (_isRemoved)
            {
                return;
            }

            _isRemoved = true;

            if (_attachedParticles != null)
            {
                foreach (KeyValuePair<string, int> item in _attachedParticles)
                {
                    ParticleManager.DestroyParticle(item.Value, ObjectId);
                }

                _attachedParticles.Clear();
                _attachedParticles = null;
            }

            if (_attachedSoundIds != null && _attachedSoundIds.Count > 0)
            {
                foreach (KeyValuePair<string, int> item in _attachedSoundIds)
                {
                    GameManager.Sound.StopSound(item.Value, 0f);
                }

                _attachedSoundIds.Clear();
                _attachedSoundIds = null;
            }

            if (_kvActions != null)
            {
                if (_kvActions.ContainsKey((int)ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH))
                {
                    BattleData.RemoveFulltimeHandler(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH, this);
                }

                if (_kvActions.ContainsKey((int)ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH2))
                {
                    BattleData.RemoveFulltimeHandler(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH2, this);
                }
            }

            if (_auraModifierSerialId != 0)
            {
                if (_auraModifier != null)
                {
                    _auraModifier.Remove();
                    _auraModifier = null;
                }

                if (_auraUnitTargetModifiers.Count > 0)
                {
                    foreach (KeyValuePair<BaseUnit, Modifier> modifier in _auraUnitTargetModifiers)
                    {
                        if (modifier.Value != null)
                        {
                            modifier.Value.IsAura = false;
                            modifier.Key.RemoveModifier(modifier.Value);
                        }
                    }

                    _auraUnitTargetModifiers.Clear();
                }

                _auraUnitTargets.Clear();
            }
        }

        public string[] Precache()
        {
            if (_kvPrecache == null || !_kvPrecache.HasChildren)
            {
                IsPrecached = true;
                return null;
            }

            string[] assets = new string[_kvPrecache.ChildCount];
            foreach (var child in _kvPrecache.Children)
            {

            }

            return assets;
        }

        public override void UpdateLogic(DFix64 frameLength)
        {
            _time += frameLength;

            if (_isNeedUpdateTime)
            {
                if (_time >= _duration)
                {
                    _target.RemoveModifier(this);
                    return;
                }
            }

            if (_auraModifierSerialId != 0)
            {
                BattleData.FindTargets(_caster, _target, _auraUnitTargetInfo, _auraUnitTargets);

                if (_auraUnitTargets.Count > 0)
                {
                    LinkedListNode<BaseUnit> first = _auraUnitTargets.First;
                    while (first != null)
                    {
                        if (first.Value == Target && !_isAuraApplyToCaster)
                        {
                            first = first.Next;
                            continue;
                        }

                        if (!CheckAuraTarget(first.Value))
                        {
                            first = first.Next;
                            continue;
                        }

                        if (!_auraUnitTargetModifiers.ContainsKey(first.Value))
                        {
                            Modifier modifier = first.Value.ApplyModifier(_caster, _ability, _auraModifierSerialId, null, true);
                            if (modifier != null)
                            {
                                modifier.IsAura = true;
                                _auraUnitTargetModifiers.Add(first.Value, modifier);
                            }
                        }
                        else if (_auraUnitTargetModifiers[first.Value].IsRemoved)
                        {
                            _auraUnitTargetModifiers[first.Value].IsAura = false;
                            _auraUnitTargetModifiers.Remove(first.Value);

                            Modifier modifier = first.Value.ApplyModifier(_caster, _ability, _auraModifierSerialId, null, true);
                            if (modifier != null)
                            {
                                modifier.IsAura = true;
                                _auraUnitTargetModifiers.Add(first.Value, modifier);
                            }
                        }

                        first = first.Next;
                    }
                }

                foreach (KeyValuePair<BaseUnit, Modifier> modifier in _auraUnitTargetModifiers)
                {
                    if (!_auraUnitTargets.Contains(modifier.Key))
                    {
                        modifier.Key.RemoveModifier(modifier.Value);
                    }
                }
            }
            else
            {
                if (_isNeedUpdateThink)
                {
                    if (_time >= _nextThinkTime)
                    {
                        _nextThinkTime += _thinkInterval;

                        if (_kvActions.ContainsKey((int)ModifierEventType.MODIFIER_EVENT_ON_INTERVALTHINK))
                        {
                            HandleEvent(ModifierEventType.MODIFIER_EVENT_ON_INTERVALTHINK, BattleData.CreateEventData());
                        }
                    }
                }
            }
        }

        public ModifierStateValue GetStateValue(int stateType)
        {
            if (_states == null || !_states.ContainsKey(stateType))
            {
                return ModifierStateValue.MODIFIER_STATE_VALUE_NO_ACTION;
            }

            return _states[stateType];
        }

        public virtual void OnCreated()
        {
            Log.Info("{0} 获得来自 {1} 的Modifier {2},持续时间:{3}", _target.LogName, _ability != null ? _ability.LogName : string.Empty, LogName, _duration == -DFix64.One ? "永久" : _duration.ToString() + " 秒");

            BattleModifierCreateEventArgs ne = GameFramework.ReferencePool.Acquire<BattleModifierCreateEventArgs>();
            ne.Modifier = this;
            BattleData.FireEvent(ne);

            if (_kvActions != null && _kvActions.ContainsKey((int)ModifierEventType.MODIFIER_EVENT_ON_CREATED))
            {
                HandleEvent(ModifierEventType.MODIFIER_EVENT_ON_CREATED, BattleData.CreateEventData());
            }
        }

        public virtual void OnDestroy(bool isPurged)
        {
            Log.Info("{0} 失去{1}来自 {2} 的Modifier {3}", _target.LogName, isPurged ? "(被驱散)" : string.Empty, _ability != null ? _ability.LogName : string.Empty, LogName);

            BattleModifierRemoveEventArgs ne = GameFramework.ReferencePool.Acquire<BattleModifierRemoveEventArgs>();
            ne.Modifier = this;
            ne.IsPurged = isPurged;
            BattleData.FireEvent(ne);

            if (_kvActions != null)
            {
                if (isPurged)
                {
                    if (_kvActions.ContainsKey((int)ModifierEventType.MODIFIER_EVENT_ON_PURGED))
                    {
                        HandleEvent(ModifierEventType.MODIFIER_EVENT_ON_PURGED, BattleData.CreateEventData());
                    }
                }
                else
                {
                    if (_kvActions.ContainsKey((int)ModifierEventType.MODIFIER_EVENT_ON_DESTROY))
                    {
                        HandleEvent(ModifierEventType.MODIFIER_EVENT_ON_DESTROY, BattleData.CreateEventData());
                    }
                }
            }
        }

        public virtual void RefreshDuration()
        {
            Log.Info("Modifier {0} 重置时间 {1}秒", LogName, _duration.ToString());

            _time = DFix64.Zero;

            if (_isNeedUpdateThink)
            {
                _nextThinkTime = _thinkInterval;
            }
        }

        public virtual void RefreshDuration(DFix64 duration)
        {
            Log.Info("Modifier {0} 重置时间 {1}秒", LogName, duration.ToString());

            _time = DFix64.Zero;
            _duration = duration;

            if (_duration != -DFix64.One)
            {
                _isNeedUpdateTime = true;
            }
            else
            {
                _isNeedUpdateTime = false;
            }

            if (_isNeedUpdateThink)
            {
                _nextThinkTime = _thinkInterval;
            }
        }

        public virtual void AttachEffectNew(string effectName, int particleId)
        {
            if (_attachedParticles == null)
            {
                _attachedParticles = new Dictionary<string, int>();
                _attachedParticles.Add(effectName, particleId);
                ParticleManager.SetParticleAttachTarget(particleId, ObjectId);
            }
            else if (!_attachedParticles.ContainsKey(effectName))
            {
                _attachedParticles.Add(effectName, particleId);
                ParticleManager.SetParticleAttachTarget(particleId, ObjectId);
            }
            else if (_attachedParticles.ContainsKey(effectName))
            {
                Log.Fatal("重复AttachEffect : Ability:{0} {1} Modifier:{2} {3} Effect:{4}", Ability != null ? Ability.SerialId.ToString() : string.Empty, Ability != null ? Ability.LogName : string.Empty, SerialId.ToString(), LogName, effectName);

                ParticleManager.DestroyParticle(_attachedParticles[effectName], ObjectId);

                _attachedParticles[effectName] = particleId;
                ParticleManager.SetParticleAttachTarget(particleId, ObjectId);
            }
        }

        public virtual void AttachSound(string soundName, int soundId)
        {
            if (_attachedSoundIds == null)
            {
                _attachedSoundIds = new Dictionary<string, int>();
                _attachedSoundIds.Add(soundName, soundId);
            }
            else if (!_attachedSoundIds.ContainsKey(soundName))
            {
                _attachedSoundIds.Add(soundName, soundId);
            }
            else if (_attachedSoundIds.ContainsKey(soundName))
            {
                Log.Fatal("重复AttachSound : Ability:{0} {1} Modifier:{2} {3} Sound:{4}", Ability != null ? Ability.SerialId.ToString() : string.Empty, Ability != null ? Ability.LogName : string.Empty, SerialId.ToString(), LogName, soundName);

                GameManager.Sound.StopSound(_attachedSoundIds[soundName]);

                _attachedSoundIds[soundName] = soundId;
            }
        }

        public virtual void HandleEvent(ModifierEventType modifierEventType, EventData eventData)
        {
            if (eventData == null)
            {
                Log.Fatal("Modifier {0} 处理事件 {1} 失败, eventData为空", LogName, modifierEventType.ToString());
                return;
            }

            if (_kvActions != null && _kvActions.ContainsKey((int)modifierEventType))
            {
                eventData.Caster = _caster;
                eventData.Target = _target;
                eventData.Ability = _ability;
                eventData.Modifier = this;

                BattleData.ExecuteActions(_kvActions[(int)modifierEventType], eventData);
            }
        }
    }
}