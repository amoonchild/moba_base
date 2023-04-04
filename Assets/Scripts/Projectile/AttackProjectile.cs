using System;
using KVLib;
using UnityEngine;


namespace LiaoZhai.Runtime
{
    public class CreateAttackProjectileData
    {
        // 攻击方
        public BaseUnit Attacker { get; set; }
        // 发射时的属性
        public BattleUnitAttribute AttackerAtt { get; set; }
        // 攻击目标
        public BaseUnit Target { get; set; }
        // 是否暴击
        public bool IsCrit { get; set; }
        // 移动速度
        public DFix64 MoveSpeed { get; set; }
        // 起始挂点
        public string SpawnOriginAttachPoint { get; set; }
        // 击中特效
        public string HitEffectName { get; set; }
        // 击中音效
        public KeyValue HitSoundKv { get; set; }
    }

    public class AttackProjectile : BaseProjectile
    {
        private CreateAttackProjectileData _createData = null;                              // 初始数据


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
                DFixVector3 dir = DFixVector3.Zero;

                DFixVector3 hitPosition = _createData.Target.LogicPosition;
                Transform attachHitPoint = _createData.Target.GetAttachPoint(Constant.Battle.UNIT_MODEL_ATTACH_POINT_XIONG);
                if (attachHitPoint != null)
                {
                    hitPosition = new DFixVector3((DFix64)attachHitPoint.position.x, (DFix64)attachHitPoint.position.y, (DFix64)attachHitPoint.position.z);
                }

                if (_logicPosition != hitPosition)
                {
                    dir = hitPosition - _logicPosition;
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

                    if (!string.IsNullOrEmpty(_createData.HitEffectName))
                    {
                        int particleId = ParticleManager.CreateParticle(_createData.HitEffectName, false);
                        ParticleManager.SetParticleAttach(particleId, _createData.Target, Constant.Battle.UNIT_MODEL_ATTACH_POINT_XIONG, true, false, true, false);
                        ParticleManager.SetParticleControl(particleId, ParticleControlType.LookAt, dir, true);
                    }

                    if (_createData.HitSoundKv != null)
                    {
                        BattleData.FireSound(_createData.Target, _createData.HitSoundKv);
                    }

                    BattleData.ApplyAttackDamage(_createData.Attacker, _createData.AttackerAtt, _createData.Target, _createData.IsCrit);
                   
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

        public override void Destroy()
        {
            _createData = null;

            base.Destroy();
        }

        public void Launch(CreateAttackProjectileData createData)
        {
            _createData = createData;

            _logicPosition = _createData.Attacker.LogicPosition;
            if (!string.IsNullOrEmpty(_createData.SpawnOriginAttachPoint))
            {
                Transform attachSpawnPoint = _createData.Attacker.GetAttachPoint(_createData.SpawnOriginAttachPoint);
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

        protected override void OnSetFree()
        {
            _createData = null;

            base.OnSetFree();
        }

        protected override Type GetEntityType()
        {
            return typeof(BattleProjectileEntityLogic);
        }
    }
}