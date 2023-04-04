using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 首领
    public class Boss : BaseUnit
    {
        private int _occId = 0;
        private int _lastAutoCastAbilityIndex = 0;

        public override UnitType Type
        {
            get
            {
                return UnitType.UNIT_BOSS;
            }
        }
        public int OccId
        {
            get
            {
                return _occId;
            }
        }


        public Boss(CreateBossData data)
            : base(data)
        {
            _occId = data.OccId;
        }

        public override void Kill(BaseUnit killer, Ability killerAbility)
        {
            base.Kill(killer, killerAbility);

            if (_isDeadState)
            {
                BattleData.CheckResult(BattleCheckResultType.BossDead);
            }
        }

        protected override void UpdateAbility(DFix64 frameLength)
        {
            if (IsDeadState)
            {
                return;
            }

            if (_lastAutoCastAbilityIndex >= _allAbilities.Count)
            {
                _lastAutoCastAbilityIndex = 0;
            }

            for (int i = 0; i < _allAbilities.Count; i++)
            {
                _allAbilities[i].UpdateLogic(frameLength);

                if (_lastAutoCastAbilityIndex == i)
                {
                    if (_allAbilities[i].IsAutoCast && _allAbilities[i].IsAutoCastState)
                    {
                        if (!_allAbilities[i].IsInCooldown)
                        {
                            if (_allAbilities[i].AutoCast())
                            {

                            }
                            else if (CurrAtt.Mana >= _allAbilities[i].ManaCost)
                            {
                                _lastAutoCastAbilityIndex++;
                            }
                        }
                        else
                        {
                            _lastAutoCastAbilityIndex++;
                        }
                    }
                    else
                    {
                        _lastAutoCastAbilityIndex++;
                    }
                }
            }
        }

        protected override void FindAttackTarget()
        {
            if (_attackTarget != null)
            {
                if (_attackTarget.IsDeadState || _attackTarget.IsUnselectable || _attackTarget.IsUnselectableAttack || _attackTarget.IsInvincible || _attackTarget.IsInvisible)
                {
                    _attackTarget = null;
                }
                else if (_currNode.MaxRange(_attackTarget.CurrNode) > _currAtt.AttackNodeRange)
                {
                    _attackTarget = null;
                }
            }

            if (_froceAttackTarget != null)
            {
                if (_froceAttackTarget.IsDeadState || _froceAttackTarget.IsUnselectable || _froceAttackTarget.IsUnselectableAttack || _froceAttackTarget.IsInvincible || _froceAttackTarget.IsInvisible)
                {
                    _froceAttackTarget = null;
                }
                else
                {
                    _attackTarget = _froceAttackTarget;
                    _froceAttackTarget = null;

                    if (_currNode.MaxRange(_attackTarget.CurrNode) > _currAtt.AttackNodeRange)
                    {
                        _attackTarget = null;
                    }
                }
            }

            if (_attackTarget == null && _orders.Count == 0 && _currOrder == null && _attackOrders != null && !IsResticted && !IsStunned && !IsAttackDisabled)
            {
                for (int i = 0; i < _attackOrders.Count; i++)
                {
                    BattleData.FindTargets(this, this, _attackOrders[i], _cachedAttackTargets);
                    if (_cachedAttackTargets.Count > 0)
                    {
                        foreach (BaseUnit target in _cachedAttackTargets)
                        {
                            if (_currNode.MaxRange(target.CurrNode) <= _currAtt.AttackNodeRange)
                            {
                                _attackTarget = target;
                                break;
                            }
                        }

                        if (_attackTarget != null)
                        {
                            break;
                        }
                    }
                }
            }
        }

        protected override void UpdateAttack(DFix64 frameLength)
        {
            if (IsDeadState)
            {
                return;
            }

            if (_orders.Count != 0 || _currOrder != null)
            {
                return;
            }

            if (_attackTarget != null)
            {
                if (_currNode.MaxRange(_attackTarget.CurrNode) <= _currAtt.AttackNodeRange)
                {
                    AttackTarget(_attackTarget);
                    return;
                }
            }
        }
    }
}