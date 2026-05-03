using UnityEngine;

public static class HapticService
{
    // Single short pulse on tube select
    public static void PlaySelect()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#elif UNITY_ANDROID && !UNITY_EDITOR
        Vibrate(new long[] { 0, 60 });
#endif
    }

    // ~1s win pattern: two short taps + long pulse
    public static void PlayWin()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#elif UNITY_ANDROID && !UNITY_EDITOR
        Vibrate(new long[] { 0, 80, 50, 80, 50, 600 });
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static void Vibrate(long[] pattern)
    {
        using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        using var vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        if (vibrator == null) return;

        using var buildVersion = new AndroidJavaClass("android.os.Build$VERSION");
        if (buildVersion.GetStatic<int>("SDK_INT") >= 26)
        {
            using var effectClass = new AndroidJavaClass("android.os.VibrationEffect");
            using var effect = effectClass.CallStatic<AndroidJavaObject>("createWaveform", pattern, -1);
            vibrator.Call("vibrate", effect);
        }
        else
        {
            vibrator.Call("vibrate", pattern, -1);
        }
    }
#endif
}
