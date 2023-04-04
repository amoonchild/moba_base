using System.Collections.Generic;
using UnityEngine;


namespace LiaoZhai.Runtime
{
    public class BattleParticleEntityLogic : BattleEntityLogic
    {
        public List<Animation> _animaitons = new List<Animation>();                      // 所有动画组建
        public List<ParticleSystem> _particlies = new List<ParticleSystem>();            // 所有粒子组建
        public List<TrailRenderer> _trailRenderers = new List<TrailRenderer>();          // 所有拖尾组建


        public void ChangeSpeed(float speed)
        {
            for (int i = 0; i < _animaitons.Count; i++)
            {
                Animation animation = _animaitons[i];
                if (animation.clip != null)
                {
                    animation[animation.clip.name].speed = speed;
                }
            }

            for (int i = 0; i < _particlies.Count; i++)
            {
                ParticleSystem.MainModule mainModule = _particlies[i].main;
                mainModule.simulationSpeed = speed;
            }
        }

        public override void Recycle()
        {
            //for (int i = 0, count = _animaitons.Count; i < count; i++)
            //{
            //    foreach (AnimationState animationState in _animaitons[i])
            //    {
            //        animationState.speed = 1f;
            //    }
            //}

            //for (int i = 0, count = _particlies.Count; i < count; i++)
            //{
            //    ParticleSystem.MainModule main = _particlies[i].main;
            //    main.simulationSpeed = 1f;

            //    ParticleSystem.EmissionModule emission = _particlies[i].emission;
            //    emission.enabled = true;

            //    //_particlies[i].Clear();
            //}

            //for (int i = 0, count = _trailRenderers.Count; i < count; i++)
            //{
            //    _trailRenderers[i].Clear();
            //}

            gameObject.SetActive(false);
        }

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);

            CachedTransform.GetComponentsInChildren<Animation>(_animaitons);
            CachedTransform.GetComponentsInChildren<ParticleSystem>(_particlies);
            CachedTransform.GetComponentsInChildren<TrailRenderer>(_trailRenderers);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
        }

        protected internal override void OnHide(bool isShutdown, object userData)
        {

            base.OnHide(isShutdown, userData);
        }

        protected override void InternalSetVisible(bool visible)
        {

        }
    }
}