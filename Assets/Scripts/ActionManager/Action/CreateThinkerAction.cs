using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 创建定时器
    /*
        "Target"						目标
	    "ModifierSerialId"				Modifier序列号
    */
    public class CreateThinkerAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "CreateThinker";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvModifierSerialId = kv["ModifierSerialId"];
            KeyValue kvDuration = kv["Duration"];

            if (kvTarget == null)
            {
                Log.Error("CreateThinker: 缺少参数:Target");
                return;
            }

            if (kvModifierSerialId == null)
            {
                Log.Error("CreateThinker: 缺少参数:ModifierSerialId");
                return;
            }

            int modifierSerialId = BattleData.ParseInt(kvModifierSerialId.GetString());
            if (modifierSerialId == 0)
            {
                Log.Error("CreateThinker :ModifierSerialId为0");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("CreateThinker: 找不到目标");
                return;
            }

            CreateModifierData createModifierData = new CreateModifierData();
            if (kvDuration != null)
            {
                createModifierData.Duration = BattleData.EvaluateDFix64(kvDuration.GetString(), eventData);
                if (createModifierData.Duration == DFix64.Zero)
                {
                    Log.Error("CreateThinker: Duration为0");
                    return;
                }
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                BattleNode node = null;
                DFixVector3 point = DFixVector3.Zero;
                if (first.Value.Type == BattleTargetType.UNIT)
                {
                    BaseUnit unitTarget = (BaseUnit)first.Value.Target;
                    node = unitTarget.CurrNode;
                    point = unitTarget.LogicPosition;
                }
                else if (first.Value.Type == BattleTargetType.POINT)
                {
                    point = (DFixVector3)first.Value.Target;
                    node = BattleData.FindNodeByPoint(point);
                }
                else if (first.Value.Type == BattleTargetType.NODE)
                {
                    point = ((BattleNode)first.Value.Target).WorldPosition;
                    node = (BattleNode)first.Value.Target;
                }

                CreateThinkerData createThinkerData = new CreateThinkerData();
                createThinkerData.player_id = eventData.Caster.PlayerId;
                createThinkerData.player_camp = eventData.Caster.TeamId;
                createThinkerData.Caster = eventData.Caster;
                createThinkerData.Node = node;
                createThinkerData.Point = point;
                createThinkerData.EulerAngles = createThinkerData.Caster.LogicEulerAngles;

                Thinker thinker = (Thinker)BattleData.CreateThinker(createThinkerData);
                if (thinker == null)
                {
                    Log.Error("CreateThinker :创建定时器失败");
                    first = first.Next;
                    continue;
                }

                Log.Info("CreateThinker: 来源:{0} 位置:{1}", eventData.Caster.LogName, point.ToString());

                UnitManager.Units.Add(thinker);
                thinker.Spawn(false);

                thinker.ApplyModifier(eventData.Caster, eventData.Ability, modifierSerialId, null, true);
                if (createModifierData.IsDurationSpecified && createModifierData.Duration > DFix64.Zero)
                {
                    thinker.ApplyModifier(eventData.Caster, null, Constant.Battle.BUILTIN_MODIFIER_THINKER_Kill, createModifierData, false);
                }

                first = first.Next;
            }
        }
    }
}