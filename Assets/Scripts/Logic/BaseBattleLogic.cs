using GameFramework.Event;
using GameFramework.Resource;
using LoadSceneSuccessEventArgs = UnityGameFramework.Runtime.LoadSceneSuccessEventArgs;
using LoadSceneFailureEventArgs = UnityGameFramework.Runtime.LoadSceneFailureEventArgs;
using OpenUIFormSuccessEventArgs = UnityGameFramework.Runtime.OpenUIFormSuccessEventArgs;
using OpenUIFormFailureEventArgs = UnityGameFramework.Runtime.OpenUIFormFailureEventArgs;
using UnloadSceneSuccessEventArgs = UnityGameFramework.Runtime.UnloadSceneSuccessEventArgs;
using UnloadSceneFailureEventArgs = UnityGameFramework.Runtime.UnloadSceneFailureEventArgs;
using UnityGameFramework.Runtime;
using KVLib;
using System.Collections.Generic;
using UnityEngine;
using LiaoZhai.RT;


namespace LiaoZhai.Runtime
{
    public abstract class BaseBattleLogic
    {
        protected float _totalTime = 0f;
        protected float _nextUpdateTime = 0f;
        protected float _sendResultDelay = 0f;
        protected bool _isPause = false;
        protected string _sceneName = string.Empty;


        public BaseBattleLogic()
        {

        }

        protected void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;

            Log.Info("加载场景完成:{0}...", BattleData.DrBattleNode.SceneAssetName);

            if (BattleData.State == BattleState.Release
                    || BattleData.State == BattleState.Release2
                    || BattleData.State == BattleState.WaitRelease
                    || BattleData.State == BattleState.WaitRelease2
                    || BattleData.State == BattleState.Exit)
            {
                return;
            }

            if (BattleData.State == BattleState.WaitExit)
            {
                BattleData.Destroy();
                GameManager.Scene.SetSceneOrder(_sceneName, 0);
                GameManager.Scene.UnloadScene(_sceneName);
                BattleData.State = BattleState.Exit;
                return;
            }

            UpdateSceneLoadingEventArgs eUpdateLoading = GameFramework.ReferencePool.Acquire<UpdateSceneLoadingEventArgs>();
            eUpdateLoading.Progress = 0.95f;
            GameManager.Event.Fire(null, eUpdateLoading);

            GameManager.Scene.SetSceneOrder(AssetUtility.GetDefaultAssetPath(BattleData.DrBattleNode.SceneAssetName), 10);

            BattleData.Camera = GameManager.Scene.MainCamera;
            BattleData.CreateCameraController();

            BattleData.CreateBattleGrid(8, 8, DFixVector3.Zero, (DFix64)1.8f);
            BattleData.BattleGrid.InitArea(true);

            for (int x = 4; x < BattleData.BattleGrid.Height; x++)
            {
                for (int y = 0; y < BattleData.BattleGrid.Width; y++)
                {
                    BattleData.BattleGrid[x, y].IsSelfNode = true;
                }
            }

            for (int i = 0; i < BattleInitData.InitPlayerInfos.Count; i++)
            {
                BattleData.CreatePlayer(BattleInitData.InitPlayerInfos[i]);
            }

            for (int i = 0; i < BattleInitData.InitEnemyUnitInfos.Count; i++)
            {
                BaseUnit unit = BattleData.CreateUnit(BattleInitData.InitEnemyUnitInfos[i]);
                if (unit != null)
                {
                    UnitManager.AddUnitToLineup(unit);
                }
                else
                {
                    Log.Fatal("创建敌方单位失败, 单位id:{0}", BattleInitData.InitEnemyUnitInfos[i].card_id.ToString());
                }
            }

            if (BattleData.BattleType == BattleType.Rift)
            {
                BattleData.CurrLineup = GameManager.GlobalData.SelfPlayer.RiftLineup;
            }
            else
            {
                BattleData.CurrLineup = GameManager.GlobalData.SelfPlayer.AttackLineup;
            }

            BattleData.LineupHeroLimit = BattleData.DrBattleNode.LimitHeroNumber;
            BattleData.LimitTime = (DFix64)BattleData.DrBattleNode.LimitTime;

            BattleData.LineupBoss = BattleData.CreatBoss(GameManager.GlobalData.SelfPlayer.GetOccupation(BattleData.CurrLineup.OccId));
            if (BattleData.LineupBoss != null)
            {
                UnitManager.AddUnitToLineup(BattleData.LineupBoss);
            }

            for (int i = BattleData.CurrLineup.Cards.Count - 1; i >= 0; i--)
            {
                PlayerBagCardNew bagCard = GameManager.GlobalData.SelfPlayer.Cards.Find(BattleData.CurrLineup.Cards[i].BagIndex);
                if (bagCard != null)
                {
                    if (BattleData.BattleType == BattleType.Rift)
                    {
                        if (bagCard.RiftHpPct == 0)
                        {
                            BattleData.CurrLineup.Cards.RemoveAt(i);
                            continue;
                        }
                    }

                    if (i < BattleData.LineupHeroLimit)
                    {
                        BaseUnit unit = BattleData.CreateHero(bagCard, BattleData.CurrLineup.Cards[i].NodeX, BattleData.CurrLineup.Cards[i].NodeY);
                        if (unit != null)
                        {
                            BattleData.LineupHeros.Add(unit);
                            UnitManager.AddUnitToLineup(unit);
                        }
                    }
                    else
                    {
                        if (BattleData.BattleType == BattleType.Rift)
                        {
                            BattleData.CurrLineup.Cards.RemoveAt(i);
                            continue;
                        }
                    }
                }
            }

            GameManager.Sound.PlayBGM(BattleData.DrBattleNode.BGMId);

            RuntimeGameEntry.UI.OpenUIFormById(Constant.UIFormId.BattleNew);
        }

        protected void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs)e;

            Log.Error("加载场景失败:{0}", ne.ErrorMessage);

            BattleData.State = BattleState.Exit;
        }

        protected void OnLoadSceneUpdate(object sender, GameEventArgs e)
        {
            UnityGameFramework.Runtime.LoadSceneUpdateEventArgs ne = (UnityGameFramework.Runtime.LoadSceneUpdateEventArgs)e;

            UpdateSceneLoadingEventArgs eUpdateLoading = GameFramework.ReferencePool.Acquire<UpdateSceneLoadingEventArgs>();
            eUpdateLoading.Progress = ne.Progress * 0.9f;
            GameManager.Event.Fire(null, eUpdateLoading);
        }

        protected void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;

            OpenUIFormInfo uiOpenData = (OpenUIFormInfo)ne.UserData;
            if (uiOpenData.UIFormType == (int)UIFormType.BattleNew)
            {
                if (BattleData.State == BattleState.Release
                   || BattleData.State == BattleState.Release2
                   || BattleData.State == BattleState.WaitRelease
                   || BattleData.State == BattleState.WaitRelease2
                   || BattleData.State == BattleState.WaitExit
                   || BattleData.State == BattleState.Exit)
                {
                    return;
                }

                GameManager.UI.CloseAllLoadingUIForms();
                UIForm[] uiforms = GameManager.UI.GetAllLoadedUIForms();
                for (int i = 0; i < uiforms.Length; i++)
                {
                    if (uiforms[i] != ne.UIForm && (uiforms[i].UIGroup.Name == Constant.UIGroup.NORMAL_GROUP || uiforms[i].UIGroup.Name == Constant.UIGroup.FIXED_GROUP || uiforms[i].UIGroup.Name == Constant.UIGroup.POPUP_GROUP))
                    {
                        GameManager.UI.CloseUIForm(uiforms[i]);
                    }
                }

                GameManager.UI.CloseUIForm(UIFormType.Waiting);
            }
            else if (uiOpenData.UIFormType == (int)UIFormType.BattleResultNew)
            {
                if (BattleData.State == BattleState.Release
                     || BattleData.State == BattleState.Release2
                     || BattleData.State == BattleState.WaitRelease
                     || BattleData.State == BattleState.WaitRelease2
                     || BattleData.State == BattleState.WaitExit
                     || BattleData.State == BattleState.Exit)
                {
                    return;
                }

                GameManager.UI.CloseUIForm(UIFormType.BattleTraitDetail);
                GameManager.UI.CloseUIForm(UIFormType.LineupUnitInfo);
                GameManager.UI.CloseUIForm(UIFormType.TraitLinkageGroupInfo);
                GameManager.UI.CloseUIForm(UIFormType.Waiting);
            }
            else if (uiOpenData.UIFormType == (int)UIFormType.BattleSceneLoading)
            {
                //GameManager.UI.CloseUIGroup(Constant.UI.NORMAL_GROUP);
                //GameManager.UI.CloseUIGroup(Constant.UI.FIXED_GROUP);
                //GameManager.UI.CloseUIGroup(Constant.UI.POPUP_GROUP);
                BattleData.State = BattleState.WaitRelease;
            }
        }

        protected void OnOpenUIFormFailure(object sender, GameEventArgs e)
        {
            OpenUIFormFailureEventArgs ne = (OpenUIFormFailureEventArgs)e;

            OpenUIFormInfo uiOpenData = (OpenUIFormInfo)ne.UserData;
            if (uiOpenData.UIFormType == (int)UIFormType.BattleNew)
            {
                Log.Error("打开战斗界面失败");
                BattleData.State = BattleState.Release;
            }
            else if (uiOpenData.UIFormType == (int)UIFormType.BattleResultNew)
            {
                Log.Error("打开战斗结算界面失败");
                BattleData.State = BattleState.Release;
            }
            else if (uiOpenData.UIFormType == (int)UIFormType.BattleSceneLoading)
            {
                Log.Error("打开载入界面失败");

                BattleData.State = BattleState.WaitRelease2;
            }
        }

        protected void OnUnloadSceneSuccess(object sender, GameEventArgs e)
        {
            UnloadSceneSuccessEventArgs ne = (UnloadSceneSuccessEventArgs)e;

            Log.Info("卸载场景完成");

            BattleData.State = BattleState.Exit;
        }

        protected void OnUnloadSceneFailure(object sender, GameEventArgs e)
        {
            UnloadSceneFailureEventArgs ne = (UnloadSceneFailureEventArgs)e;

            Log.Error("卸载场景失败");

            BattleData.State = BattleState.Exit;
        }

        protected virtual void SendResult(bool isGiveup)
        {
            BattleCheckResultCompleteEventArgs ne = GameFramework.ReferencePool.Acquire<BattleCheckResultCompleteEventArgs>();
            ne.ISGiveup = isGiveup;
            GameManager.Event.Fire(null, ne);
        }

        protected virtual void UpdateState(BattleState state)
        {
            switch (BattleData.State)
            {
                case BattleState.Lineup:
                    {

                    }
                    break;
                case BattleState.Start:
                    {
                        if (BattleData.DrBattleNode.LimitTime > 0 && BattleData.LogicTime >= BattleData.LimitTime)
                        {
                            BattleData.CheckResult(BattleCheckResultType.TimeEnd);
                        }

                        if (BattleData.Result != BattleResult.None)
                        {
                            for (int i = 0; i < UnitManager.Units.Count; i++)
                            {
                                UnitManager.Units[i].ApplyModifier(null, null, Constant.Battle.BUILTIN_MODIFIER_BATTLE_END, null, false);
                            }

                            BattleData.State = BattleState.WaitEnd;
                            BattleData.BattleEndTime = GameManager.SysTime.LocalTime;
                            _sendResultDelay = 0f;
                        }
                    }
                    break;
                case BattleState.WaitEnd:
                    {
                        _sendResultDelay += Time.deltaTime;
                        if (_sendResultDelay > 0.5f)
                        {
                            BattleData.State = BattleState.Result;
                            SendResult(false);
                        }
                    }
                    break;
                case BattleState.Giveup:
                    {
                        for (int i = 0; i < UnitManager.Units.Count; i++)
                        {
                            UnitManager.Units[i].ApplyModifier(null, null, Constant.Battle.BUILTIN_MODIFIER_BATTLE_END, null, false);
                        }

                        BattleData.Result = BattleResult.Defeat;
                        BattleData.BattleEndTime = GameManager.SysTime.LocalTime;

                        BattleData.State = BattleState.Result;
                        SendResult(true);
                    }
                    break;
                case BattleState.Result:
                    {

                    }
                    break;
                case BattleState.Release:
                    {
                        RuntimeGameEntry.UI.OpenUIFormById(Constant.UIFormId.BattleSceneLoading, new UIBattleSceneLoadingFormOpenData() { IsShowCard = false });
                        BattleData.State = BattleState.Release2;
                    }
                    break;
                case BattleState.Release2:
                    {

                    }
                    break;
                case BattleState.WaitRelease:
                    {
                        if (GameManager.Scene.SceneIsLoaded(_sceneName))
                        {
                            BattleData.Destroy();

                            GameManager.Scene.SetSceneOrder(_sceneName, 0);
                            GameManager.Scene.UnloadScene(_sceneName);

                            BattleData.State = BattleState.WaitExit;
                        }
                        else if (GameManager.Scene.SceneIsLoading(_sceneName))
                        {
                            BattleData.State = BattleState.WaitExit;
                        }
                        else if (GameManager.Scene.SceneIsUnloading(_sceneName))
                        {
                            BattleData.State = BattleState.WaitExit;
                        }
                        else
                        {
                            GameManager.UI.CloseUIForm(UIFormType.BattleNew);
                            GameManager.UI.CloseUIForm(UIFormType.BattleResultNew);
                            GameManager.UI.CloseUIForm(UIFormType.BattleBossAbilityFemale1);
                            GameManager.UI.CloseUIForm(UIFormType.BattleBossAbilityFemale2);
                            GameManager.UI.CloseUIForm(UIFormType.BattleBossAbilityFemale3);
                            GameManager.UI.CloseUIForm(UIFormType.BattleBossAbility1);
                            GameManager.UI.CloseUIForm(UIFormType.BattleBossAbility2);
                            GameManager.UI.CloseUIForm(UIFormType.BattleBossAbility3);
                            GameManager.UI.CloseUIForm(UIFormType.BattleTraitDetail);
                            GameManager.UI.CloseUIForm(UIFormType.LineupUnitInfo);
                            GameManager.UI.CloseUIForm(UIFormType.TraitLinkageGroupInfo);

                            BattleData.State = BattleState.Exit;
                        }
                    }
                    break;
                case BattleState.WaitRelease2:
                    {
                        if (GameManager.Scene.SceneIsLoaded(_sceneName))
                        {
                            BattleData.Destroy();

                            GameManager.Scene.SetSceneOrder(_sceneName, 0);
                            GameManager.Scene.UnloadScene(_sceneName);

                            BattleData.State = BattleState.WaitExit;
                        }
                        else if (GameManager.Scene.SceneIsLoading(_sceneName))
                        {
                            BattleData.State = BattleState.WaitExit;

                        }
                        else if (GameManager.Scene.SceneIsUnloading(_sceneName))
                        {
                            BattleData.State = BattleState.WaitExit;
                        }
                        else
                        {
                            GameManager.UI.CloseUIForm(UIFormType.BattleNew);
                            GameManager.UI.CloseUIForm(UIFormType.BattleResultNew);
                            GameManager.UI.CloseUIForm(UIFormType.BattleBossAbilityFemale1);
                            GameManager.UI.CloseUIForm(UIFormType.BattleBossAbilityFemale2);
                            GameManager.UI.CloseUIForm(UIFormType.BattleBossAbilityFemale3);
                            GameManager.UI.CloseUIForm(UIFormType.BattleBossAbility1);
                            GameManager.UI.CloseUIForm(UIFormType.BattleBossAbility2);
                            GameManager.UI.CloseUIForm(UIFormType.BattleBossAbility3);
                            GameManager.UI.CloseUIForm(UIFormType.BattleTraitDetail);
                            GameManager.UI.CloseUIForm(UIFormType.LineupUnitInfo);
                            GameManager.UI.CloseUIForm(UIFormType.TraitLinkageGroupInfo);

                            BattleData.State = BattleState.Exit;
                        }
                    }
                    break;
                case BattleState.WaitExit:
                    {

                    }
                    break;
                case BattleState.Exit:
                    {
                        GameManager.UI.CloseUIGroup(Constant.UIGroup.NORMAL_GROUP);
                        //GameManager.UI.CloseUIForm(UIFormType.BattleNew);
                        //GameManager.UI.CloseUIForm(UIFormType.BattleResultNew);
                        //GameManager.UI.CloseUIForm(UIFormType.BattleBossAbilityFemale1);
                        //GameManager.UI.CloseUIForm(UIFormType.BattleBossAbilityFemale2);
                        //GameManager.UI.CloseUIForm(UIFormType.BattleBossAbilityFemale3);
                        //GameManager.UI.CloseUIForm(UIFormType.BattleBossAbility1);
                        //GameManager.UI.CloseUIForm(UIFormType.BattleBossAbility2);
                        //GameManager.UI.CloseUIForm(UIFormType.BattleBossAbility3);
                        //GameManager.UI.CloseUIForm(UIFormType.BattleTraitDetail);
                        //GameManager.UI.CloseUIForm(UIFormType.LineupUnitInfo);
                        //GameManager.UI.CloseUIForm(UIFormType.TraitLinkageGroupInfo);

                        BattleData.State = BattleState.ExitComplete;
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
        }

        public abstract void Init();

        public abstract void Release();

        public virtual void LoadScene()
        {
            _sceneName = AssetUtility.GetDefaultAssetPath(BattleData.DrBattleNode.SceneAssetName);
            Log.Info("加载场景:{0}...", BattleData.DrBattleNode.SceneAssetName);
            GameManager.Scene.LoadScene(_sceneName);
        }

        public virtual void Pause()
        {
            if (BattleData.State != BattleState.Start)
            {
                return;
            }

            _isPause = true;
            BattleData.ChangeBattleSpeed(0f);
        }

        public virtual void Resume()
        {
            if (_isPause)
            {
                _isPause = false;
                BattleData.ChangeBattleSpeed(BattleData.GetFinalBattleSpeed());
            }
        }

        public virtual void UpdateLogic()
        {
            if (BattleData.State != BattleState.ExitComplete)
            {
                float frameLength = (float)BattleData.LogicFrameLength;
                float deltaTime = 0f;
                if (!_isPause)
                {
                    //deltaTime = (float)(System.DateTime.Now - _startTime).TotalSeconds;
                    deltaTime = Time.deltaTime;
                    deltaTime = deltaTime * BattleData.GetFinalBattleSpeed();
                }

                float interpolation = 0f;

                _totalTime = _totalTime + deltaTime;
                // 如果真实累计的时间超过游戏帧逻辑原本应有的时间,则循环执行逻辑,确保整个逻辑的运算不会因为帧间隔时间的波动而计算出不同的结果
                while (_totalTime > _nextUpdateTime)
                {
                    // 运行与游戏相关的具体逻辑
                    // 更新操作逻辑
                    BattleData.UpdateAction(BattleData.LogicFrameLength);
                    // 跟新弹道逻辑
                    ProjectileManager.UpdateLogic(BattleData.LogicFrameLength);
                    // 更新单位逻辑
                    UnitManager.UpdateLogic(BattleData.LogicFrameLength);
                    // 更新特效逻辑
                    ParticleManager.UpdateLogic(BattleData.LogicFrameLength);

                    // 游戏逻辑帧自增
                    //if (BattleData.State == BattleState.Lineup || BattleData.State == BattleState.Start)
                    {
                        BattleData.LogicFrame++;
                    }

                    if (BattleData.State == BattleState.Start)
                    {
                        BattleData.LogicTime += BattleData.LogicFrameLength;
                    }

                    UpdateState(BattleData.State);

                    BattleData.RecycleEventDatas(false);

                    // 计算下一个逻辑帧应有的时间
                    //while (_totalTime > _nextUpdateTime)
                    {
                        _nextUpdateTime += frameLength;
                    }
                }

                ModifierManager.RecycleModifiers();

                // 计算两帧的时间差,用于运行补间动画
                interpolation = (_totalTime + frameLength - _nextUpdateTime) / frameLength;

                // 跟新单位渲染
                UnitManager.UpdateRender(interpolation, deltaTime);
                // 跟新弹道渲染
                ProjectileManager.UpdateRender(interpolation, deltaTime);
                // 更新特效渲染
                ParticleManager.UpdateRender(interpolation, deltaTime);
            }
        }
    }
}