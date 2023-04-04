using GameFramework;


namespace LiaoZhai.Runtime
{
    public abstract class BaseOrder : IReference
    {
        protected BaseUnit _source = null;
        protected bool _isFinished = false;

        public abstract UnitOrderType Type
        {
            get;
        }
        public BaseUnit Source
        {
            get
            {
                return _source;
            }
        }
        public bool IsFinished
        {
            get
            {
                return _isFinished;
            }
        }


        public BaseOrder(BaseUnit source)
        {
            _source = source;
        }

        public virtual bool IsExecutable()
        {
            return false;
        }

        public virtual bool Begin()
        {
            return false;
        }

        public virtual void UpdateLogic(DFix64 frameLength)
        {

        }

        public virtual void Interrupt()
        {
            _isFinished = true;
        }

        public virtual void Quit()
        {
            _isFinished = true;
        }

        public virtual void Clear()
        {
            _source = null;
            _isFinished = false;
        }
    }
}
