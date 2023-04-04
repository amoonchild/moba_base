using UnityEngine;


namespace LiaoZhai.Runtime
{
    public class BattleEntityLogic : EntityLogicBase
    {

        public virtual void Recycle()
        {

        }

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);

            BattleEntityData battleEntityData = (BattleEntityData)userData;
            battleEntityData.Entity.OnShowEntitySuccess(this);
        }

        protected internal override void OnHide(bool isShutdown, object userData)
        {
            gameObject.SetActive(false);

            base.OnHide(isShutdown, userData);
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
        }
    }
}