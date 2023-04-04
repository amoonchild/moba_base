

namespace LiaoZhai.Runtime
{
    public abstract class CastAbilityOrder : BaseOrder
    {
        protected Ability _ability = null;
        protected DFix64 _time = DFix64.Zero;
        protected bool _isPlayedAnimation = false;
        protected bool _isAnimationEnd = false;

        public Ability Ability
        {
            get
            {
                return _ability;
            }
        }


        public CastAbilityOrder(BaseUnit source, Ability ability)
            : base(source)
        {
            _ability = ability;
        }

        public override bool IsExecutable()
        {
            if (!_ability.IsFullyCastable())
            {
                return false;
            }

            if (_source.IsDeadState || _source.IsResticted || _source.IsStunned || _source.IsActivatesAbilityDisabled)
            {
                return false;
            }

            return true;
        }
    }
}
