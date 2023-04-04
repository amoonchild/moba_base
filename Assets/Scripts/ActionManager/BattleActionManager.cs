using KVLib;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public partial class BattleActionManager
    {
        private Dictionary<string, BaseAction> _actionControllers = new Dictionary<string, BaseAction>();
        private List<DelayActionTimer> _delayedActionTimers = new List<DelayActionTimer>();
        private List<EventData> _eventDatas = new List<EventData>();


        public void Init()
        {
            _actionControllers.Clear();
            _delayedActionTimers.Clear();

            ActOnTargetsAction actOnTargetsAction = new ActOnTargetsAction();
            _actionControllers.Add(actOnTargetsAction.Name, actOnTargetsAction);

            ApplyModifierAction applyModifierAction = new ApplyModifierAction();
            _actionControllers.Add(applyModifierAction.Name, applyModifierAction);

            AttachEffectAction attachEffectAction = new AttachEffectAction();
            _actionControllers.Add(attachEffectAction.Name, attachEffectAction);

            AttachSoundAction attachSoundAction = new AttachSoundAction();
            _actionControllers.Add(attachSoundAction.Name, attachSoundAction);

            BlinkAction blinkAction = new BlinkAction();
            _actionControllers.Add(blinkAction.Name, blinkAction);

            ChangeAbilityRemainingCooldownAction changeAbilityRemainingCooldown = new ChangeAbilityRemainingCooldownAction();
            _actionControllers.Add(changeAbilityRemainingCooldown.Name, changeAbilityRemainingCooldown); ;

            ChangeAttackTargetAction changeAttackTarget = new ChangeAttackTargetAction();
            _actionControllers.Add(changeAttackTarget.Name, changeAttackTarget);

            CreateThinkerAction createThinkerAction = new CreateThinkerAction();
            _actionControllers.Add(createThinkerAction.Name, createThinkerAction);

            DamageAction damageAction = new DamageAction();
            _actionControllers.Add(damageAction.Name, damageAction);

            DelayedAction delayedAction = new DelayedAction();
            _actionControllers.Add(delayedAction.Name, delayedAction);

            DisableAbilityAction disableAbilityAction = new DisableAbilityAction();
            _actionControllers.Add(disableAbilityAction.Name, disableAbilityAction);

            EnableAbilityAction enableAbilityAction = new EnableAbilityAction();
            _actionControllers.Add(enableAbilityAction.Name, enableAbilityAction);

            FireEffectAction fireEffectAction = new FireEffectAction();
            _actionControllers.Add(fireEffectAction.Name, fireEffectAction);

            FireSoundAction fireSoundAction = new FireSoundAction();
            _actionControllers.Add(fireSoundAction.Name, fireSoundAction);

            HealAction healAction = new HealAction();
            _actionControllers.Add(healAction.Name, healAction);

            LinearProjectileAction linearProjectileAction = new LinearProjectileAction();
            _actionControllers.Add(linearProjectileAction.Name, linearProjectileAction);

            LogAction logAction = new LogAction();
            _actionControllers.Add(logAction.Name, logAction);

            PurgeModifierAction purgeModifierAction = new PurgeModifierAction();
            _actionControllers.Add(purgeModifierAction.Name, purgeModifierAction);

            RandomAction randomAction = new RandomAction();
            _actionControllers.Add(randomAction.Name, randomAction);

            RecoverManaAction recoverManaAction = new RecoverManaAction();
            _actionControllers.Add(recoverManaAction.Name, recoverManaAction);

            RemoveModifierAction removeModifierAction = new RemoveModifierAction();
            _actionControllers.Add(removeModifierAction.Name, removeModifierAction);

            RespawnUnitAction respawnUnitAction = new RespawnUnitAction();
            _actionControllers.Add(respawnUnitAction.Name, respawnUnitAction);

            RunScriptAction runScriptAction = new RunScriptAction();
            _actionControllers.Add(runScriptAction.Name, runScriptAction);

            ShakeScreenAction shakeScreenAction = new ShakeScreenAction();
            _actionControllers.Add(shakeScreenAction.Name, shakeScreenAction);

            SpawnIllusionAction spawnIllusionAction = new SpawnIllusionAction();
            _actionControllers.Add(spawnIllusionAction.Name, spawnIllusionAction);

            SpawnSummonedCreatureAction spawnSummonedCreatureAction = new SpawnSummonedCreatureAction();
            _actionControllers.Add(spawnSummonedCreatureAction.Name, spawnSummonedCreatureAction);

            SwapNodeAction swapNodeAction = new SwapNodeAction();
            _actionControllers.Add(swapNodeAction.Name, swapNodeAction);

            SwitchModelAction switchModelAction = new SwitchModelAction();
            _actionControllers.Add(switchModelAction.Name, switchModelAction);

            TrackingProjectileAction trackingProjectileAction = new TrackingProjectileAction();
            _actionControllers.Add(trackingProjectileAction.Name, trackingProjectileAction);

            SetCustomValueAction setCustomValueAction = new SetCustomValueAction();
            _actionControllers.Add(setCustomValueAction.Name, setCustomValueAction);

            SetTeamValueAction setTeamValueAction = new SetTeamValueAction();
            _actionControllers.Add(setTeamValueAction.Name, setTeamValueAction);
        }

        public void Release()
        {
            RecycleEventDatasNew(false);
            _delayedActionTimers.Clear();
            _actionControllers.Clear();
        }

        public void UpdateAction(DFix64 frameLength)
        {
            for (int i = 0, count = _delayedActionTimers.Count; i < count;)
            {
                if (_delayedActionTimers[i].CreatedFrame == BattleData.LogicFrame)
                {
                    break;
                }

                _delayedActionTimers[i].UpdateLogic(frameLength);
                if (_delayedActionTimers[i].IsExecuted)
                {
                    _delayedActionTimers.RemoveAt(i);
                    count--;
                    continue;
                }

                i++;
            }
        }

        public void AddDelayAction(DFix64 delayTime, KeyValue keyValue, EventData eventData)
        {
            EventData delayEventData = eventData.Clone();
            delayEventData.IsOld = false;

            if (delayEventData.Modifier != null)
            {
                delayEventData.Modifier.DelayCount++;
            }
            _delayedActionTimers.Add(new DelayActionTimer(delayTime, keyValue, delayEventData));
        }

        public void Execute(KeyValue keyValue, EventData eventData)
        {
            if (eventData == null)
            {
                return;
            }

            if (BattleData.State != BattleState.Start)
            {
                return;
            }

            if (keyValue == null || !keyValue.HasChildren)
            {
                return;
            }

            if (_actionControllers.ContainsKey(keyValue.Key))
            {
                _actionControllers[keyValue.Key].Execute(keyValue, eventData);
            }
            else
            {
                Log.Error("找不到操作:{0} 技能:{1} Modifier:{2}", keyValue.Key, eventData.Ability != null ? eventData.Ability.LogName : string.Empty, eventData.Modifier != null ? eventData.Modifier.LogName : string.Empty);
            }
        }

        public void Executes(KeyValue keyValue, EventData eventData)
        {
            if (eventData == null)
            {
                return;
            }

            if (BattleData.State != BattleState.Start)
            {
                return;
            }

            if (keyValue == null || !keyValue.HasChildren)
            {
                return;
            }

            foreach (var child in keyValue.Children)
            {
                Execute(child, eventData);
            }
        }

        public EventData CreateEventData()
        {
            EventData eventData = GameFramework.ReferencePool.Acquire<EventData>();
            _eventDatas.Add(eventData);
            return eventData;
        }

        public void RecycleEventDatas(bool isRelease)
        {
            for (int i = _eventDatas.Count - 1; i >= 0; i--)
            {
                if (isRelease || _eventDatas[i].IsOld)
                {
                    GameFramework.ReferencePool.Release(_eventDatas[i]);
                    _eventDatas.RemoveAt(i);
                }
            }
        }

        public void RecycleEventDatasNew(bool isRelease)
        {
            for (int i = _eventDatas.Count - 1; i >= 0; i--)
            {
                if (isRelease || _eventDatas[i].IsOld)
                {
                    GameFramework.ReferencePool.Release(_eventDatas[i]);
                    _eventDatas.RemoveAt(i);
                }
            }
        }
    }
}