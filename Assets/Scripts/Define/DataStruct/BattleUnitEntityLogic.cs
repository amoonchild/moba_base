using System.Collections.Generic;
using UnityEngine;
using LiaoZhai.RT;


namespace LiaoZhai.Runtime
{
    public class BattleUnitEntityLogic : BattleEntityLogic
    {
        private Transform _shadow = null;
        private Dictionary<string, Transform> _searchedAttachPoints = new Dictionary<string, Transform>();      // 已找到的挂点
        private HashSet<string> _invalidAttachPoints = new HashSet<string>();                                         // 找不到的挂点
        private List<Animation> allAnimations = new List<Animation>();


        public AnimationState PlayerAnimation(string name, float speed, float time, bool crossFade)
        {
            if (allAnimations.Count == 0)
            {
                return null;
            }

            AnimationState mainState = null;
            for (int i = 0; i < allAnimations.Count; i++)
            {
                AnimationState playState = null;
                foreach (AnimationState aniState in allAnimations[i])
                {
                    if (aniState.name == name)
                    {
                        if (time > aniState.clip.length)
                        {
                            if (aniState.clip.wrapMode == WrapMode.Loop)
                            {
                                time = Mathf.Repeat(time, aniState.clip.length);
                            }
                            else
                            {
                                return null;
                            }
                        }

                        playState = aniState;
                        aniState.speed = speed;
                        aniState.time = time;

                        if (i == 0)
                        {
                            mainState = aniState;
                        }
                    }
                }

                if (playState != null)
                {
                    if (crossFade && allAnimations[i].isPlaying)
                    {
                        allAnimations[i].CrossFade(name, 0.3f);
                    }
                    else
                    {
                        allAnimations[i].Play(name);
                    }
                }
                else
                {
                    Log.Warning("BattleUnitEntityLogic line:67, {0} 找不到animation clip: {1}", CachedTransform.name, name);
                }
            }

            return mainState;
        }

        public void SetAnimationSpeed(float speed)
        {
            for (int i = 0; i < allAnimations.Count; i++)
            {
                foreach (AnimationState aniState in allAnimations[i])
                {
                    aniState.speed = speed;
                }
            }
        }

        public void SetAnimationTime(float time)
        {
            for (int i = 0; i < allAnimations.Count; i++)
            {
                foreach (AnimationState aniState in allAnimations[i])
                {
                    if (aniState.name == name)
                    {
                        aniState.time = time;
                    }
                }
            }
        }

        public void StopAnimation()
        {
            for (int i = 0; i < allAnimations.Count; i++)
            {
                allAnimations[i].Stop();
            }
        }

        public void ShowShadow(bool isShow)
        {
            if (_shadow != null)
            {
                _shadow.gameObject.SetActive(isShow);
            }
        }

        public virtual bool HasAttachPoint(string attachPointName)
        {
            if (string.IsNullOrEmpty(attachPointName))
            {
                return false;
            }

            if (_invalidAttachPoints.Contains(attachPointName))
            {
                return false;
            }

            if (_searchedAttachPoints.ContainsKey(attachPointName))
            {
                return true;
            }

            Transform attachPoint = RecursiveFindAttachPoint(CachedTransform, attachPointName);
            if (attachPoint == null)
            {
                _invalidAttachPoints.Add(attachPointName);
                return false;
            }

            _searchedAttachPoints.Add(attachPointName, attachPoint);
            return true;
        }

        public virtual Transform GetAttachPoint(string attachPointName)
        {
            if (!HasAttachPoint(attachPointName))
            {
                return CachedTransform;
            }

            return _searchedAttachPoints[attachPointName];
        }

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);

            ReferenceCollector referenceCollector = CachedTransform.GetComponent<ReferenceCollector>();
            if (referenceCollector != null)
            {
                Transform subChild = referenceCollector.GetTransform(Constant.Battle.UNIT_MODEL_ATTACH_POINT_BLOODBAR);
                if (subChild != null)
                {
                    AddAttachPoint(Constant.Battle.UNIT_MODEL_ATTACH_POINT_BLOODBAR, subChild);
                }

                subChild = referenceCollector.GetTransform(Constant.Battle.UNIT_MODEL_ATTACH_POINT_XIONG);
                if (subChild != null)
                {
                    AddAttachPoint(Constant.Battle.UNIT_MODEL_ATTACH_POINT_XIONG, subChild);
                }

                subChild = referenceCollector.GetTransform(Constant.Battle.UNIT_MODEL_ATTACH_POINT_DANDAO);
                if (subChild != null)
                {
                    AddAttachPoint(Constant.Battle.UNIT_MODEL_ATTACH_POINT_DANDAO, subChild);
                }

                subChild = referenceCollector.GetTransform(Constant.Battle.UNIT_MODEL_ATTACH_POINT_GUANGHUAN);
                if (subChild != null)
                {
                    AddAttachPoint(Constant.Battle.UNIT_MODEL_ATTACH_POINT_GUANGHUAN, subChild);
                }

                _shadow = referenceCollector.GetTransform("yinying");
            }

            if (_shadow == null)
            {
                _shadow = RecursiveFindAttachPoint(CachedTransform, "yinying");
            }

            Animation[] animations = CachedTransform.GetComponentsInChildren<Animation>();
            if (animations != null)
            {
                for (int i = 0; i < animations.Length; i++)
                {
                    allAnimations.Add(animations[i]);
                }
            }
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

        protected virtual Transform RecursiveFindAttachPoint(Transform child, string attachPointName)
        {
            for (int i = 0; i < child.childCount; i++)
            {
                Transform subChild = child.GetChild(i);
                if (subChild.name.Equals(attachPointName))
                {
                    return subChild;
                }

                subChild = RecursiveFindAttachPoint(subChild, attachPointName);
                if (subChild != null)
                {
                    return subChild;
                }
            }

            return null;
        }

        protected virtual void AddAttachPoint(string name, Transform child)
        {
            if (child != null)
            {
                _searchedAttachPoints.Add(name, child);
            }
            else
            {
                _invalidAttachPoints.Add(name);
            }
        }
    }
}