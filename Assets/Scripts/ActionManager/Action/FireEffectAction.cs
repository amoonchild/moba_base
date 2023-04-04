using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 播放特效
    /*
	    "Target"						目标
        "EffectName"					特效资源路径
	    "EffectAttachPoint"				特效挂点
        "IsAttachRot"				    同步挂点旋转
        "IsAttachTargetScale"		    同步目标缩放
    */
    public class FireEffectAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "FireEffect";
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
            KeyValue kvEffectName = kv["EffectName"];

            if (kvTarget == null)
            {
                Log.Error("FireEffect: 缺少参数:Target");
                return;
            }

            if (kvEffectName == null)
            {
                Log.Error("FireEffect: 缺少参数:EffectName");
                return;
            }

            if (string.IsNullOrEmpty(kvEffectName.GetString()))
            {
                Log.Error("FireEffect: EffectName为空");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("FireEffect: 找不到目标");
                return;
            }

            KeyValue kvEffectAttachPoint = kv["EffectAttachPoint"];
            KeyValue kvIsAttachPos = kv["IsAttachPos"];
            KeyValue kvIsAttachRot = kv["IsAttachRot"];
            KeyValue kvIsAttachTargetScale = kv["IsAttachTargetScale"];
            KeyValue kvIsKeepAttach = kv["IsKeepAttach"];
            KeyValue kvIsLoop = kv["IsLoop"];

            bool isAttachPos = kvIsAttachPos != null ? BattleData.ParseBool01(kvIsAttachPos.GetString()) : true;
            bool isAttachRot = kvIsAttachRot != null ? BattleData.ParseBool01(kvIsAttachRot.GetString()) : false;
            bool isAttachScale = kvIsAttachTargetScale != null ? BattleData.ParseBool01(kvIsAttachTargetScale.GetString()) : false;
            bool isKeepAttach = kvIsKeepAttach != null ? BattleData.ParseBool01(kvIsKeepAttach.GetString()) : isAttachPos;
            bool isLoop = kvIsLoop != null ? BattleData.ParseBool01(kvIsLoop.GetString()) : false;

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                int particleId = ParticleManager.CreateParticle(kvEffectName.GetString(), isLoop);
                if (particleId == 0)
                {
                    first = first.Next;
                    continue;
                }

                if (first.Value.Type == BattleTargetType.UNIT)
                {
                    BaseUnit attachUnitTarget = (BaseUnit)first.Value.Target;
                    if (kvEffectAttachPoint != null && !string.IsNullOrEmpty(kvEffectAttachPoint.GetString()))
                    {
                        ParticleManager.SetParticleAttach(particleId, attachUnitTarget, kvEffectAttachPoint.GetString(), isAttachPos, isAttachRot, isAttachScale, isKeepAttach);
                    }
                    else
                    {
                        ParticleManager.SetParticleAttach(particleId, attachUnitTarget, string.Empty, isAttachPos, isAttachRot, isAttachScale, isKeepAttach);
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