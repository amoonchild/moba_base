using GameFramework;
using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public class EventData : IReference
    {
        private bool _isOld = true;
        private BaseUnit _caster = null;
        private Ability _ability = null;
        private Modifier _modifier = null;
        private BaseUnit _target = null;
        private BaseUnit _attacker = null;
        private BaseUnit _unit = null;
        private DFixVector3 _targetPoint = DFixVector3.Zero;
        private BattleNode _targetNode = null;
        private Dictionary<string, DFix64> _parms = null;

        public bool IsCanRecycle
        {
            get
            {
                if (!_isOld)
                {
                    return false;
                }

                return true;
            }
        }
        public bool IsOld
        {
            get { return _isOld; }
            set { _isOld = value; }
        }
        public BaseUnit Caster
        {
            get
            {
                if (_modifier != null && _modifier.Target.Type == UnitType.UNIT_THINKER)
                {
                    return _modifier.Target.Master;
                }

                return _caster;
            }
            set
            {
                _caster = value;
            }
        }
        public Ability Ability
        {
            get
            {
                return _ability;
            }
            set
            {
                _ability = value;
            }
        }
        public Modifier Modifier
        {
            get
            {
                return _modifier;
            }
            set
            {
                _modifier = value;
            }
        }
        public BaseUnit Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }
        public BaseUnit Attacker
        {
            get
            {
                return _attacker;
            }
            set
            {
                _attacker = value;
            }
        }
        public BaseUnit Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
            }
        }
        public BaseUnit Thinker
        {
            get
            {
                if (_modifier != null && _modifier.Target.Type == UnitType.UNIT_THINKER)
                {
                    return _modifier.Target;
                }

                return null;
            }
        }
        public DFixVector3 Point
        {
            get
            {
                return _targetPoint;
            }
            set
            {
                _targetPoint = value;
            }
        }
        public BattleNode Node
        {
            get
            {
                return _targetNode;
            }
            set
            {
                _targetNode = value;
            }
        }
        public Dictionary<string, DFix64> Parms
        {
            get
            {
                if (_parms == null)
                {
                    _parms = new Dictionary<string, DFix64>();
                }

                return _parms;
            }
        }


        public EventData()
        {

        }

        public EventData Clone()
        {
            EventData eventData = BattleData.CreateEventData();

            eventData.Caster = _caster;
            eventData.Ability = _ability;
            eventData.Modifier = _modifier;
            eventData.Target = _target;
            eventData.Attacker = _attacker;
            eventData.Unit = _unit;
            eventData.Point = _targetPoint;
            eventData.Node = _targetNode;

            if (_parms != null)
            {
                foreach (var parm in _parms)
                {
                    eventData.Parms.Add(parm.Key, parm.Value);
                }
            }

            return eventData;
        }

        public void Clear()
        {
            _isOld = true;
            _caster = null;
            _ability = null;
            _modifier = null;
            _target = null;
            _attacker = null;
            _unit = null;
            _targetPoint = DFixVector3.Zero;
            _targetNode = null;

            if (_parms != null)
            {
                _parms.Clear();
            }
            _parms = null;
        }
    }
}