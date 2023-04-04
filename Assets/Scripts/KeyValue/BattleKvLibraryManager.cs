using KVLib;
using KVLib.KeyValues;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public class BattleKvLibraryManager
    {
        private static Dictionary<int, BattleKvLibrary> _battleKeyValues = new Dictionary<int, BattleKvLibrary>();


        private static bool HasKeyValue(int kvLibId, int kvSerialId)
        {
            if (_battleKeyValues.ContainsKey(kvLibId) && _battleKeyValues[kvLibId] != null)
            {
                return _battleKeyValues[kvLibId].HasValue(kvSerialId);
            }

            return false;
        }

        private static KeyValue GetKeyValue(int kvLibId, int kvSerialId)
        {
            if (_battleKeyValues.ContainsKey(kvLibId) && _battleKeyValues[kvLibId] != null)
            {
                return _battleKeyValues[kvLibId].GetValue(kvSerialId);
            }

            return null;
        }

        public static bool ParseKeyValues(int id, string text)
        {
            if (_battleKeyValues.ContainsKey(id))
            {
                if (!_battleKeyValues[id].Add(text))
                {
                    return false;
                }

                return true;
            }

            BattleKvLibrary battleDataLibrary = new BattleKvLibrary();
            if (!battleDataLibrary.Add(text))
            {
                return false;
            }

            _battleKeyValues.Add(id, battleDataLibrary);
            return true;
        }

        public static bool HasUnitKv(int serialId)
        {
            return HasKeyValue(Constant.Battle.UNIT_KV_LIBRARY_ID, serialId);
        }
        public static KeyValue GetUnitKv(int serialId)
        {
            return GetKeyValue(Constant.Battle.UNIT_KV_LIBRARY_ID, serialId);
        }
        public static bool HasSkinKv(int serialId)
        {
            return HasKeyValue(Constant.Battle.SKIN_KV_LIBRARY_ID, serialId);
        }
        public static KeyValue GetSkinKv(int serialId)
        {
            return GetKeyValue(Constant.Battle.SKIN_KV_LIBRARY_ID, serialId);
        }
        public static bool HasModelKv(int serialId)
        {
            return HasKeyValue(Constant.Battle.MODEL_KV_LIBRARY_ID, serialId);
        }
        public static KeyValue GetModelKv(int serialId)
        {
            return GetKeyValue(Constant.Battle.MODEL_KV_LIBRARY_ID, serialId);
        }
        public static bool HasAbilityKv(int serialId)
        {
            return HasKeyValue(Constant.Battle.ABILITY_KV_LIBRARY_ID, serialId);
        }
        public static KeyValue GetAbilityKv(int serialId)
        {
            return GetKeyValue(Constant.Battle.ABILITY_KV_LIBRARY_ID, serialId);
        }
        public static bool HasBuiltinModifierKv(int serialId)
        {
            return HasKeyValue(Constant.Battle.BUILTIN_KV_LIBRARY_ID, serialId);
        }
        public static KeyValue GetBuiltinModifierKv(int serialId)
        {
            return GetKeyValue(Constant.Battle.BUILTIN_KV_LIBRARY_ID, serialId);
        }

        public static void ClearAllBattleKvLibrary()
        {
            _battleKeyValues.Clear();
        }

        public static Modifier CreateBuiltinModifier(int serialId)
        {
            if (!HasBuiltinModifierKv(serialId))
            {
                return null;
            }

            return ModifierManager.CreateModifier(GetBuiltinModifierKv(serialId));
        }

        public static Modifier ApplyBuiltinModifier(BaseUnit caster, BaseUnit target, int serialId, CreateModifierData createData)
        {
            if (!HasBuiltinModifierKv(serialId))
            {
                return null;
            }

            return ModifierManager.ApplyModifier(GetBuiltinModifierKv(serialId), caster, null, target, createData);
        }
    }
}