using UnityEngine;


namespace LiaoZhai.Runtime
{
    // 移动命令
    public class MoveToNodeOrder : BaseOrder
    {
        protected BattleNode _startNdoe = null;                             // 起始格子
        protected DFixVector3 _startPosition = DFixVector3.Zero;              // 起始坐标
        protected BattleNode _targetNdoe = null;                            // 目标格子
        protected DFixVector3 _moveDistance = DFixVector3.Zero;               // 移动距离
        protected DFixVector3 _moveVelocity = DFixVector3.Zero;               // 移动方向
        protected DFixVector3 _movedLength = DFixVector3.Zero;                // 已经移动的距离
        protected DFix64 _moveSpeed = DFix64.One;
        protected DFixVector3 _startAngel = DFixVector3.Zero;
        protected DFixVector3 _targetAngel = DFixVector3.Zero;
        protected DFix64 _angelSpeed = DFix64.Zero;
        protected DFix64 _angelTime = DFix64.Zero;
        protected bool _isInTargetNode = false;

        public override UnitOrderType Type
        {
            get
            {
                return UnitOrderType.UNIT_ORDER_MOVE_TO_NODE;
            }
        }


        public MoveToNodeOrder(BaseUnit source, BattleNode targetNdoe)
            : base(source)
        {
            _targetNdoe = targetNdoe;
        }

        public override bool IsExecutable()
        {
            if (_source.IsDeadState)
            {
                return false;
            }

            if (_source.IsResticted)
            {
                return false;
            }

            if (_source.IsStunned)
            {
                return false;
            }

            if (_source.IsMoveDisabled)
            {
                return false;
            }

            if (_source.CurrNode == _targetNdoe)
            {
                if (_source.LogicPosition == _targetNdoe.WorldPosition)
                {
                    return false;
                }

                //_isInTargetNode = true;
            }
            else
            {
                if (!_targetNdoe.IsWalkable)
                {
                    return false;
                }
            }

            if (_source.CurrAtt.MoveSpeed <= DFix64.Zero)
            {
                return false;
            }

            return true;
        }

        public override bool Begin()
        {
            if (!IsExecutable())
            {
                _isFinished = true;
                return false;
            }

            _startNdoe = _source.CurrNode;
            _startPosition = _source.LogicPosition;
            _moveDistance = _targetNdoe.WorldPosition - _startPosition;
            _moveVelocity = _moveDistance.GetNormalized();

            _source.IsMoving = true;
            _targetNdoe.IsWalkable = false;
            _source.SetNode(_targetNdoe);
         
            if (_source.CurrAtt.RotateSpeed == DFix64.Zero)
            {
                _source.LookToPositionY(_targetNdoe.WorldPosition);
            }
            else
            {
            
            }

            //_source.LookToPositionY(_targetNdoe.WorldPosition);
            //_source.SyncRenderRotation();

            if (_source.InitAtt.MoveSpeed > DFix64.Zero)
            {
                _moveSpeed = _source.CurrAtt.MoveSpeed / _source.InitAtt.MoveSpeed;
            }
            else
            {
                _moveSpeed = _source.CurrAtt.MoveSpeed / Constant.Battle.BASE_MOVE_SPEED;
            }

            _source.PlayAnimation(UnitAnimationType.ACT_RUN, _moveSpeed, true);

            return true;
        }

        public override void UpdateLogic(DFix64 frameLength)
        {
            if (_source.CurrAtt.MoveSpeed > DFix64.Zero)
            {
                if (!_source.IsResticted && !_source.IsMoveDisabled && !_source.IsStunned)
                {
                    _movedLength += _source.CurrAtt.MoveSpeed / DFix64.Hundred * frameLength * _moveVelocity;

                    if(_angelSpeed > DFix64.Zero)
                    {
                        _angelTime += frameLength * _angelSpeed;

                        if (_angelTime >= DFix64.One)
                        {
                            _angelTime = DFix64.One;
                        }

                        _source.LogicEulerAngles = DFixVector3.Lerp(_startAngel, _targetAngel, _angelTime);
                    }

                    _source.PlayAnimation(UnitAnimationType.ACT_RUN, _moveSpeed, true);
                }
                else
                {
                    _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
                    return;
                }
            }

            if (DFixVector3.SqrMagnitude(_movedLength) >= DFixVector3.SqrMagnitude(_moveDistance))
            {
                _isFinished = true;

                _source.LogicPosition = _source.CurrNode.WorldPosition;
                _source.IsMoving = false;
                _source.LookToPositionY(_targetNdoe.WorldPosition);

                //if (!_isInTargetNode)
                //{
                //    _isInTargetNode = true;

                //    _source.SetNode(_targetNdoe);
                //}
            }
            else
            {
                _source.LogicPosition = _startPosition + _movedLength;

                //if (!_isInTargetNode)
                //{
                //    if (_source.CurrNode == _startNdoe)
                //    {
                //        if (DFixVector3.Distance(_source.LogicPosition, _startNdoe.WorldPosition) >= DFixVector3.Distance(_source.LogicPosition, _targetNdoe.WorldPosition))
                //        {
                //            _isInTargetNode = true;

                //            _source.SetNode(_targetNdoe);
                //        }
                //    }
                //}
            }
        }

        public override void Interrupt()
        {
            _isFinished = true;

            if (_source.IsMoving)
            {
                _source.IsMoving = false;
                _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
            }

            //if (!_isInTargetNode)
            //{
            //    _targetNdoe.IsWalkable = true;
            //}
        }

        public override void Quit()
        {
            _isFinished = true;

            if (_source.IsMoving)
            {
                _source.IsMoving = false;
                _source.PlayAnimation(UnitAnimationType.ACT_IDLE, true);
            }

            //if (!_isInTargetNode)
            //{
            //    _targetNdoe.IsWalkable = true;
            //}
        }

        private void UpdateAngel()
        {
            Vector3 curr = _source.LogicPosition.ToVector3();
            Vector3 lookTo = _targetNdoe.WorldPosition.ToVector3();
            Vector3 lookDir = lookTo - curr;
            lookDir = lookDir - Vector3.Project(lookDir, Vector3.up);
            //lookDir.Normalize();

            float angle = Vector3.Angle(Vector3.forward, lookDir);
            float dir = (Vector3.Dot(Vector3.up, Vector3.Cross(Vector3.forward, lookDir)) < 0 ? -1 : 1);
            angle *= dir;

            DFix64 newAngle = (DFix64)angle;
            if (newAngle < DFix64.Zero && _targetAngel.y > DFix64.Zero)
            {
                if (_targetAngel.y - DFix64.Angel180 > newAngle)
                {
                    _targetAngel.y -= DFix64.Angel360;
                }
            }
            else if (newAngle > DFix64.Zero && _targetAngel.y < DFix64.Zero)
            {
                if (_targetAngel.y + DFix64.Angel180 < newAngle)
                {
                    _targetAngel.y += DFix64.Angel360;
                }
            }

            _targetAngel.y = newAngle;
        }

    }
}