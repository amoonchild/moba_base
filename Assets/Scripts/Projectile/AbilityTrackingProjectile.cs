using KVLib;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace LiaoZhai.Runtime
{
    public class CreateAbilityTrackingProjectileData
    {
        // 攻击方
        public BaseUnit Attacker { get; set; }
        // 技能
        public Ability Ability { get; set; }
        // 起始目标
        public BaseUnit From { get; set; }
        // 攻击目标
        public BaseUnit Target { get; set; }
        // 移动速度
        public DFix64 MoveSpeed { get; set; }
        // 弹道特效
        public string EffectName { get; set; }
        // 起始挂点
        public string SpawnOriginAttachPoint { get; set; }
        // 命中操作
        public KeyValue ProjectileHitKv { get; set; }
    }

    public class AbilityTrackingProjectile : BaseProjectile
    {
        private CreateAbilityTrackingProjectileData _createData = null;                     // 初始数据


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
                DFixVector3 hitPosition = _createData.Target.LogicPosition;
                Transform attachHitPoint = _createData.Target.GetAttachPoint(Constant.Battle.UNIT_MODEL_ATTACH_POINT_XIONG);
                if (attachHitPoint != null)
                {
                    hitPosition = new DFixVector3((DFix64)attachHitPoint.position.x, (DFix64)attachHitPoint.position.y, (DFix64)attachHitPoint.position.z);
                }

                if (_logicPosition != hitPosition)
                {
                    DFixVector3 dir = hitPosition - _logicPosition;
                    DFixVector3 moveDir = _createData.MoveSpeed / DFix64.Hundred * frameLength * dir.GetNormalized();

                    if (DFixVector3.SqrMagnitude(moveDir) >= DFixVector3.SqrMagnitude(dir))
                    {
                        _isLanded = true;
                    }
                    else
                    {
                        LookToPositionY(hitPosition);
                        _logicPosition += moveDir;
                    }
                }
                else
                {
                    _isLanded = true;
                }

                if (_isLanded)
                {
                    _logicPosition = hitPosition;

                    if (_createData.ProjectileHitKv != null)
                    {
                        EventData eventData = BattleData.CreateEventData();
                        eventData.Caster = _createData.Attacker;
                        eventData.Ability = _createData.Ability;
                        eventData.Target = _createData.Target;
                        eventData.Attacker = _createData.Attacker;
                        eventData.Unit = _createData.Target;
                        eventData.Point = hitPosition;

                        BattleData.ExecuteActions(_createData.ProjectileHitKv, eventData);
                    }

                    FadeOut();
                }
            }
            else
            {
                if (_isFadeOut)
                {
                    _trailFadeOutTime += frameLength;
                    if (_trailFadeOutTime >= _trailFadeOutDuration)
                    {
                        SetFree();
                    }
                }
            }
        }

        public void Launch(CreateAbilityTrackingProjectileData createData)
        {
            _createData = createData;

            _logicPosition = _createData.From.LogicPosition;
            if (!string.IsNullOrEmpty(_createData.SpawnOriginAttachPoint))
            {
                Transform attachSpawnPoint = _createData.From.GetAttachPoint(_createData.SpawnOriginAttachPoint);
                if (attachSpawnPoint != null)
                {
                    _logicPosition = new DFixVector3((DFix64)attachSpawnPoint.position.x, (DFix64)attachSpawnPoint.position.y, (DFix64)attachSpawnPoint.position.z);
                }
            }

            Transform attachHitPoint = _createData.Target.GetAttachPoint(Constant.Battle.UNIT_MODEL_ATTACH_POINT_XIONG);
            if (attachHitPoint != null)
            {
                DFixVector3 hitPosition = new DFixVector3((DFix64)attachHitPoint.position.x, (DFix64)attachHitPoint.position.y, (DFix64)attachHitPoint.position.z);
                LookToPositionY(hitPosition);
            }
            else
            {
                LookToPositionY(_createData.Target.LogicPosition);
            }

            SetActive(true);
        }

        public override void Destroy()
        {
            _createData = null;

            base.Destroy();
        }

        protected override Type GetEntityType()
        {
            return typeof(BattleProjectileEntityLogic);
        }

        protected override void OnSetFree()
        {
            _createData = null;

            base.OnSetFree();
        }
    }
}