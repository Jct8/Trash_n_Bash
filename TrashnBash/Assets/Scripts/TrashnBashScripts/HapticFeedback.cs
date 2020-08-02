using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HapticFeedback : MonoBehaviour
{
    public enum HapticType
    {
        Normal,
        Duration,
        Pattern
    }

    public HapticType hapticType = HapticType.Normal;
    private Button button;
    [Header("Duration Type Only")]
    public long durationInMilliSeconds = 200;
    [Header("Pattern Type Only")]
    public long delay = 0;
    public long patternDurationInMilliseconds = 200;
    public long sleepDurationInMilliseconds = 0;
    public long pattern2DurationInMilliseconds = 0;
    public long sleep2DurationInMilliseconds = 0;
    private int repetitions = -1;

    private static AndroidJavaClass unityPlayer = null;
    private static AndroidJavaObject currentActivity = null;
    private static AndroidJavaObject vibrator = null;
    private static UIbutton uibutton = null;

    private void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
        button = GetComponent<Button>();
        uibutton = GetComponent<UIbutton>();
        long[] parameters = { delay, patternDurationInMilliseconds, sleepDurationInMilliseconds,
            pattern2DurationInMilliseconds, sleep2DurationInMilliseconds };
        if (button && !uibutton)
        {
            switch (hapticType)
            {
                case HapticType.Normal:
                    button.onClick.AddListener(Vibrate);
                    break;
                case HapticType.Duration:
                    button.onClick.AddListener(delegate { Vibrate(durationInMilliSeconds); });
                    break;
                case HapticType.Pattern:
                    button.onClick.AddListener(delegate { Vibrate(parameters, repetitions); });
                    break;
                default:
                    Handheld.Vibrate();
                    break;
            }
        }
    }

    public void Activate()
    {
        long[] parameters = { delay, patternDurationInMilliseconds, sleepDurationInMilliseconds,
            pattern2DurationInMilliseconds, sleep2DurationInMilliseconds };
        switch (hapticType)
        {
            case HapticType.Normal:
                Vibrate();
                break;
            case HapticType.Duration:
                Vibrate(durationInMilliSeconds);
                break;
            case HapticType.Pattern:
                Vibrate(parameters, repetitions);
                break;
            default:
                break;
        }
    }

    public static void Vibrate()
    {
        //Debug.Log("Vibrate");
        if (isAndroid())
            vibrator.Call("vibrate");
    }

    public static void Vibrate(long milliseconds)
    {
       // Debug.Log("Vibrate");

        if (isAndroid())
            vibrator.Call("vibrate", milliseconds);
    }

    public static void Vibrate(long[] pattern, int repeat)
    {
       // Debug.Log("Vibrate");
        if (isAndroid())
            vibrator.Call("vibrate", pattern, repeat);
    }

    private static bool isAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
	return true;
#else
        return false;
#endif
    }
}
