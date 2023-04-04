using System;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public static class UnitManager
    {
        private static List<BaseUnit> _units = null;

        public static List<BaseUnit> Units
        {
            get
            {
                return _units;
            }
        }


        public static void Init()
        {
            _units = new List<BaseUnit>();
        }

        public static void Destroy()
        {
            for (int i = 0; i < _units.Count; i++)
            {
                _units[i].Release();
            }

            _units.Clear();
        }

        public static void Release()
        {
            _units = null;
        }

        public static void ChangeSpeed(float speed)
        {
            for (int i = 0, count = _units.Count; i < count; i++)
            {
                _units[i].ChangeSpeed(speed);
            }
        }

        public static void UpdateLogic(DFix64 frameLength)
        {
            for (int i = 0, count = _units.Count; i < count;)
            {
                BaseUnit baseUnit = _units[i];

                if (baseUnit.CreatedFrame == BattleData.LogicFrame)
                {
                    break;
                }
                else if (baseUnit.IsSpawned)
                {
                    if (baseUnit.Type == UnitType.UNIT_THINKER)
                    {
                        if (baseUnit.IsHideState)
                        {
                            //baseUnit.Release();
                            //_units.RemoveAt(i);
                            //count--;
                            //continue;
                        }
                        else
                        {
                            baseUnit.UpdateLogic(frameLength);
                        }
                    }
                    else
                    {
                        baseUnit.UpdateLogic(frameLength);
                    }
                }
                else if (baseUnit.IsSpawnedToLineup)
                {
                    if (BattleData.State == BattleState.Start)
                    {
                        if (baseUnit.IsResetAtt)
                        {
                            baseUnit.Spawn(true);
                        }
                    }
                    else
                    {
                        baseUnit.UpdateLogic(frameLength);
                    }
                }
                else
                {
                    if (BattleData.State == BattleState.Start)
                    {
                        if (BattleData.LogicTime >= baseUnit.DelaySpawnTime && baseUnit.IsResetAtt)
                        {
                            if (baseUnit.CurrNode.Unit != baseUnit)
                            {
                                BattleNode node = BattleData.FindWalkableNeighbourNearest(baseUnit.CurrNode);
                                if (node != null)
                                {
                                    baseUnit.SetNode(node);
                                    baseUnit.Spawn(true);
                                }
                            }
                            else
                            {
                                baseUnit.Spawn(true);
                            }
                        }
                    }
                }

                i++;
            }
        }

        public static void UpdateRender(float interpolation, float deltaTime)
        {
            for (int i = 0; i < _units.Count; i++)
            {
                if (!_units[i].IsHideState)
                {
                    _units[i].UpdateRender(interpolation, deltaTime);
                }
            }
        }

        public static void AddUnitToLineup(BaseUnit unit)
        {
            unit.ApplyModifier(null, null, Constant.Battle.BUILTIN_MODIFIER_BATTLE_LINEUP, null, false);
            _units.Add(unit);
        }

        public static void RemoveUnitFromLineup(BaseUnit unit)
        {
            for (int i = 0, count = _units.Count; i < count; i++)
            {
                if (_units[i] == unit)
                {
                    _units[i].RemoveModifier(Constant.Battle.BUILTIN_MODIFIER_BATTLE_LINEUP);
                    _units.RemoveAt(i);
                    break;
                }
            }
        }

        public static void PrepareBattle()
        {
            for (int i = 0, count = _units.Count; i < count; i++)
            {
                if (_units[i].IsSpawnedToLineup)
                {
                    if (_units[i].IsResetAtt)
                    {
                        _units[i].RemoveModifier(Constant.Battle.BUILTIN_MODIFIER_BATTLE_LINEUP);
                        _units[i].Spawn(false);
                    }
                    else
                    {
                        Log.Fatal("{0} 没有收到服务器属性", _units[i].LogName);
                    }
                }
                else
                {
                    if (!_units[i].IsResetAtt)
                    {
                        Log.Fatal("{0} 没有收到服务器属性", _units[i].LogName);
                    }
                }
            }
        }

        public static BaseUnit CreateUnit(CreateUnitData initUnitData)
        {
            switch (initUnitData.unit_type)
            {
                case UnitType.UNIT_HERO:
                    {
                        if (BattleKvLibraryManager.HasUnitKv(initUnitData.card_id))
                        {
                            Hero hero = new Hero(initUnitData);
                            hero.ChangeSpeed(BattleData.GetFinalBattleSpeed());

                            return hero;
                        }
                    }
                    break;
                case UnitType.UNIT_BOSS:
                    {
                        if (BattleKvLibraryManager.HasUnitKv(initUnitData.card_id))
                        {
                            Boss boss = new Boss((CreateBossData)initUnitData);
                            boss.ChangeSpeed(BattleData.GetFinalBattleSpeed());

                            return boss;
                        }
                    }
                    break;
                case UnitType.UNIT_THINKER:
                    {
                        Thinker thinker = new Thinker((CreateThinkerData)initUnitData);
                        return thinker;
                    }
                    break;
                default:
                    {

                    }
                    break;
            }

            return null;
        }
    }
}