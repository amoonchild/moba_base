using KVLib;
using GameDevWare.Dynamic.Expressions;
using GameDevWare.Dynamic.Expressions.CSharp;
using Assets;
using GameFramework.Sound;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using LiaoZhai.RT;


namespace LiaoZhai.Runtime
{
    // 战斗数据
    public static class BattleData
    {
        public static void Init()
        {
            GameManager.Event.Subscribe(SaveLineupSuccessEventArgs.EventId, OnSaveLineupSuccess);
            GameManager.Event.Subscribe(SaveLineupFailureEventArgs.EventId, OnSaveLineupFailure);
            GameManager.Event.Subscribe(BeginBattleSuccessEventArgs.EventId, OnBeginBattleSuccess);
            GameManager.Event.Subscribe(BeginBattleFailureEventArgs.EventId, OnBeginBattleFailure);
            GameManager.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameManager.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);

            _sRandom = new SRandom((ulong)(System.DateTime.Now.ToUnixTimeStamp() % 1000000));
            InitEvaluate();
            InitActionManager();

            ParticleManager.Init();
            BattlePlayerManager.Init();
            UnitManager.Init();
            ProjectileManager.Init();

            _battleType = BattleType.Undefined;
            _targetId = 0UL;
            _drBattleNode = null;

            _players = new Dictionary<ulong, BasePlayer>();

            _lineupHeros = new List<BaseUnit>();

            _cachedBattleNodes = new List<BattleNode>();

            State = BattleState.Init;
            Result = BattleResult.None;
            BattleSpeed = 1f;

            _state = BattleState.Init;
            _result = BattleResult.None;
            _battleResultType = 0;

            _logicFrame = 0;
            _logicTime = DFix64.Zero;
            _battleSpeed = DFix64.One;

            Parm.Load();
        }

        public static void Destroy()
        {
            GameManager.Sound.GetSoundGroup(RT.Constant.SoundGroup.Sound).StopAllLoadedSounds();

            ProjectileManager.Destroy();
            UnitManager.Destroy();
            BattlePlayerManager.Destroy();
            ParticleManager.Destroy();

            if (_cameraController != null)
            {
                GameObject.DestroyImmediate(_cameraController);
                _cameraController = null;
            }

            if (_battleGrid != null)
            {
                _battleGrid.Destroy();
            }

            if (BattleData.Camera != null)
            {
                UnityStandardAssets.ImageEffects.BloomOptimized bloom = BattleData.Camera.GetComponent<UnityStandardAssets.ImageEffects.BloomOptimized>();
                if (bloom != null)
                {
                    bloom.enabled = false;
                }

                UnityStandardAssets.ImageEffects.Bloom bloom2 = BattleData.Camera.GetComponent<UnityStandardAssets.ImageEffects.Bloom>();
                if (bloom2 != null)
                {
                    bloom2.enabled = false;
                }
            }
        }

        public static void Release()
        {
            GameManager.Event.Unsubscribe(SaveLineupSuccessEventArgs.EventId, OnSaveLineupSuccess);
            GameManager.Event.Unsubscribe(SaveLineupFailureEventArgs.EventId, OnSaveLineupFailure);
            GameManager.Event.Unsubscribe(BeginBattleSuccessEventArgs.EventId, OnBeginBattleSuccess);
            GameManager.Event.Unsubscribe(BeginBattleFailureEventArgs.EventId, OnBeginBattleFailure);
            GameManager.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameManager.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);

            ProjectileManager.Release();
            UnitManager.Release();
            BattlePlayerManager.Release();
            ParticleManager.Release();
            ModifierManager.Release();

            ReleaseActionManager();
            ReleaseEvaluate();
            ReleaseTargetInfos();
            ModifierManager.RemoveAll();

            _sRandom = null;
            _objectId = 0;

            _camera = null;
            if (_battleGrid != null)
            {
                _battleGrid.Release();
                _battleGrid = null;
            }

            _cachedBattleNodes.Clear();
            _cachedBattleNodes = null;

            BattleInitData.Clear();

            _selfPlayer = null;
            _enemyPlayer = null;
            _players.Clear();
            _players = null;

            _currLineup = null;
            _lineupHeros.Clear();
            _lineupHeros = null;
            _lineupBoss = null;
            _lineupHeroLimit = 0;
            _isSendSaveLineup = false;
            _isWaitStartBattle = false;
            _isSendStartBattle = false;

            _findTargets.Clear();
            _fulltimeModifierHandlers.Clear();
        }

        #region 表达式
        private static BattleExpression<EventData> _battleExpression = new BattleExpression<EventData>();

        public static DFix64 ParseDFix64(string pattern)
        {
            return _battleExpression.ParseDFix64(pattern);
        }
        public static int ParseInt(string pattern)
        {
            return (int)_battleExpression.ParseLong(pattern);
        }
        public static long ParseLong(string pattern)
        {
            return _battleExpression.ParseLong(pattern);
        }
        public static bool ParseBool01(string pattern)
        {
            return _battleExpression.ParseBool01(pattern);
        }
        public static DFix64 EvaluateDFix64(string pattern, EventData instance)
        {
            return _battleExpression.EvaluateDFix64(pattern, instance);
        }
        public static long EvaluateLong(string pattern, EventData instance)
        {
            return _battleExpression.EvaluateLong(pattern, instance);
        }
        public static bool EvaluateBool(string pattern, EventData instance)
        {
            return _battleExpression.EvaluateBool(pattern, instance);
        }
        public static T EvaluateEnum<T>(string pattern)
        {
            return _battleExpression.EvaluateEnum<T>(pattern);
        }
        public static T EvaluateEnum<T>(string pattern, T defaultValue)
        {
            return _battleExpression.EvaluateEnum<T>(pattern, defaultValue);
        }
        public static bool TryEvaluateEnum<T>(string pattern, out T result)
        {
            return _battleExpression.TryEvaluateEnum<T>(pattern, out result);
        }
        public static bool TryEvaluateEnum<T>(string pattern, out T result, T defaultValue)
        {
            return _battleExpression.TryEvaluateEnum<T>(pattern, out result, defaultValue);
        }
        public static T EvaluateEnums<T>(string pattern, T defaultValue)
        {
            return _battleExpression.EvaluateEnums<T>(pattern, '|', defaultValue);
        }
        public static bool TryEvaluateEnums<T>(string pattern, out T result)
        {
            return _battleExpression.TryEvaluateEnums<T>(pattern, '|', out result);
        }
        public static bool TryEvaluateEnums<T>(string pattern, out T result, T defaultValue)
        {
            return _battleExpression.TryEvaluateEnums<T>(pattern, '|', out result, defaultValue);
        }
        public static HashSet<int> EvaluateTrait(string pattern)
        {
            return _battleExpression.EvaluateTraits<CardTraitType>(pattern, '|');
        }
        public static int[] EvaluateModifierGroup(string pattern)
        {
            return _battleExpression.EvaluateModifierGroups(pattern, '|');
        }
        public static void AddPattern(string pattern)
        {
            _battleExpression.AddPattern(pattern);
        }

        private static void InitEvaluate()
        {
            //_battleExpression = new BattleExpression<EventData>();
        }
        private static void ReleaseEvaluate()
        {
            _battleExpression.Release();
            //_battleExpression = null;
        }
        #endregion

        #region 随机数
        private static SRandom _sRandom = null;

        public static SRandom SRandom
        {
            get
            {
                return _sRandom;
            }

        }
        #endregion

        #region 目标信息
        private static Dictionary<int, MulTargetInfo> _mulTargetInfos = new Dictionary<int, MulTargetInfo>();

        public static MulTargetInfo GetTargetInfo(KeyValue kvTarget)
        {
            if (_mulTargetInfos.ContainsKey(kvTarget.Id))
            {
                return _mulTargetInfos[kvTarget.Id];
            }

            MulTargetInfo mulTargetInfo = MulTargetInfo.Parse(kvTarget);
            _mulTargetInfos.Add(kvTarget.Id, mulTargetInfo);

            return mulTargetInfo;
        }

        public static void ReleaseTargetInfos()
        {
            _mulTargetInfos.Clear();
        }
        #endregion

        #region ActionManager
        private static BattleActionManager _battleActionManager = null;
        private static Queue<EventData> _eventDatas = new Queue<EventData>();

        public static void RecycleEventDatas(bool isRelease)
        {
            _battleActionManager.RecycleEventDatasNew(isRelease);
        }

        public static EventData CreateEventData()
        {
            return _battleActionManager.CreateEventData();
        }

        public static void ExecuteActions(KeyValue keyValue, EventData eventData)
        {
            _battleActionManager.Executes(keyValue, eventData);
        }

        public static void ExecuteAction(KeyValue keyValue, EventData eventData)
        {
            _battleActionManager.Execute(keyValue, eventData);
        }

        public static void AddDelayAction(DFix64 delayTime, KeyValue keyValue, EventData eventData)
        {
            _battleActionManager.AddDelayAction(delayTime, keyValue, eventData);
        }

        public static void UpdateAction(DFix64 frameLength)
        {
            _battleActionManager.UpdateAction(frameLength);
        }

        private static void InitActionManager()
        {
            _battleActionManager = new BattleActionManager();
            _battleActionManager.Init();
        }

        private static void ReleaseActionManager()
        {
            _battleActionManager.Release();
            _battleActionManager = null;
        }

        #endregion


        private static DFix64 _logicFrameLength = (DFix64)0.033f;

        private static BattleType _battleType = BattleType.Undefined;
        private static ulong _targetId = 0UL;
        private static DRBattleNode _drBattleNode = null;

        private static Dictionary<ulong, BasePlayer> _players = null;
        private static BasePlayer _selfPlayer = null;
        private static BasePlayer _enemyPlayer = null;

        private static PlayerLineup _currLineup = null;
        private static int _lineupHeroLimit = 0;
        private static Boss _lineupBoss = null;
        private static List<BaseUnit> _lineupHeros = null;
        private static bool _isSendSaveLineup = false;
        private static bool _isWaitStartBattle = false;
        private static bool _isSendStartBattle = false;

        private static int _logicFrame = 0;
        private static DFix64 _logicTime = DFix64.Zero;
        private static DFix64 _battleSpeed = DFix64.One;

        private static int _objectId = 0;
        private static Camera _camera = null;
        private static ThirdpersonCameraController _cameraController = null;
        private static BattleGrid _battleGrid = null;
        private static List<BattleNode> _cachedBattleNodes = null;

        private static BattleState _state = BattleState.Init;
        private static BattleResult _result = BattleResult.None;
        private static int _battleResultType = 0;



        public static BattleType BattleType
        {
            get { return _battleType; }
            set { _battleType = value; }
        }
        public static ulong TargetId
        {
            get { return _targetId; }
            set { _targetId = value; }
        }
        public static DRBattleNode DrBattleNode
        {
            get { return _drBattleNode; }
            set { _drBattleNode = value; }
        }

        public static BasePlayer SelfPlayer
        {
            get
            {
                return _selfPlayer;
            }
        }
        public static BasePlayer EnemyPlayer
        {
            get
            {
                return _enemyPlayer;
            }
        }

        public static PlayerLineup CurrLineup
        {
            get { return _currLineup; }
            set { _currLineup = value; }
        }
        public static List<BaseUnit> LineupHeros
        {
            get { return _lineupHeros; }
        }
        public static Boss LineupBoss
        {
            get { return _lineupBoss; }
            set { _lineupBoss = value; }
        }
        public static int LineupHeroLimit
        {
            get { return _lineupHeroLimit; }
            set { _lineupHeroLimit = value; }
        }

        public static DFix64 LogicFrameLength
        {
            get
            {
                return _logicFrameLength;
            }
        }
        public static int LogicFrame
        {
            get
            {
                return _logicFrame;
            }
            set
            {
                _logicFrame = value;
            }
        }
        public static DFix64 LogicTime
        {
            get
            {
                return _logicTime;
            }
            set
            {
                _logicTime = value;
            }
        }
        public static DFix64 LimitTime
        {
            get;
            set;
        }


        public static float BattleSpeed
        {
            get;
            set;
        }

        public static float GetBattleSpeedUp()
        {
            return GameManager.Config.GetFloat(Constant.Config.BATTLE_SPEED, 1.5f);
        }

        public static float GetFinalBattleSpeed()
        {
            return BattleSpeed * GameManager.Base.GameSpeed;
        }

        public static void ChangeBattleSpeed(float speed)
        {
            ParticleManager.ChangeSpeed(speed);
            ProjectileManager.ChangeSpeed(speed);
            UnitManager.ChangeSpeed(speed);
        }

        public static int NextObjectId
        {
            get { return ++_objectId; }
        }
        public static Camera Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }
        public static ThirdpersonCameraController CameraController
        {
            get { return _cameraController; }
        }
        public static BattleGrid BattleGrid
        {
            get { return _battleGrid; }
        }

        public static BattleState State
        {
            get { return _state; }
            set { _state = value; }
        }
        public static BattleResult Result
        {
            get { return _result; }
            set { _result = value; }
        }
        public static int BattleResultType
        {
            get { return _battleResultType; }
            set { _battleResultType = value; }
        }


        public static void AddLineupHero(BaseUnit target, BattleNode node)
        {
            target.SpawnToLineup(node, true);
            UnitManager.AddUnitToLineup(target);
            LineupHeros.Add(target);

            //GameManager.ThinkingAnalytics.Track_Lineup_Spawn(target, node.X, node.Y);
        }

        public static void RemoveLineupHero(BaseUnit target)
        {
            target.UnspawnFromLineup();
            UnitManager.RemoveUnitFromLineup(target);
            LineupHeros.Remove(target);
        }

        public static bool IsBagCardInLineup(int bagIndex)
        {
            for (int i = 0; i < _lineupHeros.Count; i++)
            {
                if (_lineupHeros[i].BagIndex == bagIndex)
                {
                    return true;
                }
            }

            return false;
        }
        public static BaseUnit GetLineupUnit(int bagIndex)
        {
            for (int i = 0; i < _lineupHeros.Count; i++)
            {
                if (_lineupHeros[i].BagIndex == bagIndex)
                {
                    return _lineupHeros[i];
                }
            }

            return null;
        }

        public static bool IsLineupChanged()
        {
            if (CurrLineup.OccId != LineupBoss.OccId)
            {
                return true;
            }
            else if (CurrLineup.Cards.Count != LineupHeros.Count)
            {
                return true;
            }
            else
            {
                bool isFind = false;
                for (int i = 0; i < CurrLineup.Cards.Count; i++)
                {
                    isFind = false;
                    for (int j = 0; j < LineupHeros.Count; j++)
                    {
                        if (LineupHeros[j].BagIndex == CurrLineup.Cards[i].BagIndex)
                        {
                            if (LineupHeros[j].CurrNode.X != CurrLineup.Cards[i].NodeX || LineupHeros[j].CurrNode.Y != CurrLineup.Cards[i].NodeY)
                            {
                                return true;
                            }

                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static int SortUnitAction(BaseUnit a, BaseUnit b)
        {
            if (a.TraitType == (int)CardTraitType.Ruthless && b.TraitType != (int)CardTraitType.Ruthless)
            {
                return -1;
            }
            else if (a.TraitType != (int)CardTraitType.Ruthless && b.TraitType == (int)CardTraitType.Ruthless)
            {
                return 1;
            }
            else if (a.TraitType == (int)CardTraitType.Boss && b.TraitType != (int)CardTraitType.Boss)
            {
                return 1;
            }
            else if (a.TraitType != (int)CardTraitType.Boss && b.TraitType == (int)CardTraitType.Boss)
            {
                return -1;
            }
            else if (a.AttackCapability == UnitAttackCapabilityType.UNIT_CAP_MELEE_ATTACK && b.AttackCapability != UnitAttackCapabilityType.UNIT_CAP_MELEE_ATTACK)
            {
                return -1;
            }
            else if (a.AttackCapability != UnitAttackCapabilityType.UNIT_CAP_MELEE_ATTACK && b.AttackCapability == UnitAttackCapabilityType.UNIT_CAP_MELEE_ATTACK)
            {
                return 1;
            }

            return b.RandomSpeed.CompareTo(a.RandomSpeed);
        }

        public static void SortUnits()
        {
            UnitManager.Units.Sort(SortUnitAction);
        }

        public static void SendSaveLineup()
        {
            if (_isSendSaveLineup || _isSendStartBattle || _state != BattleState.Lineup)
            {
                return;
            }

            if (LineupHeros.Count == 0)
            {
                RuntimeGameEntry.UI.OpenUIFormById(Constant.UIFormId.ErrorMsg, 198);
                return;
            }
            else if (LineupHeros.Count > LineupHeroLimit)
            {
                if (BattleType == BattleType.Rift)
                {
                    RuntimeGameEntry.UI.OpenUIFormById(Constant.UIFormId.ErrorMsg, Constant.ErrorMsgId.Id10041);
                }
                else
                {
                    RuntimeGameEntry.UI.OpenUIFormById(Constant.UIFormId.ErrorMsg, Constant.ErrorMsgId.Id10040);
                }

                return;
            }

            //if (!IsLineupChanged())
            //{
            //    return;
            //}

            RoomChannelCSSaveLineupPacket csPacket = new RoomChannelCSSaveLineupPacket();
            csPacket.army_id = (uint)CurrLineup.SerialId;
            csPacket.using_occ_id = (uint)LineupBoss.OccId;
            csPacket.uHeroIndex = new msg_battle_army[LineupHeros.Count];
            for (int i = 0; i < csPacket.uHeroIndex.Length; i++)
            {
                msg_battle_army msg_Battle_Army = new msg_battle_army
                {
                    hero_indexSpecified = true,
                    hero_index = (uint)LineupHeros[i].BagIndex,
                    hero_xSpecified = true,
                    hero_x = LineupHeros[i].CurrNode.X,
                    hero_ySpecified = true,
                    hero_y = LineupHeros[i].CurrNode.Y,
                };

                csPacket.uHeroIndex[i] = msg_Battle_Army;
            }

            _isSendSaveLineup = true;
            NetworkUtility.SendRoomPacket(csPacket);
        }
        public static void SendStartBattle()
        {
            if (_isSendSaveLineup || _isSendStartBattle || _state != BattleState.Lineup)
            {
                return;
            }

            if (LineupHeros.Count == 0)
            {
                RuntimeGameEntry.UI.OpenUIFormById(Constant.UIFormId.ErrorMsg, 198);
                return;
            }
            else if (LineupHeros.Count > LineupHeroLimit)
            {
                if (BattleType == BattleType.Rift)
                {
                    RuntimeGameEntry.UI.OpenUIFormById(Constant.UIFormId.ErrorMsg, Constant.ErrorMsgId.Id10041);
                }
                else
                {
                    RuntimeGameEntry.UI.OpenUIFormById(Constant.UIFormId.ErrorMsg, Constant.ErrorMsgId.Id10040);
                }

                return;
            }

            if (IsLineupChanged())
            {
                if (!GameManager.Guide.IsInGuide && LineupHeros.Count < LineupHeroLimit)
                {
                    GameManager.UI.OpenUIForm(Runtime.UIFormType.TextConfirm, new UITextConfirmFormOpenData()
                    {
                        Title = "开 战",
                        Content = "上阵人数未满，是否确定开战？",
                        ConfirmEvent = _SendStartBattle,
                    });
                }
                else
                {
                    _isWaitStartBattle = true;
                    SendSaveLineup();
                }

                return;
            }
            else if (CurrLineup.IsRemovedEmpty)
            {
                _isWaitStartBattle = true;
                SendSaveLineup();
                return;
            }

            _isSendStartBattle = true;

            RoomChannelCSGetBattleNodeAttributePacket csPacket = new RoomChannelCSGetBattleNodeAttributePacket();
            csPacket.battle_type = (uint)BattleData.BattleType;
            NetworkUtility.SendRoomPacket(csPacket);
        }
        private static void _SendStartBattle()
        {
            _isWaitStartBattle = true;
            SendSaveLineup();
        }

        public static bool IsGuideMode
        {
            get;
            set;
        }
        public static bool IsGuideLineupComplete
        {
            get;
            set;
        }

        public static DateTime BattleStartTime
        {
            get;
            set;
        }
        public static DateTime BattleEndTime
        {
            get;
            set;
        }

        public static float GetBattleTimeSec()
        {
            return (float)(BattleEndTime - BattleStartTime).TotalSeconds;
        }

        public static BattleType LastBattleType
        {
            get;
            set;
        }
        public static ulong LastBattleTarget
        {
            get;
            set;
        }
        public static BattleResult LastBattleResult
        {
            get;
            set;
        }

        public static void ResetAll()
        {
            LastBattleType = BattleType.Undefined;
            LastBattleTarget = 0UL;
            LastBattleResult = BattleResult.None;
        }

        public static void Clear()
        {
            _state = BattleState.Init;
            _result = BattleResult.None;
            BattleResultType = 0;
            BattleStartTime = DateTime.MinValue;
            BattleEndTime = DateTime.MinValue;
        }

        public static bool CreateCameraController()
        {
            Transform sceneCenter = GameObject.Find("zhongxindian").transform;
            if (sceneCenter == null)
            {
                return true;
            }

            _cameraController = GameManager.Scene.MainCamera.gameObject.AddComponent<ThirdpersonCameraController>();
            _cameraController.SetTarget(sceneCenter);
            _cameraController.enabled = false;

            //_dragCameraController.LateUpdate();

            return true;
        }

        public static BasePlayer GetPlayer(ulong id)
        {
            if (!_players.ContainsKey(id))
            {
                return null;
            }

            return _players[id];
        }

        public static BasePlayer CreatePlayer(CreatePlayerData initPlayerData)
        {
            if (!_players.ContainsKey(initPlayerData.RoleId))
            {
                RealPlayer player = new RealPlayer(initPlayerData.RoleId, initPlayerData.Name, initPlayerData.Level, initPlayerData.TeamId);
                _players.Add(player.RoleId, player);

                if (player.RoleId == GameManager.GlobalData.SelfPlayer.RoleId)
                {
                    _selfPlayer = player;
                }
                else
                {
                    _enemyPlayer = player;
                }

                return player;
            }

            return null;
        }

        public static BaseUnit CreateUnit(CreateUnitData createUnitData)
        {
            BaseUnit unit = UnitManager.CreateUnit(createUnitData);
            //unit.ResetAttAndSkill(initAttribute, abilityIds);
            //UnitManager.AddUnit(unit);

            return unit;
        }

        public static BaseUnit CreateIllusion(BaseUnit master, BaseUnit source, int nodeX, int nodeY, BattleUnitAttribute initAttribute, ObscuredInt[] abilityIds)
        {
            CreateUnitData initUnitData = new CreateUnitData();

            initUnitData.unit_type = UnitType.UNIT_HERO;
            initUnitData.player_id = master.PlayerId;
            initUnitData.card_id = source.UnitId;
            initUnitData.level = source.Level;
            initUnitData.player_camp = master.TeamId;
            initUnitData.skin_id = source.SkinId;
            initUnitData.unit_in_x = nodeX;
            initUnitData.unit_in_y = nodeY;
            initUnitData.name = source.Name;
#if UNITY_EDITOR
            initUnitData.name = GameFramework.Utility.Text.Format("{0} 幻象", source.Name);
#endif
            initUnitData.skill_id = abilityIds;

            BaseUnit unit = UnitManager.CreateUnit(initUnitData);
            unit.ResetAtt(initAttribute);
            unit.ResetAbility(abilityIds);
            //UnitManager.AddUnit(unit);

            unit.Master = master;
            unit.LogicEulerAngles = master.LogicEulerAngles;

            return unit;
        }
        public static BaseUnit CreateSummoned(BaseUnit master, int unitId, int skinId, int nodeX, int nodeY, BattleUnitAttribute initAttribute, ObscuredInt[] abilityIds)
        {
            CreateUnitData initUnitData = new CreateUnitData();

            initUnitData.unit_type = UnitType.UNIT_HERO;
            initUnitData.player_id = master.PlayerId;
            initUnitData.card_id = unitId;
            initUnitData.player_camp = master.TeamId;
            initUnitData.skin_id = skinId;
            initUnitData.unit_in_x = nodeX;
            initUnitData.unit_in_y = nodeY;
            initUnitData.skill_id = abilityIds;
#if UNITY_EDITOR
            initUnitData.name = GameFramework.Utility.Text.Format("{0}-{1} 召唤物", master.Name, unitId.ToString());
#endif
            BaseUnit unit = UnitManager.CreateUnit(initUnitData);
            unit.ResetAtt(initAttribute);
            unit.ResetAbility(abilityIds);
            //UnitManager.AddUnit(unit);

            unit.Master = master;
            unit.LogicEulerAngles = master.LogicEulerAngles;

            return unit;
        }
        public static BaseUnit CreateThinker(CreateThinkerData createThinkerData)
        {
            BaseUnit unit = UnitManager.CreateUnit(createThinkerData);
            //UnitManager.AddUnit(unit);

            return unit;
        }
        public static BaseUnit CreateHero(PlayerBagCardNew playerBagCard, int nodeX, int nodeY)
        {
            CreateUnitData initUnitData = new CreateUnitData();

            initUnitData.unit_type = UnitType.UNIT_HERO;
            initUnitData.player_id = GameManager.GlobalData.SelfPlayer.RoleId;
            initUnitData.bag_index = playerBagCard.Index;
            initUnitData.card_id = playerBagCard.Id;
            initUnitData.level = playerBagCard.LevelId;
            initUnitData.player_camp = BattleData.SelfPlayer.TeamId;
            initUnitData.skin_id = playerBagCard.SkinId;
            initUnitData.score = playerBagCard.Power;
            initUnitData.unit_in_x = nodeX;
            initUnitData.unit_in_y = nodeY;
            initUnitData.name = playerBagCard.Name;

            initUnitData.skill_id = new ObscuredInt[playerBagCard.SkillCount];
            for (int i = 0; i < initUnitData.skill_id.Length; i++)
            {
                initUnitData.skill_id[i] = playerBagCard.GetSkillByIndex(i).Id;
            }

            BaseUnit unit = UnitManager.CreateUnit(initUnitData);
            //UnitManager.AddUnit(unit);

            return unit;
        }
        public static Boss CreatBoss(PlayerOccupation playerOccupation)
        {
            if (playerOccupation == null)
            {
                return null;
            }

            CreateBossData initUnitData = new CreateBossData();

            initUnitData.unit_type = UnitType.UNIT_BOSS;
            initUnitData.player_id = GameManager.GlobalData.SelfPlayer.RoleId;
            initUnitData.card_id = playerOccupation.Id;
            initUnitData.skin_id = playerOccupation.BattleUnitSkinId;

            initUnitData.level = playerOccupation.Level;
            initUnitData.player_camp = BattleData.SelfPlayer.TeamId;
            initUnitData.OccId = playerOccupation.Id;
            initUnitData.unit_in_x = 7;
            initUnitData.unit_in_y = 3;
            initUnitData.name = playerOccupation.Name;
            initUnitData.score = playerOccupation.Power;

            List<ObscuredInt> skillid = new List<ObscuredInt>();
            for (int i = 0; i < 2; i++)
            {
                PlayerOccupationArtifact playerOccupationArtifact = playerOccupation.GetEquipedArtifact(i);
                if (playerOccupationArtifact != null)
                {
                    skillid.Add(playerOccupationArtifact.SkillId);
                }
            }
            initUnitData.skill_id = skillid.ToArray();

            BaseUnit unit = UnitManager.CreateUnit(initUnitData);
            //UnitManager.AddUnit(unit);

            return (Boss)unit;
        }

        public static void ResetUnitAttributes(ResetUnitAttributeData resetUnitAttributeData)
        {
            for (int i = 0; i < UnitManager.Units.Count; i++)
            {
                BaseUnit unit = UnitManager.Units[i];
                if (unit.PlayerId == resetUnitAttributeData.player_id)
                {
                    if (unit.PlayerId == SelfPlayer.RoleId)
                    {
                        if (unit.UnitId == resetUnitAttributeData.unit_id)
                        {
                            unit.ResetAttAndAbility(resetUnitAttributeData);

                            break;
                        }
                    }
                    else
                    {
                        if (unit.LineupIndex == resetUnitAttributeData.unit_index)
                        {
                            unit.ResetAttAndAbility(resetUnitAttributeData);

                            break;
                        }
                    }
                }
            }
        }

        #region 格子


        public static BattleGrid CreateBattleGrid(int nodeSizeX, int nodeSizeY, DFixVector3 nodeCenterPosition, DFix64 nodeSize)
        {
            _battleGrid = new BattleGrid(nodeSizeX, nodeSizeY, nodeCenterPosition, nodeSize);

            return _battleGrid;
        }
        public static void DestroyBattleGrid()
        {
            if (_battleGrid != null)
            {
                _battleGrid.Release();
                _battleGrid = null;
            }
        }

        public static BattleNode FindNode(int nodeX, int nodeY)
        {
            if (_battleGrid == null || !_battleGrid.IsNodeInGrid(nodeX, nodeY))
            {
                return null;
            }

            return (BattleNode)_battleGrid[nodeX, nodeY];
        }
        public static BattleNode FindNodeByPoint(DFixVector3 point)
        {
            return _battleGrid.FindNode(point);
        }

        public static BattleNode FindNoItemNeighbourNearest(int nodeX, int nodeY)
        {
            return FindNoItemNeighbourNearest(_battleGrid[nodeX, nodeY], 1);
        }
        public static BattleNode FindNoItemNeighbourNearest(BattleNode node, int range)
        {
            if (node == null)
            {
                return null;
            }

            _battleGrid.GetNeighbourEmptyItems(node.X, node.Y, DistanceType.Euclidean, range, _cachedBattleNodes, null);
            if (_cachedBattleNodes.Count > 0)
            {
                if (_cachedBattleNodes.Count > 0)
                {
                    if (_cachedBattleNodes.Count > 1)
                    {
                        _cachedBattleNodes.Sort(
                            (a, b) =>
                            {
                                int asRangeX = node.MaxRange(a);
                                int bsRangeX = node.MaxRange(b);

                                return asRangeX.CompareTo(bsRangeX);
                            });
                    }

                    return _cachedBattleNodes[0];
                }
            }

            return null;
        }

        public static BattleNode FindWalkableNeighbourNearest(int nodeX, int nodeY)
        {
            return FindWalkableNeighbourNearest(_battleGrid[nodeX, nodeY]);
        }
        public static BattleNode FindWalkableNeighbourNearest(DFixVector3 point)
        {
            BattleNode node = _battleGrid.FindNode(point);
            if (node == null)
            {
                return null;
            }

            if (node.IsWalkable)
            {
                return node;
            }

            return FindWalkableNeighbourNearest(node);
        }
        public static BattleNode FindWalkableNeighbourNearest(BattleNode node)
        {
            return FindWalkableNeighbourNearest(node, _battleGrid.MaxSize);
        }
        public static BattleNode FindWalkableNeighbourNearest(BattleNode node, int range)
        {
            if (node == null)
            {
                return null;
            }

            if (node.IsWalkable)
            {
                return node;
            }

            _battleGrid.GetWalkableNeighbours(node.X, node.Y, DistanceType.Euclidean, range, _cachedBattleNodes, null);
            if (_cachedBattleNodes.Count > 0)
            {
                if (_cachedBattleNodes.Count > 1)
                {
                    _cachedBattleNodes.Sort(
                        (a, b) =>
                        {
                            int asRangeX = node.MaxRange(a);
                            int bsRangeX = node.MaxRange(b);

                            return asRangeX.CompareTo(bsRangeX);
                        });
                }

                return _cachedBattleNodes[0];
            }

            return null;
        }

        public static void FindWalkableNeighboursNearest(BattleNode node, int range, List<BattleNode> results)
        {
            results.Clear();

            if (node == null)
            {
                return;
            }

            _battleGrid.GetWalkableNeighbours(node.X, node.Y, DistanceType.Euclidean, range, results, null);
            if (results.Count > 0)
            {
                if (results.Count > 1)
                {
                    results.Sort(
                        (a, b) =>
                        {
                            int asRangeX = node.MaxRange(a);
                            int bsRangeX = node.MaxRange(b);

                            return asRangeX.CompareTo(bsRangeX);
                        });
                }
            }

            if (node.IsWalkable)
            {
                results.Insert(0, node);
            }
        }

        public static void SpawnUnit(BaseUnit unit)
        {

        }

        public static void UnspawnUnit(BaseUnit unit)
        {

        }

        public static void ShowLineupDragUnit(BaseUnit unit)
        {

        }

        #endregion

        private static List<BaseUnit> _findTargets = new List<BaseUnit>();

        public static BattleNode FindAttackMoveNode(BaseUnit source, BaseUnit target, bool bSort)
        {
            int currRangeX = Math.Abs(target.CurrNode.X - source.CurrNode.X);
            int currRangeY = Math.Abs(target.CurrNode.Y - source.CurrNode.Y);

            if (currRangeX <= source.CurrAtt.AttackNodeRange && currRangeY <= source.CurrAtt.AttackNodeRange)
            {
                return source.CurrNode;
            }

            int searchNodeRange = (int)source.CurrAtt.AttackNodeRange;
            while (searchNodeRange > 0)
            {
                _battleGrid.GetWalkableNeighbours(target.CurrNode.X, target.CurrNode.Y, DistanceType.Euclidean, searchNodeRange, _cachedBattleNodes, null);
                if (_cachedBattleNodes.Count > 0)
                {
                    if (bSort)
                    {
                        _cachedBattleNodes.Sort((a, b) =>
                        {
                            int asRangeX = Math.Abs(source.CurrNode.X - a.X);
                            int asRangeY = Math.Abs(source.CurrNode.Y - a.Y);

                            int bsRangeX = Math.Abs(source.CurrNode.X - b.X);
                            int bsRangeY = Math.Abs(source.CurrNode.Y - b.Y);

                            return (asRangeX + asRangeY).CompareTo((bsRangeX + bsRangeY));
                        });
                    }

                    for (int k = 0; k < _cachedBattleNodes.Count; k++)
                    {
                        List<BattleNode> path = _battleGrid.FindAPath(source.CurrNode.X, source.CurrNode.Y, _cachedBattleNodes[k].X, _cachedBattleNodes[k].Y);
                        if (path.Count > 0)
                        {
                            return path[0];
                        }

                        //List<Point> parh = _grid.FindPath(source.CurrNode.X, source.CurrNode.Y, _nodes[k].X, _nodes[k].Y);
                        //if (parh != null && parh.Count > 1)
                        //{
                        //    BattleNode node = _grid[parh[1].row, parh[1].column];

                        //    int xRange = Math.Abs(target.CurrNode.X - node.X);
                        //    int yRange = Math.Abs(target.CurrNode.Y - node.Y);

                        //    if (xRange < currRangeX || yRange < currRangeY)
                        //    {
                        //        int moveX = 0;
                        //        int moveY = 0;

                        //        if (node.X > source.CurrNode.X)
                        //        {
                        //            moveX = 1;
                        //        }
                        //        else if (node.X < source.CurrNode.X)
                        //        {
                        //            moveX = -1;
                        //        }

                        //        if (node.Y > source.CurrNode.Y)
                        //        {
                        //            moveY = 1;
                        //        }
                        //        else if (node.Y < source.CurrNode.Y)
                        //        {
                        //            moveY = -1;
                        //        }

                        //        node = _grid[source.CurrNode.X + moveX, source.CurrNode.Y + moveY];
                        //        return node;
                        //    }

                        //    return null;
                        //}
                    }
                }

                searchNodeRange = 0;
            }

            return null;
        }
        public static List<BattleNode> FindPath(BattleNode start, BattleNode end, DistanceType distance = DistanceType.Euclidean)
        {
            return _battleGrid.FindAPath(start, end, distance);
        }

        public static void FindActionTargets(BaseUnit source, BattleTarget center, MulTargetInfo mulTargetInfo, LinkedList<BattleTarget> targets)
        {
            targets.Clear();

            if ((mulTargetInfo.UnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_SELF) == AbilityUnitTargetFlag.UNIT_TARGET_FLAG_SELF)
            {
                if (source.Type == UnitType.UNIT_THINKER || !source.IsSpawnedToLineup)
                {
                    return;
                }

                if (source.IsUnselectable)
                {
                    if ((mulTargetInfo.ExcludedUnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE) == AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE)
                    {
                        return;
                    }
                }

                if (!BattleData.IsUnitTargetValid(source, source, mulTargetInfo.UnitTargetTeams, mulTargetInfo.UnitTargetTypes, mulTargetInfo.UnitTargetFlags, mulTargetInfo.ExcludedUnitTargetFlags,
                        mulTargetInfo.UnitTargetTraits, mulTargetInfo.ExcludedUnitTargetTraits, mulTargetInfo.UnitTargetModifierGroups, mulTargetInfo.ExcludedUnitTargetModifierGroups))
                {
                    return;
                }

                targets.AddLast(new BattleUnitTarget(source));

                return;
            }

            BaseUnit centerUnit = null;
            BattleNode centerNode = null;
            DFixVector3 centerPos = DFixVector3.Zero;

            if (mulTargetInfo.NodeRange != -1)
            {
                if (center.GetType() == typeof(BattlePointTarget))
                {
                    centerPos = (DFixVector3)center.Target;
                    centerNode = BattleGrid.FindNode(centerPos);
                    centerUnit = centerNode.Unit;
                }
                else if (center.GetType() == typeof(BattleUnitTarget))
                {
                    centerUnit = (BaseUnit)center.Target;
                    centerPos = centerUnit.LogicPosition;
                    centerNode = BattleGrid.FindNode(centerPos);
                }
                else if (center.GetType() == typeof(BattleNodeTarget))
                {
                    centerNode = (BattleNode)center.Target;
                    centerPos = centerNode.WorldPosition;
                    centerUnit = centerNode.Unit;
                }
                else
                {
                    return;
                }
            }
            else if (mulTargetInfo.RadiusRange != -DFix64.One)
            {
                if (center.GetType() == typeof(BattlePointTarget))
                {
                    centerPos = (DFixVector3)center.Target;
                    centerNode = BattleGrid.FindNode(centerPos);
                    centerUnit = centerNode.Unit;
                }
                else if (center.GetType() == typeof(BattleUnitTarget))
                {
                    centerUnit = (BaseUnit)center.Target;
                    centerPos = centerUnit.LogicPosition;
                    centerNode = BattleGrid.FindNode(centerPos);
                }
                else if (center.GetType() == typeof(BattleNodeTarget))
                {
                    centerNode = (BattleNode)center.Target;
                    centerPos = centerNode.WorldPosition;
                    centerUnit = centerNode.Unit;
                }
                else
                {
                    return;
                }
            }

            if ((mulTargetInfo.UnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_CENTER) == AbilityUnitTargetFlag.UNIT_TARGET_FLAG_CENTER)
            {
                if (centerUnit == null)
                {
                    return;
                }

                if (centerUnit.Type == UnitType.UNIT_THINKER || !centerUnit.IsSpawnedToLineup)
                {
                    return;
                }

                if (centerUnit.IsUnselectable)
                {
                    if ((mulTargetInfo.ExcludedUnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE) == AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE)
                    {
                        return;
                    }
                }

                if (!BattleData.IsUnitTargetValid(source, centerUnit, mulTargetInfo.UnitTargetTeams, mulTargetInfo.UnitTargetTypes, mulTargetInfo.UnitTargetFlags, mulTargetInfo.ExcludedUnitTargetFlags,
                        mulTargetInfo.UnitTargetTraits, mulTargetInfo.ExcludedUnitTargetTraits, mulTargetInfo.UnitTargetModifierGroups, mulTargetInfo.ExcludedUnitTargetModifierGroups))
                {
                    return;
                }

                targets.AddLast(new BattleUnitTarget(centerUnit));

                return;
            }

            for (int j = 0; j < UnitManager.Units.Count; j++)
            {
                BaseUnit target = UnitManager.Units[j];
                if (target.Type == UnitType.UNIT_THINKER || !target.IsSpawnedToLineup)
                {
                    continue;
                }

                if (target.IsUnselectable)
                {
                    if ((mulTargetInfo.ExcludedUnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE) == AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE)
                    {
                        continue;
                    }
                }

                if (target == centerUnit)
                {
                    if ((mulTargetInfo.ExcludedUnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_CENTER) != 0)
                    {
                        continue;
                    }
                }

                if (mulTargetInfo.NodeRange != -1)
                {
                    if (centerNode.MaxRange(target.CurrNode) > mulTargetInfo.NodeRange)
                    {
                        continue;
                    }
                }
                else if (mulTargetInfo.RadiusRange != -DFix64.One)
                {
                    if (DFixVector3.Distance(centerPos, target.LogicPosition) > mulTargetInfo.RadiusRange)
                    {
                        continue;
                    }
                }

                if (!BattleData.IsUnitTargetValid(source, target, mulTargetInfo.UnitTargetTeams, mulTargetInfo.UnitTargetTypes, mulTargetInfo.UnitTargetFlags, mulTargetInfo.ExcludedUnitTargetFlags,
                        mulTargetInfo.UnitTargetTraits, mulTargetInfo.ExcludedUnitTargetTraits, mulTargetInfo.UnitTargetModifierGroups, mulTargetInfo.ExcludedUnitTargetModifierGroups))
                {
                    continue;
                }

                if (targets.Count > 0 && mulTargetInfo.UnitTargetSort != AbilityUnitTargetSort.UNIT_TARGET_SORT_NONE)
                {
                    AddSortUnit(source, source, mulTargetInfo.UnitTargetSort, targets, target);
                }
                else
                {
                    targets.AddLast(new BattleUnitTarget(target));
                }
            }
        }
        public static void FindActionTargets(BaseUnit source, BattleTarget center, int nodeRange, MulTargetInfo mulTargetInfo, LinkedList<BattleTarget> targets, bool findNodeOnly)
        {
            if (nodeRange == -1)
            {
                FindActionTargets(source, center, mulTargetInfo, targets);
                return;
            }

            targets.Clear();

            if ((mulTargetInfo.UnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_SELF) == AbilityUnitTargetFlag.UNIT_TARGET_FLAG_SELF)
            {
                if (source.Type == UnitType.UNIT_THINKER || !source.IsSpawnedToLineup)
                {
                    return;
                }

                if (source.IsUnselectable)
                {
                    if ((mulTargetInfo.ExcludedUnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE) == AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE)
                    {
                        return;
                    }
                }

                if (!BattleData.IsUnitTargetValid(source, source, mulTargetInfo.UnitTargetTeams, mulTargetInfo.UnitTargetTypes, mulTargetInfo.UnitTargetFlags, mulTargetInfo.ExcludedUnitTargetFlags,
                        mulTargetInfo.UnitTargetTraits, mulTargetInfo.ExcludedUnitTargetTraits, mulTargetInfo.UnitTargetModifierGroups, mulTargetInfo.ExcludedUnitTargetModifierGroups))
                {
                    return;
                }

                targets.AddLast(new BattleUnitTarget(source));

                return;
            }

            BaseUnit centerUnit = null;
            BattleNode centerNode = null;
            DFixVector3 centerPos = DFixVector3.Zero;

            if (mulTargetInfo.NodeRange != -1)
            {
                if (center.GetType() == typeof(BattlePointTarget))
                {
                    centerPos = (DFixVector3)center.Target;
                    centerNode = _battleGrid.FindNode(centerPos);
                    centerUnit = centerNode.Unit;
                }
                else if (center.GetType() == typeof(BattleUnitTarget))
                {
                    centerUnit = (BaseUnit)center.Target;
                    centerPos = centerUnit.LogicPosition;
                    centerNode = centerUnit.CurrNode;
                }
                else if (center.GetType() == typeof(BattleNodeTarget))
                {
                    centerNode = (BattleNode)center.Target;
                    centerPos = centerNode.WorldPosition;
                    centerUnit = centerNode.Unit;
                }
                else
                {
                    return;
                }
            }
            else if (mulTargetInfo.RadiusRange != -DFix64.One)
            {
                if (center.GetType() == typeof(BattlePointTarget))
                {
                    centerPos = (DFixVector3)center.Target;
                    centerNode = BattleGrid.FindNode(centerPos);
                    centerUnit = centerNode.Unit;
                }
                else if (center.GetType() == typeof(BattleUnitTarget))
                {
                    centerUnit = (BaseUnit)center.Target;
                    centerPos = centerUnit.LogicPosition;
                    centerNode = BattleGrid.FindNode(centerPos);
                }
                else if (center.GetType() == typeof(BattleNodeTarget))
                {
                    centerNode = (BattleNode)center.Target;
                    centerPos = centerNode.WorldPosition;
                    centerUnit = centerNode.Unit;
                }
                else
                {
                    return;
                }
            }

            if ((mulTargetInfo.UnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_CENTER) == AbilityUnitTargetFlag.UNIT_TARGET_FLAG_CENTER)
            {
                if (centerUnit == null)
                {
                    return;
                }

                if (centerUnit.Type == UnitType.UNIT_THINKER || !centerUnit.IsSpawnedToLineup)
                {
                    return;
                }

                if (centerUnit.IsUnselectable)
                {
                    if ((mulTargetInfo.ExcludedUnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE) == AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE)
                    {
                        return;
                    }
                }

                if (!BattleData.IsUnitTargetValid(source, centerUnit, mulTargetInfo.UnitTargetTeams, mulTargetInfo.UnitTargetTypes, mulTargetInfo.UnitTargetFlags, mulTargetInfo.ExcludedUnitTargetFlags,
                        mulTargetInfo.UnitTargetTraits, mulTargetInfo.ExcludedUnitTargetTraits, mulTargetInfo.UnitTargetModifierGroups, mulTargetInfo.ExcludedUnitTargetModifierGroups))
                {
                    return;
                }

                targets.AddLast(new BattleUnitTarget(centerUnit));

                return;
            }

            if (findNodeOnly)
            {
                _findTargets.Clear();
                BattleGrid.GetNeighbourUnits(centerNode.X, centerNode.Y, DistanceType.Euclidean, nodeRange == -1 ? BattleGrid.MaxSize : nodeRange, _findTargets);
                if (centerNode.Unit != null)
                {
                    _findTargets.Add(centerNode.Unit);
                }
            }

            for (int i = 0, count = findNodeOnly ? _findTargets.Count : UnitManager.Units.Count; i < count; i++)
            {
                BaseUnit target = findNodeOnly ? _findTargets[i] : UnitManager.Units[i];
                if (target.Type == UnitType.UNIT_THINKER || !target.IsSpawnedToLineup)
                {
                    continue;
                }

                if (target.IsUnselectable)
                {
                    if ((mulTargetInfo.ExcludedUnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE) == AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE)
                    {
                        continue;
                    }
                }

                if (target == centerUnit)
                {
                    if ((mulTargetInfo.ExcludedUnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_CENTER) != 0)
                    {
                        continue;
                    }
                }

                if (!findNodeOnly)
                {
                    if (nodeRange != -1)
                    {
                        if (source.CurrNode.MaxRange(target.CurrNode) > nodeRange)
                        {
                            continue;
                        }
                    }
                }

                if (!BattleData.IsUnitTargetValid(source, target, mulTargetInfo.UnitTargetTeams, mulTargetInfo.UnitTargetTypes, mulTargetInfo.UnitTargetFlags, mulTargetInfo.ExcludedUnitTargetFlags,
                        mulTargetInfo.UnitTargetTraits, mulTargetInfo.ExcludedUnitTargetTraits, mulTargetInfo.UnitTargetModifierGroups, mulTargetInfo.ExcludedUnitTargetModifierGroups))
                {
                    continue;
                }

                if (targets.Count > 0 && mulTargetInfo.UnitTargetSort != AbilityUnitTargetSort.UNIT_TARGET_SORT_NONE)
                {
                    AddSortUnit(source, source, mulTargetInfo.UnitTargetSort, targets, target);
                }
                else
                {
                    targets.AddLast(new BattleUnitTarget(target));
                }
            }
        }

        public static void FindTargets(BaseUnit source, BaseUnit centerSource, MulTargetInfo mulTargetInfo, LinkedList<BaseUnit> targets)
        {
            targets.Clear();

            if (mulTargetInfo == null)
            {
                return;
            }

            for (int i = 0, count = UnitManager.Units.Count; i < count; i++)
            {
                BaseUnit target = UnitManager.Units[i];
                if (target.Type == UnitType.UNIT_THINKER || !target.IsSpawnedToLineup)
                {
                    continue;
                }

                if (target.IsUnselectable)
                {
                    if ((mulTargetInfo.ExcludedUnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE) != 0)
                    {
                        continue;
                    }
                }

                if (!IsUnitTargetValid(source, target, mulTargetInfo.UnitTargetTeams, mulTargetInfo.UnitTargetTypes, mulTargetInfo.UnitTargetFlags,
                    mulTargetInfo.ExcludedUnitTargetFlags, mulTargetInfo.UnitTargetTraits, mulTargetInfo.ExcludedUnitTargetTraits, mulTargetInfo.UnitTargetModifierGroups,
                    mulTargetInfo.ExcludedUnitTargetModifierGroups))
                {
                    continue;
                }

                if (mulTargetInfo.NodeRange != -1)
                {
                    if (centerSource.CurrNode.MaxRange(target.CurrNode) > mulTargetInfo.NodeRange)
                    {
                        continue;
                    }
                }
                else if (mulTargetInfo.RadiusRange > DFix64.Zero)
                {
                    if (centerSource.CurrNode.Distance(target.CurrNode) > mulTargetInfo.RadiusRange)
                    {
                        continue;
                    }
                }

                if (mulTargetInfo.UnitTargetSort != AbilityUnitTargetSort.UNIT_TARGET_SORT_NONE && !mulTargetInfo.IsRandom && targets.Count > 0)
                {
                    AddSortUnit(source, centerSource, mulTargetInfo.UnitTargetSort, targets, target);
                }
                else
                {
                    targets.AddLast(target);
                }
            }

            if (mulTargetInfo.MaxNumber > 0 && targets.Count > mulTargetInfo.MaxNumber)
            {
                if (mulTargetInfo.IsRandom)
                {
                    HashSet<int> removeIndexs = new HashSet<int>();
                    for (int i = 0, count = targets.Count - mulTargetInfo.MaxNumber; i < count; i++)
                    {
                        int randomIndex = BattleData.SRandom.RangeI(0, targets.Count);
                        LinkedListNode<BaseUnit> first = targets.First;
                        int index = 0;
                        while (first != null)
                        {
                            if (index == randomIndex)
                            {
                                targets.Remove(first);
                                break;
                            }

                            first = first.Next;
                            index++;
                        }
                    }
                }
                else
                {
                    while (targets.Count > mulTargetInfo.MaxNumber)
                    {
                        targets.RemoveLast();
                    }
                }
            }
        }
        public static void FindTargets(BaseUnit source, BaseUnit centerSource, MulTargetInfo mulTargetInfo, LinkedList<BaseUnit> targets, Func<BaseUnit, BaseUnit, bool> checkTarget)
        {
            if (checkTarget == null)
            {
                FindTargets(source, centerSource, mulTargetInfo, targets);
                return;
            }

            targets.Clear();

            if (mulTargetInfo == null)
            {
                return;
            }

            for (int i = 0, count = UnitManager.Units.Count; i < count; i++)
            {
                BaseUnit target = UnitManager.Units[i];
                if (target.Type == UnitType.UNIT_THINKER || !target.IsSpawnedToLineup)
                {
                    continue;
                }

                if (!checkTarget(source, target))
                {
                    continue;
                }

                if (target.IsUnselectable)
                {
                    if ((mulTargetInfo.ExcludedUnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE) != 0)
                    {
                        continue;
                    }
                }

                if (!IsUnitTargetValid(source, target, mulTargetInfo.UnitTargetTeams, mulTargetInfo.UnitTargetTypes, mulTargetInfo.UnitTargetFlags,
                     mulTargetInfo.ExcludedUnitTargetFlags, mulTargetInfo.UnitTargetTraits, mulTargetInfo.ExcludedUnitTargetTraits, mulTargetInfo.UnitTargetModifierGroups,
                     mulTargetInfo.ExcludedUnitTargetModifierGroups))
                {
                    continue;
                }

                if (mulTargetInfo.NodeRange != -1)
                {
                    if (centerSource.CurrNode.MaxRange(target.CurrNode) > mulTargetInfo.NodeRange)
                    {
                        continue;
                    }
                }
                else if (mulTargetInfo.RadiusRange > DFix64.Zero)
                {
                    if (centerSource.CurrNode.Distance(target.CurrNode) > mulTargetInfo.RadiusRange)
                    {
                        continue;
                    }
                }

                if (mulTargetInfo.UnitTargetSort != AbilityUnitTargetSort.UNIT_TARGET_SORT_NONE && !mulTargetInfo.IsRandom && targets.Count > 0)
                {
                    AddSortUnit(source, centerSource, mulTargetInfo.UnitTargetSort, targets, target);
                }
                else
                {
                    targets.AddLast(target);
                }
            }

            if (mulTargetInfo.MaxNumber > 0 && targets.Count > mulTargetInfo.MaxNumber)
            {
                if (mulTargetInfo.IsRandom)
                {
                    HashSet<int> removeIndexs = new HashSet<int>();
                    for (int i = 0, count = targets.Count - mulTargetInfo.MaxNumber; i < count; i++)
                    {
                        int randomIndex = BattleData.SRandom.RangeI(0, targets.Count);
                        LinkedListNode<BaseUnit> first = targets.First;
                        int index = 0;
                        while (first != null)
                        {
                            if (index == randomIndex)
                            {
                                targets.Remove(first);
                                break;
                            }

                            first = first.Next;
                            index++;
                        }
                    }
                }
                else
                {
                    while (targets.Count > mulTargetInfo.MaxNumber)
                    {
                        targets.RemoveLast();
                    }
                }
            }
        }

        public static void AddSortUnit(BaseUnit source, BaseUnit centerSource, AbilityUnitTargetSort sort, LinkedList<BattleTarget> targets, BaseUnit target)
        {
            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                BattleUnitTarget battleUnitTarget = (BattleUnitTarget)first.Value;
                BaseUnit baseUnit = (BaseUnit)battleUnitTarget.Target;

                switch (sort)
                {
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_NEAREST:
                        {
                            int disT = centerSource.CurrNode.GetDistance(target.CurrNode);
                            int disF = centerSource.CurrNode.GetDistance(baseUnit.CurrNode);

                            if (disT <= disF)
                            {
                                if (disT < disF || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_CURR_ATTACK:
                        {
                            if (target.CurrAtt.Attack >= baseUnit.CurrAtt.Attack)
                            {
                                if (target.CurrAtt.Attack > baseUnit.CurrAtt.Attack || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_INIT_ATTACK:
                        {
                            if (target.InitAtt.Attack >= baseUnit.InitAtt.Attack)
                            {
                                if (target.InitAtt.Attack > baseUnit.InitAtt.Attack || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_CURR_LEVEL:
                        {
                            if (target.Level >= baseUnit.Level)
                            {
                                if (target.Level > baseUnit.Level || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_LEAST_CURR_HP:
                        {
                            if (target.CurrAtt.Hp <= baseUnit.CurrAtt.Hp)
                            {
                                if (target.CurrAtt.Hp < baseUnit.CurrAtt.Hp || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_CURR_HP:
                        {
                            if (target.CurrAtt.Hp >= baseUnit.CurrAtt.Hp)
                            {
                                if (target.CurrAtt.Hp > baseUnit.CurrAtt.Hp || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_FARTHEST:
                        {
                            int disT = centerSource.CurrNode.GetDistance(target.CurrNode);
                            int disF = centerSource.CurrNode.GetDistance(baseUnit.CurrNode);

                            if (disT >= disF)
                            {
                                if (disT > disF || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_TOTAL_DAMAGE_OUT_MAX:
                        {
                            if (target.StatisticsData.TotalDamageOut >= baseUnit.StatisticsData.TotalDamageOut)
                            {
                                if (target.StatisticsData.TotalDamageOut > baseUnit.StatisticsData.TotalDamageOut || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_TOTAL_DAMAGE_OUT_MIN:
                        {
                            if (target.StatisticsData.TotalDamageOut <= baseUnit.StatisticsData.TotalDamageOut)
                            {
                                if (target.StatisticsData.TotalDamageOut < baseUnit.StatisticsData.TotalDamageOut || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_TOTAL_DAMAGE_IN_MAX:
                        {
                            if (target.StatisticsData.TotalDamageIn >= baseUnit.StatisticsData.TotalDamageIn)
                            {
                                if (target.StatisticsData.TotalDamageIn > baseUnit.StatisticsData.TotalDamageIn || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_TOTAL_DAMAGE_IN_MIN:
                        {
                            if (target.StatisticsData.TotalDamageIn <= baseUnit.StatisticsData.TotalDamageIn)
                            {
                                if (target.StatisticsData.TotalDamageIn < baseUnit.StatisticsData.TotalDamageIn || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_LEAST_CURR_HPPCT:
                        {
                            if (target.CurrAtt.HpPct <= baseUnit.CurrAtt.HpPct)
                            {
                                if (target.CurrAtt.HpPct < baseUnit.CurrAtt.HpPct || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_CURR_DEFENSE:
                        {
                            if (target.CurrAtt.Defense >= baseUnit.CurrAtt.Defense)
                            {
                                if (target.CurrAtt.Defense > baseUnit.CurrAtt.Defense || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_INIT_DEFENSE:
                        {
                            if (target.InitAtt.Defense >= baseUnit.InitAtt.Defense)
                            {
                                if (target.InitAtt.Defense > baseUnit.InitAtt.Defense || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_POWER:
                        {
                            if (target.BattleScore >= baseUnit.BattleScore)
                            {
                                if (target.BattleScore > baseUnit.BattleScore || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_LEAST_POWER:
                        {
                            if (target.BattleScore <= baseUnit.BattleScore)
                            {
                                if (target.BattleScore < baseUnit.BattleScore || target.RandomSpeed < baseUnit.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, new BattleUnitTarget(target));
                                }
                                else
                                {
                                    first = targets.AddAfter(first, new BattleUnitTarget(target));
                                }
                            }
                        }
                        break;
                }

                baseUnit = (BaseUnit)first.Value.Target;
                if (baseUnit == target)
                {
                    break;
                }

                first = first.Next;
            }

            if (first == null)
            {
                targets.AddLast(new BattleUnitTarget(target));
            }
        }
        public static void AddSortUnit(BaseUnit source, BaseUnit centerSource, AbilityUnitTargetSort sort, LinkedList<BaseUnit> targets, BaseUnit target)
        {
            LinkedListNode<BaseUnit> first = targets.First;
            while (first != null)
            {
                switch (sort)
                {
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_NEAREST:
                        {
                            int disT = centerSource.CurrNode.GetDistance(target.CurrNode);
                            int disF = centerSource.CurrNode.GetDistance(first.Value.CurrNode);

                            if (disT <= disF)
                            {
                                if (disT < disF || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_CURR_ATTACK:
                        {
                            if (target.CurrAtt.Attack >= first.Value.CurrAtt.Attack)
                            {
                                if (target.CurrAtt.Attack > first.Value.CurrAtt.Attack || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_INIT_ATTACK:
                        {
                            if (target.InitAtt.Attack >= first.Value.InitAtt.Attack)
                            {
                                if (target.InitAtt.Attack > first.Value.InitAtt.Attack || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_CURR_LEVEL:
                        {
                            if (target.Level >= first.Value.Level)
                            {
                                if (target.Level > first.Value.Level || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_LEAST_CURR_HP:
                        {
                            if (target.CurrAtt.Hp <= first.Value.CurrAtt.Hp)
                            {
                                if (target.CurrAtt.Hp < first.Value.CurrAtt.Hp || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_CURR_HP:
                        {
                            if (target.CurrAtt.Hp >= first.Value.CurrAtt.Hp)
                            {
                                if (target.CurrAtt.Hp > first.Value.CurrAtt.Hp || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_FARTHEST:
                        {
                            int disT = centerSource.CurrNode.GetDistance(target.CurrNode);
                            int disF = centerSource.CurrNode.GetDistance(first.Value.CurrNode);

                            if (disT >= disF)
                            {
                                if (disT > disF || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_TOTAL_DAMAGE_OUT_MAX:
                        {
                            if (target.StatisticsData.TotalDamageOut >= first.Value.StatisticsData.TotalDamageOut)
                            {
                                if (target.StatisticsData.TotalDamageOut > first.Value.StatisticsData.TotalDamageOut || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_TOTAL_DAMAGE_OUT_MIN:
                        {
                            if (target.StatisticsData.TotalDamageOut <= first.Value.StatisticsData.TotalDamageOut)
                            {
                                if (target.StatisticsData.TotalDamageOut < first.Value.StatisticsData.TotalDamageOut || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_TOTAL_DAMAGE_IN_MAX:
                        {
                            if (target.StatisticsData.TotalDamageIn >= first.Value.StatisticsData.TotalDamageIn)
                            {
                                if (target.StatisticsData.TotalDamageIn > first.Value.StatisticsData.TotalDamageIn || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_TOTAL_DAMAGE_IN_MIN:
                        {
                            if (target.StatisticsData.TotalDamageIn <= first.Value.StatisticsData.TotalDamageIn)
                            {
                                if (target.StatisticsData.TotalDamageIn < first.Value.StatisticsData.TotalDamageIn || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_LEAST_CURR_HPPCT:
                        {
                            if (target.CurrAtt.HpPct <= first.Value.CurrAtt.HpPct)
                            {
                                if (target.CurrAtt.HpPct < first.Value.CurrAtt.HpPct || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_CURR_DEFENSE:
                        {
                            if (target.CurrAtt.Defense >= first.Value.CurrAtt.Defense)
                            {
                                if (target.CurrAtt.Defense > first.Value.CurrAtt.Defense || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_INIT_DEFENSE:
                        {
                            if (target.InitAtt.Defense >= first.Value.InitAtt.Defense)
                            {
                                if (target.InitAtt.Defense > first.Value.InitAtt.Defense || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_MOST_POWER:
                        {
                            if (target.BattleScore >= first.Value.BattleScore)
                            {
                                if (target.BattleScore > first.Value.BattleScore || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                    case AbilityUnitTargetSort.UNIT_TARGET_SORT_LEAST_POWER:
                        {
                            if (target.BattleScore <= first.Value.BattleScore)
                            {
                                if (target.BattleScore < first.Value.BattleScore || target.RandomSpeed < first.Value.RandomSpeed)
                                {
                                    first = targets.AddBefore(first, target);
                                }
                                else
                                {
                                    first = targets.AddAfter(first, target);
                                }
                            }
                        }
                        break;
                }

                if (first.Value == target)
                {
                    break;
                }

                first = first.Next;
            }

            if (first == null)
            {
                targets.AddLast(target);
            }
        }

        public static bool IsUnitTargetValid(BaseUnit source, BaseUnit target, MulTargetInfo mulTargetInfo)
        {
            if (mulTargetInfo == null)
            {
                return false;
            }

            return IsUnitTargetValid(source, target, mulTargetInfo.UnitTargetTeams, mulTargetInfo.UnitTargetTypes, mulTargetInfo.UnitTargetFlags,
                mulTargetInfo.ExcludedUnitTargetFlags, mulTargetInfo.UnitTargetTraits, mulTargetInfo.ExcludedUnitTargetTraits, mulTargetInfo.UnitTargetModifierGroups, mulTargetInfo.ExcludedUnitTargetModifierGroups);
        }
        public static bool IsUnitTargetValid(BaseUnit source, BaseUnit target, AbilityUnitTargetTeam targetTeams, AbilityUnitTargetType targetTypes,
            AbilityUnitTargetFlag targetFlags, AbilityUnitTargetFlag excludedTargetFlags, HashSet<int> traits, HashSet<int> excludedTraits,
            int[] modifierGroups, int[] excludedModifierGroups)
        {
            if (targetTeams == AbilityUnitTargetTeam.UNIT_TARGET_TEAM_NONE || targetTypes == AbilityUnitTargetType.UNIT_TARGET_NONE)
            {
                return false;
            }

            if ((targetTeams & AbilityUnitTargetTeam.UNIT_TARGET_TEAM_BOTH) == 0)
            {
                if ((targetTeams & AbilityUnitTargetTeam.UNIT_TARGET_TEAM_FRIENDLY) == 0)
                {
                    if (target.TeamId == source.TeamId)
                    {
                        return false;
                    }
                }

                if ((targetTeams & AbilityUnitTargetTeam.UNIT_TARGET_TEAM_ENEMY) == 0)
                {
                    if (target.TeamId != source.TeamId)
                    {
                        return false;
                    }
                }
            }

            if ((targetTypes & AbilityUnitTargetType.UNIT_TARGET_ALL) == 0)
            {
                if ((targetTypes & AbilityUnitTargetType.UNIT_TARGET_HERO) == 0)
                {
                    if (target.Type == UnitType.UNIT_HERO)
                    {
                        return false;
                    }
                }

                if ((targetTypes & AbilityUnitTargetType.UNIT_TARGET_BOSS) == 0)
                {
                    if (target.Type == UnitType.UNIT_BOSS)
                    {
                        return false;
                    }
                }
            }

            if (targetFlags != AbilityUnitTargetFlag.UNIT_TARGET_FLAG_NONE)
            {
                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_ATTACK_UNIT) != 0)
                {
                    if (target != source.AttackUnitTarget)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_ATTACKER) != 0)
                {
                    if (target.AttackUnitTarget != source)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_DEAD) != 0)
                {
                    if (!target.IsDeadState)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_MELEE_ONLY) != 0)
                {
                    if (target.AttackCapability != UnitAttackCapabilityType.UNIT_CAP_MELEE_ATTACK)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_RANGED_ONLY) != 0)
                {
                    if (target.AttackCapability != UnitAttackCapabilityType.UNIT_CAP_RANGED_ATTACK)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_NOT_ILLUSIONS) != 0)
                {
                    if (target.IsIllusion)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_NOT_SUMMONED) != 0)
                {
                    if (target.IsSummoned)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_HAVE_PURGABLE_BUFF) != 0)
                {
                    if (target.GetPurgableBuffCount() == 0)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_HAVE_PURGABLE_DEBUFF) != 0)
                {
                    if (target.GetPurgableDebuffCount() == 0)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_MYILLUSION) != 0)
                {
                    if (!target.IsIllusion || target.Master != source)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_MYSUMMONED) != 0)
                {
                    if (!target.IsSummoned || target.Master != source)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_INVISIBLE) != 0)
                {
                    if (!target.IsInvisible)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_INVINCIBLE) != 0)
                {
                    if (!target.IsInvincible)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_SELF) != 0)
                {
                    if (target != source)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE) != 0)
                {
                    if (!target.IsUnselectable)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE_ATTACK) != 0)
                {
                    if (!target.IsUnselectableAttack)
                    {
                        return false;
                    }
                }

                if ((targetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE_ABILITY) != 0)
                {
                    if (!target.IsUnselectableAbility)
                    {
                        return false;
                    }
                }
            }

            if (excludedTargetFlags != AbilityUnitTargetFlag.UNIT_TARGET_FLAG_NONE)
            {
                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_ATTACK_UNIT) != 0)
                {
                    if (target == source.AttackUnitTarget)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_ATTACKER) != 0)
                {
                    if (target.AttackUnitTarget == source)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_DEAD) != 0)
                {
                    if (target.IsDeadState)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_MELEE_ONLY) != 0)
                {
                    if (target.AttackCapability == UnitAttackCapabilityType.UNIT_CAP_MELEE_ATTACK)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_RANGED_ONLY) != 0)
                {
                    if (target.AttackCapability == UnitAttackCapabilityType.UNIT_CAP_RANGED_ATTACK)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_NOT_ILLUSIONS) != 0)
                {
                    if (!target.IsIllusion)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_NOT_SUMMONED) != 0)
                {
                    if (!target.IsSummoned)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_HAVE_PURGABLE_BUFF) != 0)
                {
                    if (target.GetPurgableBuffCount() > 0)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_HAVE_PURGABLE_DEBUFF) != 0)
                {
                    if (target.GetPurgableDebuffCount() > 0)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_MYILLUSION) != 0)
                {
                    if (target.IsIllusion && target.Master == source)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_MYSUMMONED) != 0)
                {
                    if (target.IsSummoned && target.Master == source)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_INVISIBLE) != 0)
                {
                    if (target.IsInvisible)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_INVINCIBLE) != 0)
                {
                    if (target.IsInvincible)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_SELF) != 0)
                {
                    if (target == source)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE) != 0)
                {
                    if (target.IsUnselectable)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE_ATTACK) != 0)
                {
                    if (target.IsUnselectableAttack)
                    {
                        return false;
                    }
                }

                if ((excludedTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE_ABILITY) != 0)
                {
                    if (target.IsUnselectableAbility)
                    {
                        return false;
                    }
                }
            }

            if (traits != null && traits.Count > 0)
            {
                if (!traits.Contains(target.TraitType))
                {
                    return false;
                }
            }

            if (excludedTraits != null && excludedTraits.Count > 0)
            {
                if (excludedTraits.Contains(target.TraitType))
                {
                    return false;
                }
            }

            if (modifierGroups != null && modifierGroups.Length > 0)
            {
                bool isModifierGroupMatched = false;
                for (int i = 0; i < modifierGroups.Length; i++)
                {
                    if (target.GetModifierGroupCount(modifierGroups[i]) > 0)
                    {
                        isModifierGroupMatched = true;
                        break;
                    }
                }

                if (!isModifierGroupMatched)
                {
                    return false;
                }
            }

            if (excludedModifierGroups != null && excludedModifierGroups.Length > 0)
            {
                for (int i = 0; i < excludedModifierGroups.Length; i++)
                {
                    if (target.GetModifierGroupCount(excludedModifierGroups[i]) > 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static void CheckResult(BattleCheckResultType checkResultType)
        {
            if (Result != BattleResult.None)
            {
                return;
            }

            if (BattleData.DrBattleNode.WinConditions != null && BattleData.DrBattleNode.WinConditions.Length > 0)
            {
                for (int i = 0; i < BattleData.DrBattleNode.WinConditions.Length; i++)
                {
                    switch (BattleData.DrBattleNode.WinConditions[i].Type)
                    {
                        case 1:
                            {
                                if (checkResultType != BattleCheckResultType.HeroDead)
                                {
                                    continue;
                                }

                                int enemyNumber = 0;
                                for (int j = 0, count = UnitManager.Units.Count; j < count; j++)
                                {
                                    BaseUnit unit = UnitManager.Units[j];
                                    if (unit.Type == UnitType.UNIT_NONE || unit.Type == UnitType.UNIT_THINKER)
                                    {
                                        continue;
                                    }

                                    if (unit.IsDeadState || unit.TeamId == BattleData.SelfPlayer.TeamId || unit.IsIllusion || unit.IsSummoned)
                                    {
                                        continue;
                                    }

                                    enemyNumber++;
                                    if (enemyNumber > BattleData.DrBattleNode.WinConditions[i].Param1)
                                    {
                                        break;
                                    }
                                }

                                if (enemyNumber <= BattleData.DrBattleNode.WinConditions[i].Param1)
                                {
                                    Log.Info("战斗结束, 规则:1, 结果:{0}", ((BattleResult)BattleData.DrBattleNode.WinConditions[i].Result).ToString());
                                    Result = (BattleResult)BattleData.DrBattleNode.WinConditions[i].Result;
                                    BattleResultType = 1;

                                    for (int k = 0; k < UnitManager.Units.Count; k++)
                                    {
                                        UnitManager.Units[k].ApplyModifier(null, null, Constant.Battle.BUILTIN_MODIFIER_BATTLE_RESULT, null, false);
                                    }

                                    return;
                                }
                            }
                            break;
                        case 2:
                            {
                                if (checkResultType != BattleCheckResultType.HeroDead)
                                {
                                    continue;
                                }

                                int enemyNumber = 0;
                                for (int j = 0, count = UnitManager.Units.Count; j < count; j++)
                                {
                                    BaseUnit unit = UnitManager.Units[j];
                                    if (unit.Type == UnitType.UNIT_NONE || unit.Type == UnitType.UNIT_THINKER)
                                    {
                                        continue;
                                    }

                                    if (unit.IsDeadState || unit.TeamId == BattleData.SelfPlayer.TeamId)
                                    {
                                        continue;
                                    }

                                    enemyNumber++;
                                    if (enemyNumber > BattleData.DrBattleNode.WinConditions[i].Param1)
                                    {
                                        break;
                                    }
                                }

                                if (enemyNumber <= BattleData.DrBattleNode.WinConditions[i].Param1)
                                {
                                    Log.Info("战斗结束, 规则:2, 结果:{0}", ((BattleResult)BattleData.DrBattleNode.WinConditions[i].Result).ToString());
                                    Result = (BattleResult)BattleData.DrBattleNode.WinConditions[i].Result;
                                    BattleResultType = 2;

                                    for (int k = 0; k < UnitManager.Units.Count; k++)
                                    {
                                        UnitManager.Units[k].ApplyModifier(null, null, Constant.Battle.BUILTIN_MODIFIER_BATTLE_RESULT, null, false);
                                    }

                                    return;
                                }
                            }
                            break;
                        case 3:
                            {
                                if (checkResultType != BattleCheckResultType.TimeEnd)
                                {
                                    continue;
                                }

                                if (BattleData.DrBattleNode.LimitTime > 0 && BattleData.LogicTime >= (DFix64)BattleData.DrBattleNode.LimitTime)
                                {
                                    int enemyNumber = 0;
                                    for (int j = 0, count = UnitManager.Units.Count; j < count; j++)
                                    {
                                        BaseUnit unit = UnitManager.Units[j];
                                        if (unit.Type == UnitType.UNIT_NONE || unit.Type == UnitType.UNIT_THINKER)
                                        {
                                            continue;
                                        }

                                        if (!unit.IsDeadState || unit.TeamId == BattleData.SelfPlayer.TeamId)
                                        {
                                            continue;
                                        }

                                        enemyNumber++;
                                        if (enemyNumber >= BattleData.DrBattleNode.WinConditions[i].Param1)
                                        {
                                            Log.Info("战斗结束, 规则:3, 结果:{0}", ((BattleResult)BattleData.DrBattleNode.WinConditions[i].Result).ToString());
                                            Result = (BattleResult)BattleData.DrBattleNode.WinConditions[i].Result;
                                            BattleResultType = 3;

                                            for (int k = 0; k < UnitManager.Units.Count; k++)
                                            {
                                                UnitManager.Units[k].ApplyModifier(null, null, Constant.Battle.BUILTIN_MODIFIER_BATTLE_RESULT, null, false);
                                            }

                                            return;
                                        }
                                    }
                                }
                            }
                            break;
                        case 4:
                            {
                                if (checkResultType != BattleCheckResultType.HeroDead)
                                {
                                    continue;
                                }

                                int enemyNumber = 0;
                                for (int j = 0, count = UnitManager.Units.Count; j < count; j++)
                                {
                                    BaseUnit unit = UnitManager.Units[j];
                                    if (unit.Type == UnitType.UNIT_NONE || unit.Type == UnitType.UNIT_THINKER)
                                    {
                                        continue;
                                    }

                                    if (!unit.IsDeadState || unit.TeamId == BattleData.SelfPlayer.TeamId)
                                    {
                                        continue;
                                    }

                                    enemyNumber++;
                                    if (enemyNumber >= BattleData.DrBattleNode.WinConditions[i].Param1)
                                    {
                                        Log.Info("战斗结束, 规则:4, 结果:{0}", ((BattleResult)BattleData.DrBattleNode.WinConditions[i].Result).ToString());
                                        Result = (BattleResult)BattleData.DrBattleNode.WinConditions[i].Result;
                                        BattleResultType = 4;

                                        for (int k = 0; k < UnitManager.Units.Count; k++)
                                        {
                                            UnitManager.Units[k].ApplyModifier(null, null, Constant.Battle.BUILTIN_MODIFIER_BATTLE_RESULT, null, false);
                                        }

                                        return;
                                    }
                                }
                            }
                            break;
                        case 5:
                            {
                                if (checkResultType != BattleCheckResultType.BossDead)
                                {
                                    continue;
                                }

                                for (int j = 0, count = UnitManager.Units.Count; j < count; j++)
                                {
                                    BaseUnit unit = UnitManager.Units[j];
                                    if (unit.Type != UnitType.UNIT_BOSS)
                                    {
                                        continue;
                                    }

                                    if (!unit.IsDeadState || unit.TeamId != BattleData.SelfPlayer.TeamId)
                                    {
                                        continue;
                                    }

                                    Log.Info("战斗结束, 规则:5, 结果:{0}", ((BattleResult)BattleData.DrBattleNode.WinConditions[i].Result).ToString());
                                    Result = (BattleResult)BattleData.DrBattleNode.WinConditions[i].Result;
                                    BattleResultType = 5;

                                    for (int k = 0; k < UnitManager.Units.Count; k++)
                                    {
                                        UnitManager.Units[k].ApplyModifier(null, null, Constant.Battle.BUILTIN_MODIFIER_BATTLE_RESULT, null, false);
                                    }

                                    return;
                                }
                            }
                            break;
                        case 6:
                            {
                                if (checkResultType != BattleCheckResultType.BossDead)
                                {
                                    continue;
                                }

                                for (int j = 0, count = UnitManager.Units.Count; j < count; j++)
                                {
                                    BaseUnit unit = UnitManager.Units[j];
                                    if (unit.Type != UnitType.UNIT_BOSS)
                                    {
                                        continue;
                                    }

                                    if (!unit.IsDeadState || unit.TeamId == BattleData.SelfPlayer.TeamId)
                                    {
                                        continue;
                                    }

                                    Log.Info("战斗结束, 规则:6, 结果:{0}", ((BattleResult)BattleData.DrBattleNode.WinConditions[i].Result).ToString());
                                    Result = (BattleResult)BattleData.DrBattleNode.WinConditions[i].Result;
                                    BattleResultType = 6;

                                    for (int k = 0; k < UnitManager.Units.Count; k++)
                                    {
                                        UnitManager.Units[k].ApplyModifier(null, null, Constant.Battle.BUILTIN_MODIFIER_BATTLE_RESULT, null, false);
                                    }

                                    return;
                                }
                            }
                            break;
                        case 7:
                            {
                                if (checkResultType != BattleCheckResultType.TimeEnd)
                                {
                                    continue;
                                }

                                if (BattleData.DrBattleNode.LimitTime > 0 && BattleData.LogicTime >= BattleData.LimitTime)
                                {
                                    Log.Info("战斗结束, 规则:7, 结果:{0}", ((BattleResult)BattleData.DrBattleNode.WinConditions[i].Result).ToString());
                                    Result = (BattleResult)BattleData.DrBattleNode.WinConditions[i].Result;
                                    BattleResultType = 7;

                                    for (int k = 0; k < UnitManager.Units.Count; k++)
                                    {
                                        UnitManager.Units[k].ApplyModifier(null, null, Constant.Battle.BUILTIN_MODIFIER_BATTLE_RESULT, null, false);
                                    }

                                    return;
                                }
                            }
                            break;
                        default:
                            {

                            }
                            break;
                    }
                }
            }

            Result = BattleResult.None;
            BattleResultType = 0;
        }


        private static Dictionary<int, List<Modifier>> _fulltimeModifierHandlers = new Dictionary<int, List<Modifier>>();

        public static void SendFulltimeHandle(ModifierEventType modifierEventType, EventData eventData)
        {
            if (!_fulltimeModifierHandlers.ContainsKey((int)modifierEventType))
            {
                return;
            }

            List<Modifier> modifiers = _fulltimeModifierHandlers[(int)modifierEventType];
            for (int i = 0; i < modifiers.Count; i++)
            {
                if (!modifiers[i].IsRemoved)
                {
                    modifiers[i].HandleEvent(modifierEventType, eventData);
                }
            }
        }
        public static void AddFulltimeHandler(ModifierEventType modifierEventType, Modifier modifier)
        {
            if (!_fulltimeModifierHandlers.ContainsKey((int)modifierEventType))
            {
                _fulltimeModifierHandlers.Add((int)modifierEventType, new List<Modifier>());
            }

            List<Modifier> modifiers = _fulltimeModifierHandlers[(int)modifierEventType];
            modifiers.Add(modifier);
        }
        public static void RemoveFulltimeHandler(ModifierEventType modifierEventType, Modifier modifier)
        {
            if (!_fulltimeModifierHandlers.ContainsKey((int)modifierEventType))
            {
                return;
            }

            _fulltimeModifierHandlers[(int)modifierEventType].Remove(modifier);
        }

        public static void ApplyAttackDamage(BaseUnit attacker, BattleUnitAttribute bakAtt, BaseUnit target, bool isCrit)
        {
            if (target.IsDeadState || target.IsInvincible)
            {
                return;
            }

            if (target.CurrAtt.Hp == DFix64.Zero)
            {
                return;
            }

            DFix64 finalDamage = DFix64.Zero;
            bool isHit = false;

            finalDamage = DFix64.Floor(bakAtt.Attack * SRandom.RangeDFxMax(Parm.Parm22, Parm.Parm23));
            if (isCrit)
            {
                finalDamage = finalDamage * DFix64.Clamp((bakAtt.CritDamage - target.CurrAtt.CritResistance) / DFix64.Thousand, Parm.Parm18, Parm.Parm19);
            }
            finalDamage = finalDamage * DFix64.Clamp(DFix64.One + bakAtt.AttackDamageOutBonus / DFix64.Thousand, Parm.Parm24, Parm.Parm25);
            finalDamage = finalDamage * DFix64.Clamp(bakAtt.DamageOutBonus / DFix64.Thousand, Parm.Parm26, Parm.Parm27);
            DFix64 p2 = DFix64.Zero;
            if (bakAtt.TraitDamageOutBouns.ContainsKey(target.TraitType))
            {
                p2 = bakAtt.TraitDamageOutBouns[target.TraitType] / DFix64.Thousand;
            }
            if (bakAtt.TraitDamageOutBouns.ContainsKey(target.TraitType2))
            {
                p2 += bakAtt.TraitDamageOutBouns[target.TraitType2] / DFix64.Thousand;
            }
            if (p2 > DFix64.Zero)
            {
                finalDamage = finalDamage * DFix64.Clamp(DFix64.One + p2, Parm.Parm28, Parm.Parm29);
            }

            DFix64 hitRange = DFix64.Clamp((bakAtt.HitOdds - target.CurrAtt.DodgeOdds) / DFix64.Thousand, Parm.Parm30, Parm.Parm31);
            if (hitRange > DFix64.Zero)
            {
                isHit = SRandom.RangeDFx(DFix64.Zero, DFix64.One) < hitRange;
            }

            if (isHit)
            {
                DFix64 p1 = DFix64.Max(target.CurrAtt.Defense - bakAtt.PhysicalArmorBreak, DFix64.Zero);
                DFix64 p4 = DFix64.Max(target.CurrAtt.DefenseRatio, Parm.Parm47);
                finalDamage = finalDamage * (p4 + Parm.Parm15 * p1) / (p4 + (DFix64.One + Parm.Parm15) * p1);
                finalDamage = finalDamage * DFix64.Clamp((DFix64.Thousand - target.CurrAtt.AttackDamageInBonus) / DFix64.Thousand, Parm.Parm32, Parm.Parm33);
                finalDamage = finalDamage * DFix64.Clamp((DFix64.Thousand - target.CurrAtt.DamageInBonus) / DFix64.Thousand, Parm.Parm34, Parm.Parm35);
                finalDamage = finalDamage * (DFix64.One + bakAtt.FinalDamageOutBouns / DFix64.Thousand) * (DFix64.One + target.CurrAtt.FinalDamageInBouns / DFix64.Thousand);
                finalDamage = DFix64.Max(DFix64.Floor(finalDamage), DFix64.Zero);
            }
            else
            {
                finalDamage = DFix64.Max(DFix64.Floor(finalDamage), DFix64.Zero);
            }

            if (isHit)
            {
                EventData beforeDealEventData = CreateEventData();
                beforeDealEventData.Attacker = attacker;
                beforeDealEventData.Unit = target;
                beforeDealEventData.Point = target.LogicPosition;
                beforeDealEventData.Node = target.CurrNode;
                beforeDealEventData.Parms.Add("finalDamage", finalDamage);

                attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_DAMAGE, beforeDealEventData);

                attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_ATTACK_DAMAGE, beforeDealEventData);

                if (isCrit)
                {
                    attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_ATTACK_NOCRIT_DAMAGE, beforeDealEventData);
                }
                else
                {
                    attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_ATTACK_CRIT_DAMAGE, beforeDealEventData);
                }

                target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_DAMAGE, beforeDealEventData);

                target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_ATTACK_DAMAGE, beforeDealEventData);

                if (isCrit)
                {
                    target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_ATTACK_CRIT_DAMAGE, beforeDealEventData);
                }
                else
                {
                    target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_ATTACK_NOCRIT_DAMAGE, beforeDealEventData);
                }

                finalDamage = DFix64.Max(beforeDealEventData.Parms["finalDamage"], DFix64.Zero);

                if (finalDamage < Parm.Parm36)
                {
                    finalDamage = Parm.Parm36;
                }

                if (target.IsDeadState || target.IsInvincible)
                {
                    return;
                }

                bool isFirstKill = false;
                DFix64 realDamage = DFix64.Zero;
                DFix64 excessDamage = DFix64.Zero;

                // 修改生命值
                if (finalDamage >= target.CurrAtt.Hp)
                {
                    if (target.CurrAtt.Hp > DFix64.Zero)
                    {
                        isFirstKill = true;
                    }

                    realDamage = target.CurrAtt.Hp;
                    excessDamage = finalDamage - realDamage;
                    target.CurrAtt.Hp = DFix64.Zero;
                }
                else
                {
                    realDamage = finalDamage;
                    target.CurrAtt.Hp = target.CurrAtt.Hp - realDamage;
                }

                if (realDamage > DFix64.Zero)
                {
                    attacker.StatisticsData.AttackDamageOut += realDamage;
                    if ((attacker.IsIllusion || attacker.IsSummoned) && attacker.Master != null)
                    {
                        attacker.Master.StatisticsData.AttackDamageOut += realDamage;
                    }

                    target.StatisticsData.AttackDamageIn += realDamage;
                }

                BattleUnitDamageEventArgs ne = GameFramework.ReferencePool.Acquire<BattleUnitDamageEventArgs>();
                ne.Attacker = attacker;
                ne.Target = target;
                ne.DamageType = DamageType.DAMAGE_TYPE_ATTACK_PHYSICAL;
                ne.IsCrit = isCrit;
                ne.IsHit = true;
                ne.FinalDamage = finalDamage;
                ne.RealDamage = realDamage;
                ne.ExcessDamage = excessDamage;
                FireEvent(ne);

                EventData takeEventData = CreateEventData();
                takeEventData.Attacker = attacker;
                takeEventData.Unit = target;
                takeEventData.Point = target.LogicPosition;
                takeEventData.Node = target.CurrNode;
                takeEventData.Parms.Add("finalDamage", finalDamage);
                takeEventData.Parms.Add("realDamage", realDamage);
                takeEventData.Parms.Add("excessDamage", excessDamage);
                target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_TAKE_DAMAGE, takeEventData);

                target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_TAKE_ATTACK_DAMAGE, takeEventData);

                if (isCrit)
                {
                    target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_TAKE_ATTACK_CRIT_DAMAGE, takeEventData);
                }
                else
                {
                    target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_TAKE_ATTACK_NOCRIT_DAMAGE, takeEventData);
                }

                attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEAL_DAMAGE, takeEventData);

                attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEAL_ATTACK_DAMAGE, takeEventData);

                if (isCrit)
                {
                    attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEAL_ATTACK_CRIT_DAMAGE, takeEventData);
                }
                else
                {
                    attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEAL_ATTACK_NOCRIT_DAMAGE, takeEventData);
                }

                realDamage = takeEventData.Parms["realDamage"];

                Log.Info("{0} 的普攻, 对 {1} 造成 <color=red>{2}</color> 点伤害{3}, 初始伤害{4}", attacker.LogName, target.LogName,
                   realDamage.ToString(), isCrit ? "<color=red>(暴击)</color>" : string.Empty, finalDamage.ToString());

                if (!target.IsAlive && !target.IsDeadState)
                {
                    if (isFirstKill)
                    {
                        if (target.IsIllusion)
                        {
                            attacker.StatisticsData.KillIllusionCount += 1;

                            if ((attacker.IsIllusion || attacker.IsSummoned) && attacker.Master != null)
                            {
                                attacker.Master.StatisticsData.KillIllusionCount += 1;
                            }
                        }
                        else if (target.IsSummoned)
                        {
                            attacker.StatisticsData.KillSummonedCount += 1;
                            if ((attacker.IsIllusion || attacker.IsSummoned) && attacker.Master != null)
                            {
                                attacker.Master.StatisticsData.KillSummonedCount += 1;
                            }

                            attacker.Player.TeamKillNumber += 1;
                        }
                        else
                        {
                            attacker.StatisticsData.KillUnitCount += 1;
                            if ((attacker.IsIllusion || attacker.IsSummoned) && attacker.Master != null)
                            {
                                attacker.Master.StatisticsData.KillUnitCount += 1;
                            }

                            attacker.Player.TeamKillNumber += 1;
                        }

                        target.Kill(attacker, null);

                        EventData deathEventData = CreateEventData();
                        deathEventData.Attacker = attacker;
                        deathEventData.Unit = target;
                        deathEventData.Point = target.LogicPosition;
                        deathEventData.Node = target.CurrNode;
                        target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEATH, deathEventData);

                        SendFulltimeHandle(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH, deathEventData);
                        SendFulltimeHandle(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH2, deathEventData);

                        attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_KILL_UNIT, deathEventData);
                    }
                }

                if (!attacker.IsDeadState)
                {
                    if (attacker.CurrAtt.HpSteal > DFix64.Zero)
                    {
                        if (realDamage > DFix64.Zero)
                        {
                            DFix64 hpSteal = DFix64.Zero;
                            DFix64 realHpSteal = DFix64.Zero;
                            DFix64 excessHpSteal = DFix64.Zero;

                            hpSteal = realDamage * attacker.CurrAtt.HpSteal / DFix64.Thousand;
                            hpSteal = DFix64.Max(DFix64.Floor(hpSteal), DFix64.Zero);

                            if (hpSteal > DFix64.Zero)
                            {
                                if (attacker.CurrAtt.Hp + hpSteal >= attacker.CurrAtt.MaxHp)
                                {
                                    realHpSteal = attacker.CurrAtt.MaxHp - attacker.CurrAtt.Hp;
                                    excessHpSteal = hpSteal - realHpSteal;
                                    attacker.CurrAtt.Hp = attacker.CurrAtt.MaxHp;
                                }
                                else
                                {
                                    realHpSteal = hpSteal;
                                    attacker.CurrAtt.Hp = attacker.CurrAtt.Hp + hpSteal;
                                }

                                attacker.StatisticsData.HealIn += realHpSteal;

                                Log.Info("{0} 吸取 <color=green>{1}</color> 点生命值", attacker.LogName, realHpSteal.ToString());

                                BattleUnitHealEventArgs ne2 = GameFramework.ReferencePool.Acquire<BattleUnitHealEventArgs>();
                                ne2.Attacker = attacker;
                                ne2.Target = attacker;
                                ne2.HealType = HealType.HEAL_TYPE_NORMAL;
                                ne2.FinalHeal = hpSteal;
                                ne2.RealHeal = realHpSteal;
                                ne2.ExcessHeal = excessHpSteal;
                                FireEvent(ne2);
                            }
                        }
                    }
                }
            }
            else
            {
                Log.Info("{0} 的普攻{2} <color=yellow>未命中</color> {1}, 初始伤害 {3}", attacker.LogName, target.LogName, isCrit ? "<color=red>(暴击)</color>" : string.Empty, finalDamage.ToString());

                BattleUnitDamageEventArgs ne = GameFramework.ReferencePool.Acquire<BattleUnitDamageEventArgs>();
                ne.Attacker = attacker;
                ne.Target = target;
                ne.DamageType = DamageType.DAMAGE_TYPE_ATTACK_PHYSICAL;
                ne.IsCrit = isCrit;
                ne.IsHit = false;
                ne.FinalDamage = finalDamage;
                ne.RealDamage = DFix64.Zero;
                ne.ExcessDamage = DFix64.Zero;
                FireEvent(ne);

                EventData missEventData = CreateEventData();
                missEventData.Attacker = attacker;
                missEventData.Unit = target;
                missEventData.Point = target.LogicPosition;
                missEventData.Node = target.CurrNode;
                attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_MISS, missEventData);

                attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ATTACK_MISS, missEventData);

                target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DODGE, missEventData);

                target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ATTACK_DODGE, missEventData);
            }
        }

        public static void ApplyAbilityDamage(BaseUnit target, BaseUnit attacker, Ability ability, Modifier modifier, DFix64 damage, DamageType damageType, DamageFlag damageFlag, bool isCrit, DFix64 defenseIgnore, DFix64 hitBuff)
        {
            if (target == null)
            {
                return;
            }

            if (damageType == DamageType.DAMAGE_TYPE_NONE)
            {
                return;
            }

            if (target.IsDeadState || target.IsInvincible)
            {
                return;
            }

            if (target.CurrAtt.Hp == DFix64.Zero)
            {
                return;
            }

            DFix64 finalDamage = DFix64.Zero;             // 实际伤害量

            bool isHit = false;

            // 计算伤害量
            switch (damageType)
            {
                case DamageType.DAMAGE_TYPE_PHYSICAL:
                    {
                        finalDamage = DFix64.Floor(damage * SRandom.RangeDFxMax(Parm.Parm22, Parm.Parm23));
                        if (isCrit)
                        {
                            finalDamage = finalDamage * DFix64.Clamp((attacker.CurrAtt.CritDamage - target.CurrAtt.CritResistance) / DFix64.Thousand, Parm.Parm18, Parm.Parm19);
                        }
                        finalDamage = finalDamage * DFix64.Clamp(DFix64.One + attacker.CurrAtt.AttackDamageOutBonus / DFix64.Thousand, Parm.Parm24, Parm.Parm25);
                        finalDamage = finalDamage * DFix64.Clamp(attacker.CurrAtt.DamageOutBonus / DFix64.Thousand, Parm.Parm26, Parm.Parm27);
                        DFix64 p2 = DFix64.Zero;
                        if (attacker.CurrAtt.TraitDamageOutBouns.ContainsKey(target.TraitType))
                        {
                            p2 = attacker.CurrAtt.TraitDamageOutBouns[target.TraitType] / DFix64.Thousand;
                        }
                        if (attacker.CurrAtt.TraitDamageOutBouns.ContainsKey(target.TraitType2))
                        {
                            p2 += attacker.CurrAtt.TraitDamageOutBouns[target.TraitType2] / DFix64.Thousand;
                        }
                        if (p2 > DFix64.Zero)
                        {
                            finalDamage = finalDamage * DFix64.Clamp(DFix64.One + p2, Parm.Parm28, Parm.Parm29);
                        }

                        DFix64 hitRange = DFix64.Clamp((attacker.CurrAtt.HitOdds - target.CurrAtt.DodgeOdds) / DFix64.Thousand, Parm.Parm30, Parm.Parm31);
                        if (hitRange > DFix64.Zero)
                        {
                            isHit = SRandom.RangeDFx(DFix64.Zero, DFix64.One) < hitRange;
                        }

                        if (isHit)
                        {
                            DFix64 p1 = DFix64.Max(target.CurrAtt.Defense - attacker.CurrAtt.PhysicalArmorBreak, DFix64.Zero);
                            DFix64 p4 = DFix64.Max(target.CurrAtt.DefenseRatio, Parm.Parm47);
                            finalDamage = finalDamage * (p4 + Parm.Parm15 * p1) / (p4 + (DFix64.One + Parm.Parm15) * p1);
                            finalDamage = finalDamage * DFix64.Clamp((DFix64.Thousand - target.CurrAtt.AttackDamageInBonus) / DFix64.Thousand, Parm.Parm32, Parm.Parm33);
                            finalDamage = finalDamage * DFix64.Clamp((DFix64.Thousand - target.CurrAtt.DamageInBonus) / DFix64.Thousand, Parm.Parm34, Parm.Parm35);
                            finalDamage = finalDamage * (DFix64.One + attacker.CurrAtt.FinalDamageOutBouns / DFix64.Thousand) * (DFix64.One + target.CurrAtt.FinalDamageInBouns / DFix64.Thousand);
                            finalDamage = DFix64.Max(DFix64.Floor(finalDamage), DFix64.Zero);
                        }
                        else
                        {
                            finalDamage = DFix64.Max(DFix64.Floor(finalDamage), DFix64.Zero);
                        }
                    }
                    break;
                case DamageType.DAMAGE_TYPE_MAGICAL:
                    {
                        finalDamage = DFix64.Floor(damage * _sRandom.RangeDFxMax(Parm.Parm22, Parm.Parm23));
                        if (isCrit)
                        {
                            finalDamage = finalDamage * DFix64.Clamp((attacker.CurrAtt.CritDamage - target.CurrAtt.CritResistance) / DFix64.Thousand, Parm.Parm18, Parm.Parm19);
                        }
                        finalDamage = finalDamage * DFix64.Clamp(DFix64.One + attacker.CurrAtt.AbilityDamageOutBonus / DFix64.Thousand, Parm.Parm37, Parm.Parm38);
                        finalDamage = finalDamage * DFix64.Clamp(attacker.CurrAtt.DamageOutBonus / DFix64.Thousand, Parm.Parm26, Parm.Parm27);
                        DFix64 p2 = DFix64.Zero;
                        if (attacker.CurrAtt.TraitDamageOutBouns.ContainsKey(target.TraitType))
                        {
                            p2 = attacker.CurrAtt.TraitDamageOutBouns[target.TraitType] / DFix64.Thousand;
                        }
                        if (attacker.CurrAtt.TraitDamageOutBouns.ContainsKey(target.TraitType2))
                        {
                            p2 += attacker.CurrAtt.TraitDamageOutBouns[target.TraitType2] / DFix64.Thousand;
                        }
                        if (p2 > DFix64.Zero)
                        {
                            finalDamage = finalDamage * DFix64.Clamp(DFix64.One + p2, Parm.Parm28, Parm.Parm29);
                        }

                        DFix64 hitRange = DFix64.Clamp((attacker.CurrAtt.HitOdds - target.CurrAtt.DodgeOdds + hitBuff) / DFix64.Thousand, Parm.Parm30, Parm.Parm31);
                        if (hitRange > DFix64.Zero)
                        {
                            isHit = SRandom.RangeDFx(DFix64.Zero, DFix64.One) < hitRange;
                        }

                        if (isHit)
                        {
                            DFix64 p1 = DFix64.Max(target.CurrAtt.Defense - attacker.CurrAtt.PhysicalArmorBreak, DFix64.Zero);
                            DFix64 p3 = DFix64.One - defenseIgnore;
                            DFix64 p4 = DFix64.Max(target.CurrAtt.DefenseRatio, Parm.Parm47);
                            finalDamage = finalDamage * (p4 + Parm.Parm15 * p1 * p3) / (p4 + (DFix64.One + Parm.Parm15) * p1 * p3);
                            finalDamage = finalDamage * DFix64.Clamp((DFix64.Thousand - target.CurrAtt.AbilityDamageInBonus) / DFix64.Thousand, Parm.Parm39, Parm.Parm40);
                            finalDamage = finalDamage * DFix64.Clamp((DFix64.Thousand - target.CurrAtt.DamageInBonus) / DFix64.Thousand, Parm.Parm34, Parm.Parm35);
                            finalDamage = finalDamage * (DFix64.One + attacker.CurrAtt.FinalDamageOutBouns / DFix64.Thousand) * (DFix64.One + target.CurrAtt.FinalDamageInBouns / DFix64.Thousand);
                            finalDamage = DFix64.Max(DFix64.Floor(finalDamage), DFix64.Zero);
                        }
                        else
                        {
                            finalDamage = DFix64.Max(DFix64.Floor(finalDamage), DFix64.Zero);
                        }

                    }
                    break;
                case DamageType.DAMAGE_TYPE_HP_REMOVAL:
                    {
                        finalDamage = DFix64.Max(DFix64.Floor(damage), DFix64.Zero);

                        isCrit = false;
                        isHit = true;

                        if ((damageFlag & DamageFlag.DAMAGE_FLAG_NO_DIRECTOR_EVENT) == 0)
                        {
                            damageFlag |= DamageFlag.DAMAGE_FLAG_NO_DIRECTOR_EVENT;
                        }
                    }
                    break;
                default:
                    {
                        return;
                    }
                    break;
            }

            if (isHit)
            {
                if ((damageFlag & DamageFlag.DAMAGE_FLAG_NO_DIRECTOR_EVENT) == 0)
                {
                    EventData beforeDealDamageEventData = CreateEventData();
                    beforeDealDamageEventData.Attacker = attacker;
                    beforeDealDamageEventData.Unit = target;
                    beforeDealDamageEventData.Point = target.LogicPosition;
                    beforeDealDamageEventData.Node = target.CurrNode;
                    beforeDealDamageEventData.Parms.Add("finalDamage", finalDamage);
                    attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_DAMAGE, beforeDealDamageEventData);

                    attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_ABILITY_DAMAGE, beforeDealDamageEventData);

                    if (isCrit)
                    {
                        attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_ABILITY_CRIT_DAMAGE, beforeDealDamageEventData);
                    }
                    else
                    {
                        attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_ABILITY_NOCRIT_DAMAGE, beforeDealDamageEventData);
                    }

                    target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_DAMAGE, beforeDealDamageEventData);

                    target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_ABILITY_DAMAGE, beforeDealDamageEventData);

                    if (isCrit)
                    {
                        target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_ABILITY_CRIT_DAMAGE, beforeDealDamageEventData);
                    }
                    else
                    {
                        target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_ABILITY_NOCRIT_DAMAGE, beforeDealDamageEventData);
                    }

                    finalDamage = DFix64.Max(beforeDealDamageEventData.Parms["finalDamage"], DFix64.Zero);
                }

                if (finalDamage < Parm.Parm41)
                {
                    finalDamage = Parm.Parm41;
                }

                if (target.IsDeadState || target.IsInvincible)
                {
                    return;
                }

                bool isFirstKill = false;
                DFix64 realDamage = DFix64.Zero;              // 有效伤害量
                DFix64 excessDamage = DFix64.Zero;            // 过量伤害量

                if (finalDamage >= target.CurrAtt.Hp)
                {
                    if (target.CurrAtt.Hp > DFix64.Zero)
                    {
                        isFirstKill = true;
                    }

                    realDamage = target.CurrAtt.Hp;
                    excessDamage = finalDamage - realDamage;
                    target.CurrAtt.Hp = DFix64.Zero;
                }
                else
                {
                    realDamage = finalDamage;
                    target.CurrAtt.Hp = target.CurrAtt.Hp - realDamage;
                }

                if (realDamage > DFix64.Zero)
                {
                    attacker.StatisticsData.AbilityDamageOut += realDamage;
                    if ((attacker.IsIllusion || attacker.IsSummoned) && attacker.Master != null)
                    {
                        attacker.Master.StatisticsData.AbilityDamageOut += realDamage;
                    }

                    target.StatisticsData.AbilityDamageIn += realDamage;
                }

                BattleUnitDamageEventArgs ne = GameFramework.ReferencePool.Acquire<BattleUnitDamageEventArgs>();
                ne.Attacker = attacker;
                ne.Ability = ability;
                ne.Target = target;
                ne.DamageType = damageType;
                ne.IsCrit = isCrit;
                ne.IsHit = true;
                ne.FinalDamage = finalDamage;
                ne.RealDamage = realDamage;
                ne.ExcessDamage = excessDamage;
                FireEvent(ne);

                if ((damageFlag & DamageFlag.DAMAGE_FLAG_NO_DIRECTOR_EVENT) == 0)
                {
                    EventData takeDamageEventData = CreateEventData();
                    takeDamageEventData.Attacker = attacker;
                    takeDamageEventData.Unit = target;
                    takeDamageEventData.Point = target.LogicPosition;
                    takeDamageEventData.Node = target.CurrNode;
                    takeDamageEventData.Parms.Add("finalDamage", finalDamage);
                    takeDamageEventData.Parms.Add("realDamage", realDamage);
                    takeDamageEventData.Parms.Add("excessDamage", excessDamage);
                    target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_TAKE_DAMAGE, takeDamageEventData);

                    target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_TAKE_ABILITY_DAMAGE, takeDamageEventData);

                    if (isCrit)
                    {
                        target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_TAKE_ABILITY_CRIT_DAMAGE, takeDamageEventData);
                    }
                    else
                    {
                        target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_TAKE_ABILITY_NOCRIT_DAMAGE, takeDamageEventData);
                    }

                    attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEAL_DAMAGE, takeDamageEventData);

                    attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEAL_ABILITY_DAMAGE, takeDamageEventData);

                    if (isCrit)
                    {
                        attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEAL_ABILITY_CRIT_DAMAGE, takeDamageEventData);
                    }
                    else
                    {
                        attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEAL_ABILITY_NOCRIT_DAMAGE, takeDamageEventData);
                    }

                    realDamage = takeDamageEventData.Parms["realDamage"];
                }

                Log.Info("{0} 的技能 {1}{5},对 {2} 造成 <color=red>{3}</color> 点{7}伤害({4}), 初始伤害 {6}", attacker.LogName, ability.LogName, target.LogName,
                    realDamage.ToString(), damageType.ToString(), modifier != null ? GameFramework.Utility.Text.Format("({0})", modifier.LogName) : string.Empty, finalDamage.ToString(), isCrit ? "<color=red>暴击</color>" : string.Empty);

                if (!target.IsAlive && !target.IsDeadState)
                {
                    if (isFirstKill)
                    {
                        if (target.IsIllusion)
                        {
                            attacker.StatisticsData.KillIllusionCount += 1;

                            if ((attacker.IsIllusion || attacker.IsSummoned) && attacker.Master != null)
                            {
                                attacker.Master.StatisticsData.KillIllusionCount += 1;
                            }
                        }
                        else if (target.IsSummoned)
                        {
                            attacker.StatisticsData.KillSummonedCount += 1;
                            if ((attacker.IsIllusion || attacker.IsSummoned) && attacker.Master != null)
                            {
                                attacker.Master.StatisticsData.KillSummonedCount += 1;
                            }

                            attacker.Player.TeamKillNumber += 1;
                        }
                        else
                        {
                            attacker.StatisticsData.KillUnitCount += 1;
                            if ((attacker.IsIllusion || attacker.IsSummoned) && attacker.Master != null)
                            {
                                attacker.Master.StatisticsData.KillUnitCount += 1;
                            }

                            attacker.Player.TeamKillNumber += 1;
                        }

                        target.Kill(attacker, ability);

                        //if ((damageFlag & DamageFlag.DAMAGE_FLAG_NO_DIRECTOR_EVENT) == 0)
                        {
                            EventData deathEventData = CreateEventData();
                            deathEventData.Attacker = attacker;
                            deathEventData.Unit = target;
                            deathEventData.Point = target.LogicPosition;
                            deathEventData.Node = target.CurrNode;
                            target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEATH, deathEventData);

                            SendFulltimeHandle(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH, deathEventData);
                            SendFulltimeHandle(ModifierEventType.MODIFIER_EVENT_ON_UNIT_DEATH2, deathEventData);

                            attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_KILL_UNIT, deathEventData);
                        }
                    }
                }
            }
            else
            {
                Log.Info("{0} 的技能 {1}{3}, {5}伤害 <color=yellow>未命中</color> {2}, 初始伤害 {4}", attacker.LogName, ability.LogName, target.LogName, modifier != null ? GameFramework.Utility.Text.Format("({0})", modifier.LogName) : string.Empty, finalDamage.ToString(), isCrit ? "<color=red>暴击</color>" : string.Empty);

                BattleUnitDamageEventArgs ne = GameFramework.ReferencePool.Acquire<BattleUnitDamageEventArgs>();
                ne.Attacker = attacker;
                ne.Ability = ability;
                ne.Target = target;
                ne.DamageType = damageType;
                ne.IsCrit = isCrit;
                ne.IsHit = false;
                ne.FinalDamage = finalDamage;
                ne.RealDamage = DFix64.Zero;
                ne.ExcessDamage = DFix64.Zero;
                FireEvent(ne);

                if ((damageFlag & DamageFlag.DAMAGE_FLAG_NO_DIRECTOR_EVENT) == 0)
                {

                    EventData missEventData = CreateEventData();
                    missEventData.Attacker = attacker;
                    missEventData.Unit = target;
                    missEventData.Point = target.LogicPosition;
                    missEventData.Node = target.CurrNode;
                    attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_MISS, missEventData);

                    attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_MISS, missEventData);

                    target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DODGE, missEventData);

                    target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_ABILITY_DODGE, missEventData);
                }
            }
        }

        public static void ApplyAbilityHeal(BaseUnit attacker, Ability ability, Modifier modifier, DFix64 heal, HealType healType, DamageFlag damageFlag, BaseUnit target)
        {
            if (healType == HealType.HEAL_TYPE_NONE)
            {
                return;
            }

            if (target.IsDeadState || target.IsInvincible)
            {
                return;
            }

            if (target.CurrAtt.Hp == target.CurrAtt.MaxHp)
            {
                return;
            }

            DFix64 finalHeal = DFix64.Zero;               // 实际治疗量
            bool _isHit = false;

            // 计算治疗量
            switch (healType)
            {
                case HealType.HEAL_TYPE_NORMAL:
                    {
                        _isHit = true;

                        DFix64 p1 = DFix64.Clamp((DFix64.One + attacker.CurrAtt.HealOutBonus / DFix64.Thousand + target.CurrAtt.HealInBonus / DFix64.Thousand), Parm.Parm42, Parm.Parm43);
                        finalHeal = DFix64.Max(DFix64.Floor(heal * p1), DFix64.Zero);
                    }
                    break;
                case HealType.HEAL_TYPE_HP_REMOVAL:
                    {
                        _isHit = true;

                        finalHeal = DFix64.Max(DFix64.Floor(heal), DFix64.Zero);

                        if ((damageFlag & DamageFlag.DAMAGE_FLAG_NO_DIRECTOR_EVENT) == 0)
                        {
                            damageFlag |= DamageFlag.DAMAGE_FLAG_NO_DIRECTOR_EVENT;
                        }
                    }
                    break;
                default:
                    {
                        _isHit = false;
                    }
                    break;
            }

            if (_isHit)
            {
                if ((damageFlag & DamageFlag.DAMAGE_FLAG_NO_DIRECTOR_EVENT) == 0)
                {
                    EventData beforeDealEventData = CreateEventData();
                    beforeDealEventData.Attacker = attacker;
                    beforeDealEventData.Unit = target;
                    beforeDealEventData.Point = target.LogicPosition;
                    beforeDealEventData.Node = target.CurrNode;
                    beforeDealEventData.Parms.Add("finalHeal", finalHeal);
                    attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_DEAL_HEAL, beforeDealEventData);

                    target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_BEFORE_TAKE_HEAL, beforeDealEventData);

                    finalHeal = DFix64.Max(beforeDealEventData.Parms["finalHeal"], DFix64.Zero);
                }

                if (finalHeal < Parm.Parm44)
                {
                    finalHeal = Parm.Parm44;
                }

                if (target.IsDeadState || target.IsInvincible)
                {
                    return;
                }

                DFix64 realHeal = DFix64.Zero;                // 有效治疗量
                DFix64 excessHeal = DFix64.Zero;              // 过量治疗量

                // 修改生命值
                if (target.CurrAtt.Hp + finalHeal >= target.CurrAtt.MaxHp)
                {
                    realHeal = target.CurrAtt.MaxHp - target.CurrAtt.Hp;
                    excessHeal = finalHeal - realHeal;

                    target.CurrAtt.Hp = target.CurrAtt.MaxHp;
                }
                else
                {
                    realHeal = finalHeal;
                    target.CurrAtt.Hp = target.CurrAtt.Hp + realHeal;
                }

                if (realHeal > DFix64.Zero)
                {
                    attacker.StatisticsData.HealOut += realHeal;
                    if ((attacker.IsIllusion || attacker.IsSummoned) && attacker.Master != null)
                    {
                        attacker.Master.StatisticsData.HealOut += realHeal;
                    }

                    target.StatisticsData.HealIn += realHeal;
                }

                BattleUnitHealEventArgs ne = GameFramework.ReferencePool.Acquire<BattleUnitHealEventArgs>();
                ne.Attacker = attacker;
                ne.Ability = ability;
                ne.Target = target;
                ne.HealType = healType;
                ne.FinalHeal = finalHeal;
                ne.RealHeal = realHeal;
                ne.ExcessHeal = excessHeal;
                FireEvent(ne);

                if ((damageFlag & DamageFlag.DAMAGE_FLAG_NO_DIRECTOR_EVENT) == 0)
                {
                    EventData dealEventData = CreateEventData();
                    dealEventData.Attacker = attacker;
                    dealEventData.Unit = target;
                    dealEventData.Point = target.LogicPosition;
                    dealEventData.Node = target.CurrNode;
                    dealEventData.Parms.Add("finalHeal", finalHeal);
                    dealEventData.Parms.Add("realHeal", realHeal);
                    dealEventData.Parms.Add("excessHeal", excessHeal);
                    attacker.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_DEAL_HEAL, dealEventData);

                    target.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_TAKE_HEAL, dealEventData);

                    realHeal = dealEventData.Parms["realHeal"];
                }

                Log.Info("{0} 的技能 {1}{5}, 对 {2} 造成 <color=green>{3}</color> 点治疗({4}), 初始治疗 {6}", attacker.LogName, ability.LogName, target.LogName,
                   realHeal.ToString(), healType.ToString(), modifier != null ? GameFramework.Utility.Text.Format("({0})", modifier.LogName) : string.Empty, finalHeal.ToString());
            }
            else
            {
                Log.Info("{0} 的技能 {1}{3},治疗 <color=yellow>未命中 {2}, 初始治疗 {4}", attacker.LogName, ability.LogName, target.LogName, modifier != null ? GameFramework.Utility.Text.Format("({0})", modifier.LogName) : string.Empty, finalHeal.ToString());
            }
        }


        public static int FireSound(BaseUnit target, string attachPointName, KeyValue kvSound)
        {
            if (target == null || kvSound == null)
            {
                return 0;
            }

            KeyValue kvSoundName = kvSound["SoundName"];
            if (kvSoundName == null)
            {
                return 0;
            }

            DFixVector3 point = target.LogicPosition;

            PlaySoundParams parms = new PlaySoundParams();

            if (!string.IsNullOrEmpty(attachPointName))
            {
                //Transform attachPoint = target.GetAttachPoint(attachPointName);
                //if (attachPoint != null)
                {
                    point = target.LogicPosition;
                    //point = new DFixVector3((DFix64)attachPoint.position.x, (DFix64)attachPoint.position.y, (DFix64)attachPoint.position.z);
                }
            }

            KeyValue kvVolumeInSoundGroup = kvSound["VolumeInSoundGroup"];
            KeyValue kvLoop = kvSound["Loop"];
            KeyValue kvTime = kvSound["Time"];
            KeyValue kvPitch = kvSound["Pitch"];
            KeyValue kvFadeInSeconds = kvSound["FadeInSeconds"];

            if (kvVolumeInSoundGroup != null)
            {
                parms.VolumeInSoundGroup = (float)BattleData.ParseDFix64(kvVolumeInSoundGroup.GetString());
            }
            else
            {
                parms.VolumeInSoundGroup = 1f;
            }

            parms.VolumeInSoundGroup = parms.VolumeInSoundGroup * GetDefaultVolume();

            if (kvLoop != null)
            {
                parms.Loop = BattleData.ParseBool01(kvLoop.GetString());
            }
            else
            {
                parms.Loop = false;
            }

            if (kvTime != null)
            {
                parms.Time = (float)BattleData.ParseDFix64(kvTime.GetString());
            }
            else
            {
                parms.Time = 0f;
            }

            if (kvPitch != null)
            {
                parms.Pitch = (float)BattleData.ParseDFix64(kvPitch.GetString());
            }
            else
            {
                parms.Pitch = 1f;
            }

            if (kvFadeInSeconds != null)
            {
                parms.FadeInSeconds = (float)BattleData.ParseDFix64(kvFadeInSeconds.GetString());
            }
            else
            {
                parms.FadeInSeconds = 0f;
            }

            parms.SpatialBlend = 1f;

            string soundName = kvSoundName.GetString();
            return GameManager.Sound.PlaySound(soundName, RT.Constant.SoundGroup.Sound, 0, parms, point.ToVector3(), null);
        }

        public static int FireSound(BaseUnit target, KeyValue kvSound)
        {
            if (target == null || kvSound == null)
            {
                return 0;
            }

            KeyValue kvSoundName = kvSound["SoundName"];
            if (kvSoundName == null)
            {
                return 0;
            }

            DFixVector3 point = target.LogicPosition;

            PlaySoundParams parms = new PlaySoundParams();

            KeyValue kvAttachPointName = kvSound["AttachPointName"];
            if (kvAttachPointName != null)
            {
                point = target.LogicPosition;
            }

            KeyValue kvVolumeInSoundGroup = kvSound["VolumeInSoundGroup"];
            KeyValue kvLoop = kvSound["Loop"];
            KeyValue kvTime = kvSound["Time"];
            KeyValue kvPitch = kvSound["Pitch"];
            KeyValue kvFadeInSeconds = kvSound["FadeInSeconds"];

            if (kvVolumeInSoundGroup != null)
            {
                parms.VolumeInSoundGroup = (float)BattleData.ParseDFix64(kvVolumeInSoundGroup.GetString());
            }
            else
            {
                parms.VolumeInSoundGroup = 1f;
            }

            parms.VolumeInSoundGroup = parms.VolumeInSoundGroup * GetDefaultVolume();

            if (kvLoop != null)
            {
                parms.Loop = BattleData.ParseBool01(kvLoop.GetString());
            }
            else
            {
                parms.Loop = false;
            }

            if (kvTime != null)
            {
                parms.Time = (float)BattleData.ParseDFix64(kvTime.GetString());
            }
            else
            {
                parms.Time = 0f;
            }

            if (kvPitch != null)
            {
                parms.Pitch = (float)BattleData.ParseDFix64(kvPitch.GetString());
            }
            else
            {
                parms.Pitch = 1f;
            }

            if (kvFadeInSeconds != null)
            {
                parms.FadeInSeconds = (float)BattleData.ParseDFix64(kvFadeInSeconds.GetString());
            }
            else
            {
                parms.FadeInSeconds = 0f;
            }

            parms.SpatialBlend = 1f;

            string soundName = kvSoundName.GetString();
            return GameManager.Sound.PlaySound(soundName, RT.Constant.SoundGroup.Sound, 0, parms, point.ToVector3(), null);
        }

        public static int FireSound(DFixVector3 point, KeyValue kvSound)
        {
            if (kvSound == null)
            {
                return 0;
            }

            KeyValue kvSoundName = kvSound["SoundName"];
            if (kvSoundName == null)
            {
                return 0;
            }

            PlaySoundParams parms = new PlaySoundParams();

            KeyValue kvVolumeInSoundGroup = kvSound["VolumeInSoundGroup"];
            KeyValue kvLoop = kvSound["Loop"];
            KeyValue kvTime = kvSound["Time"];
            KeyValue kvPitch = kvSound["Pitch"];
            KeyValue kvFadeInSeconds = kvSound["FadeInSeconds"];

            if (kvVolumeInSoundGroup != null)
            {
                parms.VolumeInSoundGroup = (float)BattleData.ParseDFix64(kvVolumeInSoundGroup.GetString());
            }
            else
            {
                parms.VolumeInSoundGroup = 1f;
            }

            parms.VolumeInSoundGroup = parms.VolumeInSoundGroup * GetDefaultVolume();

            if (kvLoop != null)
            {
                parms.Loop = BattleData.ParseBool01(kvLoop.GetString());
            }
            else
            {
                parms.Loop = false;
            }

            if (kvTime != null)
            {
                parms.Time = (float)BattleData.ParseDFix64(kvTime.GetString());
            }
            else
            {
                parms.Time = 0f;
            }

            if (kvPitch != null)
            {
                parms.Pitch = (float)BattleData.ParseDFix64(kvPitch.GetString());
            }
            else
            {
                parms.Pitch = 1f;
            }

            if (kvFadeInSeconds != null)
            {
                parms.FadeInSeconds = (float)BattleData.ParseDFix64(kvFadeInSeconds.GetString());
            }
            else
            {
                parms.FadeInSeconds = 0f;
            }

            parms.SpatialBlend = 1f;

            string soundName = kvSoundName.GetString();
            return GameManager.Sound.PlaySound(soundName, RT.Constant.SoundGroup.Sound, 0, parms, point.ToVector3(), null);
        }


        public static float GetDefaultVolume()
        {
            return 0.5f;
        }

        public static float GetDelayHideDuration()
        {
            return 0.5f;
        }

        public static float GetDelayHide2Duration()
        {
            return 1.5f;
        }

        public static void FireEvent(GameEventArgs e)
        {
#if UNITY_EDITOR
            if (BattleData.TestQuickBattle)
            {
                return;
            }
#endif
            GameManager.Event.Fire(null, e);
        }


        private static void OnSaveLineupSuccess(object sender, GameEventArgs e)
        {
            SaveLineupSuccessEventArgs ne = (SaveLineupSuccessEventArgs)e;

            _isSendSaveLineup = false;

            if (_isWaitStartBattle)
            {
                SendStartBattle();
            }
            else
            {
                RuntimeGameEntry.UI.OpenUIFormById(Constant.UIFormId.ErrorMsg, "保存阵容成功");
            }
        }

        private static void OnSaveLineupFailure(object sender, GameEventArgs e)
        {
            _isSendSaveLineup = false;
        }

        private static void OnBeginBattleSuccess(object sender, GameEventArgs e)
        {
            _isSendStartBattle = false;

            BattleData.BattleGrid.HideDragStartEffect();
            BattleData.BattleGrid.HideDragEndEffect();
            BattleData.BattleGrid.HideDragArrowEffect();
            BattleData.BattleGrid.HideSelfLineupArea();

            BattleData.State = BattleState.Start;
            BattleData.BattleStartTime = GameManager.SysTime.LocalTime;
            BattleSpeed = GameManager.Setting.GetCustomBool(GameManager.GlobalData.SelfPlayer.RoleId, Constant.Setting.BATTLE_SPEED) ? GetBattleSpeedUp() : 1f;
            ChangeBattleSpeed(GetFinalBattleSpeed());

            SortUnits();
            UnitManager.PrepareBattle();
        }

        private static void OnBeginBattleFailure(object sender, GameEventArgs e)
        {
            _isSendStartBattle = false;
        }

        private static void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;

            //if (ne.UserData != null && ne.UserData.GetType() == typeof(BattleEntityData))
            //{
            //    BattleEntityData showBattleEntityData = (BattleEntityData)ne.UserData;
            //    showBattleEntityData.Entity.OnShowEntitySuccess((BattleEntityLogic)ne.Entity.Logic);
            //}
        }

        private static void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;

            if (ne.UserData != null && ne.UserData.GetType() == typeof(BattleEntityData))
            {
                Log.Error("加载实体失败, error:{0}", ne.ErrorMessage);

                BattleEntityData showBattleEntityData = (BattleEntityData)ne.UserData;
                showBattleEntityData.Entity.OnShowEntityFailure();
            }
        }


        public static int Abs(int a)
        {
            return (a ^ (a >> 31)) - (a >> 31);
        }

        public static bool IsOdd(long a)
        {
            return (a & 1) == 1;
        }

        public static BattleParm Parm = new BattleParm();

#if UNITY_EDITOR
        public static bool TestQuickBattle = false;
        public static bool TestBattleNoParticle = false;
#endif
    }
}