// -----------------------------------------------
// Copyright © Jeffrey. All rights reserved.
// CreateTime: 2021/7/20   13:8:23
// -----------------------------------------------

using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace LiaoZhai.Runtime
{
    public class DropItemParticle : Particle
    {
        private DFixVector3 _startPos = DFixVector3.Zero;
        private DFixVector3 _jumpToPos = DFixVector3.Zero;
        private float _power = 0f;
        private float _duration = 0f;
        private bool _isStarted = false;


        public override void UpdateLogic(DFix64 frameLength)
        {

        }

        public override void UpdateRender(float interpolation, float deltaTime)
        {

        }

        public override void Destroy()
        {
            if (_battleEntityLogic != null && _isStarted)
            {
                _battleEntityLogic.CachedTransform.DOKill(false);
            }

            base.Destroy();
        }

        protected override void OnInstantiateSuccess()
        {
            base.OnInstantiateSuccess();

        }

        protected override void OnSetActive(bool isActivated)
        {
            base.OnSetActive(isActivated);

            if (isActivated)
            {
                if (!_isStarted)
                {
                    _isStarted = true;

                    if (_battleEntityLogic != null && _jumpToPos != DFixVector3.Zero)
                    {
                        if (_jumpToPos != DFixVector3.Zero)
                        {
                            DoJump(_startPos, _jumpToPos, _power, _duration);
                        }
                    }
                }
            }
        }

        protected override void OnSetFree()
        {
            _jumpToPos = DFixVector3.Zero;
            _isStarted = false;

            base.OnSetFree();
        }

        public void DoJump(DFixVector3 startPos, DFixVector3 jumpToPos, float power, float duration)
        {
            if (_isStarted)
            {
                return;
            }

            _startPos = startPos;
            _jumpToPos = jumpToPos;
            _power = power;
            _duration = duration;

            if (_isInstantiateComplete)
            {
                _isStarted = true;

                if (_battleEntityLogic != null)
                {
                    _battleEntityLogic.CachedTransform.DOKill(false);
                    _battleEntityLogic.CachedTransform.position = _startPos.ToVector3();
                    //_battleEntityLogic.CachedTransform.LookAt(_jumpToPos.ToVector3());

                    //DFix64 dis = DFixVector3.Distance(startPos, _jumpToPos);
                    //dis = dis / BattleData.BattleGrid.NodeSize;
                    //_duration = 0.833f * _duration * (float)dis;
                    _duration = 0.833f;

                    DoJump();
                }
            }
        }

        private void DoJump()
        {
            _battleEntityLogic.CachedTransform.DOJump(_jumpToPos.ToVector3(), _power, 1, _duration).SetEase(Ease.Linear).SetAutoKill(true);
            _jumpToPos = DFixVector3.Zero;
        }
    }
}