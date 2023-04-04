using KVLib;
using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    public class MulTargetInfo
    {
        public AbilityUnitTargetTeam UnitTargetTeams = AbilityUnitTargetTeam.UNIT_TARGET_TEAM_NONE;
        public AbilityUnitTargetType UnitTargetTypes = AbilityUnitTargetType.UNIT_TARGET_NONE;
        public AbilityUnitTargetFlag UnitTargetFlags = AbilityUnitTargetFlag.UNIT_TARGET_FLAG_NONE;
        public AbilityUnitTargetFlag ExcludedUnitTargetFlags = AbilityUnitTargetFlag.UNIT_TARGET_FLAG_NONE;
        public AbilityUnitTargetSort UnitTargetSort = AbilityUnitTargetSort.UNIT_TARGET_SORT_NONE;
        public HashSet<int> UnitTargetTraits = null;
        public HashSet<int> ExcludedUnitTargetTraits = null;
        public int[] UnitTargetModifierGroups = null;
        public int[] ExcludedUnitTargetModifierGroups = null;
        public int NodeRange = -1;
        public DFix64 RadiusRange = DFix64.MOne;
        public int MaxNumber = -1;
        public bool IsRandom = false;


        public static MulTargetInfo Parse(KeyValue kv)
        {
            if (kv == null)
            {
                return null;
            }

            MulTargetInfo mulTargetInfo = new MulTargetInfo();

            KeyValue kvTeams = kv["Teams"];
            if (kvTeams != null)
            {
                mulTargetInfo.UnitTargetTeams = BattleData.EvaluateEnums<AbilityUnitTargetTeam>(kvTeams.GetString(), AbilityUnitTargetTeam.UNIT_TARGET_TEAM_NONE);
            }

            KeyValue kvTypes = kv["Types"];
            if (kvTypes != null)
            {
                mulTargetInfo.UnitTargetTypes = BattleData.EvaluateEnums<AbilityUnitTargetType>(kvTypes.GetString(), AbilityUnitTargetType.UNIT_TARGET_NONE);
            }

            KeyValue kvFlags = kv["Flags"];
            if (kvFlags != null)
            {
                mulTargetInfo.UnitTargetFlags = BattleData.EvaluateEnums<AbilityUnitTargetFlag>(kvFlags.GetString(), AbilityUnitTargetFlag.UNIT_TARGET_FLAG_NONE);
            }

            KeyValue kvExcludedFlags = kv["ExcludedFlags"];
            if (kvExcludedFlags != null)
            {
                mulTargetInfo.ExcludedUnitTargetFlags = BattleData.EvaluateEnums<AbilityUnitTargetFlag>(kvExcludedFlags.GetString(), AbilityUnitTargetFlag.UNIT_TARGET_FLAG_NONE);
            }

            KeyValue kvSort = kv["Sort"];
            if (kvSort != null)
            {
                mulTargetInfo.UnitTargetSort = BattleData.EvaluateEnum<AbilityUnitTargetSort>(kvSort.GetString(), AbilityUnitTargetSort.UNIT_TARGET_SORT_NONE);
            }

            KeyValue kvTraits = kv["Traits"];
            if (kvTraits != null)
            {
                mulTargetInfo.UnitTargetTraits = BattleData.EvaluateTrait(kvTraits.GetString());
            }

            KeyValue kvExcludedTraits = kv["ExcludedTraits"];
            if (kvExcludedTraits != null)
            {
                mulTargetInfo.ExcludedUnitTargetTraits = BattleData.EvaluateTrait(kvExcludedTraits.GetString());
            }

            KeyValue kvModifierGroups = kv["ModifierGroups"];
            if (kvModifierGroups != null)
            {
                mulTargetInfo.UnitTargetModifierGroups = BattleData.EvaluateModifierGroup(kvModifierGroups.GetString());
            }

            KeyValue kvExcludedModifierGroups = kv["ExcludedModifierGroups"];
            if (kvExcludedModifierGroups != null)
            {
                mulTargetInfo.ExcludedUnitTargetModifierGroups = BattleData.EvaluateModifierGroup(kvExcludedModifierGroups.GetString());
            }

            KeyValue kvNodeRange = kv["NodeRange"];
            if (kvNodeRange != null)
            {
                mulTargetInfo.NodeRange = BattleData.ParseInt(kvNodeRange.GetString());
            }
            else
            {
                KeyValue kvRadius = kv["Radius"];
                if (kvRadius != null)
                {
                    mulTargetInfo.RadiusRange = BattleData.ParseDFix64(kvRadius.GetString());
                }
            }

            KeyValue kvMaxNumber = kv["MaxNumber"];
            if (kvMaxNumber != null)
            {
                mulTargetInfo.MaxNumber = BattleData.ParseInt(kvMaxNumber.GetString());
            }

            KeyValue kvRandom = kv["Random"];
            if (kvRandom != null)
            {
                mulTargetInfo.IsRandom = BattleData.ParseBool01(kvRandom.GetString());
            }

            return mulTargetInfo;
        }
    }
}