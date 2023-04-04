using KVLib;


namespace LiaoZhai.Runtime
{
    public class Thinker : BaseUnit
    {
        public override UnitType Type
        {
            get
            {
                return UnitType.UNIT_THINKER;
            }
        }


        public Thinker(CreateThinkerData createThinkerData)
            : base()
        {
            Master = createThinkerData.Caster;

            _playerId = Master.PlayerId;
            _player = BattleData.GetPlayer(Master.PlayerId);
            _bagIndex = Master.BagIndex;
            _lineupIndex = Master.LineupIndex;
            TeamId = Master.TeamId;
            _name = GameFramework.Utility.Text.Format("{0} Thinker", Master.Name);
            _level = Master.Level;
            _battleScore = Master.BattleScore;
            _unitId = Master.UnitId;

            _traitType = Master.TraitType;
            _attackCapability = Master.AttackCapability;
            _skinId = Master.SkinId;

            _initAtt = Master.CurrAtt.Clone();
            _currAtt = Master.CurrAtt.Clone();
            _statisticsData = new BattleUnitStatisticsData();

            if (createThinkerData.Node != null)
            {
                _currNode = createThinkerData.Node;
                ResetToNodePosition();
            }
            else
            {
                LogicPosition = createThinkerData.Point;
                _currNode = BattleData.FindNodeByPoint(LogicPosition);
            }

            LogicEulerAngles = createThinkerData.EulerAngles;

            _realScale = DFix64.Max(DFix64.Zero, _currAtt.Scale * _modelScale);
        }

        public override void SpawnToLineup(BattleNode node, bool isShowSpawnTX, bool isInitLineup)
        {
            if (_isSpawnedToLineup)
            {
                return;
            }

            if (node == null)
            {
                return;
            }

            _isSpawnedToLineup = true;
        }

        public override void Spawn(bool isShowSpawnEffects)
        {
            if (_isSpawned)
            {
                return;
            }

            _isSpawned = true;
            _isSpawnedToLineup = true;

            Log.Info("{0} 上场", LogName);

            ApplyModifier(Master, null, Constant.Battle.BUILTIN_MODIFIER_THINKER, null, false);
        }

        public override void Kill(BaseUnit killer, Ability killerAbility)
        {
            if (!_isSpawned)
            {
                return;
            }

            if (_isDeadState)
            {
                return;
            }

            _isDeadState = true;
            _isHideState = true;
            _currNode = null;

            Log.Info("{0} 死亡", LogName);

            RemoveAllModifersOnDeath();

            ApplyModifier(null, null, Constant.Battle.BUILTIN_MODIFIER_DEATH, null, false);
        }

        public override void UpdateLogic(DFix64 frameLength)
        {
            UpdateModifiers(frameLength);
        }

        protected override void UpdateModelRender(float interpolation, float deltaTime)
        {

        }
    }
}