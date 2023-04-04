using KVLib;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace LiaoZhai.Runtime
{
    public class CreateAbilityLinearProjectileData
    {
        private HashSet<int> _unitTargetTraits = null;
        private HashSet<int> _unitTargetExclutedTraits = null;
        private int[] _unitTargetModifierGroups = null;
        private int[] _unitTargetExclutedModifierGroups = null;

        // 攻击方
        public BaseUnit Attacker { get; set; }
        // 技能
        public Ability Ability { get; set; }
        // 移动速度
        public DFix64 MoveSpeed { get; set; }
        // 起始方向
        public DFixVector3 Velocity { get; set; }
        // 移动距离
        public DFix64 Distance { get; set; }
        // 起始半径
        public DFix64 StartRadius { get; set; }
        // 结束半径
        public DFix64 EndRadius { get; set; }
        // 是否为锥形区域,忽略起始和结束的外半径
        public bool IsFrontalCone { get; set; }
        // 目标队伍
        public AbilityUnitTargetTeam UnitTargetTeams { get; set; }
        // 目标类型
        public AbilityUnitTargetType UnitTargetTypes { get; set; }
        // 目标筛选
        public AbilityUnitTargetFlag UnitTargetFlags { get; set; }
        // 目标排除筛选
        public AbilityUnitTargetFlag UnitTargetExclutedFlags { get; set; }
        // 目标满足特质
        public HashSet<int> UnitTargetTraits { get { return _unitTargetTraits; } set { _unitTargetTraits = value; } }
        // 目标排除特质
        public HashSet<int> UnitTargetExclutedTraits { get { return _unitTargetExclutedTraits; } set { _unitTargetExclutedTraits = value; } }
        // 目标满足Modifier组
        public int[] UnitTargetModifierGroups { get { return _unitTargetModifierGroups; } set { _unitTargetModifierGroups = value; } }
        // 目标排除Modifier组
        public int[] UnitTargetExclutedModifierGroups { get { return _unitTargetExclutedModifierGroups; } set { _unitTargetExclutedModifierGroups = value; } }
        // 击中操作
        public KeyValue ProjectileHitKv { get; set; }
        // 结束操作
        public KeyValue ProjectileFinishKv { get; set; }
        // 弹道特效
        public string EffectName { get; set; }
        // 起始挂点
        public string SpawnOriginAttachPoint { get; set; }
        // 击中第一个目标后删除
        public bool IsDeleteOnHit { get; set; }
    }

    public class AbilityLinearProjectile : BaseProjectile
    {
        private CreateAbilityLinearProjectileData _createData = null;                       // 初始数据

        private List<DFixVector3> _conePos = new List<DFixVector3>();                                             // 当前半径
        private DFixVector3 _velocity = DFixVector3.Zero;
        private DFixVector3 _startPosition = DFixVector3.Zero;                                // 起始坐标
        private DFixVector3 _endPosition = DFixVector3.Zero;                                  // 终点坐标
        private DFixVector3 _movedLength = DFixVector3.Zero;                                             // 当前半径
        private DFix64 _movedDistance = DFix64.Zero;                                             // 当前半径
        private DFix64 _currRadius = DFix64.Zero;                                             // 当前半径
        private HashSet<BaseUnit> hitTargets = new HashSet<BaseUnit>();                     // 命中的目标
        private HashSet<BaseUnit> newTargets = new HashSet<BaseUnit>();


        public override void UpdateLogic(DFix64 frameLength)
        {
            if (IsDestroyed || !IsActivated || IsFree)
            {
                return;
            }

            _lastLogicPosition = _logicPosition;
            _lastLogicAngles = _logicEulerAngles;
            _lastLogicScale = _logicScale;

            if (!_isLanded)
            {
                _movedLength = _velocity * _createData.MoveSpeed * frameLength;
                _logicPosition = _logicPosition + _movedLength;

                _movedDistance = DFixVector3.Distance(_startPosition, _logicPosition);
                if (_movedDistance >= _createData.Distance)
                {
                    _logicPosition = _endPosition;
                    _movedDistance = _createData.Distance;
                    _isLanded = true;
                }

                if (_createData.EndRadius == _createData.StartRadius)
                {
                    _currRadius = _createData.StartRadius;
                }
                else
                {
                    if (_createData.Distance > DFix64.Zero)
                    {
                        _currRadius = _createData.StartRadius + (_createData.EndRadius - _createData.StartRadius) * (_movedDistance / _createData.Distance);
                    }
                    else
                    {
                        _currRadius = _createData.StartRadius;
                    }
                }

                CheckTargets();

                if (newTargets.Count > 0)
                {
                    foreach (BaseUnit newTarget in newTargets)
                    {
                        if (_createData.ProjectileHitKv != null)
                        {
                            EventData eventData = BattleData.CreateEventData();
                            eventData.Caster = _createData.Attacker;
                            eventData.Ability = _createData.Ability;
                            eventData.Target = newTarget;
                            eventData.Attacker = _createData.Attacker;
                            eventData.Unit = newTarget;
                            eventData.Point = _logicPosition;

                            BattleData.ExecuteActions(_createData.ProjectileHitKv, eventData);
                        }

                        if (_createData.IsDeleteOnHit)
                        {
                            _isLanded = true;
                            break;
                        }
                    }

                    newTargets.Clear();
                }

                if (_isLanded)
                {
                    if (_createData.ProjectileFinishKv != null)
                    {
                        EventData eventData = BattleData.CreateEventData();
                        eventData.Caster = _createData.Attacker;
                        eventData.Ability = _createData.Ability;
                        eventData.Attacker = _createData.Attacker;
                        eventData.Point = _logicPosition;

                        BattleData.ExecuteActions(_createData.ProjectileFinishKv, eventData);
                    }

                    FadeOut();
                }
            }
            else
            {
                if (_isFadeOut)
                {
                    _trailFadeOutTime += frameLength;

                    //for (int i = 0; i < _projectileEntityLogic._particlies.Count; i++)
                    //{
                    //    if (_projectileEntityLogic._particlies[i].particleCount > 0)
                    //    {
                    //        return;
                    //    }
                    //}

                    if (_trailFadeOutTime >= _trailFadeOutDuration)
                    {
                        SetFree();
                    }
                }
            }
        }

        public void Launch(CreateAbilityLinearProjectileData createData)
        {
            hitTargets.Clear();

            _createData = createData;

            if (!string.IsNullOrEmpty(_createData.SpawnOriginAttachPoint))
            {
                Transform attachSpawnPoint = _createData.Attacker.GetAttachPoint(_createData.SpawnOriginAttachPoint);
                if (attachSpawnPoint != null)
                {
                    _logicPosition = new DFixVector3((DFix64)attachSpawnPoint.position.x, (DFix64)attachSpawnPoint.position.y, (DFix64)attachSpawnPoint.position.z);
                }
                else
                {
                    _logicPosition = _createData.Attacker.LogicPosition;
                }
            }
            else
            {
                _logicPosition = _createData.Attacker.LogicPosition;
            }

            _logicPosition.y = DFix64.Zero;
            _velocity.y = DFix64.Zero;

            _velocity = _createData.Velocity;
            _startPosition = _logicPosition;
            _endPosition = _startPosition + _velocity * createData.Distance;

            _conePos.Clear();

            if (_createData.IsFrontalCone)
            {
                if (_createData.StartRadius > DFix64.Zero || _createData.EndRadius > DFix64.Zero)
                {
                    DFixVector3 start = _startPosition - _velocity * (DFix64)0.1f;
                    DFixVector3 end = _endPosition + _velocity * (DFix64)0.1f;

                    if (_createData.StartRadius > DFix64.Zero)
                    {
                        DFixVector3 right = DFixVector3.RotateInXZ2(_velocity * _createData.StartRadius, -(DFix64)90);
                        _conePos.Add(start + right);
                        _conePos.Add(start - right);
                    }
                    else
                    {
                        _conePos.Add(_startPosition);
                    }

                    if (_createData.EndRadius > DFix64.Zero)
                    {
                        DFixVector3 right = DFixVector3.RotateInXZ2(_velocity * _createData.EndRadius, -(DFix64)90);
                        _conePos.Add(end - right);
                        _conePos.Add(end + right);
                    }
                    else
                    {
                        _conePos.Add(_endPosition);
                    }
                }
            }

            LookToPositionY(_endPosition);
            SetActive(true);
        }

        public override void Destroy()
        {
            _createData = null;
            hitTargets.Clear();

            base.Destroy();
        }

        protected override void OnSetFree()
        {
            _createData = null;
            _conePos.Clear();
            hitTargets.Clear();
            newTargets.Clear();

            base.OnSetFree();
        }

        protected override Type GetEntityType()
        {
            return typeof(BattleProjectileEntityLogic);
        }

        protected void CheckTargets()
        {
            newTargets.Clear();

            if (_createData.IsFrontalCone && _conePos.Count == 0)
            {
                return;
            }

            for (int j = 0; j < UnitManager.Units.Count; j++)
            {
                BaseUnit baseUnit = UnitManager.Units[j];
                if (baseUnit.Type == UnitType.UNIT_THINKER || !baseUnit.IsSpawned)
                {
                    continue;
                }

                if (baseUnit == _createData.Attacker || hitTargets.Contains(baseUnit))
                {
                    continue;
                }

                if (!BattleData.IsUnitTargetValid(_createData.Attacker, baseUnit, _createData.UnitTargetTeams, _createData.UnitTargetTypes,
                    _createData.UnitTargetFlags, _createData.UnitTargetExclutedFlags, _createData.UnitTargetTraits, _createData.UnitTargetExclutedTraits,
                    _createData.UnitTargetModifierGroups, _createData.UnitTargetExclutedModifierGroups))
                {
                    continue;
                }

                if (_createData.IsFrontalCone)
                {
                    if (DFixVector3.Distance(_logicPosition, baseUnit.LogicPosition) > _currRadius)
                    {
                        continue;
                    }

                    if (!PointInPolygon(baseUnit.LogicPosition, _conePos.ToArray()))
                    {
                        continue;
                    }
                }
                else
                {
                    if (DFixVector3.Distance(_logicPosition, baseUnit.LogicPosition) > _currRadius)
                    {
                        continue;
                    }
                }

                newTargets.Add(baseUnit);
                hitTargets.Add(baseUnit);
            }
        }

        public static bool IsInPolygon(DFixVector3 checkPoint, List<DFixVector3> polygonPoints)
        {
            bool inside = false;
            int pointCount = polygonPoints.Count;
            DFixVector3 p1, p2;
            for (int i = 0, j = pointCount - 1; i < pointCount; j = i, i++)//第一个点和最后一个点作为第一条线，之后是第一个点和第二个点作为第二条线，之后是第二个点与第三个点，第三个点与第四个点...
            {
                p1 = polygonPoints[i];
                p2 = polygonPoints[j];
                if (checkPoint.z < p2.z)
                {//p2在射线之上
                    if (p1.z <= checkPoint.z)
                    {//p1正好在射线中或者射线下方
                        if ((checkPoint.z - p1.z) * (p2.x - p1.x) > (checkPoint.x - p1.x) * (p2.z - p1.z))//斜率判断,在P1和P2之间且在P1P2右侧
                        {
                            //射线与多边形交点为奇数时则在多边形之内，若为偶数个交点时则在多边形之外。
                            //由于inside初始值为false，即交点数为零。所以当有第一个交点时，则必为奇数，则在内部，此时为inside=(!inside)
                            //所以当有第二个交点时，则必为偶数，则在外部，此时为inside=(!inside)
                            inside = (!inside);
                        }
                    }
                }
                else if (checkPoint.z < p1.z)
                {
                    //p2正好在射线中或者在射线下方，p1在射线上
                    if ((checkPoint.z - p1.z) * (p2.x - p1.x) < (checkPoint.x - p1.x) * (p2.z - p1.z))//斜率判断,在P1和P2之间且在P1P2右侧
                    {
                        inside = (!inside);
                    }
                }
            }
            return inside;
        }

        public static bool PointInPolygon(DFixVector3 pnt, DFixVector3[] plg)
        {
            if (plg == null || plg.Length < 3)
            {
                return false;
            }
            bool flag = false;
            int i = 0;
            int num = plg.Length - 1;
            while (i < plg.Length)
            {
                DFixVector3 vInt = plg[i];
                DFixVector3 vInt2 = plg[num];
                if ((vInt.z <= pnt.z && pnt.z < vInt2.z) || (vInt2.z <= pnt.z && pnt.z < vInt.z))
                {
                    int num2 = (int)(vInt2.z - vInt.z);
                    long num3 = (long)(pnt.z - vInt.z) * (long)(vInt2.x - vInt.x) - (long)(pnt.x - vInt.x) * (long)num2;
                    if (num2 > 0)
                    {
                        if (num3 > 0L)
                        {
                            flag = !flag;
                        }
                    }
                    else if (num3 < 0L)
                    {
                        flag = !flag;
                    }
                }
                num = i++;
            }
            return flag;
        }
    }
}