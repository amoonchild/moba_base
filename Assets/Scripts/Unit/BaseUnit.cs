using KVLib;
using GameFramework.Entity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    public abstract class BaseUnit : BaseEntity
    {
        protected int _randomSpeed = 0;
        protected ulong _playerId = 0UL;                                                                                // 玩家id
        protected BasePlayer _player = null;                                                                                // 玩家id
        protected int _bagIndex = -1;                                                                                   // 背包索引
        protected int _lineupIndex = -1;                                                                                // 阵容索引
        protected int _teamId = 0;                                                                                      // 阵营
        protected string _name = string.Empty;                                                                          // 单位名字
        protected int _level = 0;                                                                                       // 等级
        protected ulong _battleScore = 0;                                                                             // 战斗评分
        protected int _unitId = 0;                                                                                      // 单位流水号
        protected int _skinId = 0;                                                                                      // 皮肤id
        protected DFix64 _delaySpawnTime = DFix64.Zero;                                                                   // 延迟上场时间
        protected BattleNode _currNode = null;                                                                          // 当前格子
        protected BattleNode _deadNode = null;                                                                          // 当前格子
        protected List<Ability> _allAbilities = new List<Ability>();                                                    // 技能
        protected int _traitType = 0;                                                                                   // 特性
        protected int _traitType2 = 0;                                                                                   // 特性
        protected UnitAttackCapabilityType _attackCapability = UnitAttackCapabilityType.UNIT_CAP_NONE;                  // 攻击类型
        protected List<MulTargetInfo> _attackOrders = new List<MulTargetInfo>();                                        // 攻击序列
        protected BattleUnitAttribute _initAtt = null;                                                                        // 初始属性
        protected BattleUnitAttribute _currAtt = null;                                                                        // 当前属性
        protected BattleUnitStatisticsData _statisticsData = null;                                                            // 统计数据
        protected BaseUnit _master = null;                                                                              // 所属单位
        protected bool _isSpawnedToLineup = false;                                                                      // 是否已出场
        protected bool _isSpawned = false;                                                                              // 是否已出场
        protected int _lifeIndex = 0;                                                                                   // 生命次数
        protected bool _isDeadState = false;                                                                            // 是否已死亡
        protected bool _isHideState = false;                                                                            // 死亡并且消失
        protected bool _isHideState2 = false;                                                                            // 死亡并且消失
        protected DFix64 _delayHideTime = DFix64.Zero;                                                                    // 死亡后消失计时
        protected DFix64 _manaRegenTime = DFix64.Zero;
        protected DFix64 _realScale = DFix64.One;
        protected List<BaseOrder> _orders = new List<BaseOrder>();                                                      // 行为列表
        protected BaseOrder _currOrder = null;                                                                          // 当前行为
        protected bool _isMoving = false;
        protected LinkedList<BaseUnit> _cachedAttackTargets = new LinkedList<BaseUnit>();
        protected BaseUnit _attackTarget = null;                                                                        // 攻击目标
        protected BaseUnit _froceAttackTarget = null;                                                                   // 嘲讽攻击目标
        protected bool _isAttacking = false;                                                                            // 攻击中
        protected bool _isAttackingPS = false;                                                                          // 攻击前摇中
        protected bool _isAttackingPE = false;                                                                          // 攻击后摇               
        protected bool _isCasting = false;
        protected bool _isResetAtt = false;

        protected Dictionary<string, DFix64> _numberValues = new Dictionary<string, DFix64>();
        protected BattleUnitEntityLogic _unitEntityLogic = null;


        public abstract UnitType Type
        {
            get;
        }

        public int RandomSpeed
        {
            get { return _randomSpeed; }
        }
        public int BattleIndex
        {
            get
            {
                return base.ObjectId;
            }
        }
        public ulong PlayerId
        {
            get { return _playerId; }
            private set { _playerId = value; }
        }
        public BasePlayer Player
        {
            get { return _player; }
        }
        public int BagIndex
        {
            get { return _bagIndex; }
            private set { _bagIndex = value; }
        }
        public int LineupIndex
        {
            get { return _lineupIndex; }
            private set { _lineupIndex = value; }
        }
        public int TeamId
        {
            get { return _teamId; }
            set { _teamId = value; }
        }
        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }
        public string LogName
        {
            get { return GameFramework.Utility.Text.Format("<color=#FFFFFFFF>{0}</color>", Name); }
        }
        public int Level
        {
            get { return _level; }
            private set { _level = value; }
        }
        public ulong BattleScore
        {
            get { return _battleScore; }
            private set { _battleScore = value; }
        }
        public virtual int UnitId
        {
            get { return _unitId; }
            private set { _unitId = value; }
        }
        public int SkinId
        {
            get { return _skinId; }
            private set { _skinId = value; }
        }

        public int TraitType
        {
            get { return _traitType; }
            private set { _traitType = value; }
        }
        public int TraitType2
        {
            get { return _traitType2; }
            private set { _traitType2 = value; }
        }
        public UnitAttackCapabilityType AttackCapability
        {
            get { return _attackCapability; }
            private set { _attackCapability = value; }
        }

        public BaseUnit Master
        {
            get { return _master; }
            set { _master = value; }
        }

        public DFix64 DelaySpawnTime
        {
            get { return _delaySpawnTime; }
        }
        public BattleNode CurrNode
        {
            get
            {
                return _currNode;
            }
        }
        public BattleNode DeadNode
        {
            get
            {
                return _deadNode;
            }
        }

        public BattleUnitAttribute InitAtt
        {
            get
            {
                return _initAtt;
            }
        }
        public BattleUnitAttribute CurrAtt
        {
            get
            {
                return _currAtt;
            }
        }
        public BattleUnitStatisticsData StatisticsData
        {
            get
            {
                return _statisticsData;
            }
        }
        public int TeamTotalNumber
        {
            get { return _player.TeamTotalNumber; }
        }
        public int TeamDeathNumber
        {
            get { return _player.TeamDeathNumber; }
        }
        public int TeamKillNumber
        {
            get { return _player.TeamKillNumber; }
        }
        public BasePlayer Team
        {
            get { return _player; }
        }
        public bool IsResetAtt
        {
            get { return _isResetAtt; }
        }
        public bool IsMoving
        {
            get
            {
                return _isMoving;
            }
            set
            {
                _isMoving = value;
            }
        }
        public BaseUnit AttackUnitTarget
        {
            get
            {
                return _attackTarget;
            }
        }
        /// <summary>
        /// 正在攻击
        /// </summary>
        public bool IsAttacking
        {
            get
            {
                return _isAttacking;
            }
            set
            {
                _isAttacking = value;
            }
        }
        /// <summary>
        /// 攻击前摇中
        /// </summary>
        public bool IsAttackingPS
        {
            get;
            set;
        }
        /// <summary>
        /// 攻击后摇中
        /// </summary>
        public bool IsAttackingPE
        {
            get;
            set;
        }
        /// <summary>
        /// 正在施法
        /// </summary>
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
        /// <summary>
        /// 施法前摇中
        /// </summary>
        public bool IsCastingPS
        {
            get;
            set;
        }
        /// <summary>
        /// 施法后摇中
        /// </summary>
        public bool IsCastingPE
        {
            get;
            set;
        }

        public bool IsSpawnedToLineup
        {
            get { return _isSpawnedToLineup; }
            private set { _isSpawnedToLineup = value; }
        }
        public bool IsSpawned
        {
            get { return _isSpawned; }
            private set { _isSpawned = value; }
        }
        public bool IsAlive
        {
            get { return CurrAtt.Hp > DFix64.Zero; }
        }
        public bool IsCheckDeadState
        {
            get;
            set;
        }
        public bool IsDeadState
        {
            get { return _isDeadState; }
            private set { _isDeadState = value; }
        }
        public bool IsHideState
        {
            get { return _isHideState; }
            private set { _isHideState = value; }
        }

        public virtual bool IsIllusion
        {
            get
            {
                return _currAtt.IsIllusion > DFix64.Zero;
            }
        }
        public virtual bool IsSummoned
        {
            get
            {
                return _currAtt.IsSummoned > DFix64.Zero;
            }
        }

        public bool IsMoveDisabled
        {
            get;
            private set;
        }
        public bool IsAttackDisabled
        {
            get;
            private set;
        }
        public bool IsActivatesAbilityDisabled
        {
            get;
            private set;
        }
        public bool IsPassivesAbilityDisabled
        {
            get;
            private set;
        }
        public bool IsStunned
        {
            get;
            private set;
        }
        public bool IsBuffImmune
        {
            get;
            private set;
        }
        public bool IsDebuffImmune
        {
            get;
            private set;
        }
        public bool IsNebuffImmune
        {
            get;
            private set;
        }
        public bool IsAllModifierImmune
        {
            get;
            private set;
        }
        public bool IsInvisible
        {
            get;
            private set;
        }
        public bool IsNoHealthBar
        {
            get;
            private set;
        }
        public bool IsInvincible
        {
            get;
            private set;
        }
        public bool IsResticted
        {
            get;
            private set;
        }
        public bool IsUnselectable
        {
            get;
            private set;
        }
        public bool IsUnselectableAttack
        {
            get;
            private set;
        }
        public bool IsUnselectableAbility
        {
            get;
            private set;
        }

        public string HeadIcon
        {
            get
            {
                return _headIconAssetPath;
            }
        }
        public string SpawnEffect
        {
            get
            {
                return _spawnEffect;
            }
        }
        public DFix64 ModelScale
        {
            get
            {
                return _modelScale;
            }
        }

        public override DFixVector3 LogicScale
        {
            get
            {
                return _logicScale;
            }
            set
            {
                _logicScale = value;
            }
        }
        public DFix64 RealScale
        {
            get { return _realScale; }
        }

        public DFix64 this[string key]
        {
            get
            {
                if (!_numberValues.ContainsKey(key))
                {
                    return DFix64.Zero;
                }

                return _numberValues[key];
            }
            set
            {
                if (!_numberValues.ContainsKey(key))
                {
                    _numberValues.Add(key, value);
                }
                else
                {
                    _numberValues[key] = value;
                }
            }
        }

#if UNITY_EDITOR
        private LogUnitAttribute _logUnitAttribute = null;
#endif

        public BaseUnit()
            : base()
        {

        }

        public BaseUnit(CreateUnitData data)
            : this()
        {
            Init(data);
            ResetAbility(data.skill_id);
        }

        public virtual void Init(CreateUnitData data)
        {
            PlayerId = data.player_id;
            _player = BattleData.GetPlayer(data.player_id);
            BagIndex = data.bag_index;
            LineupIndex = data.unit_index;
            TeamId = data.player_camp;
            Name = data.name;
            Level = data.level;
            BattleScore = data.score;
            UnitId = data.card_id;

            KeyValue kvUnit = BattleKvLibraryManager.GetUnitKv(_unitId);
            if (kvUnit != null)
            {
                TraitType = (int)BattleData.EvaluateEnum<CardTraitType>(kvUnit["Trait"].GetString(), CardTraitType.None);
                if (kvUnit["Trait2"] != null)
                {
                    TraitType2 = (int)BattleData.EvaluateEnum<CardTraitType>(kvUnit["Trait2"].GetString(), CardTraitType.None);
                }
                AttackCapability = BattleData.EvaluateEnum(kvUnit["AttackCapability"].GetString(), UnitAttackCapabilityType.UNIT_CAP_NONE);
                SkinId = BattleData.ParseInt(kvUnit["Skin"].GetString());

                KeyValue kvAttackOrder = kvUnit["AttackOrder"];
                if (kvAttackOrder != null)
                {
                    foreach (var child in kvAttackOrder.Children)
                    {
                        if (child.ChildCount > 0)
                        {
                            MulTargetInfo attackOrder = BattleData.GetTargetInfo(child);
                            if (attackOrder != null)
                            {
                                // 普通自带排除标记
                                attackOrder.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE;
                                attackOrder.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE_ATTACK;
                                attackOrder.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_INVINCIBLE;
                                attackOrder.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_INVISIBLE;
                                attackOrder.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_SELF;
                                attackOrder.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_DEAD;

                                _attackOrders.Add(attackOrder);
                            }
                        }
                    }
                }
            }

            if (data.skin_id > 0)
            {
                SkinId = data.skin_id;
            }

            _currNode = BattleData.FindNode(data.unit_in_x, data.unit_in_y);

            _initAtt = new BattleUnitAttribute();
            _initAtt.Scale = data.model_radii;
            _currAtt = _initAtt.Clone();
            _statisticsData = new BattleUnitStatisticsData();

            _delaySpawnTime = (DFix64)data.DelaySpawnTime;

            InitSkinParm();

            _realScale = DFix64.Max(DFix64.Zero, _currAtt.Scale * _modelScale);

            _randomSpeed = BattleData.SRandom.RangeI(0, 1000);
        }

        public virtual void ResetAttAndAbility(ResetUnitAttributeData resetUnitAttributeData)
        {
            _isResetAtt = true;

            LineupIndex = resetUnitAttributeData.unit_index;

            _initAtt = resetUnitAttributeData.Att.Clone();
            _currAtt = resetUnitAttributeData.Att.Clone();

            ResetAbility(resetUnitAttributeData.skill_id);
        }

        public virtual void ResetAtt(BattleUnitAttribute unitAttribute)
        {
            _isResetAtt = true;

            _initAtt = unitAttribute.Clone();
            _currAtt = unitAttribute.Clone();

            _realScale = DFix64.Max(DFix64.Zero, _currAtt.Scale * _modelScale);
        }

        public virtual void ResetAbility(ObscuredInt[] skillIds)
        {
            for (int i = 0; i < _allAbilities.Count; i++)
            {
                _allAbilities[i].Release();
            }
            _allAbilities.Clear();

            if (skillIds != null && skillIds.Length > 0)
            {
                for (int i = 0; i < skillIds.Length; i++)
                {
                    AddAbility(skillIds[i]);
                }
            }
        }

        public override void UpdateLogic(DFix64 frameLength)
        {
            if (IsFree)
            {
                return;
            }

            _lastLogicPosition = _logicPosition;
            _lastLogicAngles = _logicEulerAngles;
            _lastLogicScale = _logicScale;

            if (IsSpawned)
            {
                UpdateModifiers(frameLength);

                if (IsDeadState)
                {
                    UpdateDeath(frameLength);
                }
                else
                {
                    UpdateOrder(frameLength);
                    FindAttackTarget();
                    UpdateAbility(frameLength);
                    UpdateAttack(frameLength);
                    UpdateMana(frameLength);

                    if (_currOrder == null && _orders.Count == 0)
                    {
                        PlayAnimation(UnitAnimationType.ACT_IDLE, true);
                    }
                }
            }

            UpdateModelLogic(frameLength);
        }

        public override void UpdateRender(float interpolation, float deltaTime)
        {
            if (IsFree || IsHideState)
            {
                return;
            }

            _renderPosition = Vector3.Lerp(_lastLogicPosition.ToVector3(), _logicPosition.ToVector3(), interpolation);
            _renderEulerAngles = Vector3.Lerp(_lastLogicAngles.ToVector3(), _logicEulerAngles.ToVector3(), interpolation);
            _renderScale = Vector3.Lerp((_lastLogicScale * _realScale).ToVector3(), (_logicScale * _realScale).ToVector3(), interpolation);

            if (_unitEntityLogic != null)
            {
                _unitEntityLogic.CachedTransform.position = _renderPosition;
                _unitEntityLogic.CachedTransform.eulerAngles = _renderEulerAngles;
                _unitEntityLogic.CachedTransform.localScale = _renderScale;
            }

            UpdateModelRender(interpolation, deltaTime);
        }

        public override void Destroy()
        {
            DestroyModel();

            base.Destroy();
        }

        public override void Release()
        {
#if UNITY_EDITOR
            if (_logUnitAttribute != null)
            {
                GameObject.Destroy(_logUnitAttribute.gameObject);
                _logUnitAttribute = null;
            }
#endif

            ReleaseAbilities();
            ReleaseModifiers();

            base.Release();
        }

        protected virtual void UpdateDeath(DFix64 frameLength)
        {
            if (!_isHideState)
            {
                _delayHideTime += frameLength;
                if (_delayHideTime >= BattleData.GetDelayHideDuration())
                {
                    SetHideState(false);
                }
            }
            else
            {
                if (!_isHideState2)
                {
                    _delayHideTime += frameLength;
                    if (_delayHideTime >= BattleData.GetDelayHide2Duration())
                    {
                        _isHideState2 = true;

                        StopAnimation();
                        Destroy();
                    }
                }
            }
        }

        protected virtual void UpdateOrder(DFix64 frameLength)
        {
            if (_currOrder != null && !_currOrder.IsFinished)
            {
                _currOrder.UpdateLogic(frameLength);
            }
            else
            {
                _currOrder = null;
                while (_orders.Count > 0)
                {
                    _currOrder = _orders[0];
                    _orders.RemoveAt(0);

                    if (!_currOrder.Begin())
                    {
                        _currOrder = null;
                        continue;
                    }

                    break;
                }
            }
        }

        protected virtual void FindAttackTarget()
        {
            if (_attackTarget != null)
            {
                if (_attackTarget.IsDeadState || _attackTarget.IsUnselectable || _attackTarget.IsUnselectableAttack || _attackTarget.IsInvincible || _attackTarget.IsInvisible)
                {
                    _attackTarget = null;
                }
                else if (_currNode.MaxRange(_attackTarget.CurrNode) > _currAtt.AttackNodeRange)
                {
                    _attackTarget = null;
                }
            }

            if (_froceAttackTarget != null)
            {
                if (_froceAttackTarget.IsDeadState || _froceAttackTarget.IsUnselectable || _froceAttackTarget.IsUnselectableAttack || _froceAttackTarget.IsInvincible || _froceAttackTarget.IsInvisible)
                {
                    _froceAttackTarget = null;
                }
                else
                {
                    _attackTarget = _froceAttackTarget;
                    _froceAttackTarget = null;

                    if (_currNode.MaxRange(_attackTarget.CurrNode) > _currAtt.AttackNodeRange)
                    {
                        List<BattleNode> movePathNodes = BattleData.FindPath(_currNode, _attackTarget.CurrNode);
                        if (movePathNodes == null || movePathNodes.Count < 2)
                        {
                            _attackTarget = null;
                        }
                    }
                }
            }

            if (_attackTarget == null && _orders.Count == 0 && _currOrder == null && _attackOrders != null && !IsResticted && !IsStunned && !IsAttackDisabled)
            {
                for (int i = 0; i < _attackOrders.Count; i++)
                {
                    BattleData.FindTargets(this, this, _attackOrders[i], _cachedAttackTargets);
                    if (_cachedAttackTargets.Count > 0)
                    {
                        foreach (BaseUnit target in _cachedAttackTargets)
                        {
                            if (_currNode.MaxRange(target.CurrNode) <= _currAtt.AttackNodeRange)
                            {
                                _attackTarget = target;
                                break;
                            }
                            else
                            {
                                List<BattleNode> movePathNodes = BattleData.FindPath(_currNode, target.CurrNode);
                                if (movePathNodes != null && movePathNodes.Count >= 2)
                                {
                                    _attackTarget = target;
                                    break;
                                }
                            }
                        }

                        if (_attackTarget != null)
                        {
                            break;
                        }
                    }
                }
            }
        }

        protected virtual void UpdateAbility(DFix64 frameLength)
        {
            if (IsDeadState)
            {
                return;
            }

            for (int i = 0; i < _allAbilities.Count; i++)
            {
                _allAbilities[i].UpdateLogic(frameLength);

                if (_allAbilities[i].IsAutoCast)
                {
                    _allAbilities[i].AutoCast();
                }
            }
        }

        protected virtual void UpdateAttack(DFix64 frameLength)
        {
            if (IsDeadState)
            {
                return;
            }

            if (_orders.Count != 0 || _currOrder != null)
            {
                return;
            }

            if (_attackTarget != null)
            {
                if (_currNode.MaxRange(_attackTarget.CurrNode) <= _currAtt.AttackNodeRange)
                {
                    AttackTarget(_attackTarget);
                    return;
                }

                List<BattleNode> movePathNodes = BattleData.FindPath(_currNode, _attackTarget.CurrNode);
                if (movePathNodes != null && movePathNodes.Count >= 2)
                {
                    MoveToNode(movePathNodes[0]);
                    return;
                }
            }
        }

        protected virtual void UpdateMana(DFix64 frameLength)
        {
            if (IsDeadState)
            {
                return;
            }

            if (_currAtt.ManaRegen != DFix64.Zero)
            {
                _manaRegenTime += _currAtt.ManaRegen * frameLength;
                if (_manaRegenTime >= DFix64.One)
                {
                    DFix64 finalMana = DFix64.Floor(_manaRegenTime);
                    DFix64 realMana = finalMana;
                    //DFix64 excessMana = DFix64.Zero;

                    _manaRegenTime = _manaRegenTime - finalMana;

                    if (_currAtt.Mana < _currAtt.MaxMana)
                    {
                        if (_currAtt.Mana + finalMana > _currAtt.MaxMana)
                        {
                            realMana = _currAtt.MaxMana - _currAtt.Mana;
                            //excessMana = _currAtt.Mana + finalMana - _currAtt.MaxMana;
                            _manaRegenTime = DFix64.Zero;

                            _currAtt.Mana = _currAtt.MaxMana;
                        }
                        else
                        {
                            _currAtt.Mana = _currAtt.Mana + realMana;
                        }
                    }
                }
                else if (_manaRegenTime <= -DFix64.One)
                {
                    DFix64 finalMana = DFix64.Ceiling(_manaRegenTime);
                    DFix64 realMana = finalMana;

                    _manaRegenTime = _manaRegenTime - finalMana;

                    if (_currAtt.Mana > DFix64.Zero)
                    {
                        if (_currAtt.Mana + finalMana < DFix64.Zero)
                        {
                            realMana = -_currAtt.Mana;
                            _manaRegenTime = DFix64.Zero;

                            _currAtt.Mana = DFix64.Zero;
                        }
                        else
                        {
                            _currAtt.Mana = _currAtt.Mana + realMana;
                        }
                    }
                }
            }
        }

        public void RemoveCustomValue(string key)
        {
            if (_numberValues.ContainsKey(key))
            {
                _numberValues.Remove(key);
            }
        }

        protected override void OnInstantiateSuccess()
        {
            _unitEntityLogic = _battleEntityLogic as BattleUnitEntityLogic;

            if (IsActivated)
            {
                InstantiateModel();
            }

            base.OnInstantiateSuccess();
        }

        public override void SyncRender()
        {
            _lastLogicPosition = _logicPosition;
            _lastLogicAngles = _logicEulerAngles;
            _lastLogicScale = _logicScale;

            _renderPosition = _logicPosition.ToVector3();
            _renderEulerAngles = _logicEulerAngles.ToVector3();
            _renderScale = (_logicScale * _realScale).ToVector3();

            if (_unitEntityLogic != null)
            {
                _unitEntityLogic.CachedTransform.position = _renderPosition;
                _unitEntityLogic.CachedTransform.eulerAngles = _renderEulerAngles;
                _unitEntityLogic.CachedTransform.localScale = _renderScale;
            }
        }

        public override void SyncRenderScale()
        {
            _lastLogicScale = _logicScale;
            _renderScale = (_logicScale * _realScale).ToVector3();

            if (_unitEntityLogic != null)
            {
                _unitEntityLogic.CachedTransform.localScale = _renderScale;
            }
        }

        protected override void OnSetActive(bool isActivated)
        {
            base.OnSetActive(isActivated);

            ShowShadow(isActivated);
        }

        public virtual void SetNode(BattleNode node)
        {
            if (_currNode != null && _currNode.Unit == this)
            {
                _currNode.Unit = null;
            }

            _currNode = node;
            if (_currNode != null)
            {
                _currNode.Unit = this;
            }
        }

        public virtual void ResetToNodePosition()
        {
            if (_currNode != null)
            {
                _logicPosition = _currNode.WorldPosition;
                //SyncRenderPosition();
            }
        }

        public virtual void SpawnToLineup(BattleNode node, bool isShowSpawnTX, bool isInitLineup = false)
        {
            if (IsSpawnedToLineup || node == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (_logUnitAttribute == null)
            {
                GameObject obj = new GameObject(Name);
                _logUnitAttribute = obj.AddComponent<LogUnitAttribute>();
                _logUnitAttribute.Unit = this;
            }
#endif

            IsSpawnedToLineup = true;

            SetNode(node);

            ResetToNodePosition();

            if (TeamId == BattleData.SelfPlayer.TeamId)
            {
                LookToPositionY(LogicPosition + DFixVector3.Forward);
            }
            else
            {
                LookToPositionY(LogicPosition - DFixVector3.Forward);
            }

            SetActive(true);
            PlayAnimation(UnitAnimationType.ACT_IDLE, DFix64.One, false);

            if (isShowSpawnTX)
            {
                PlayModelSpawnEffect();
            }

            if (!isInitLineup)
            {
                BattleUnitSpawnToLineupEventArgs ne = GameFramework.ReferencePool.Acquire<BattleUnitSpawnToLineupEventArgs>();
                ne.Unit = this;
                GameManager.Event.Fire(null, ne);
            }
        }

        public virtual void UnspawnFromLineup()
        {
            if (!IsSpawnedToLineup)
            {
                return;
            }

#if UNITY_EDITOR
            if (_logUnitAttribute != null)
            {
                GameObject.Destroy(_logUnitAttribute.gameObject);
                _logUnitAttribute = null;
            }
#endif

            IsSpawnedToLineup = false;

            if (_currNode == null)
            {
                return;
            }

            SetNode(null);
            SetActive(false);

            BattleUnitUnspawnFromLineupEventArgs ne = GameFramework.ReferencePool.Acquire<BattleUnitUnspawnFromLineupEventArgs>();
            ne.Unit = this;
            GameManager.Event.Fire(null, ne);
        }

        public virtual void Spawn(bool isShowSpawnTX)
        {
            if (IsSpawned)
            {
                return;
            }

            if (CurrNode == null || (CurrNode.Unit != null && CurrNode.Unit != this))
            {
                return;
            }

#if UNITY_EDITOR
            if (_logUnitAttribute == null)
            {
                GameObject obj = new GameObject(Name);
                _logUnitAttribute = obj.AddComponent<LogUnitAttribute>();
                _logUnitAttribute.Unit = this;
            }
#endif
            if (Type != UnitType.UNIT_THINKER && !IsIllusion)
            {
                _player.TeamTotalNumber += 1;
            }

            IsSpawned = true;
            _lifeIndex = 0;

            if (IsSpawnedToLineup)
            {
                PlayAnimation(UnitAnimationType.ACT_IDLE, DFix64.One, false);

                BattleUnitSpawnEventArgs ne = GameFramework.ReferencePool.Acquire<BattleUnitSpawnEventArgs>();
                ne.Unit = this;
                GameManager.Event.Fire(null, ne);

                int lifeIndex = _lifeIndex;
                for (int i = 0; i < _allAbilities.Count; i++)
                {
                    _allAbilities[i].StartFirstCooldown();
                    _allAbilities[i].Enable();
                    if (IsDeadState || lifeIndex != _lifeIndex)
                    {
                        return;
                    }

                    _allAbilities[i].OnOwnerSpawned();
                    if (IsDeadState || lifeIndex != _lifeIndex)
                    {
                        return;
                    }
                }
            }
            else
            {
                IsSpawnedToLineup = true;

                SetNode(CurrNode);
                ResetToNodePosition();
                if (TeamId == BattleData.SelfPlayer.TeamId)
                {
                    LookToPositionY(LogicPosition + DFixVector3.Forward);
                }
                else
                {
                    LookToPositionY(LogicPosition - DFixVector3.Forward);
                }
                SetActive(true);
                PlayAnimation(UnitAnimationType.ACT_IDLE, DFix64.One, false);

                BattleUnitSpawnEventArgs ne = GameFramework.ReferencePool.Acquire<BattleUnitSpawnEventArgs>();
                ne.Unit = this;
                GameManager.Event.Fire(null, ne);

                int lifeIndex = _lifeIndex;
                for (int i = 0; i < _allAbilities.Count; i++)
                {
                    _allAbilities[i].StartFirstCooldown();
                    _allAbilities[i].Enable();
                    if (IsDeadState || lifeIndex != _lifeIndex)
                    {
                        return;
                    }

                    _allAbilities[i].OnOwnerSpawned();
                    if (IsDeadState || lifeIndex != _lifeIndex)
                    {
                        return;
                    }
                }

                if (IsDeadState || lifeIndex != _lifeIndex)
                {
                    return;
                }

                if (isShowSpawnTX && !IsInvisible)
                {
                    PlayModelSpawnEffect();
                }
            }
        }

        public virtual void Kill(BaseUnit killer, Ability killerAbility)
        {
            if (_isDeadState)
            {
                return;
            }

            if (!_isSpawned)
            {
                return;
            }

            Log.Info("{0} 死亡", LogName);

            int lifeIndex = _lifeIndex;

            _isDeadState = true;
            _isHideState = false;
            _isHideState2 = false;
            _delayHideTime = DFix64.Zero;
            _deadNode = _currNode;

            if (Type != UnitType.UNIT_THINKER && !IsIllusion)
            {
                _player.TeamTotalNumber -= 1;
                _player.TeamDeathNumber += 1;
            }

            StopOrder();

            if (!_isDeadState || lifeIndex != _lifeIndex)
            {
                return;
            }

            RemoveAllModifersOnDeath();

            if (!_isDeadState || lifeIndex != _lifeIndex)
            {
                return;
            }

            for (int i = 0; i < _allAbilities.Count; i++)
            {
                _allAbilities[i].Disable();
                if (!_isDeadState || lifeIndex != _lifeIndex)
                {
                    return;
                }

                _allAbilities[i].OnOwnerDied();
                if (!_isDeadState || lifeIndex != _lifeIndex)
                {
                    return;
                }
            }

            ShowShadow(false);
            PlayAnimation(UnitAnimationType.ACT_DEAD, DFix64.One, true);

            ApplyModifier(null, null, Constant.Battle.BUILTIN_MODIFIER_DEATH, null, false);

            BattleUnitDeathEventArgs ne = GameFramework.ReferencePool.Acquire<BattleUnitDeathEventArgs>();
            ne.Unit = this;
            GameManager.Event.Fire(null, ne);
        }

        public virtual void SetHideState(bool isHideNow)
        {
            if (!_isDeadState)
            {
                return;
            }

            if (_isHideState)
            {
                return;
            }

            _isHideState = true;
            _isHideState2 = false;
            PlayModelDeadEffect();

            if (_currNode != null && _currNode.Unit == this)
            {
                _currNode.Unit = null;
            }

            if (isHideNow)
            {
                _isHideState2 = true;

                StopAnimation();
                Destroy();
            }
        }

        public virtual void Respawn(DFix64 hp, BattleNode node, BaseUnit source, Ability sourceAbility)
        {
            if (!_isSpawned)
            {
                return;
            }

            if (!_isDeadState)
            {
                return;
            }

            _isDeadState = false;
            _isHideState = false;
            _isHideState2 = false;
            _delayHideTime = DFix64.Zero;
            _deadNode = null;
            _lifeIndex++;
            if (hp > _initAtt.MaxHp)
            {
                _currAtt.Hp = _initAtt.MaxHp;
            }
            else
            {
                _currAtt.Hp = hp;
            }

            if (Type != UnitType.UNIT_THINKER && !IsIllusion)
            {
                _player.TeamTotalNumber += 1;
            }

            RemoveModifier(Constant.Battle.BUILTIN_MODIFIER_DEATH);

            Log.Info("{0} 复活, 次数:{1}, 当前生命值 {2}/{3}", LogName, (_lifeIndex + 1).ToString(), hp.ToString(), _currAtt.MaxHp.ToString());

            int lifeIndex = _lifeIndex;

            SetNode(node);
            ResetToNodePosition();
            SetActive(true);
            PlayAnimation(UnitAnimationType.ACT_IDLE, DFix64.One, false);

            for (int i = 0; i < _allAbilities.Count; i++)
            {
                _allAbilities[i].Enable();

                if (_isDeadState || lifeIndex != _lifeIndex)
                {
                    break;
                }

                _allAbilities[i].StartCooldown();
                if (_isDeadState || lifeIndex != _lifeIndex)
                {
                    break;
                }
            }
        }


        #region 技能

        public int AbilityCount
        {
            get
            {
                return _allAbilities.Count;
            }
        }

        public virtual Ability GetAbility(int index)
        {
            return _allAbilities[index];
        }
        public virtual List<Ability> FindAbilities(Predicate<Ability> condition)
        {
            return _allAbilities.FindAll(condition);
        }
        public virtual List<Ability> FindAbilities(Predicate<Ability> condition, Comparison<Ability> comparison)
        {
            List<Ability> results = _allAbilities.FindAll(condition);
            results.Sort(comparison);
            return results;
        }
        public virtual Ability FindAbilityByAbilityId(int abilityId)
        {
            for (int i = 0; i < _allAbilities.Count; i++)
            {
                if (_allAbilities[i].AbilityId == abilityId)
                {
                    return _allAbilities[i];
                }
            }

            return null;
        }

        public virtual void ChangAbilityCooldown(int abilityId, DFix64 timeBouns)
        {
            for (int i = 0; i < _allAbilities.Count; i++)
            {
                if (_allAbilities[i].AbilityId == abilityId)
                {
                    Log.Info("ChangeAbilityCooldown: {0} 的技能 {1} 冷却时间增加 {2}", LogName, _allAbilities[i].LogName, timeBouns.ToString());
                    _allAbilities[i].AddCooldown(timeBouns);

                    break;
                }
            }
        }
        public virtual void ChangeAllAbilityCooldown(DFix64 timeBouns)
        {
            for (int i = 0; i < _allAbilities.Count; i++)
            {
                Log.Info("ChangeAbilityCooldown: {0} 的技能 {1} 冷却时间增加 {2}", LogName, _allAbilities[i].LogName, timeBouns.ToString());
                _allAbilities[i].AddCooldown(timeBouns);
            }
        }

        public void EnableAllAbilities()
        {
            for (int i = 0; i < _allAbilities.Count; i++)
            {
                _allAbilities[i].Enable();
            }
        }
        public void DisableAllAbilities()
        {
            for (int i = 0; i < _allAbilities.Count; i++)
            {
                _allAbilities[i].Disable();
            }
        }

        private void AddAbility(int abilityId)
        {
            if (abilityId == 0)
            {
                return;
            }

            if (BattleKvLibraryManager.HasAbilityKv(abilityId))
            {
                KeyValue kvAbility = BattleKvLibraryManager.GetAbilityKv(abilityId);

                Ability ability = new Ability(abilityId, kvAbility, this, _allAbilities.Count);
                _allAbilities.Add(ability);

                if (!ability.IsPassive && ability.IsAutoCast)
                {
                    ability.IsAutoCastState = true;
                }
            }
            else
            {
                //Ability ability = new Ability(abilityId, null, this, _allAbilities.Count);
                //_allAbilities.Add(ability);

                Log.Error("AddAbility 找不到技能, id:{0}", abilityId.ToString());
            }

#if UNITY_EDITOR
            if (_logUnitAttribute != null)
            {
                _logUnitAttribute.Unit = this;
            }
#endif
        }

        private void ReleaseAbilities()
        {
            for (int i = 0; i < _allAbilities.Count; i++)
            {
                _allAbilities[i].Release();
            }
            _allAbilities.Clear();
        }


        #endregion

        #region Modifier
        protected bool _isModiferChanged = false;
        protected LinkedList<Modifier> _allModifiers = new LinkedList<Modifier>();                                              // Modifier列表
        protected HashSet<Modifier> _allModifiersValue = new HashSet<Modifier>();                                               // Modifier列表
        protected Dictionary<int, LinkedList<Modifier>> _allModifiersBySerialId = new Dictionary<int, LinkedList<Modifier>>();  // Modifier列表, id
        protected Dictionary<int, LinkedList<Modifier>> _modifiersByGroup = new Dictionary<int, LinkedList<Modifier>>();  // Modifier列表, 组

        protected Dictionary<int, LinkedList<Modifier>> _modifierStatePriorities = new Dictionary<int, LinkedList<Modifier>>(); // Modifier状态列表, 优先级从低到高
        protected Dictionary<int, int> _immuneModifierGroupCounts = new Dictionary<int, int>();                           // 免疫的Modifier组
        protected HashSet<Modifier> _purgableBuffModifiers = new HashSet<Modifier>();                                           // 可驱散的Buff
        protected HashSet<Modifier> _purgableDebuffModifiers = new HashSet<Modifier>();                                         // 可驱散的Debuff
        protected HashSet<Modifier> _purgableNebuffModifiers = new HashSet<Modifier>();                                         // 可驱散的中立Buff


        protected void ReleaseModifiers()
        {
            LinkedListNode<Modifier> first = _allModifiers.First;
            while (first != null)
            {
                Modifier modifier = first.Value;
                first = first.Next;
                modifier.Remove();
            }
            _allModifiers.Clear();
            _allModifiersValue.Clear();
            _allModifiersBySerialId.Clear();
            _modifiersByGroup.Clear();
            _modifierStatePriorities.Clear();
            _immuneModifierGroupCounts.Clear();
            _purgableBuffModifiers.Clear();
            _purgableDebuffModifiers.Clear();
            _purgableNebuffModifiers.Clear();
        }

        protected void UpdateModifiers(DFix64 frameLength)
        {
            if (_isModiferChanged)
            {
                _isModiferChanged = false;
            }

            LinkedListNode<Modifier> cache = null;
            LinkedListNode<Modifier> first = _allModifiers.First;
            while (first != null && first.Value.CreatedFrame < BattleData.LogicFrame)
            {
                //if (first.Value.IsRemoved)
                //{
                //    first = first.Next;
                //    _allModifiers2.Remove(first.Previous);
                //    continue;
                //}

                if (!first.Value.IsRemoved)
                {
                    first.Value.UpdateLogic(frameLength);
                }
                else
                {
                    cache = first;
                }

                first = first.Next;
            }

            if (cache != null)
            {
                first = cache;
                while (first != null)
                {
                    if (first.Value.IsRemoved)
                    {
                        cache = first;
                        first = first.Previous;
                        _allModifiers.Remove(cache);
                    }
                    else
                    {
                        first = first.Previous;
                    }
                }
            }
        }

        protected void RemoveAllModifersOnDeath()
        {
            List<Modifier> removeModifiers = new List<Modifier>();
            LinkedListNode<Modifier> first = _allModifiers.First;
            while (first != null)
            {
                if (!first.Value.IsRemoveOnDeath || first.Value.IsPassive)
                {
                    first = first.Next;
                    continue;
                }

                removeModifiers.Add(first.Value);
                first = first.Next;
            }

            for (int i = 0; i < removeModifiers.Count; i++)
            {
                _RemoveModifier(removeModifiers[i]);
            }
        }

        protected LinkedList<Modifier> GetModifiersByGroup(int modifierGroup)
        {
            if (modifierGroup == 0)
            {
                return null;
            }

            if (!_modifiersByGroup.ContainsKey(modifierGroup))
            {
                return null;
            }

            return _modifiersByGroup[modifierGroup];
        }

        public int GetModifierGroupCount(int modifierGroup)
        {
            if (modifierGroup == 0)
            {
                return 0;
            }

            if (!_modifiersByGroup.ContainsKey(modifierGroup))
            {
                return 0;
            }

            return _modifiersByGroup[modifierGroup].Count;
        }
        protected void AddModifierGroupCount(Modifier modifier)
        {
            if (modifier.Group == 0)
            {
                return;
            }

            if (_modifiersByGroup.ContainsKey(modifier.Group))
            {
                _modifiersByGroup[modifier.Group].AddLast(modifier);
            }
            else
            {
                LinkedList<Modifier> modifiers = new LinkedList<Modifier>();
                modifiers.AddLast(modifier);
                _modifiersByGroup.Add(modifier.Group, modifiers);
            }
        }
        protected void RemoveModifierGroupCount(Modifier modifier)
        {
            if (modifier.Group == 0)
            {
                return;
            }

            if (_modifiersByGroup.ContainsKey(modifier.Group))
            {
                _modifiersByGroup[modifier.Group].Remove(modifier);
            }
        }

        public bool IsImmuneModifierGroup(int modifierGroup)
        {
            return _immuneModifierGroupCounts.ContainsKey(modifierGroup) && _immuneModifierGroupCounts[modifierGroup] > 0;
        }
        protected void AddImmuneModifier(int modifierGroup)
        {
            if (_immuneModifierGroupCounts.ContainsKey(modifierGroup))
            {
                _immuneModifierGroupCounts[modifierGroup] += 1;
            }
            else
            {
                _immuneModifierGroupCounts.Add(modifierGroup, 1);
            }
        }
        protected void RemoveImmuneModifer(int modifierGroup)
        {
            if (_immuneModifierGroupCounts.ContainsKey(modifierGroup))
            {
                _immuneModifierGroupCounts[modifierGroup] = Mathf.Min(_immuneModifierGroupCounts[modifierGroup] - 1, 0);
            }
        }

        public int GetPurgableBuffCount()
        {
            return _purgableBuffModifiers.Count;
        }
        protected void AddPurgableBuff(Modifier modifier)
        {
            _purgableBuffModifiers.Add(modifier);
        }
        protected void RemovePurgableBuff(Modifier modifier)
        {
            _purgableBuffModifiers.Remove(modifier);
        }

        public int GetPurgableDebuffCount()
        {
            return _purgableDebuffModifiers.Count;
        }
        protected void AddPurgableDebuff(Modifier modifier)
        {
            _purgableDebuffModifiers.Add(modifier);
        }
        protected void RemovePurgableDebuff(Modifier modifier)
        {
            _purgableDebuffModifiers.Remove(modifier);
        }

        public int GetPurgableNebuffCount()
        {
            return _purgableNebuffModifiers.Count;
        }
        protected void AddPurgableNebuff(Modifier modifier)
        {
            _purgableNebuffModifiers.Add(modifier);
        }
        protected void RemovePurgableNebuff(Modifier modifier)
        {
            _purgableNebuffModifiers.Remove(modifier);
        }

        public Modifier ApplyModifier(BaseUnit caster, Ability ability, int modifierSerialId, CreateModifierData createData, bool isCheck)
        {
            Modifier newModifier = null;
            if (ability != null)
            {
                newModifier = ability.ApplyModifier(caster, this, modifierSerialId, createData);
            }
            else
            {
                newModifier = BattleKvLibraryManager.ApplyBuiltinModifier(caster, this, modifierSerialId, createData);
            }

            return AddModifier(newModifier, isCheck);
        }
        public Modifier AddModifier(Modifier newModifier, bool isCheck)
        {
            if (newModifier == null)
            {
                return null;
            }

            if (isCheck)
            {
                if (newModifier.IsBuff && (IsBuffImmune || IsAllModifierImmune))
                {
                    BattleModifierImmuneEventArgs ne = GameFramework.ReferencePool.Acquire<BattleModifierImmuneEventArgs>();
                    ne.Target = this;
                    ne.Modifier = newModifier;
                    BattleData.FireEvent(ne);

                    newModifier.Remove();

                    return null;
                }
                else if (newModifier.IsDebuff && (IsDebuffImmune || IsAllModifierImmune))
                {
                    BattleModifierImmuneEventArgs ne = GameFramework.ReferencePool.Acquire<BattleModifierImmuneEventArgs>();
                    ne.Target = this;
                    ne.Modifier = newModifier;
                    BattleData.FireEvent(ne);

                    newModifier.Remove();

                    return null;
                }
                else if (newModifier.IsNebuff && (IsNebuffImmune || IsAllModifierImmune))
                {
                    BattleModifierImmuneEventArgs ne = GameFramework.ReferencePool.Acquire<BattleModifierImmuneEventArgs>();
                    ne.Target = this;
                    ne.Modifier = newModifier;
                    BattleData.FireEvent(ne);

                    newModifier.Remove();

                    return null;
                }

                if (newModifier.Group != 0)
                {
                    if (IsImmuneModifierGroup(newModifier.Group))
                    {
                        BattleModifierImmuneEventArgs ne = GameFramework.ReferencePool.Acquire<BattleModifierImmuneEventArgs>();
                        ne.Target = this;
                        ne.Modifier = newModifier;
                        BattleData.FireEvent(ne);

                        newModifier.Remove();

                        return null;
                    }

                    switch (newModifier.GroupStackType)
                    {
                        case ModifierGroupStackType.MODIFIER_GROUP_STACK_IGNORE:
                            {

                            }
                            break;
                        case ModifierGroupStackType.MODIFIER_GROUP_STACK_REFRESH_OTHER_DURATION:
                            {
                                LinkedList<Modifier> sameGroupModifiers = GetModifiersByGroup(newModifier.Group);
                                if (sameGroupModifiers != null && sameGroupModifiers.Count > 0)
                                {
                                    LinkedListNode<Modifier> first = sameGroupModifiers.First;
                                    while (first != null)
                                    {
                                        first.Value.RefreshDuration();
                                        first = first.Next;
                                    }
                                }
                            }
                            break;
                        case ModifierGroupStackType.MODIFIER_GROUP_STACK_CLEAR_OTHER:
                            {
                                LinkedList<Modifier> sameGroupModifiers = GetModifiersByGroup(newModifier.Group);
                                if (sameGroupModifiers != null && sameGroupModifiers.Count > 0)
                                {
                                    List<Modifier> removeModifiers = new List<Modifier>();

                                    LinkedListNode<Modifier> first = sameGroupModifiers.First;
                                    while (first != null)
                                    {
                                        removeModifiers.Add(first.Value);
                                        first = first.Next;
                                    }

                                    for (int i = 0; i < removeModifiers.Count; i++)
                                    {
                                        _RemoveModifier(removeModifiers[i]);
                                    }
                                }
                            }
                            break;
                        case ModifierGroupStackType.MODIFIER_GROUP_STACK_ABSORB_OTHER_DURATION_AND_CLEAR:
                            {
                                LinkedList<Modifier> sameGroupModifiers = GetModifiersByGroup(newModifier.Group);
                                if (sameGroupModifiers != null && sameGroupModifiers.Count > 0)
                                {
                                    DFix64 duration = newModifier.Duration;
                                    List<Modifier> removeModifiers = new List<Modifier>();

                                    LinkedListNode<Modifier> first = sameGroupModifiers.First;
                                    while (first != null)
                                    {
                                        if (first.Value.Duration != -DFix64.One)
                                        {
                                            duration += first.Value.RemainingTime;
                                        }

                                        removeModifiers.Add(first.Value);
                                        first = first.Next;
                                    }

                                    newModifier.RefreshDuration(duration);

                                    for (int i = 0; i < removeModifiers.Count; i++)
                                    {
                                        _RemoveModifier(removeModifiers[i]);
                                    }
                                }
                            }
                            break;
                        default:
                            {

                            }
                            break;
                    }

                    if (newModifier.IsBuff && (IsBuffImmune || IsAllModifierImmune))
                    {
                        BattleModifierImmuneEventArgs ne = GameFramework.ReferencePool.Acquire<BattleModifierImmuneEventArgs>();
                        ne.Target = this;
                        ne.Modifier = newModifier;
                        BattleData.FireEvent(ne);

                        newModifier.Remove();

                        return null;
                    }
                    else if (newModifier.IsDebuff && (IsDebuffImmune || IsAllModifierImmune))
                    {
                        BattleModifierImmuneEventArgs ne = GameFramework.ReferencePool.Acquire<BattleModifierImmuneEventArgs>();
                        ne.Target = this;
                        ne.Modifier = newModifier;
                        BattleData.FireEvent(ne);

                        newModifier.Remove();

                        return null;
                    }
                    else if (newModifier.IsNebuff && (IsNebuffImmune || IsAllModifierImmune))
                    {
                        BattleModifierImmuneEventArgs ne = GameFramework.ReferencePool.Acquire<BattleModifierImmuneEventArgs>();
                        ne.Target = this;
                        ne.Modifier = newModifier;
                        BattleData.FireEvent(ne);

                        newModifier.Remove();

                        return null;
                    }

                    if (IsImmuneModifierGroup(newModifier.Group))
                    {
                        BattleModifierImmuneEventArgs ne = GameFramework.ReferencePool.Acquire<BattleModifierImmuneEventArgs>();
                        ne.Target = this;
                        ne.Modifier = newModifier;
                        BattleData.FireEvent(ne);

                        newModifier.Remove();

                        return null;
                    }

                    if (newModifier.GroupStackCount != -1 && GetModifierGroupCount(newModifier.Group) >= newModifier.GroupStackCount)
                    {
                        newModifier.Remove();

                        return null;
                    }

                    _AddModifier(newModifier);

                }
                else
                {
                    _AddModifier(newModifier);
                }
            }
            else
            {
                _AddModifier(newModifier);
            }

            if (!newModifier.IsRemoved)
            {
                return newModifier;
            }

            newModifier.Remove();

            return null;
        }
        protected void _AddModifier(Modifier modifier)
        {
            if (_allModifiersValue.Contains(modifier))
            {
                Log.Error("重复添加Modifier");

                modifier.Remove();

                return;
            }

            _allModifiersValue.Add(modifier);

            if (modifier.Priority != -1)
            {
                _allModifiers.AddLast(modifier);
            }
            else
            {
                LinkedListNode<Modifier> first = _allModifiers.First;
                while (first != null)
                {
                    if (first.Value.Priority == -1 || modifier.Priority > first.Value.Priority)
                    {
                        _allModifiers.AddBefore(first, modifier);
                        break;
                    }

                    first = first.Next;
                }

                if (first == null)
                {
                    _allModifiers.AddLast(modifier);
                }
            }

            if (!_allModifiersBySerialId.ContainsKey(modifier.SerialId))
            {
                LinkedList<Modifier> modifiersById = new LinkedList<Modifier>();
                modifiersById.AddLast(modifier);

                _allModifiersBySerialId.Add(modifier.SerialId, modifiersById);
            }
            else
            {
                _allModifiersBySerialId[modifier.SerialId].AddLast(modifier);
            }

            AddModifierGroupCount(modifier);

            if (modifier.ImmuneModifierGroups != null)
            {
                for (int i = 0; i < modifier.ImmuneModifierGroups.Length; i++)
                {
                    AddImmuneModifier(modifier.ImmuneModifierGroups[i]);
                }
            }

            if (modifier.IsPurgable)
            {
                if (modifier.IsBuff)
                {
                    AddPurgableBuff(modifier);
                }
                if (modifier.IsDebuff)
                {
                    AddPurgableDebuff(modifier);
                }
                if (modifier.IsNebuff)
                {
                    AddPurgableNebuff(modifier);
                }
            }

            Dictionary<int, DFix64> modifierProperties = modifier.Properties;
            if (modifierProperties != null)
            {
                foreach (KeyValuePair<int, DFix64> modifierProperty in modifierProperties)
                {
                    AddModifierPropertyValue((ModifierPropertyType)modifierProperty.Key, modifierProperty.Value);
                }
            }

            Dictionary<int, ModifierStateValue> modifierStates = modifier.ModifierStates;
            if (modifierStates != null)
            {
                Dictionary<int, ModifierStateValue> changedModifierStates = new Dictionary<int, ModifierStateValue>();
                foreach (KeyValuePair<int, ModifierStateValue> modifierState in modifierStates)
                {
                    LinkedList<Modifier> modifiers = null;
                    if (!_modifierStatePriorities.ContainsKey(modifierState.Key))
                    {
                        modifiers = new LinkedList<Modifier>();
                        modifiers.AddLast(modifier);
                        _modifierStatePriorities.Add(modifierState.Key, modifiers);

                        SetModifierState(modifierState.Key, modifierState.Value);
                        changedModifierStates.Add(modifierState.Key, modifierState.Value);
                    }
                    else
                    {
                        modifiers = _modifierStatePriorities[modifierState.Key];
                        LinkedListNode<Modifier> last = modifiers.Last;
                        while (last != null)
                        {
                            if (last.Value.Priority < modifier.Priority)
                            {
                                modifiers.AddAfter(last, modifier);

                                if (modifier == modifiers.Last.Value)
                                {
                                    SetModifierState(modifierState.Key, modifierState.Value);
                                    changedModifierStates.Add(modifierState.Key, modifierState.Value);
                                }

                                break;
                            }

                            last = last.Previous;
                        }

                        if (last == null)
                        {
                            modifiers.AddLast(modifier);
                            SetModifierState(modifierState.Key, modifierState.Value);
                            changedModifierStates.Add(modifierState.Key, modifierState.Value);
                        }
                    }
                }

                ChangeModifierStates(changedModifierStates);
            }

            modifier.OnCreated();
        }

        public void RemoveModifier(Ability ability, int serialId, int number = -1)
        {
            if (ability == null)
            {
                RemoveModifier(serialId, number);
            }
            else
            {
                if (!_allModifiersBySerialId.ContainsKey(serialId))
                {
                    return;
                }

                if (number > 1)
                {
                    List<Modifier> removedModifiers = new List<Modifier>();
                    LinkedList<Modifier> modifiers = _allModifiersBySerialId[serialId];
                    LinkedListNode<Modifier> cache = null;
                    LinkedListNode<Modifier> first = modifiers.First;
                    while (first != null && number > 0)
                    {
                        if (first.Value.Ability == ability)
                        {
                            removedModifiers.Add(first.Value);
                            cache = first;
                            first = first.Next;
                            modifiers.Remove(cache);
                            number--;
                        }
                        else
                        {
                            first = first.Next;
                        }
                    }
                }
                else if (number == 1)
                {
                    LinkedListNode<Modifier> first = _allModifiersBySerialId[serialId].First;
                    if (first != null)
                    {
                        Modifier modifier = first.Value;
                        _allModifiersBySerialId[serialId].RemoveFirst();
                        _RemoveModifier(modifier);
                    }
                }
                else
                {
                    List<Modifier> removedModifiers = new List<Modifier>();
                    LinkedList<Modifier> modifiers = _allModifiersBySerialId[serialId];
                    LinkedListNode<Modifier> cache = null;
                    LinkedListNode<Modifier> first = modifiers.First;
                    while (first != null)
                    {
                        if (first.Value.Ability == ability)
                        {
                            removedModifiers.Add(first.Value);
                            cache = first;
                            first = first.Next;
                            modifiers.Remove(cache);
                        }
                        else
                        {
                            first = first.Next;
                        }
                    }

                    for (int i = 0; i < removedModifiers.Count; i++)
                    {
                        _RemoveModifier(removedModifiers[i]);
                    }
                }
            }
        }
        public void RemoveModifier(int serialId, int number = -1)
        {
            if (!_allModifiersBySerialId.ContainsKey(serialId))
            {
                return;
            }

            if (number > 1)
            {
                List<Modifier> removedModifiers = new List<Modifier>();
                LinkedList<Modifier> modifiers = _allModifiersBySerialId[serialId];
                LinkedListNode<Modifier> first = modifiers.First;
                while (first != null && number > 0)
                {
                    if (!first.Value.IsPassive)
                    {
                        removedModifiers.Add(first.Value);
                        first = first.Next;
                        modifiers.RemoveFirst();
                        number--;
                    }
                    else
                    {
                        first = first.Next;
                    }
                }

                for (int i = 0; i < removedModifiers.Count; i++)
                {
                    _RemoveModifier(removedModifiers[i]);
                }
            }
            else if (number == 1)
            {
                LinkedListNode<Modifier> first = _allModifiersBySerialId[serialId].First;
                while (first != null)
                {
                    if (!first.Value.IsPassive)
                    {
                        Modifier modifier = first.Value;
                        _allModifiersBySerialId[serialId].Remove(first);
                        _RemoveModifier(modifier);
                        break;
                    }
                }
            }
            else
            {
                LinkedList<Modifier> modifiers = _allModifiersBySerialId[serialId];
                _allModifiersBySerialId.Remove(serialId);

                LinkedListNode<Modifier> first = modifiers.First;
                while (first != null)
                {
                    if (!first.Value.IsPassive)
                    {
                        _RemoveModifier(first.Value, false);
                    }

                    first = first.Next;
                }
            }
        }
        public void RemoveModifier(Modifier modifier)
        {
            _RemoveModifier(modifier);
        }
        protected void _RemoveModifier(Modifier modifier, bool isClearId = true, bool isPurge = false)
        {
            if (!_allModifiersValue.Contains(modifier))
            {
                //Log.Error("删除不存在的Modifier");
                return;
            }

            _allModifiersValue.Remove(modifier);

            if (isClearId && _allModifiersBySerialId.ContainsKey(modifier.SerialId))
            {
                LinkedList<Modifier> modifiersById = _allModifiersBySerialId[modifier.SerialId];
                modifiersById.Remove(modifier);
            }

            //LinkedListNode<Modifier> find = _allModifiers.Find(modifier);
            //if (find != null)
            {
                //_allModifiers.Remove(find);

                RemoveModifierGroupCount(modifier);

                if (modifier.ImmuneModifierGroups != null)
                {
                    for (int i = 0; i < modifier.ImmuneModifierGroups.Length; i++)
                    {
                        RemoveImmuneModifer(modifier.ImmuneModifierGroups[i]);
                    }
                }

                if (modifier.IsPurgable)
                {
                    if (modifier.IsBuff)
                    {
                        RemovePurgableBuff(modifier);
                    }
                    if (modifier.IsDebuff)
                    {
                        RemovePurgableDebuff(modifier);
                    }
                    if (modifier.IsNebuff)
                    {
                        RemovePurgableNebuff(modifier);
                    }
                }

                Dictionary<int, DFix64> modifierProperties = modifier.Properties;
                if (modifierProperties != null)
                {
                    foreach (KeyValuePair<int, DFix64> modifierProperty in modifierProperties)
                    {
                        RemoveModifierPropertyValue((ModifierPropertyType)modifierProperty.Key, modifierProperty.Value);
                    }
                }

                Dictionary<int, ModifierStateValue> modifierStates = modifier.ModifierStates;
                if (modifierStates != null)
                {
                    Dictionary<int, ModifierStateValue> changedModifierStates = new Dictionary<int, ModifierStateValue>();
                    foreach (KeyValuePair<int, ModifierStateValue> modifierState in modifierStates)
                    {
                        LinkedList<Modifier> modifiers = null;
                        if (_modifierStatePriorities.ContainsKey(modifierState.Key))
                        {
                            modifiers = _modifierStatePriorities[modifierState.Key];

                            if (modifiers.Last.Value == modifier)
                            {
                                LinkedListNode<Modifier> findState = modifiers.Last;
                                if (findState.Previous != null)
                                {
                                    ModifierStateValue stateValue = findState.Previous.Value.GetStateValue(modifierState.Key);
                                    SetModifierState(modifierState.Key, stateValue);
                                    changedModifierStates.Add(modifierState.Key, stateValue);
                                }
                                else
                                {
                                    SetModifierState(modifierState.Key, ModifierStateValue.MODIFIER_STATE_VALUE_NO_ACTION);
                                    changedModifierStates.Add(modifierState.Key, ModifierStateValue.MODIFIER_STATE_VALUE_NO_ACTION);
                                }
                            }

                            modifiers.Remove(modifier);
                        }
                    }

                    modifier.Remove();
                    ChangeModifierStates(changedModifierStates);
                    modifier.OnDestroy(isPurge);
                }
                else
                {
                    modifier.Remove();
                    modifier.OnDestroy(isPurge);
                }
            }
        }

        public void PurgeModifier(int modifierGroup, bool isPurgeBuff, bool isPurgeDebuff, bool isPurgeNebuff, int purgableLevel, int maxCount)
        {
            List<Modifier> removedModifiers = new List<Modifier>();

            if (isPurgeBuff)
            {
                foreach (Modifier modifier in _purgableBuffModifiers)
                {
                    if (modifier.IsPassive)
                    {
                        continue;
                    }

                    if (modifierGroup != 0 && modifier.Group != 0 && modifier.Group != modifierGroup)
                    {
                        continue;
                    }

                    if (purgableLevel != -1 && modifier.PurgableLevel != -1 && modifier.PurgableLevel > purgableLevel)
                    {
                        continue;
                    }

                    removedModifiers.Add(modifier);

                    if (maxCount != -1 && removedModifiers.Count >= maxCount)
                    {
                        break;
                    }
                }
            }

            if (isPurgeDebuff)
            {
                foreach (Modifier modifier in _purgableDebuffModifiers)
                {
                    if (modifierGroup != 0 && modifier.Group != 0 && modifier.Group != modifierGroup)
                    {
                        continue;
                    }

                    if (purgableLevel != -1 && modifier.PurgableLevel != -1 && modifier.PurgableLevel > purgableLevel)
                    {
                        continue;
                    }

                    removedModifiers.Add(modifier);

                    if (maxCount != -1 && removedModifiers.Count >= maxCount)
                    {
                        break;
                    }
                }
            }

            if (isPurgeNebuff)
            {
                foreach (Modifier modifier in _purgableNebuffModifiers)
                {
                    if (modifierGroup != 0 && modifier.Group != 0 && modifier.Group != modifierGroup)
                    {
                        continue;
                    }

                    if (purgableLevel != -1 && modifier.PurgableLevel != -1 && modifier.PurgableLevel > purgableLevel)
                    {
                        continue;
                    }

                    removedModifiers.Add(modifier);

                    if (maxCount != -1 && removedModifiers.Count >= maxCount)
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < removedModifiers.Count; i++)
            {
                _RemoveModifier(removedModifiers[i], true, true);
            }
        }

        protected virtual void ChangeModifierProperty(ModifierPropertyType modifierPropertyType, DFix64 value, BattleUnitAttribute att)
        {
            switch (modifierPropertyType)
            {
                case ModifierPropertyType.MODIFIER_PROPERTY_ATTACK_BONUS: { att.Attack += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_DEFENSE_BONUS: { att.Defense += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_DEFENSE_RATIO_BONUS: { att.DefenseRatio += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_ATTACK_SPEED_BONUS: { att.AttackSpeed += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_MOVE_SPEED_BONUS: { att.MoveSpeed += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_CRIT_DAMAGE_BONUS: { att.CritDamage += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_CRIT_RESISTANCE_BONUS: { att.CritResistance += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_CRIT_ODDS_BONUS: { att.CritOdds += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_DAMAGE_OUT_BONUS: { att.DamageOutBonus += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_DAMAGE_IN_BONUS: { att.DamageInBonus += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_DODGE_ODDS_BONUS: { att.DodgeOdds += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_HIT_ODDS_BONUS: { att.HitOdds += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_ABILITY_OUT_BONUS: { att.AbilityDamageOutBonus += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_HEAL_OUT_BONUS: { att.HealOutBonus += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_PHYSICAL_ARMOR_BREAK_BONUS: { att.PhysicalArmorBreak += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_LIFE_STEAL_BONUS: { att.HpSteal += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_ATTACK_OUT_BONUS: { att.AttackDamageOutBonus += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_ATTACK_IN_BONUS: { att.AttackDamageInBonus += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_ABILITY_IN_BONUS: { att.AbilityDamageInBonus += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_HEAL_IN_BONUS: { att.HealInBonus += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_FINAL_DAMAGE_OUT_BONUS: { att.FinalDamageOutBouns += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_FINAL_DAMAGE_IN_BONUS: { att.FinalDamageInBouns += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_COOLDOWN_BONUS: { att.AbilityCDBonus += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_MAXHP_BONUS: { att.MaxHp += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_IS_ILLUSION: { att.IsIllusion += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_IS_SUMMONED: { att.IsSummoned += value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_ATTACKRANGE_BONUS: { att.AttackNodeRange += (int)value; } break;
                case ModifierPropertyType.MODIFIER_PROPERTY_MODEL_RADII_BONUS: { att.Scale += value; _realScale = DFix64.Max(DFix64.Zero, att.Scale * _modelScale); } break;
                default: { } break;
            }
        }
        protected void AddModifierPropertyValue(ModifierPropertyType modifierPropertyType, DFix64 value)
        {
            ChangeModifierProperty(modifierPropertyType, value, _currAtt);
        }
        protected void RemoveModifierPropertyValue(ModifierPropertyType modifierPropertyType, DFix64 value)
        {
            ChangeModifierProperty(modifierPropertyType, -value, _currAtt);
        }

        protected void SetModifierState(int stateType, ModifierStateValue stateValue)
        {
            switch ((ModifierStateType)stateType)
            {
                case ModifierStateType.MODIFIER_STATE_MOVE_DISABLED: { IsMoveDisabled = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_ATTACK_DISABLED: { IsAttackDisabled = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_ACTIVATES_DISABLED: { IsActivatesAbilityDisabled = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_PASSIVES_DISABLED: { IsPassivesAbilityDisabled = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_STUNNED: { IsStunned = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_BUFF_IMMUNE: { IsBuffImmune = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_DEBUFF_IMMUNE: { IsDebuffImmune = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_NEBUFF_IMMUNE: { IsNebuffImmune = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_ALLBUFF_IMMUNE: { IsAllModifierImmune = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_INVISIBLE: { IsInvisible = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_NO_HEALTH_BAR: { IsNoHealthBar = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_INVINCIBLE: { IsInvincible = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_COMMAND_RESTRICTED: { IsResticted = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_UNSELECTABLE: { IsUnselectable = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_UNSELECTABLE_ATTACK: { IsUnselectableAttack = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                case ModifierStateType.MODIFIER_STATE_UNSELECTABLE_ABILITY: { IsUnselectableAbility = stateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED; } break;
                default: { } break;
            }
        }
        protected void ChangeModifierStates(Dictionary<int, ModifierStateValue> changedModifierStates)
        {
            if (changedModifierStates.Count > 0)
            {
                foreach (KeyValuePair<int, ModifierStateValue> modifierState in changedModifierStates)
                {
                    switch ((ModifierStateType)modifierState.Key)
                    {
                        case ModifierStateType.MODIFIER_STATE_COMMAND_RESTRICTED:
                            {
                                if (IsResticted && _currOrder != null && !_currOrder.IsFinished)
                                {
                                    if (_currOrder.Type != UnitOrderType.UNIT_ORDER_MOVE_TO_NODE)
                                    {
                                        _currOrder.Interrupt();
                                    }
                                }
                            }
                            break;
                        case ModifierStateType.MODIFIER_STATE_INVISIBLE:
                            {
                                SetActive(IsInvisible);
                            }
                            break;
                        case ModifierStateType.MODIFIER_STATE_STUNNED:
                            {
                                if (IsStunned && _currOrder != null && !_currOrder.IsFinished)
                                {
                                    if (_currOrder.Type != UnitOrderType.UNIT_ORDER_MOVE_TO_NODE)
                                    {
                                        _currOrder.Interrupt();
                                    }
                                }
                            }
                            break;
                        case ModifierStateType.MODIFIER_STATE_ATTACK_DISABLED:
                            {
                                if (IsAttackDisabled && _currOrder != null && !_currOrder.IsFinished && _currOrder.Type == UnitOrderType.UNIT_ORDER_ATTACK_TARGET)
                                {
                                    _currOrder.Interrupt();
                                }
                            }
                            break;
                        case ModifierStateType.MODIFIER_STATE_ACTIVATES_DISABLED:
                            {
                                if (IsActivatesAbilityDisabled && _currOrder != null && !_currOrder.IsFinished &&
                                    (_currOrder.Type == UnitOrderType.UNIT_ORDER_CAST_NO_TARGET || _currOrder.Type == UnitOrderType.UNIT_ORDER_CAST_POSITION || _currOrder.Type == UnitOrderType.UNIT_ORDER_CAST_TARGET)
                                    && !((CastAbilityOrder)_currOrder).Ability.IsPassive)
                                {
                                    _currOrder.Interrupt();
                                }
                            }
                            break;
                        case ModifierStateType.MODIFIER_STATE_PASSIVES_DISABLED:
                            {
                                if (IsPassivesAbilityDisabled)
                                {
                                    for (int i = 0; i < _allAbilities.Count; i++)
                                    {
                                        if (_allAbilities[i].IsPassive)
                                        {
                                            _allAbilities[i].Disable();
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < _allAbilities.Count; i++)
                                    {
                                        if (_allAbilities[i].IsPassive)
                                        {
                                            _allAbilities[i].Enable();
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        public void HandleModifierEvent(ModifierEventType modifierEventType, EventData eventData)
        {
            if (eventData == null)
            {
                return;
            }

            LinkedListNode<Modifier> first = _allModifiers.First;
            while (first != null)
            {
                if (!first.Value.IsRemoved)
                {
                    first.Value.HandleEvent(modifierEventType, eventData);
                    first = first.Next;
                }
                else
                {
                    first = first.Next;
                }
            }
        }

        #endregion


        #region 行为
        public virtual bool IsAttackTargetWaiting(BaseUnit target)
        {
            AttackTargetOrder order = null;
            for (int i = 0; i < _orders.Count; i++)
            {
                BaseOrder baseOrder = _orders[i];
                if (baseOrder.Type == UnitOrderType.UNIT_ORDER_ATTACK_TARGET)
                {
                    order = (AttackTargetOrder)baseOrder;
                    if (order.Target == target)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public virtual bool IsCastAbilityWaiting(Ability ability)
        {
            CastAbilityOrder order = null;
            for (int i = 0; i < _orders.Count; i++)
            {
                BaseOrder baseOrder = _orders[i];
                if (baseOrder.Type == UnitOrderType.UNIT_ORDER_CAST_POSITION
                    || baseOrder.Type == UnitOrderType.UNIT_ORDER_CAST_TARGET
                    || baseOrder.Type == UnitOrderType.UNIT_ORDER_CAST_NODE
                    || baseOrder.Type == UnitOrderType.UNIT_ORDER_CAST_NO_TARGET)
                {
                    order = (CastAbilityOrder)baseOrder;
                    if (order.Ability == ability)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public virtual void MoveToNode(BattleNode node)
        {
            //if (IsMoving || IsAttacking || IsCasting)
            //{
            //    return;
            //}

            MoveToNodeOrder moveToNodeOrder = new MoveToNodeOrder(this, node);
            //if (!moveToNodeOrder.IsExecutable())
            //{
            //    return;
            //}

            //if (_orders.Count == 0)
            //{
            //    moveToNodeOrder.Begin();
            //    if (!moveToNodeOrder.IsFinished)
            //    {
            //        _currOrder = moveToNodeOrder;
            //    }
            //}
            //else
            {
                _orders.Add(moveToNodeOrder);
            }
        }
        public virtual void AttackTarget(BaseUnit target, bool check = true)
        {
            if (check && IsAttackTargetWaiting(target))
            {
                return;
            }

            AttackTargetOrder order = new AttackTargetOrder(this, target);
            _orders.Add(order);
        }
        public virtual void FroceAttackTarget(BaseUnit unitTarget, bool immediate)
        {
            _froceAttackTarget = unitTarget;
            // no over
            _attackTarget = _froceAttackTarget;
            if (immediate)
            {
                if (_currOrder != null && !_currOrder.IsFinished)
                {
                    _currOrder.Interrupt();
                }

                _currOrder = null;
                _orders.Clear();

                AttackTarget(_froceAttackTarget);
                _froceAttackTarget = null;
            }
        }
        public virtual void CastNoTargetAbility(Ability ability, bool check = true)
        {
            if (check && IsCastAbilityWaiting(ability))
            {
                return;
            }

            CastNoTargetAbilityOrder order = new CastNoTargetAbilityOrder(this, ability);

            if (ability.IsIgnoreBackswing)
            {
                if (IsAttacking && IsAttackingPE)
                {
                    StopOrder();
                }
                else if (IsCasting && IsCastingPE)
                {
                    StopOrder();
                }
            }

            _orders.Add(order);
        }
        public virtual void CastUnitTargetAbility(Ability ability, BaseUnit unitTarget, bool check = true)
        {
            if (check && IsCastAbilityWaiting(ability))
            {
                return;
            }

            CastUnitTargetAbilityOrder order = new CastUnitTargetAbilityOrder(this, ability, unitTarget);

            if (ability.IsIgnoreBackswing)
            {
                if (IsAttacking && IsAttackingPE)
                {
                    StopOrder();
                }
                else if (IsCasting && IsCastingPE)
                {
                    StopOrder();
                }
            }

            _orders.Add(order);
        }
        public virtual void CastPointTargetAbility(Ability ability, DFixVector3 point, bool check = true)
        {
            if (check && IsCastAbilityWaiting(ability))
            {
                return;
            }

            CastPointTargetAbilityOrder order = new CastPointTargetAbilityOrder(this, ability, point);

            if (ability.IsIgnoreBackswing)
            {
                if (IsAttacking && IsAttackingPE)
                {
                    StopOrder();
                }
                else if (IsCasting && IsCastingPE)
                {
                    StopOrder();
                }
            }

            _orders.Add(order);
        }
        public virtual void InterruptOrder()
        {
            _orders.Clear();

            if (_currOrder != null && !_currOrder.IsFinished)
            {
                _currOrder.Interrupt();
                _currOrder = null;
            }
        }
        public virtual void StopOrder()
        {
            _orders.Clear();

            if (_currOrder != null && !_currOrder.IsFinished)
            {
                _currOrder.Quit();
                _currOrder = null;
            }
        }
        public virtual void StopMoveOrder()
        {
            for (int i = _orders.Count - 1; i >= 0; i--)
            {
                if (_orders[i].GetType() == typeof(MoveToNodeOrder))
                {
                    _orders.RemoveAt(i);
                }
            }

            if (_currOrder != null && !_currOrder.IsFinished && _currOrder.GetType() == typeof(MoveToNodeOrder))
            {
                _currOrder.Quit();
                _currOrder = null;
            }
        }
        #endregion

        #region 皮肤模型
        protected List<int> _skinModelIds = new List<int>();                                                    // 皮肤包含的模型id
        protected int _currSkinModelIndex = 0;                                                                  // 当前模型索引
        private string _headIconAssetPath = string.Empty;                                                       // 头像
        protected string _spawnEffect = string.Empty;
        protected string _deadEffect = string.Empty;
        protected DFix64 _modelScale = DFix64.One;                                                                // 模型缩放
        protected List<Attack> _attacks = new List<Attack>();                                                   // 攻击列表
        protected List<Attack> _critAttacks = new List<Attack>();                                               // 暴击攻击列表
        protected bool _isShowShadow = false;                                                                     // 是否显示阴影
        protected string _playingAnimationName = string.Empty;                                                    // 当前播放的动画名
        protected DFix64 _lastLogicPlayingAnimationTime = DFix64.Zero;                                              // 当前播放动画计时
        protected DFix64 _logicPlayingAnimationTime = DFix64.Zero;                                                  // 当前播放动画计时
        protected float _renderPlayingAnimationTime = 0f;
        protected DFix64 _animationSpeed = DFix64.One;                                                              // 动画速度
        protected AnimationState _playingAnimationState = null;                                                   // 当前播放的动画State


        protected override Type GetEntityType()
        {
            return typeof(BattleUnitEntityLogic);
        }

        protected virtual void InitSkinParm()
        {
            if (BattleKvLibraryManager.HasSkinKv(SkinId))
            {
                KeyValue kvSkin = BattleKvLibraryManager.GetSkinKv(SkinId);
                KeyValue kvModels = kvSkin["Models"];
                if (kvModels != null && kvModels.HasChildren)
                {
                    foreach (var child in kvModels.Children)
                    {
                        int modelId = BattleData.ParseInt(child.GetString());
                        _skinModelIds.Add(modelId);
                    }

                    InitModelParm(_skinModelIds[0]);
                }
            }
        }

        protected virtual void InitModelParm(int modelId)
        {
            _headIconAssetPath = string.Empty;
            _spawnEffect = string.Empty;
            _deadEffect = string.Empty;
            _modelScale = DFix64.One;
            _attacks.Clear();
            _critAttacks.Clear();

            KeyValue kvModel = BattleKvLibraryManager.GetModelKv(modelId);
            if (kvModel != null)
            {
                KeyValue kvParm = kvModel["Model"];
                if (kvParm != null)
                {
                    SetAsset(kvParm.GetString());
                }

                kvParm = kvModel["HeadIcon"];
                if (kvParm != null)
                {
                    _headIconAssetPath = kvParm.GetString();
                }

                kvParm = kvModel["ModelScale"];
                if (kvParm != null)
                {
                    _modelScale = BattleData.ParseDFix64(kvParm.GetString());
                }

                if (_teamId == BattleData.SelfPlayer.TeamId)
                {
                    kvParm = kvModel["SpawnEffect"];
                    if (kvParm != null)
                    {
                        _spawnEffect = kvParm.GetString();
                    }
                    else
                    {
                        _spawnEffect = Constant.Battle.FRIENDLY_SPAWN_EFFECT_ASSET;
                    }

                    kvParm = kvModel["DeathEffect"];
                    if (kvParm != null)
                    {
                        _deadEffect = kvParm.GetString();
                    }
                    else
                    {
                        _deadEffect = Constant.Battle.SELF_TEAM_DEATH_EFFECT_ASSET;
                    }
                }
                else
                {
                    kvParm = kvModel["SpawnEffectEnemy"];
                    if (kvParm != null)
                    {
                        _spawnEffect = kvParm.GetString();
                    }
                    else
                    {
                        _spawnEffect = Constant.Battle.ENEMY_SPAWN_EFFECT_ASSET;
                    }

                    kvParm = kvModel["DeathEffectEnemy"];
                    if (kvParm != null)
                    {
                        _deadEffect = kvParm.GetString();
                    }
                    else
                    {
                        _deadEffect = Constant.Battle.ENEMY_TEAM_DEATH_EFFECT_ASSET;
                    }
                }

                kvParm = kvModel["Attack"];
                if (kvParm != null && kvParm.HasChildren)
                {
                    foreach (var child in kvParm.Children)
                    {
                        Attack attack = new Attack(_attacks.Count, child);
                        _attacks.Add(attack);
                    }
                }

                kvParm = kvModel["CritAttack"];
                if (kvParm != null && kvParm.HasChildren)
                {
                    foreach (var child in kvParm.Children)
                    {
                        Attack attack = new Attack(_critAttacks.Count, child);
                        _critAttacks.Add(attack);
                    }
                }
            }
            else
            {
                SetAsset(string.Empty);
            }
        }

        protected virtual void InstantiateModel()
        {
            if (_unitEntityLogic == null)
            {
                return;
            }

            _unitEntityLogic.ShowShadow(_isShowShadow);

            if (!string.IsNullOrEmpty(_playingAnimationName))
            {
                _playingAnimationState = _unitEntityLogic.PlayerAnimation(_playingAnimationName, (float)_animationSpeed * _speed, _renderPlayingAnimationTime, false);
                if (_playingAnimationState == null)
                {
                    PlayAnimation(UnitAnimationType.ACT_IDLE, DFix64.One, false);
                }
            }
        }

        protected virtual void DestroyModel()
        {
            _unitEntityLogic = null;
            _playingAnimationState = null;
        }

        protected virtual void PlayModelSpawnEffect()
        {
            if (!string.IsNullOrEmpty(_spawnEffect))
            {
                int spawnEffectId = ParticleManager.CreateParticle(_spawnEffect, false);
                ParticleManager.SetParticleControl(spawnEffectId, ParticleControlType.Position, _logicPosition, true);
            }
        }

        protected virtual void PlayModelDeadEffect()
        {
            if (IsIllusion)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_deadEffect))
            {
                int effectId = ParticleManager.CreateParticle(_deadEffect, false);
                ParticleManager.SetParticleControl(effectId, ParticleControlType.Position, _logicPosition, true);
            }
        }

        protected virtual void UpdateModelLogic(DFix64 frameLength)
        {
            if (!string.IsNullOrEmpty(_playingAnimationName))
            {
                _lastLogicPlayingAnimationTime = _logicPlayingAnimationTime;
                _logicPlayingAnimationTime += frameLength * _animationSpeed * _speed;

                if (_playingAnimationState != null)
                {
                    if (_logicPlayingAnimationTime >= _playingAnimationState.clip.length)
                    {
                        if (_playingAnimationState.clip.wrapMode == WrapMode.Loop)
                        {
                            _logicPlayingAnimationTime -= _playingAnimationState.clip.length;
                        }
                        else
                        {
                            if (!_isDeadState && !_isHideState)
                            {
                                //StopAnimation();
                                PlayAnimation(UnitAnimationType.ACT_IDLE, true);
                            }
                        }
                    }
                }
            }
        }

        protected virtual void UpdateModelRender(float interpolation, float deltaTime)
        {
            //_renderPlayingAnimationTime = Mathf.Lerp((float)_lastLogicPlayingAnimationTime, (float)_logicPlayingAnimationTime, interpolation);
            //if (_playingAnimationState != null)
            //{
            //    //_playingAnimationState.time = _renderPlayingAnimationTime;
            //}

            //if (_playingAnimationState != null)
            //{
            //    if (_playingAnimationState.clip.wrapMode != WrapMode.Loop)
            //    {
            //        if (!_isDeadState && !_isHideState)
            //        {
            //            if (!_unitEntityLogic._animation.isPlaying)
            //            {
            //                //StopAnimation();
            //                PlayAnimation(UnitAnimationType.ACT_IDLE, false);
            //            }
            //        }
            //    }
            //}
        }

        public virtual void ChangeModelByIndex(int modelIndex)
        {
            if (_currSkinModelIndex == modelIndex)
            {
                return;
            }

            if (modelIndex < 0 || modelIndex >= _skinModelIds.Count)
            {
                return;
            }

            _currSkinModelIndex = modelIndex;
            InitModelParm(_skinModelIds[_currSkinModelIndex]);

            if (_isActivated)
            {
                SetActive(_isActivated);
            }
        }

        public virtual void ShowShadow(bool isShow)
        {
            _isShowShadow = isShow;

            if (_unitEntityLogic != null)
            {
                _unitEntityLogic.ShowShadow(_isShowShadow);
            }
        }

        public virtual bool HasAttachPoint(string attachPointName)
        {
            if (_unitEntityLogic == null)
            {
                return false;
            }

            return _unitEntityLogic.HasAttachPoint(attachPointName);
        }

        public virtual Transform GetAttachPoint(string attachPointName)
        {
            if (_unitEntityLogic == null)
            {
                return null;
            }

            return _unitEntityLogic.GetAttachPoint(attachPointName);
        }

        public virtual Attack GetRandomAttack(bool isCrit)
        {
            if (isCrit)
            {
                if (_critAttacks.Count > 0)
                {
                    return _critAttacks[BattleData.SRandom.RangeI(0, _critAttacks.Count)];
                }
            }
            else
            {
                if (_attacks.Count > 0)
                {
                    return _attacks[BattleData.SRandom.RangeI(0, _attacks.Count)];
                }
            }

            return null;
        }

        public virtual bool IsPlayingAnimation(UnitAnimationType unitActType)
        {
            return _playingAnimationName == Constant.Battle.UnitActNames[unitActType];
        }
        public virtual void PlayAnimation(UnitAnimationType unitActType, bool crossFade)
        {
            PlayAnimation(unitActType, DFix64.One, crossFade);
        }
        public virtual void PlayAnimation(UnitAnimationType unitActType, DFix64 speed, bool crossFade)
        {
            if (IsPlayingAnimation(unitActType))
            {
                if (_playingAnimationState != null && _playingAnimationState.wrapMode == WrapMode.Loop)
                {
                    SetAnimationSpeed(speed);
                    return;
                }
            }

            _playingAnimationName = Constant.Battle.UnitActNames[unitActType];
            _lastLogicPlayingAnimationTime = DFix64.Zero;
            _logicPlayingAnimationTime = DFix64.Zero;
            _renderPlayingAnimationTime = 0f;
            _playingAnimationState = null;
            _animationSpeed = speed;

            if (_unitEntityLogic == null)
            {
                return;
            }

            _playingAnimationState = _unitEntityLogic.PlayerAnimation(_playingAnimationName, (float)_animationSpeed * _speed, _renderPlayingAnimationTime, crossFade);
            if (_playingAnimationState == null)
            {
                if (unitActType != UnitAnimationType.ACT_IDLE)
                {
                    PlayAnimation(UnitAnimationType.ACT_IDLE, DFix64.One, true);
                }
            }
        }
        public virtual void PlayAnimation(UnitAnimationType unitActType, DFix64 speed, DFix64 time, bool crossFade)
        {
            if (IsPlayingAnimation(unitActType))
            {
                _logicPlayingAnimationTime = time;
                _lastLogicPlayingAnimationTime = _logicPlayingAnimationTime;
                _renderPlayingAnimationTime = (float)_lastLogicPlayingAnimationTime;
                _animationSpeed = speed;

                if (_unitEntityLogic != null)
                {
                    _unitEntityLogic.SetAnimationSpeed((float)_animationSpeed * _speed);
                    _unitEntityLogic.SetAnimationTime((float)time);
                }

                return;
            }

            _playingAnimationName = Constant.Battle.UnitActNames[unitActType];
            _logicPlayingAnimationTime = (DFix64)time;
            _lastLogicPlayingAnimationTime = _logicPlayingAnimationTime;
            _renderPlayingAnimationTime = (float)_lastLogicPlayingAnimationTime;
            _playingAnimationState = null;
            _animationSpeed = speed;

            if (_unitEntityLogic == null)
            {
                return;
            }

            _playingAnimationState = _unitEntityLogic.PlayerAnimation(_playingAnimationName, (float)_animationSpeed * _speed, _renderPlayingAnimationTime, crossFade);
            if (_playingAnimationState == null)
            {
                if (unitActType != UnitAnimationType.ACT_IDLE)
                {
                    PlayAnimation(UnitAnimationType.ACT_IDLE, DFix64.One, true);
                }
            }
        }
        public virtual void SetAnimationSpeed(DFix64 speed)
        {
            _animationSpeed = speed;

            if (_unitEntityLogic != null)
            {
                _unitEntityLogic.SetAnimationSpeed((float)_animationSpeed * _speed);
            }
        }
        public virtual void StopAnimation()
        {
            _playingAnimationName = string.Empty;
            _lastLogicPlayingAnimationTime = DFix64.Zero;
            _logicPlayingAnimationTime = DFix64.Zero;
            _renderPlayingAnimationTime = 0f;
            _playingAnimationState = null;
            _animationSpeed = DFix64.One;

            if (_unitEntityLogic != null)
            {
                _unitEntityLogic.StopAnimation();
            }
        }
        #endregion

        public override void ChangeSpeed(float speed)
        {
            base.ChangeSpeed(speed);

            if (_unitEntityLogic != null)
            {
                _unitEntityLogic.SetAnimationSpeed((float)_animationSpeed * _speed);
            }
        }
    }
}