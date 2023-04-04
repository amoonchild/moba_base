using KVLib;
using GameFramework.Sound;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    // 播放音效
    /*
        "Target"                ""      目标
        "SoundName"             ""      音效资源名
        "AttachPointName"       ""      音效挂点
        "VolumeInSoundGroup"    "1"     在音效组内的音量
        "Loop"                  "0"     是否循环
        "Time"                  "0"     起始时间
        "Pitch"                 "1"     音调
        "FadeInSeconds"         "0"     淡入时间
    */
    public class FireSoundAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "FireSound";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

#if UNITY_EDITOR
            if (BattleData.TestQuickBattle || BattleData.TestBattleNoParticle)
            {
                return;
            }
#endif

            KeyValue kvTarget = kv["Target"];
            KeyValue kvSoundName = kv["SoundName"];

            if (kvTarget == null)
            {
                Log.Error("FireSound: 缺少参数:Target");
                return;
            }

            if (kvSoundName == null)
            {
                Log.Error("FireSound: 缺少参数:SoundName");
                return;
            }

            LinkedList<BattleTarget> targets = FindTargets(kvTarget, eventData);
            if (targets == null)
            {
                Log.Warning("FireSound: 找不到目标");
                return;
            }

            PlaySoundParams parms = new PlaySoundParams();

            KeyValue kvAttachPointName = kv["AttachPointName"];
            KeyValue kvVolumeInSoundGroup = kv["VolumeInSoundGroup"];
            KeyValue kvLoop = kv["Loop"];
            KeyValue kvTime = kv["Time"];
            KeyValue kvPitch = kv["Pitch"];
            KeyValue kvFadeInSeconds = kv["FadeInSeconds"];

            if (kvVolumeInSoundGroup != null)
            {
                parms.VolumeInSoundGroup = (float)BattleData.ParseDFix64(kvVolumeInSoundGroup.GetString());
            }
            else
            {
                parms.VolumeInSoundGroup = 1f;
            }

            parms.VolumeInSoundGroup = parms.VolumeInSoundGroup * BattleData.GetDefaultVolume();

            if (kvLoop != null)
            {
                parms.Loop = BattleData.ParseBool01(kvLoop.GetString());
            }
            else
            {
                parms.Loop = false;
            }

            if (kvTime != null)
            {
                parms.Time = (float)BattleData.ParseDFix64(kvTime.GetString());
            }
            else
            {
                parms.Time = 0f;
            }

            if (kvPitch != null)
            {
                parms.Pitch = (float)BattleData.ParseDFix64(kvPitch.GetString());
            }
            else
            {
                parms.Pitch = 1f;
            }

            if (kvFadeInSeconds != null)
            {
                parms.FadeInSeconds = (float)BattleData.ParseDFix64(kvFadeInSeconds.GetString());
            }
            else
            {
                parms.FadeInSeconds = 0f;
            }

            parms.SpatialBlend = 1f;

            int soundId = 0;

            bool playAll = false;
            if (kv["PlayAll"] != null)
            {
                playAll = kv["PlayAll"].GetInt() != 0;
            }

            LinkedListNode<BattleTarget> first = targets.First;
            while (first != null)
            {
                DFixVector3 point = DFixVector3.Zero;

                if (first.Value.Type == BattleTargetType.UNIT)
                {
                    BaseUnit attachUnitTarget = (BaseUnit)first.Value.Target;
                    if (kvAttachPointName != null && !string.IsNullOrEmpty(kvAttachPointName.GetString()))
                    {
                        point = attachUnitTarget.LogicPosition;
                    }
                    else
                    {
                        point = attachUnitTarget.LogicPosition;
                    }
                }
                else if (first.Value.Type == BattleTargetType.POINT)
                {
                    point = (DFixVector3)first.Value.Target;
                }
                else if (first.Value.Type == BattleTargetType.NODE)
                {
                    point = ((BattleNode)first.Value.Target).WorldPosition;
                }

                if (soundId == 0 || playAll)
                {
                    soundId = GameManager.Sound.PlaySound(kvSoundName.GetString(), RT.Constant.SoundGroup.Sound, 0, parms, point.ToVector3(), null);
                }

                first = first.Next;
            }
        }
    }
}