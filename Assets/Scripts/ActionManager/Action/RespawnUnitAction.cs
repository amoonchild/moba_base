using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 复活单位
    /*
        "Target"					目标
        "Hp"                        复活后生命值
    */
    public class RespawnUnitAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "RespawnUnit";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvHp = kv["Hp"];

            if (kvTarget == null)
            {
                Log.Error("RespawnUnit: 缺少参数:Target");
                return;
            }

            if (kvHp == null)
            {
                Log.Error("RespawnUnit: 缺少参数:Hp");
                return;
            }

            DFix64 hp = DFix64.Floor(BattleData.EvaluateDFix64(kvHp.GetString(), eventData));
            if (hp <= DFix64.Zero)
            {
                Log.Error("RespawnUnit: Hp小于等于0");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("RespawnUnit: 找不到目标");
                return;
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("RespawnUnit: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;
                if (!unitTarget.IsDeadState)
                {
                    Log.Warning("RemoveModifier: 目标未死亡");
                    first = first.Next;
                    continue;
                }

                BattleNode node = unitTarget.DeadNode;
                if (!node.IsWalkable || node.Unit != unitTarget)
                {
                    node = BattleData.FindWalkableNeighbourNearest(node);
                    if (node == null)
                    {
                        Log.Warning("RespawnUnit: 找不到可用的格子");
                        first = first.Next;
                        continue;
                    }
                }

                DFix64 currHp = DFix64.Min(hp, unitTarget.InitAtt.MaxHp);

                unitTarget.Respawn(currHp, node, eventData.Caster, eventData.Ability);

                if (!unitTarget.IsDeadState)
                {
                    BattleUnitSpawnEventArgs ne = GameFramework.ReferencePool.Acquire<BattleUnitSpawnEventArgs>();
                    ne.Unit = unitTarget;
                    ne.IsRespawn = true;
                    GameManager.Event.Fire(null, ne);

                    EventData newEvent = BattleData.CreateEventData();
                    newEvent.Attacker = eventData.Caster;
                    newEvent.Unit = unitTarget;
                    newEvent.Point = unitTarget.LogicPosition;
                    newEvent.Node = unitTarget.CurrNode;

                    unitTarget.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_RESPAWN, newEvent);
                }

                first = first.Next;
            }
        }
    }
}