using KVLib;
using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    public class ModifierManager
    {
        private static Dictionary<KeyValue, Queue<Modifier>> _freeModifiers = new Dictionary<KeyValue, Queue<Modifier>>();
        private static List<Modifier> _waitRecycleModifiers = new List<Modifier>();
        private static List<Modifier> _modifiers = new List<Modifier>();
        private static bool _isEnableRecycle = true;

        public static bool IsEnableRecycle
        {
            get { return _isEnableRecycle; }
            set { _isEnableRecycle = value; }
        }


        public static Modifier CreateModifier(KeyValue modifierKv)
        {
            Modifier modifier = null;

            if (IsEnableRecycle)
            {
                if (_freeModifiers.ContainsKey(modifierKv))
                {
                    Queue<Modifier> modifiers = _freeModifiers[modifierKv];
                    if (modifiers.Count > 0)
                    {
                        modifier = modifiers.Dequeue();
                        modifier.Reset();
                        _modifiers.Add(modifier);
                    }
                }
            }

            if (modifier == null)
            {
                modifier = new Modifier(modifierKv);

                if (IsEnableRecycle)
                {
                    _modifiers.Add(modifier);
                }
            }

            return modifier;
        }

        public static Modifier ApplyModifier(KeyValue modifierKv, BaseUnit caster, Ability ability, BaseUnit target, CreateModifierData createModifierData)
        {
            Modifier modifier = CreateModifier(modifierKv);
            modifier.SetData(caster, ability, target, createModifierData);

            return modifier;
        }

        public static void RecycleModifiers()
        {
            if (_waitRecycleModifiers.Count > 0)
            {
                for (int i = 0; i < _waitRecycleModifiers.Count; i++)
                {
                    Modifier modifier = _waitRecycleModifiers[i];

                    modifier.Recycle();

                    if (_freeModifiers.ContainsKey(modifier.Kv))
                    {
                        _freeModifiers[modifier.Kv].Enqueue(modifier);
                    }
                    else
                    {
                        Queue<Modifier> modifiers = new Queue<Modifier>();
                        modifiers.Enqueue(modifier);

                        _freeModifiers.Add(modifier.Kv, modifiers);
                    }
                }

                _waitRecycleModifiers.Clear();
            }

            for (int i = _modifiers.Count - 1; i >= 0; i--)
            {
                Modifier modifier = _modifiers[i];
                if (modifier.IsRemoved && modifier.DelayCount <= 0 && !modifier.IsAura)
                {
                    _waitRecycleModifiers.Add(modifier);
                    _modifiers.RemoveAt(i);
                }
            }
        }

        public static void Release()
        {
            for (int i = 0; i < _waitRecycleModifiers.Count; i++)
            {
                Modifier modifier = _waitRecycleModifiers[i];
                //if (modifier.IsRemoved && !modifier.IsDelay)
                {
                    modifier.Remove();
                    modifier.Recycle();

                    if (_freeModifiers.ContainsKey(modifier.Kv))
                    {
                        _freeModifiers[modifier.Kv].Enqueue(modifier);
                    }
                    else
                    {
                        Queue<Modifier> modifiers = new Queue<Modifier>();
                        modifiers.Enqueue(modifier);

                        _freeModifiers.Add(modifier.Kv, modifiers);
                    }
                }
            }

            _waitRecycleModifiers.Clear();

            for (int i = _modifiers.Count - 1; i >= 0; i--)
            {
                Modifier modifier = _modifiers[i];
                //if (modifier.IsRemoved && !modifier.IsDelay)
                {
                    modifier.Remove();
                    modifier.Recycle();

                    if (_freeModifiers.ContainsKey(modifier.Kv))
                    {
                        _freeModifiers[modifier.Kv].Enqueue(modifier);
                    }
                    else
                    {
                        Queue<Modifier> modifiers = new Queue<Modifier>();
                        modifiers.Enqueue(modifier);

                        _freeModifiers.Add(modifier.Kv, modifiers);
                    }

                    _modifiers.RemoveAt(i);
                }
            }
        }

        public static void RemoveAll()
        {
            _freeModifiers.Clear();
            _waitRecycleModifiers.Clear();
            _modifiers.Clear();
        }
    }
}