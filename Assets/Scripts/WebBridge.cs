using UnityEngine;
using System.Runtime.InteropServices;

public static class WebBridge
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void WebBridge_SendScore(int score);
#else
    private static void WebBridge_SendScore(int score) =>
        Debug.Log($"[Editor stub] Would send score={score}");
#endif

    public static void SendScore(int score) => WebBridge_SendScore(score);

    // Optional: only send when it changes
    private static int? _lastScore;
    public static void SendScoreIfChanged(int score)
    {
        if (_lastScore != score)
        {
            _lastScore = score;
            SendScore(score);
        }
    }
}
