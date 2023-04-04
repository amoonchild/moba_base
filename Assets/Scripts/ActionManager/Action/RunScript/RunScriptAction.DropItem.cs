// -----------------------------------------------
// Copyright © Jeffrey. All rights reserved.
// CreateTime: 2021/7/19   21:1:3
// -----------------------------------------------

using GameDevWare.Dynamic.Expressions;
using GameDevWare.Dynamic.Expressions.CSharp;
using DG.Tweening;
using KVLib;
using System.Collections.Generic;
using UnityEngine;


namespace LiaoZhai.Runtime
{
    public partial class RunScriptAction
    {
        private void DropItem(KeyValue kv, EventData eventData)
        {
#if UNITY_EDITOR
            if (BattleData.TestQuickBattle)
            {
                return;
            }
#endif

            if (eventData.Modifier == null)
            {
                return;
            }

            KeyValue kvEffect = kv["Effect"];
            if (kvEffect == null)
            {
                return;
            }

            KeyValue kvTarget = kv["Target"];
            if (kvTarget == null)
            {
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                return;
            }

            KeyValue kvPower = kv["Power"];
            KeyValue kvDuration = kv["Duration"];

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    first = first.Next;
                    continue;
                }

                BaseUnit unitTarget = (BaseUnit)first.Value.Target;

                BattleNode node = BattleData.FindNoItemNeighbourNearest(unitTarget.CurrNode, 7);
                if (node == null && unitTarget.CurrNode.DropItem == null)
                {
                    node = unitTarget.CurrNode;
                }

                if (node != null)
                {
                    DropItemParticle particle = ParticleManager.CreateDropItemParticle(kvEffect.GetString());
                    if (particle == null)
                    {
                        first = first.Next;
                        continue;
                    }

                    particle.SetParticleControl(ParticleControlType.Position, unitTarget.LogicPosition, true);
                    float duration = (float)BattleData.EvaluateDFix64(kvDuration.GetString(), eventData);

                    node.DropItem = particle;
                    particle.DoJump(unitTarget.CurrNode.WorldPosition, node.WorldPosition, (float)BattleData.EvaluateDFix64(kvPower.GetString(), eventData), duration);

                    Log.Info("{0} 掉落物品 {1}, from:{2},{3} to:{4},{5}", unitTarget.LogName, eventData.Modifier.LogName, unitTarget.CurrNode.X.ToString(),
                        unitTarget.CurrNode.Y.ToString(), node.X.ToString(), node.Y.ToString());
                }

                first = first.Next;
            }
        }
    }
}