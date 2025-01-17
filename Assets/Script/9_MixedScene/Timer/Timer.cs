﻿using Sirenix.OdinInspector;
using UnityEngine;
/// <summary>
/// 用户操作限时控制类,与硬币合并
/// </summary>
public class Timer : MonoBehaviour
{
    static bool isTimerStart;
    public static int limitTime { get; set; }
    public static float time { get; set; }
    public static bool isTimeout => time > limitTime;
    [ShowInInspector]
    public static int Process => Mathf.Clamp((int)(time / limitTime * 360), 0, 360);
    public static void SetIsTimerStart(int limit_time)
    {
        time = 0;
        isTimerStart = true;
        limitTime = limit_time;
    }
    public static void SetIsTimerClose()
    {
        time = 0;
        isTimerStart = false;
    }
    void Update()
    {
        if (isTimerStart)
        {
            time += Time.deltaTime;
        }
    }
}
