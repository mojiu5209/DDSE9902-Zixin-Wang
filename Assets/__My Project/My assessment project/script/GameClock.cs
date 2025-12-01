using System;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    public static GameClock I;

    // 当前游戏时间
    public DateTime Now = new DateTime(2025, 1, 1, 8, 0, 0);

    public float timeScale = 60f;  // 1秒现实 = 60秒游戏，按你自己需要

    // === 新增：跨天事件 ===
    public event Action OnDayPassed;

    private DateTime lastDate;

    void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);   // 如果你有多场景切换可以加这一句
        }
        else if (I != this)
        {
            Destroy(gameObject);
            return;
        }

        if (timeScale <= 0f) timeScale = 60f;
        lastDate = Now.Date;
    }

    void Update()
    {
        Now = Now.AddSeconds(Time.deltaTime * timeScale);
        // 调试看看
        // Debug.Log(Now);
        CheckDayPassed();
    }

    // 检查是否跨天
    void CheckDayPassed()
    {
        DateTime currentDate = Now.Date;

        if (currentDate > lastDate)      // 说明过了新的一天
        {
            lastDate = currentDate;

            // 触发事件（狗狗那边 TrainingManager 订阅的 OnNewDay 会被调用）
            OnDayPassed?.Invoke();
        }
    }

    // 睡到第二天早上 8 点
    public void SkipToNextMorning8AM()
    {
        DateTime oldDate = Now.Date;

        DateTime target = oldDate.AddDays(1).AddHours(8);
        Now = target;

        // 手动触发一次“跨天”
        if (target.Date > oldDate)
        {
            lastDate = target.Date;
            OnDayPassed?.Invoke();
        }

        Debug.Log("SkipToNextMorning8AM -> " + Now);
    }
}
