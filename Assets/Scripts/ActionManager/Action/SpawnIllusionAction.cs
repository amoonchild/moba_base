using KVLib;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    // 召唤幻象
    /*
        "Target"					幻象复制的单位
	    "UnitCount"					召唤单位量
	    "Duration"					持续时间
	    "Attributes"
	    {
		    "Attack"
		    "Defense"
		    "MaxHp"
		    "Hp"
		    "AttackSpeed"
		    "MoveSpeed"
		    "RotateSpeed"
		    "Lucky"
		    "Tenacity"
		    "CritOdds"
		    "CritDamage"
		    "DodgeOdds"
		    "HitOdds"	
		    "AbilityCDBonus"
		    "PhysicalArmorBreak"
		    "HpSteal"
		    "Shield"
		    "AttackNodeRange"
		    "DamageOutBonus"
		    "DamageInBonus"
		    "AttackDamageOutBonus"
		    "AttackDamageInBonus"
		    "AbilityDamageOutBonus"
		    "AbilityDamageInBonus"
		    "HealOutBonus"
		    "HealInBonus"
		    "Mana"
		    "MaxMana"
		    "ManaRegen"
		    "Scale"
		    "Trait1DamageOutBouns"
		    "Trait2DamageOutBouns"
		    "Trait3DamageOutBouns"
		    "Trait4DamageOutBouns"
		    "Trait5DamageOutBouns"
		    "Trait6DamageOutBouns"
		    "Trait99DamageOutBouns"
	    }
	
	    "OnSpawn"					召唤成功后操作
	    {
	
	    }
    */
    public class SpawnIllusionAction : BaseAction
    {
        private List<BattleNode> _findNodes = new List<BattleNode>();

        public override string Name
        {
            get
            {
                return "SpawnIllusion";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvTarget = kv["Target"];
            KeyValue kvUnitCount = kv["UnitCount"];
            KeyValue kvAttributes = kv["Attributes"];
            KeyValue kvAbility = kv["Ability"];
            KeyValue kvDuration = kv["Duration"];
            KeyValue kvOnSpawn = kv["OnSpawn"];

            if (kvTarget == null)
            {
                Log.Error("SpawnIllusion: 缺少参数:Target");
                return;
            }

            if (kvUnitCount == null)
            {
                Log.Error("SpawnIllusion: 缺少参数:UnitCount");
                return;
            }

            if (kvAttributes == null || !kvAttributes.HasChildren)
            {
                Log.Error("SpawnIllusion: 缺少参数:Attributes");
                return;
            }

            int unitCount = BattleData.ParseInt(kvUnitCount.GetString());
            if (unitCount == 0)
            {
                Log.Error("SpawnIllusion: UnitCount为0");
                return;
            }

            CreateModifierData createModifierData = new CreateModifierData();
            if (kvDuration != null)
            {
                createModifierData.Duration = BattleData.EvaluateDFix64(kvDuration.GetString(), eventData);
                if (createModifierData.Duration == DFix64.Zero)
                {
                    Log.Error("SpawnIllusion: Duration为0");
                    return;
                }
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("SpawnIllusion: 找不到目标");
                return;
            }

            BattleUnitAttribute initAttribute = new BattleUnitAttribute();
            foreach (var child in kvAttributes.Children)
            {
                switch (child.Key)
                {
                    case "Attack": { initAttribute.Attack = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "Defense": { initAttribute.Defense = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "DefenseRatio": { initAttribute.DefenseRatio = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "MaxHp": { initAttribute.MaxHp = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "Hp": { initAttribute.Hp = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "AttackSpeed": { initAttribute.AttackSpeed = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "MoveSpeed": { initAttribute.MoveSpeed = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "RotateSpeed": { initAttribute.RotateSpeed = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "CritOdds": { initAttribute.CritOdds = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "CritDamage": { initAttribute.CritDamage = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "CritResistance": { initAttribute.CritResistance = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "DodgeOdds": { initAttribute.DodgeOdds = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "HitOdds": { initAttribute.HitOdds = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "AbilityCDBonus": { initAttribute.AbilityCDBonus = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "PhysicalArmorBreak": { initAttribute.PhysicalArmorBreak = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "HpSteal": { initAttribute.HpSteal = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "AttackNodeRange": { initAttribute.AttackNodeRange = (int)BattleData.EvaluateLong(child.GetString(), eventData); } break;
                    case "DamageOutBonus": { initAttribute.DamageOutBonus = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "DamageInBonus": { initAttribute.DamageInBonus = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "AttackDamageOutBonus": { initAttribute.AttackDamageOutBonus = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "AttackDamageInBonus": { initAttribute.AttackDamageInBonus = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "AbilityDamageOutBonus": { initAttribute.AbilityDamageOutBonus = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "AbilityDamageInBonus": { initAttribute.AbilityDamageInBonus = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "HealOutBonus": { initAttribute.HealOutBonus = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "HealInBonus": { initAttribute.HealInBonus = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "FinalDamageOutBouns": { initAttribute.FinalDamageOutBouns = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "FinalDamageInBouns": { initAttribute.FinalDamageInBouns = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "Mana": { initAttribute.Mana = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "MaxMana": { initAttribute.MaxMana = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "ManaRegen": { initAttribute.ManaRegen = DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData)); } break;
                    case "Scale": { initAttribute.Scale = BattleData.EvaluateDFix64(child.GetString(), eventData); } break;
                    case "Trait1DamageOutBouns": { initAttribute.TraitDamageOutBouns.Add((int)CardTraitType.Tenacious, DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData))); } break;
                    case "Trait2DamageOutBouns": { initAttribute.TraitDamageOutBouns.Add((int)CardTraitType.Heroic, DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData))); } break;
                    case "Trait3DamageOutBouns": { initAttribute.TraitDamageOutBouns.Add((int)CardTraitType.Ruthless, DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData))); } break;
                    case "Trait4DamageOutBouns": { initAttribute.TraitDamageOutBouns.Add((int)CardTraitType.Friendly, DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData))); } break;
                    case "Trait5DamageOutBouns": { initAttribute.TraitDamageOutBouns.Add((int)CardTraitType.Mysterious, DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData))); } break;
                    case "Trait6DamageOutBouns": { initAttribute.TraitDamageOutBouns.Add((int)CardTraitType.Cunning, DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData))); } break;
                    case "Trait99DamageOutBouns": { initAttribute.TraitDamageOutBouns.Add((int)CardTraitType.Boss, DFix64.Floor(BattleData.EvaluateDFix64(child.GetString(), eventData))); } break;
                }
            }

            ObscuredInt[] abilityIds = null;
            if (kvAbility != null && kvAbility.HasChildren)
            {
                abilityIds = new ObscuredInt[kvAbility.ChildCount];
                int index = 0;
                foreach (var child in kvAbility.Children)
                {
                    abilityIds[index] = BattleData.ParseInt(child.GetString());
                    index++;
                }
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                if (first.Value.Type != BattleTargetType.UNIT)
                {
                    Log.Error("SpawnIllusion: 目标类型不是UNIT");
                    first = first.Next;
                    continue;
                }

                BaseUnit sourceTarget = (BaseUnit)first.Value.Target;

                for (int i = 0; i < unitCount; i++)
                {
                    BattleData.FindWalkableNeighboursNearest(sourceTarget.CurrNode, 7, _findNodes);
                    if (_findNodes.Count == 0)
                    {
                        break;
                    }

                    BattleNode spawnNode = _findNodes[0];

                    BaseUnit spawnedUnit = BattleData.CreateIllusion(eventData.Caster, sourceTarget, spawnNode.X, spawnNode.Y, initAttribute, abilityIds);
                    if (spawnedUnit == null)
                    {
                        Log.Error("SpawnIllusion: 创建幻象失败");
                        continue;
                    }

                    Log.Info("SpawnIllusion: 创建 {0} 的幻象{1}", sourceTarget.LogName,
                   createModifierData.IsDurationSpecified ? ",持续" + createModifierData.Duration.ToString() + "秒" : string.Empty);

                    UnitManager.Units.Add(spawnedUnit);
                    spawnedUnit.Master = eventData.Caster;
                    spawnedUnit.LogicEulerAngles = spawnedUnit.Master.LogicEulerAngles;
                    spawnedUnit.Spawn(false);

                    if (spawnedUnit.IsSpawned)
                    {
                        spawnedUnit.ApplyModifier(eventData.Caster, null, Constant.Battle.BUILTIN_MODIFIER_ILLUSION, null, false);
                        if (createModifierData.IsDurationSpecified && createModifierData.Duration > DFix64.Zero)
                        {
                            spawnedUnit.ApplyModifier(eventData.Caster, null, Constant.Battle.BUILTIN_MODIFIER_ILLUSION_Kill, createModifierData, false);
                        }

                        if (kvOnSpawn != null)
                        {
                            EventData onSpawnEvent = BattleData.CreateEventData();
                            onSpawnEvent.Caster = eventData.Caster;
                            onSpawnEvent.Ability = eventData.Ability;
                            onSpawnEvent.Target = spawnedUnit;
                            onSpawnEvent.Unit = spawnedUnit;
                            onSpawnEvent.Point = spawnedUnit.LogicPosition;
                            onSpawnEvent.Node = spawnedUnit.CurrNode;

                            BattleData.ExecuteActions(kvOnSpawn, onSpawnEvent);
                        }
                    }
                    else
                    {
                        UnitManager.Units.Remove(spawnedUnit);
                    }
                }

                first = first.Next;
            }
        }
    }
}