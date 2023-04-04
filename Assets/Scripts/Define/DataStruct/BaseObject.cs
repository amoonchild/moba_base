

namespace LiaoZhai.Runtime
{
    public class BaseObject
    {
        protected int _objectId = 0;
        protected int _createdFrame = 0;

        public int ObjectId
        {
            get
            {
                return _objectId;
            }
        }
        public int CreatedFrame
        {
            get
            {
                return _createdFrame;
            }
        }


        public BaseObject()
        {
            Reset();
        }

        public virtual void Reset()
        {
            _objectId = BattleData.NextObjectId;
            _createdFrame = BattleData.LogicFrame;
        }

        public virtual void Init()
        {

        }

        public virtual void Release()
        {

        }

        public virtual void UpdateLogic(DFix64 frameLength)
        {

        }

        public virtual void UpdateRender(float interpolation, float deltaTime)
        {

        }
    }
}