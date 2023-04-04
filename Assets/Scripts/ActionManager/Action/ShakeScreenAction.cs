using KVLib;
using DG.Tweening;


namespace LiaoZhai.Runtime
{
    // 屏幕震动
    /*
        "Duration"					The duration of the tween
	    "Strength"					The shake strength on each axis
	    "Vibrato"					Indicates how much will the shake vibrate
	    "Randomness"				Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware). Setting it to 0 will shake along a single direction.
	    "Snapping"					If 1 the tween will smoothly snap all values to integers
	    "FadeOut"					If 1 the shake will automatically fadeOut smoothly within the tween's duration,
    */
    public class ShakeScreenAction : BaseAction
    {
        public override string Name
        {
            get
            {
                return "ShakeScreen";
            }
        }


        public override void Execute(KeyValue kv, EventData eventData)
        {
            base.Execute(kv, eventData);

            KeyValue kvDuration = kv["Duration"];
            KeyValue kvStrength = kv["Strength"];
            KeyValue kvVibrato = kv["Vibrato"];
            KeyValue kvRandomness = kv["Randomness"];
            KeyValue kvSnapping = kv["Snapping"];
            KeyValue kvFadeOut = kv["FadeOut"];

            if (kvDuration == null)
            {
                Log.Error("ShakeScreen: 缺少参数:Duration");
                return;
            }

            if (kvStrength == null)
            {
                Log.Error("ShakeScreen: 缺少参数:Strength");
                return;
            }

            float duration = (float)BattleData.ParseDFix64(kvDuration.GetString());
            if (duration <= 0f)
            {
                Log.Error("ShakeScreen: Duration小于等于0");
                return;
            }

            float strength = (float)BattleData.ParseDFix64(kvStrength.GetString());
            if (strength <= 0f)
            {
                Log.Error("ShakeScreen: Strength小于等于0");
                return;
            }

            int vibrate = 10;
            if (kvVibrato != null)
            {
                vibrate = BattleData.ParseInt(kvVibrato.GetString());
            }

            float randomness = 90f;
            if (kvRandomness != null)
            {
                randomness = (float)BattleData.ParseDFix64(kvRandomness.GetString());
            }

            bool snapping = false;
            if (kvSnapping != null)
            {
                snapping = BattleData.ParseBool01(kvSnapping.GetString());
            }

            bool fadeOut = true;
            if (kvFadeOut != null)
            {
                fadeOut = BattleData.ParseBool01(kvFadeOut.GetString());
            }

#if UNITY_EDITOR
            if (BattleData.TestQuickBattle)
            {
                return;
            }

            return;
#endif
            BattleData.Camera.transform.DOShakePosition(duration, strength, vibrate, randomness, snapping, fadeOut).SetAutoKill(true);
        }
    }
}