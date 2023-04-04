using KVLib;
using System;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public abstract class BaseAction
    {
        public abstract string Name
        {
            get;
        }


        protected virtual LinkedList<BattleTarget> FindTargets(KeyValue kvTarget, EventData eventData)
        {
            if (kvTarget == null || eventData == null)
            {
                return null;
            }

            LinkedList<BattleTarget> targets = new LinkedList<BattleTarget>();
            if (!kvTarget.HasChildren)
            {
                switch (kvTarget.GetString())
                {
                    case "CASTER":
                        {
                            if (eventData.Caster != null && eventData.Caster.IsSpawnedToLineup)
                            {
                                targets.AddLast(new BattleUnitTarget(eventData.Caster));
                            }
                        }
                        break;
                    case "TARGET":
                        {
                            if (eventData.Target != null && eventData.Target.IsSpawnedToLineup)
                            {
                                targets.AddLast(new BattleUnitTarget(eventData.Target));
                            }
                        }
                        break;
                    case "ATTACKER":
                        {
                            if (eventData.Attacker != null && eventData.Attacker.IsSpawnedToLineup)
                            {
                                targets.AddLast(new BattleUnitTarget(eventData.Attacker));
                            }
                        }
                        break;
                    case "UNIT":
                        {
                            if (eventData.Unit != null && eventData.Unit.IsSpawnedToLineup)
                            {
                                targets.AddLast(new BattleUnitTarget(eventData.Unit));
                            }
                        }
                        break;
                    case "THINKER":
                        {
                            if (eventData.Thinker != null && eventData.Thinker.IsSpawnedToLineup)
                            {
                                targets.AddLast(new BattleUnitTarget(eventData.Thinker));
                            }
                        }
                        break;
                    case "POINT":
                        {
                            targets.AddLast(new BattlePointTarget(eventData.Point));
                        }
                        break;
                    case "NODE":
                        {
                            if (eventData.Node != null)
                            {
                                targets.AddLast(new BattleNodeTarget(eventData.Node));
                            }
                        }
                        break;
                }
            }
            else
            {
                KeyValue kvCenter = kvTarget["Center"];
                if (kvCenter != null)
                {
                    BattleTarget center = null;
                    switch (kvCenter.GetString())
                    {
                        case "CASTER":
                            {
                                if (eventData.Caster != null)
                                {
                                    center = new BattleUnitTarget(eventData.Caster);
                                }
                            }
                            break;
                        case "TARGET":
                            {
                                if (eventData.Target != null)
                                {
                                    center = new BattleUnitTarget(eventData.Target);
                                }
                            }
                            break;
                        case "ATTACKER":
                            {
                                if (eventData.Attacker != null)
                                {
                                    center = new BattleUnitTarget(eventData.Attacker);
                                }
                            }
                            break;
                        case "UNIT":
                            {
                                if (eventData.Unit != null)
                                {
                                    center = new BattleUnitTarget(eventData.Unit);
                                }
                            }
                            break;
                        case "POINT":
                            {
                                center = new BattlePointTarget(eventData.Point);
                            }
                            break;
                        case "NODE":
                            {
                                if (eventData.Node != null)
                                {
                                    center = new BattleNodeTarget(eventData.Node);
                                }
                            }
                            break;
                        case "THINKER":
                            {
                                if (eventData.Thinker != null)
                                {
                                    center = new BattleUnitTarget(eventData.Thinker);
                                }
                            }
                            break;
                    }

                    if (center == null)
                    {
                        return null;
                    }

                    MulTargetInfo mulTargetInfo = BattleData.GetTargetInfo(kvTarget);
                    if (mulTargetInfo == null)
                    {
                        return null;
                    }

                    // action自带排除标记
                    mulTargetInfo.ExcludedUnitTargetFlags |= AbilityUnitTargetFlag.UNIT_TARGET_FLAG_UNSELECTABLE;

                    BattleData.FindActionTargets(eventData.Caster, center, mulTargetInfo.NodeRange, mulTargetInfo, targets, (mulTargetInfo.ExcludedUnitTargetFlags & AbilityUnitTargetFlag.UNIT_TARGET_FLAG_DEAD) == AbilityUnitTargetFlag.UNIT_TARGET_FLAG_DEAD);

                    if (targets.Count > 0)
                    {
                        // 最大数量
                        if (mulTargetInfo.MaxNumber > 0)
                        {
                            // 随机
                            if (targets.Count > mulTargetInfo.MaxNumber)
                            {
                                if (mulTargetInfo.IsRandom)
                                {
                                    HashSet<int> removeIndexs = new HashSet<int>();
                                    for (int i = 0, count = targets.Count - mulTargetInfo.MaxNumber; i < count; i++)
                                    {
                                        int randomIndex = BattleData.SRandom.RangeI(0, targets.Count);
                                        LinkedListNode<BattleTarget> first = targets.First;
                                        int index = 0;
                                        while (first != null)
                                        {
                                            if (index == randomIndex)
                                            {
                                                targets.Remove(first);
                                                break;
                                            }

                                            first = first.Next;
                                            index++;
                                        }
                                    }
                                }
                                else
                                {
                                    while (targets.Count > mulTargetInfo.MaxNumber)
                                    {
                                        targets.RemoveLast();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    KeyValue kvNodeX = kvTarget["NodeX"];
                    if (kvNodeX != null)
                    {
                        KeyValue kvNodeY = kvTarget["NodeY"];
                        if (kvNodeY != null)
                        {
                            int nodeX = (int)BattleData.EvaluateLong(kvNodeX.GetString(), eventData);
                            int nodeY = (int)BattleData.EvaluateLong(kvNodeY.GetString(), eventData);

                            BattleNode node = BattleData.FindNode(nodeX, nodeY);
                            if (node != null)
                            {
                                targets.AddLast(new BattleNodeTarget(node));
                            }
                        }
                    }
                }
            }

            return targets.Count > 0 ? targets : null;
        }

        public virtual void Execute(KeyValue kv, EventData eventData)
        {
            //Log.Info(">{0}", kv.GetString());

        }
    }
}