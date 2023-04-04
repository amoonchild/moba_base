using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 恢复法力值
    /*
        "Target"					目标
	    "Mana"						法力值
    */
    public class RecoverManaAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "RecoverMana";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvMana = kv["Mana"];

            if (kvTarget == null)
            {
                Log.Error("RecoverMana: 缺少参数:Target");
                return;
            }

            if (kvMana == null)
            {
                Log.Error("RecoverMana: 缺少参数:Mana");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("RecoverMana: 找不到目标");
                return;
            }

            DFix64 mana = DFix64.Floor(BattleData.EvaluateDFix64(kvMana.GetString(), eventData));
            if (mana == DFix64.Zero)
            {
                Log.Warning("RecoverMana: Mana为0");
                return;
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("RecoverMana: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;

                if (unitTarget.IsDeadState)
                {
                    Log.Warning("RecoverMana: 目标已死亡");
                    first = first.Next;
                    continue;
                }

                DFix64 finalMana = mana;
                DFix64 realMana = mana;
                DFix64 excessMana = DFix64.Zero;

                if (unitTarget.CurrAtt.Mana + mana > unitTarget.CurrAtt.MaxMana)
                {
                    realMana = unitTarget.CurrAtt.MaxMana - mana;
                    excessMana = mana - realMana;
                    unitTarget.CurrAtt.Mana = unitTarget.CurrAtt.MaxMana;
                }
                else if (unitTarget.CurrAtt.Mana + mana < DFix64.Zero)
                {
                    realMana = unitTarget.CurrAtt.Mana;
                    excessMana = -(unitTarget.CurrAtt.Mana + mana);
                    unitTarget.CurrAtt.Mana = DFix64.Zero;
                }
                else
                {
                    realMana = mana;
                    unitTarget.CurrAtt.Mana = unitTarget.CurrAtt.Mana + mana;
                }

                Log.Info("RecoverMana: {0} 恢复 <color=green>{1}{2}</color> 点法力值", unitTarget.LogName,
                    realMana.ToString(), excessMana > DFix64.Zero ? "(*过量" + excessMana.ToString() + ")" : string.Empty);

                EventData newEvent = BattleData.CreateEventData();
                newEvent.Unit = unitTarget;
                newEvent.Point = unitTarget.LogicPosition;
                newEvent.Node = unitTarget.CurrNode;
                newEvent.Parms.Add("finalMana", finalMana);
                newEvent.Parms.Add("realMana", realMana);
                newEvent.Parms.Add("excessMana", excessMana);

                unitTarget.HandleModifierEvent(ModifierEventType.MODIFIER_EVENT_ON_MANA_INCREASE, newEvent);

                first = first.Next;
            }
        }
    }
}