using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 追踪弹道
    /*
        "Target" 					目标
	    "MoveSpeed" 				弹道速度
	    "EffectName" 				特效资源路径
	    "EffectStartPoint"			特效起始挂点
	
	    "OnProjectileHitUnit"		击中目标
	    {
		
	    }
    */
    public class TrackingProjectileAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "TrackingProjectile";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvMoveSpeed = kv["MoveSpeed"];
            KeyValue kvEffectName = kv["EffectName"];
            KeyValue kvEffectStartPoint = kv["EffectStartPoint"];
            KeyValue kvOnProjectileHitUnit = kv["OnProjectileHitUnit"];

            if (kvTarget == null)
            {
                Log.Error("TrackingProjectile: 缺少参数:Target");
                return;
            }

            if (kvMoveSpeed == null)
            {
                Log.Error("TrackingProjectile: 缺少参数:MoveSpeed");
                return;
            }

            DFix64 moveSpeed = DFix64.Floor(BattleData.EvaluateDFix64(kvMoveSpeed.GetString(), eventData));
            if (moveSpeed <= DFix64.Zero)
            {
                Log.Error("TrackingProjectile: MoveSpeed小于等于0");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("TrackingProjectile: 找不到目标");
                return;
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("TrackingProjectile: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                CreateAbilityTrackingProjectileData createData = new CreateAbilityTrackingProjectileData();
                createData.Attacker = eventData.Caster;
                createData.Ability = eventData.Ability;
                createData.Target = (BaseUnit)first.Value.Target;
                createData.MoveSpeed = moveSpeed;
                createData.ProjectileHitKv = kvOnProjectileHitUnit;

                if (eventData.Modifier != null)
                {
                    createData.From = eventData.Modifier.Target;
                }
                else
                {
                    createData.From = eventData.Caster;
                }

                if (kvEffectName != null)
                {
                    createData.EffectName = kvEffectName.GetString();
                }

                if (kvEffectStartPoint != null)
                {
                    createData.SpawnOriginAttachPoint = kvEffectStartPoint.GetString();
                }

                ProjectileManager.CreateAbilityTrackProjectile(createData);

                first = first.Next;
            }
        }
    }
}