using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 播放跟随特效
    /*
	    "Target"						目标
        "EffectName"					特效资源路径
	    "EffectAttachPoint"				特效挂点
	    "IsAttachPos"				    跟随挂点移动
        "IsAttachRot"				    跟随挂点旋转
        "IsAttachTargetScale"		    跟随目标缩放
    */
    public class AttachEffectAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "AttachEffect";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

#if UNITY_EDITOR
            if (BattleData.TestQuickBattle || BattleData.TestBattleNoParticle)
            {
                return;
            }
#endif

            KeyValue kvTarget = kv["Target"];
            if (kvTarget == null)
            {
                Log.Error("AttachEffect: 缺少参数:Target");
                return;
            }

            KeyValue kvEffectName = kv["EffectName"];
            if (kvEffectName == null)
            {
                Log.Error("AttachEffect: 缺少参数:EffectName");
                return;
            }

            if (string.IsNullOrEmpty(kvEffectName.GetString()))
            {
                Log.Error("AttachEffect: EffectName为空");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("ActOnTargets: 找不到目标");
                return;
            }

            KeyValue kvEffectAttachPoint = kv["EffectAttachPoint"];
            KeyValue kvIsAttachPos = kv["IsAttachPos"];
            KeyValue kvIsAttachRot = kv["IsAttachRot"];
            KeyValue kvIsAttachTargetScale = kv["IsAttachTargetScale"];
            KeyValue kvIsKeepAttach = kv["IsKeepAttach"];

            bool isAttachPos = kvIsAttachPos != null ? BattleData.ParseBool01(kvIsAttachPos.GetString()) : true;
            bool isAttachRot = kvIsAttachRot != null ? BattleData.ParseBool01(kvIsAttachRot.GetString()) : false;
            bool isAttachScale = kvIsAttachTargetScale != null ? BattleData.ParseBool01(kvIsAttachTargetScale.GetString()) : false;
            bool isKeepAttach = kvIsKeepAttach != null ? BattleData.ParseBool01(kvIsKeepAttach.GetString()) : true;

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                int particleId = ParticleManager.CreateParticle(kvEffectName.GetString(), true);
                if (particleId == 0)
                {
                    first = first.Next;
                    continue;
                }

                if (eventData.Modifier != null)
                {
                    eventData.Modifier.AttachEffectNew(kvEffectName.GetString(), particleId);
                }

                if (first.Value.Type == BattleTargetType.UNIT)
                {
                    BaseUnit unitTarget = (BaseUnit)first.Value.Target;

                    if (kvEffectAttachPoint != null && !string.IsNullOrEmpty(kvEffectAttachPoint.GetString()))
                    {
                        ParticleManager.SetParticleAttach(particleId, unitTarget, kvEffectAttachPoint.GetString(), isAttachPos, isAttachRot, isAttachScale, isKeepAttach);
                    }
                    else
                    {
                        ParticleManager.SetParticleAttach(particleId, unitTarget, string.Empty, isAttachPos, isAttachRot, isAttachScale, isKeepAttach);
                    }
                }
                else if (first.Value.Type == BattleTargetType.POINT)
                {
                    DFixVector3 point = (DFixVector3)first.Value.Target;
                    ParticleManager.SetParticleControl(particleId, ParticleControlType.Position, point, true);
                }
                else if (first.Value.Type == BattleTargetType.NODE)
                {
                    DFixVector3 point = ((BattleNode)first.Value.Target).WorldPosition;
                    ParticleManager.SetParticleControl(particleId, ParticleControlType.Position, point, true);
                }

                first = first.Next;
            }
        }
    }
}