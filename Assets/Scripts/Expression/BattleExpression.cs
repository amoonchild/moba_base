// -----------------------------------------------
// Copyright © Jeffrey. All rights reserved.
// CreateTime: 2021/7/6   16:8:44
// -----------------------------------------------

using GameDevWare.Dynamic.Expressions;
using GameDevWare.Dynamic.Expressions.CSharp;
using Assets;
using KVLib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    public class BattleExpression<T>
    {
        private KnownTypeResolver _fullTypeResolver = new KnownTypeResolver(
            typeof(UnityEngine.Mathf),
            typeof(UnityEngine.Random),
            typeof(Fix64),
            typeof(DFix64),
            typeof(FixVector3),
            typeof(SRandom),
            typeof(BattleUnitAttribute),
            typeof(BattleUnitStatisticsData),
            typeof(MulTargetInfo),
            typeof(Attack),
            typeof(Ability),
            typeof(Modifier),
            typeof(BattleData),
            typeof(BattleNode),
            typeof(BaseObject),
            typeof(BaseEntity),
            typeof(BaseUnit),
            typeof(Hero),
            typeof(Boss),
            typeof(Thinker),
            typeof(Enum),
            typeof(int),
            typeof(float),
            typeof(long),
            typeof(ulong),
            typeof(ObscuredBool),
            typeof(ObscuredInt),
            typeof(ObscuredFloat),
            typeof(ObscuredDouble),
            typeof(ObscuredLong),
            typeof(ObscuredUInt),
            typeof(ObscuredULong),
            typeof(bool));

        private Dictionary<string, PatternString<T>> patternStrings = new Dictionary<string, PatternString<T>>();
        private Dictionary<string, Func<DFix64>> expressionFix64Funcs = new Dictionary<string, Func<DFix64>>();
        private Dictionary<string, DFix64> expressionFix64Values = new Dictionary<string, DFix64>();
        private Dictionary<string, Func<long>> expressionLongFuncs = new Dictionary<string, Func<long>>();
        private Dictionary<string, long> expressionLongValues = new Dictionary<string, long>();
        private Dictionary<string, bool> expressionBoolValues = new Dictionary<string, bool>();
        private Dictionary<string, Func<bool>> expressionBoolFuncs = new Dictionary<string, Func<bool>>();
        private Dictionary<string, int> expressionEnums = new Dictionary<string, int>();
        private Dictionary<string, HashSet<int>> expressionTraits = new Dictionary<string, HashSet<int>>();
        private Dictionary<string, int[]> expressionModifierGroups = new Dictionary<string, int[]>();


        public void Release()
        {
            patternStrings.Clear();
            expressionFix64Funcs.Clear();
            expressionFix64Values.Clear();
            expressionLongFuncs.Clear();
            expressionLongValues.Clear();
            expressionBoolFuncs.Clear();
            expressionBoolValues.Clear();
            expressionEnums.Clear();
            expressionTraits.Clear();
            expressionModifierGroups.Clear();
        }

        public DFix64 ParseDFix64(string pattern)
        {
            try
            {
                if (!expressionFix64Values.ContainsKey(pattern))
                {
                    float ret;
                    if (!float.TryParse(pattern, out ret))
                    {
                        Log.Error("数值解析错误: {0}", pattern);
                    }

                    expressionFix64Values.Add(pattern, (DFix64)ret);
                }

                return expressionFix64Values[pattern];
            }
            catch (Exception e)
            {
                Log.Error("数值解析错误: {0}, {1}", pattern, e.Message);

                return DFix64.Zero;
            }
        }

        public long ParseLong(string pattern)
        {
            try
            {
                if (!expressionLongValues.ContainsKey(pattern))
                {
                    long ret;
                    if (!long.TryParse(pattern, out ret))
                    {
                        Log.Error("数值解析错误: {0}", pattern);
                    }

                    expressionLongValues.Add(pattern, ret);
                }

                return expressionLongValues[pattern];
            }
            catch (Exception e)
            {
                Log.Error("数值解析错误: {0}, {1}", pattern, e.Message);

                return 0L;
            }
        }

        public bool ParseBool01(string pattern)
        {
            try
            {
                if (!expressionBoolValues.ContainsKey(pattern))
                {
                    int ret;
                    if(!int.TryParse(pattern, out ret))
                    {
                        Log.Error("数值解析错误: {0}", pattern);
                    }

                    expressionBoolValues.Add(pattern, ret != 0);
                }

                return expressionBoolValues[pattern];
            }
            catch (Exception e)
            {
                Log.Error("数值解析错误: {0}, {1}", pattern, e.Message);
                return false;
            }
        }

        public DFix64 EvaluateDFix64(string pattern, T instance)
        {
            try
            {
                string expression = TransformPattern(pattern, instance);
                if (!expressionFix64Funcs.ContainsKey(expression))
                {
                    Expression<Func<DFix64>> expressionFunc = CSharpExpression.ParseFunc<DFix64>(expression, _fullTypeResolver);
                    Func<DFix64> func = expressionFunc.CompileAot(true);

                    expressionFix64Funcs.Add(expression, func);
                }

                return expressionFix64Funcs[expression].Invoke();
            }
            catch (Exception e)
            {
                Log.Error("表达式解析错误: {0}, {1}", pattern, e.Message);

                return DFix64.Zero;
            }
        }

        public long EvaluateLong(string pattern, T instance)
        {
            try
            {
                string expression = TransformPattern(pattern, instance);
                if (!expressionLongFuncs.ContainsKey(expression))
                {
                    Expression<Func<long>> expressionFunc = CSharpExpression.ParseFunc<long>(expression, _fullTypeResolver);
                    Func<long> func = expressionFunc.CompileAot(true);

                    expressionLongFuncs.Add(expression, func);
                }

                return expressionLongFuncs[expression].Invoke();
            }
            catch (Exception e)
            {
                Log.Error("表达式解析错误: {0}, {1}", pattern, e.Message);

                return 0L;
            }
        }

        public bool EvaluateBool(string pattern, T instance)
        {
            try
            {
                string expression = TransformPattern(pattern, instance);
                if (!expressionBoolFuncs.ContainsKey(expression))
                {
                    Expression<Func<bool>> expressionFunc = CSharpExpression.ParseFunc<bool>(expression, _fullTypeResolver);
                    Func<bool> func = expressionFunc.CompileAot(true);

                    expressionBoolFuncs.Add(expression, func);
                }

                return expressionBoolFuncs[expression].Invoke();
            }
            catch (Exception e)
            {
                Log.Error("表达式解析错误: {0}, {1}", pattern, e.Message);
                return false;
            }
        }

        public TEnum EvaluateEnum<TEnum>(string pattern)
        {
            try
            {
                if (expressionEnums.ContainsKey(pattern))
                {
                    return (TEnum)Enum.ToObject(typeof(TEnum), expressionEnums[pattern]);
                }
                else
                {
                    TEnum result = (TEnum)Enum.Parse(typeof(TEnum), pattern, false);
                    expressionEnums.Add(pattern, Convert.ToInt32(result));
                    return result;
                }
            }
            catch (Exception e)
            {
                Log.Error("解析枚举失败, from:{0}, {1}", pattern, e.Message);
                TEnum result = default(TEnum);
                expressionEnums.Add(pattern, Convert.ToInt32(result));
                return result;
            }
        }

        public TEnum EvaluateEnum<TEnum>(string pattern, TEnum defaultValue)
        {
            try
            {
                if (expressionEnums.ContainsKey(pattern))
                {
                    return (TEnum)Enum.ToObject(typeof(TEnum), expressionEnums[pattern]);
                }
                else
                {
                    TEnum result = (TEnum)Enum.Parse(typeof(TEnum), pattern, false);
                    expressionEnums.Add(pattern, Convert.ToInt32(result));
                    return result;
                }
            }
            catch (Exception e)
            {
                Log.Error("解析枚举失败, from:{0}, {1}", pattern, e.Message);
                expressionEnums.Add(pattern, Convert.ToInt32(defaultValue));
                return defaultValue;
            }
        }

        public bool TryEvaluateEnum<TEnum>(string pattern, out TEnum result)
        {
            return TryEvaluateEnum(pattern, out result, default(TEnum));
        }

        public bool TryEvaluateEnum<TEnum>(string pattern, out TEnum result, TEnum defaultValue)
        {
            try
            {
                if (expressionEnums.ContainsKey(pattern))
                {
                    result = (TEnum)Enum.ToObject(typeof(TEnum), expressionEnums[pattern]);
                }
                else
                {
                    result = (TEnum)Enum.Parse(typeof(TEnum), pattern, false);
                    expressionEnums.Add(pattern, Convert.ToInt32(result));
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error("解析枚举失败, from:{0}, {1}", pattern, e.Message);
                result = defaultValue;
                expressionEnums.Add(pattern, Convert.ToInt32(result));
                return false;
            }
        }

        public TEnum EvaluateEnums<TEnum>(string pattern, char split, TEnum defaultValue)
        {
            try
            {
                if (expressionEnums.ContainsKey(pattern))
                {
                    return (TEnum)Enum.ToObject(typeof(TEnum), expressionEnums[pattern]);
                }
                else
                {
                    string text = pattern.Replace(split, ',');
                    TEnum result = (TEnum)Enum.Parse(typeof(TEnum), text, false);
                    expressionEnums.Add(pattern, Convert.ToInt32(result));
                    return result;
                }
            }
            catch (Exception e)
            {
                Log.Error("解析枚举失败, from:{0}, {1}", pattern, e.Message);
                expressionEnums.Add(pattern, Convert.ToInt32(defaultValue));
                return defaultValue;
            }
        }

        public bool TryEvaluateEnums<TEnum>(string pattern, char split, out TEnum result)
        {
            return TryEvaluateEnums<TEnum>(pattern, split, out result, default(TEnum));
        }

        public bool TryEvaluateEnums<TEnum>(string pattern, char split, out TEnum result, TEnum defaultValue)
        {
            try
            {
                if (expressionEnums.ContainsKey(pattern))
                {
                    result = (TEnum)Enum.ToObject(typeof(TEnum), expressionEnums[pattern]);
                }
                else
                {
                    string text = pattern.Replace(split, ',');
                    result = (TEnum)Enum.Parse(typeof(TEnum), text, false);
                    expressionEnums.Add(pattern, Convert.ToInt32(result));
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error("解析枚举失败, from:{0}, {1}", pattern, e.Message);
                result = defaultValue;
                expressionEnums.Add(pattern, Convert.ToInt32(result));
                return false;
            }
        }

        public HashSet<int> EvaluateTraits<TEnum>(string pattern, char split)
        {
            try
            {
                if (expressionTraits.ContainsKey(pattern))
                {
                    return expressionTraits[pattern];
                }
                else
                {
                    HashSet<int> enumArray = null;

                    string[] texts = pattern.Split(new char[] { split }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (texts.Length > 0)
                    {
                        enumArray = new HashSet<int>();
                        for (int i = 0; i < texts.Length; i++)
                        {
                            TEnum result = (TEnum)Enum.Parse(typeof(TEnum), texts[i], false);
                            enumArray.Add(Convert.ToInt32(result));
                        }
                    }

                    expressionTraits.Add(pattern, enumArray);
                    return enumArray;
                }
            }
            catch (Exception e)
            {
                Log.Error("解析枚举失败, from:{0}, {1}", pattern, e.Message);
                expressionTraits.Add(pattern, null);
                return null;
            }
        }

        public int[] EvaluateModifierGroups(string pattern, char split)
        {
            try
            {
                if (expressionModifierGroups.ContainsKey(pattern))
                {
                    return expressionModifierGroups[pattern];
                }
                else
                {
                    int[] modifierGroups = null;
                    string[] texts = pattern.Split(new char[] { split }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (texts.Length > 0)
                    {
                        modifierGroups = new int[texts.Length];
                        for (int i = 0; i < texts.Length; i++)
                        {
                            if(!int.TryParse(texts[i], out modifierGroups[i]))
                            {

                            }
                        }
                    }

                    expressionModifierGroups.Add(pattern, modifierGroups);
                    return modifierGroups;
                }
            }
            catch (Exception e)
            {
                Log.Error("解析枚举失败, from:{0}, {1}", pattern, e.Message);
                expressionModifierGroups.Add(pattern, null);
                return null;
            }
        }

        public void AddPattern(string pattern)
        {
            if (pattern == null)
            {
                return;
            }

            if (!patternStrings.ContainsKey(pattern))
            {
                string newPattern = pattern.Replace("'", "\"");

                PatternString<T> patternString = new PatternString<T>(newPattern);
                patternStrings.Add(pattern, patternString);
            }
        }

        private string TransformPattern(string pattern, T instance)
        {
            if (pattern == null || instance == null)
            {
                return string.Empty;
            }

            if (patternStrings.ContainsKey(pattern))
            {
                return patternStrings[pattern].Tranform(instance);
            }
            else
            {
                string newPattern = pattern.Replace("'", "\"");

                PatternString<T> patternString = new PatternString<T>(newPattern);
                patternStrings.Add(pattern, patternString);

                return patternString.Tranform(instance);
            }
        }
    }
}