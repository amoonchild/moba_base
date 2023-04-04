using System;
using UnityEngine;


namespace LiaoZhai.Runtime
{
    public abstract class BaseProjectile : BaseEntity
    {
        protected bool _isFadeOut = false;                                                  // 是否正在淡出销毁(等待粒子发射器结束,拖尾结束)
        protected DFix64 _trailFadeOutDuration = DFix64.Zero;                                 // 拖尾延迟销毁时间
        protected DFix64 _trailFadeOutTime = DFix64.Zero;                                     // 拖尾延迟销毁计时
        protected bool _isLanded = false;                                                   // 是否到达
        protected BattleProjectileEntityLogic _projectileEntityLogic = null;


        public override void UpdateLogic(DFix64 frameLength)
        {
            base.UpdateLogic(frameLength);
        }

        public override void UpdateRender(float interpolation, float deltaTime)
        {
            base.UpdateRender(interpolation, deltaTime);
        }

        protected override void Instantiate()
        {
#if UNITY_EDITOR
            if (BattleData.TestBattleNoParticle)
            {
                _isDestroyed = false;
                _isInstantiateComplete = true;
                OnSetActive(IsActivated);

                return;
            }
#endif
            base.Instantiate();
        }

        protected override void OnInstantiateSuccess()
        {
            _projectileEntityLogic = _battleEntityLogic as BattleProjectileEntityLogic;
        }

        protected override void OnInstantiateFailure()
        {

        }

        protected override void OnSetActive(bool isActivated)
        {
            base.OnSetActive(isActivated);

            if (isActivated && _projectileEntityLogic != null)
            {
                for (int i = 0, count = _projectileEntityLogic._trailRenderers.Count; i < count; i++)
                {
                    _projectileEntityLogic._trailRenderers[i].Clear();
                }
            }
        }

        protected virtual void FadeOut()
        {
            if (_isFadeOut)
            {
                return;
            }

            if (_projectileEntityLogic != null)
            {
                float trailFadeOutDuration = 0f;
                for (int i = 0, count = _projectileEntityLogic._trailRenderers.Count; i < count; i++)
                {
                    TrailRenderer trailRenderer = _projectileEntityLogic._trailRenderers[i];
                    if (trailRenderer.time > trailFadeOutDuration)
                    {
                        trailFadeOutDuration = trailRenderer.time;
                    }
                }

                _trailFadeOutTime = DFix64.Zero;
                _trailFadeOutDuration = (DFix64)trailFadeOutDuration;

                if (_trailFadeOutDuration > DFix64.Zero)
                {
                    _isFadeOut = true;
                }
                else
                {
                    SetFree();
                }
            }
            else
            {
                SetFree();
            }
        }

        protected override void OnSetFree()
        {
            _isFadeOut = false;
            _trailFadeOutDuration = DFix64.Zero;
            _trailFadeOutTime = DFix64.Zero;
            _isLanded = false;
        }

        public override void ChangeSpeed(float speed)
        {
            base.ChangeSpeed(speed);

            if (_projectileEntityLogic != null)
            {
                _projectileEntityLogic.ChangeSpeed(speed);
            }
        }
    }
}