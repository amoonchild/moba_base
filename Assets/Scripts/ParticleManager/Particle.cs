using System;
using System.Collections.Generic;
using UnityEngine;


namespace LiaoZhai.Runtime
{
    public enum ParticleControlType
    {
        Position = 0,
        Scale,
        Angle,
        LookAt,
        Speed,
        Jump,
    }

    // 特效
    public class Particle : BaseEntity
    {
        protected bool _isFadeOut = false;                                                  // 是否正在淡出销毁(等待粒子发射器结束,拖尾结束)
        protected DFix64 _trailFadeOutDuration = DFix64.Zero;                                 // 拖尾延迟销毁时间
        protected DFix64 _trailFadeOutTime = DFix64.Zero;                                     // 拖尾延迟销毁计时
        protected BaseUnit _attachUnit = null;                                              // 挂点单位
        protected string _attachPointName = string.Empty;                                   // 挂点名
        protected bool _isFollowPosition = false;                                           // 跟随位置
        protected bool _isFollowRotation = false;                                           // 跟随旋转
        protected bool _isFollowScale = false;                                              // 跟随缩放
        protected bool _isAlwaysFollow = false;                                             // 保持跟随
        protected bool _isFirstFollow = false;                                              // 
        protected float _selfSpeed = 1f;
        protected int _playingAniamtionIndex = 0;
        protected int _playingParticleIndex = 0;
        protected BattleParticleEntityLogic _particleEntityLogic = null;
        protected int _attachTargetId = 0;
        protected bool _isLoop = false;

        public int AttachTargetId
        {
            get { return _attachTargetId; }
            set { _attachTargetId = value; }
        }


        public override void UpdateLogic(DFix64 frameLength)
        {
            if (!IsActivated || IsDestroyed || IsFree || !_isInstantiateComplete)
            {
                return;
            }

            if (_particleEntityLogic == null)
            {
                if (AttachTargetId == 0)
                {
                    SetFree();
                }

                return;
            }

            if (!_isFadeOut)
            {
                for (int i = _playingAniamtionIndex, count = _particleEntityLogic._animaitons.Count; i < count; i++)
                {
                    _playingAniamtionIndex = i;
                    if (_particleEntityLogic._animaitons[i].isPlaying)
                    {
                        return;
                    }
                }

                for (int i = _playingParticleIndex, count = _particleEntityLogic._particlies.Count; i < count; i++)
                {
                    _playingParticleIndex = i;
                    if (_particleEntityLogic._particlies[i].isPlaying)
                    {
                        return;
                    }
                }

                FadeOut();
            }
            else
            {
                _trailFadeOutTime += frameLength;
                for (int i = 0, count = _particleEntityLogic._particlies.Count; i < count; i++)
                {
                    if (_particleEntityLogic._particlies[i].particleCount > 0)
                    {
                        return;
                    }
                }

                if (_trailFadeOutTime >= _trailFadeOutDuration)
                {
                    if (AttachTargetId == 0)
                    {
                        SetFree();
                    }
                }
                else
                {

                }
            }
        }

        public override void UpdateRender(float interpolation, float deltaTime)
        {
            if (!IsActivated || IsDestroyed || IsFree || !_isInstantiateComplete)
            {
                return;
            }

            if (_particleEntityLogic != null)
            {
                if (_isAlwaysFollow || _isFirstFollow)
                {
                    UpdateAttach();
                    _isFirstFollow = false;
                }

                _particleEntityLogic.CachedTransform.position = _renderPosition;
                _particleEntityLogic.CachedTransform.eulerAngles = _renderEulerAngles;
                _particleEntityLogic.CachedTransform.localScale = _renderScale;
            }
        }

        public override void Destroy()
        {
            _isFree = true;
            _isFadeOut = false;
            _attachUnit = null;
            _attachPointName = string.Empty;
            _attachTargetId = 0;
            _particleEntityLogic = null;

            base.Destroy();
        }

        public override void SyncRender()
        {
            if (_battleEntityLogic != null)
            {
                _battleEntityLogic.CachedTransform.position = _renderPosition;
                _battleEntityLogic.CachedTransform.eulerAngles = _renderEulerAngles;
                _battleEntityLogic.CachedTransform.localScale = _renderScale;
            }
        }

        public override void SyncRenderPosition()
        {
            if (_battleEntityLogic != null)
            {
                _battleEntityLogic.CachedTransform.position = _renderPosition;
            }
        }

        public override void SyncRenderRotation()
        {
            if (_battleEntityLogic != null)
            {
                _battleEntityLogic.CachedTransform.eulerAngles = _renderEulerAngles;
            }
        }

        public override void SyncRenderScale()
        {
            if (_battleEntityLogic != null)
            {
                _battleEntityLogic.CachedTransform.localScale = _renderScale;
            }
        }

        public virtual void SetAttach(BaseUnit attachTarget, string attachPointName, bool followPosition, bool followRotation, bool followScale, bool isAlwaysFollow)
        {
            _attachUnit = attachTarget;
            _attachPointName = attachPointName;
            _isFollowPosition = followPosition;
            _isFollowRotation = followRotation;
            _isFollowScale = followScale;
            _isAlwaysFollow = isAlwaysFollow;
            _isFirstFollow = true;
        }

        public virtual void SetParticleControl(ParticleControlType controlIndex, DFixVector3 controlData, bool isSync)
        {
            switch (controlIndex)
            {
                case ParticleControlType.Position:// 坐标
                    {
                        _renderPosition = controlData.ToVector3();
                        if (isSync)
                        {
                            //SyncRenderPosition();
                        }
                    }
                    break;
                case ParticleControlType.Scale:// 半径
                    {
                        _renderScale = controlData.ToVector3() * 2f;
                        if (isSync)
                        {
                            //SyncRenderScale();
                        }
                    }
                    break;
                case ParticleControlType.Angle:// 朝向
                    {
                        _renderEulerAngles = controlData.ToVector3();
                        if (isSync)
                        {
                            //SyncRenderRotation();
                        }
                    }
                    break;
                case ParticleControlType.LookAt:// 朝向
                    {
                        Vector3 curr = _renderPosition;
                        Vector3 lookTo = _renderPosition + controlData.ToVector3();
                        Vector3 lookDir = lookTo - curr;
                        lookDir = lookDir - Vector3.Project(lookDir, Vector3.up);
                        lookDir.Normalize();

                        float angle = Vector3.Angle(Vector3.forward, lookDir);
                        float dir = (Vector3.Dot(Vector3.up, Vector3.Cross(Vector3.forward, lookDir)) < 0 ? -1 : 1);
                        angle *= dir;

                        _renderEulerAngles.y = angle;
                        if (isSync)
                        {
                            //SyncRenderRotation();
                        }
                    }
                    break;
                case ParticleControlType.Speed:
                    {
                        _selfSpeed = (float)controlData.x;
                        if (_particleEntityLogic != null)
                        {
                            _particleEntityLogic.ChangeSpeed(_speed * _selfSpeed);
                        }
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
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

        protected override Type GetEntityType()
        {
            return typeof(BattleParticleEntityLogic);
        }

        protected override void OnInstantiateSuccess()
        {
            _particleEntityLogic = _battleEntityLogic as BattleParticleEntityLogic;

            if (!_isLoop)
            {
                for (int i = 0; i < _particleEntityLogic._animaitons.Count; i++)
                {
                    Animation animation = _particleEntityLogic._animaitons[i];
                    if (animation.clip != null && animation.clip.wrapMode == WrapMode.Loop)
                    {
                        animation.clip.wrapMode = WrapMode.Once;
                        Log.Fatal("FireEffect: 循环动画 {0} -> {1}", _assetName, animation.clip.name);
                    }
                }

                for (int i = 0; i < _particleEntityLogic._particlies.Count; i++)
                {
                    ParticleSystem.MainModule mainModule = _particleEntityLogic._particlies[i].main;
                    if (mainModule.loop)
                    {
                        mainModule.loop = false;
                        Log.Fatal("FireEffect: 循环特效 {0} -> {1}", _assetName, _particleEntityLogic._particlies[i].name);
                    }
                }
            }
        }

        protected override void OnInstantiateFailure()
        {

        }

        protected override void OnSetActive(bool isActivated)
        {
            if (_particleEntityLogic != null)
            {
                if (isActivated)
                {
                    for (int i = 0, count = _particleEntityLogic._trailRenderers.Count; i < count; i++)
                    {
                        _particleEntityLogic._trailRenderers[i].Clear();
                    }

                    if (_isFirstFollow)
                    {
                        _isFirstFollow = false;
                        UpdateAttach();
                    }

                    _particleEntityLogic.CachedTransform.position = _renderPosition;
                    _particleEntityLogic.CachedTransform.eulerAngles = _renderEulerAngles;
                    _particleEntityLogic.CachedTransform.localScale = _renderScale;
                }

                _particleEntityLogic.gameObject.SetActive(isActivated);
            }
        }

        protected virtual void UpdateAttach()
        {
            if (_attachUnit != null)
            {
                Transform attachPoint = _attachUnit.GetAttachPoint(_attachPointName);
                if (attachPoint != null)
                {
                    if (_isFollowPosition || _isFirstFollow)
                    {
                        _renderPosition = attachPoint.position;
                    }

                    if (_isFollowRotation)
                    {
                        _renderEulerAngles = attachPoint.eulerAngles;
                    }

                    if (_isFollowScale)
                    {
                        _renderScale = attachPoint.lossyScale;
                    }
                }
                else
                {
                    if (_isFollowPosition || _isFirstFollow)
                    {
                        _renderPosition = _attachUnit.RenderPosition;
                    }

                    if (_isFollowRotation)
                    {
                        _renderEulerAngles = _attachUnit.RenderEulerAngles;
                    }

                    if (_isFollowScale)
                    {
                        _renderScale = _attachUnit.RenderScale;
                    }
                }
            }
        }

        protected virtual void FadeOut()
        {
            if (_isFadeOut)
            {
                return;
            }

            if (_particleEntityLogic != null)
            {
                bool hasParticle = false;
                for (int i = 0, count = _particleEntityLogic._particlies.Count; i < count; i++)
                {
                    ParticleSystem particleSystem = _particleEntityLogic._particlies[i];

                    //ParticleSystem.EmissionModule emission = particleSystem.emission;
                    //emission.enabled = false;

                    if (particleSystem.particleCount > 0)
                    {
                        hasParticle = true;
                    }
                }

                float trailFadeOutDuration = 0f;
                for (int i = 0, count = _particleEntityLogic._trailRenderers.Count; i < count; i++)
                {
                    TrailRenderer trailRenderer = _particleEntityLogic._trailRenderers[i];
                    if (trailRenderer.time > trailFadeOutDuration)
                    {
                        trailFadeOutDuration = trailRenderer.time;
                    }
                }

                _trailFadeOutTime = DFix64.Zero;
                _trailFadeOutDuration = (DFix64)trailFadeOutDuration;

                if (hasParticle || _trailFadeOutDuration > DFix64.Zero)
                {
                    _isFadeOut = true;
                }
                else
                {
                    if (AttachTargetId == 0)
                    {
                        SetFree();
                    }
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
            _attachUnit = null;
            _attachPointName = string.Empty;
            _isFollowPosition = false;
            _isFollowRotation = false;
            _isFollowScale = false;
            _isAlwaysFollow = false;
            _isFirstFollow = false;
            _selfSpeed = 1f;
            _playingAniamtionIndex = 0;
            _playingParticleIndex = 0;
            _trailFadeOutDuration = DFix64.Zero;
            _trailFadeOutTime = DFix64.Zero;
            _attachTargetId = 0;
        }

        public override void GetFree()
        {
            base.GetFree();

            //if(_particleEntityLogic!= null)
            //{
            //    for (int i = 0, count = _particleEntityLogic._trailRenderers.Count; i < count; i++)
            //    {
            //        _particleEntityLogic._trailRenderers[i].Clear();
            //    }
            //}
        }

        public override void ChangeSpeed(float speed)
        {
            base.ChangeSpeed(speed);

            if (_particleEntityLogic != null)
            {
                _particleEntityLogic.ChangeSpeed(speed * _selfSpeed);
            }
        }

        public void SetIsLoop(bool isLoop)
        {
            _isLoop = isLoop;
        }
    }
}