using KVLib;
using System;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 线性弹道
    /*
        "Target"						目标
	    "MoveSpeed"						弹道速度
	    "Distance"						最远距离
	    "StartRadius"					起始半径
	    "EndRadius"						结束半径
	    "HasForntalCone"				是否为锥形,忽略前后半圆
	    "EffectName"					特效资源路径
		
	    "TargetTeams"					目标队伍
	    "TargetTypes"					目标类型
	    "TargetFlags"					目标筛选
	    "TargetExcludedFlags"			目标排除筛选
	    "TargetTraits"					满足特质
	    "TargetExcludedTraits"			满足特质
	    "TargetModifierGroups"			满足Modifier组
	    "TargetExcludedModifierGroups"	排除Modifier组
	
	    "OnProjectileHitUnit"			击中目标
	    {
		    "DeleteOnHit"				击中第一个目标后删除
	    }
	
	    "OnProjectileFinish"			投射物结束
	    {
		
	    }
    */
    public class LinearProjectileAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "LinearProjectile";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            if (kvTarget == null)
            {
                Log.Error("缺少参数:Target");
                return;
            }

            KeyValue kvMoveSpeed = kv["MoveSpeed"];
            if (kvMoveSpeed == null)
            {
                Log.Error("缺少参数:MoveSpeed");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                return;
            }

            KeyValue kvDistance = kv["Distance"];
            if (kvDistance == null)
            {
                Log.Error("缺少参数:Distance");
                return;
            }

            KeyValue kvEffectName = kv["EffectName"];
            KeyValue kvStartRadius = kv["StartRadius"];
            KeyValue kvEndRadius = kv["EndRadius"];
            KeyValue kvHasForntalCone = kv["HasForntalCone"];
            KeyValue kvTargetTeams = kv["TargetTeams"];
            KeyValue kvTargetTypes = kv["TargetTypes"];
            KeyValue kvTargetFlags = kv["TargetFlags"];
            KeyValue kvTargetExcludedFlags = kv["TargetExcludedFlags"];
            KeyValue kvTargetTraits = kv["TargetTraits"];
            KeyValue kvTargetExcludedTraits = kv["TargetExcludedTraits"];
            KeyValue kvTargetModifierGroups = kv["TargetModifierGroups"];
            KeyValue kvTargetExcludedModifierGroups = kv["TargetExcludedModifierGroups"];

            CreateAbilityLinearProjectileData createData = new CreateAbilityLinearProjectileData();
            createData.Attacker = eventData.Caster;
            createData.Ability = eventData.Ability;
            createData.MoveSpeed = DFix64.Floor(BattleData.EvaluateDFix64(kvMoveSpeed.GetString(), eventData)) / DFix64.Hundred;
            createData.ProjectileHitKv = kv["OnProjectileHitUnit"];
            createData.ProjectileFinishKv = kv["OnProjectileFinish"];

            if (createData.ProjectileHitKv != null)
            {
                KeyValue kvDeleteOnHit = createData.ProjectileHitKv["DeleteOnHit"];
                if (kvDeleteOnHit != null)
                {
                    createData.IsDeleteOnHit = BattleData.ParseBool01(kvDeleteOnHit.GetString());
                }
            }

            if (kvEffectName != null)
            {
                createData.EffectName = kvEffectName.GetString();
            }

            createData.Distance = DFix64.Floor(BattleData.EvaluateDFix64(kvDistance.GetString(), eventData)) / DFix64.Hundred;

            if (kvStartRadius != null)
            {
                createData.StartRadius = DFix64.Floor(BattleData.EvaluateDFix64(kvStartRadius.GetString(), eventData)) / DFix64.Hundred;
            }
            if (kvEndRadius != null)
            {
                createData.EndRadius = DFix64.Floor(BattleData.EvaluateDFix64(kvEndRadius.GetString(), eventData)) / DFix64.Hundred;
            }
            if (kvHasForntalCone != null)
            {
                createData.IsFrontalCone = BattleData.ParseBool01(kvHasForntalCone.GetString());
            }
            if (kvTargetTeams != null)
            {
                createData.UnitTargetTeams = BattleData.EvaluateEnums(kvTargetTeams.GetString(), AbilityUnitTargetTeam.UNIT_TARGET_TEAM_NONE);
            }
            if (kvTargetTypes != null)
            {
                createData.UnitTargetTypes = BattleData.EvaluateEnums(kvTargetTypes.GetString(), AbilityUnitTargetType.UNIT_TARGET_NONE);
            }
            if (kvTargetFlags != null)
            {
                createData.UnitTargetFlags = BattleData.EvaluateEnums(kvTargetFlags.GetString(), AbilityUnitTargetFlag.UNIT_TARGET_FLAG_NONE);
            }
            if (kvTargetExcludedFlags != null)
            {
                createData.UnitTargetExclutedFlags = BattleData.EvaluateEnums(kvTargetExcludedFlags.GetString(), AbilityUnitTargetFlag.UNIT_TARGET_FLAG_NONE);
            }
            if (kvTargetTraits != null)
            {
                createData.UnitTargetTraits = BattleData.EvaluateTrait(kvTargetTraits.GetString());
            }
            if (kvTargetExcludedTraits != null)
            {
                createData.UnitTargetExclutedTraits = BattleData.EvaluateTrait(kvTargetExcludedTraits.GetString());
            }
            if (kvTargetModifierGroups != null)
            {
                createData.UnitTargetModifierGroups = BattleData.EvaluateModifierGroup(kvTargetModifierGroups.GetString());
            }
            if (kvTargetExcludedModifierGroups != null)
            {
                createData.UnitTargetExclutedModifierGroups = BattleData.EvaluateModifierGroup(kvTargetExcludedModifierGroups.GetString());
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type == BattleTargetType.UNIT)
                {
                    createData.Velocity = ((BaseUnit)first.Value.Target).LogicPosition - eventData.Caster.LogicPosition;
                }
                else
                {
                    createData.Velocity = (DFixVector3)first.Value.Target - eventData.Caster.LogicPosition;
                }

                if (createData.Velocity == DFixVector3.Zero)
                {
                    first = first.Next;
                    continue;
                }

                createData.Velocity = createData.Velocity.GetNormalized();

                ProjectileManager.CreateAbilityLinerProjectile(createData);

                first = first.Next;
            }
        }
    }
}