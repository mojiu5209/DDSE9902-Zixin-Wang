using System;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    public static GameClock I;

    public int startYear = 2025, startMonth = 1, startDay = 1;
    public float minutesPerRealSecond = 10f;
    public int dayStartHour = 8;

    public DateTime Now;
    public event Action OnDayPassed;

    float _accMinutes;

    void Awake()
    {
        if (I == null) I = this; else { Destroy(gameObject); return; }
        Now = new DateTime(startYear, startMonth, startDay, dayStartHour, 0, 0);
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        float addMinutes = minutesPerRealSecond * Time.deltaTime;
        _accMinutes += addMinutes;
        while (_accMinutes >= 1f)
        {
            _accMinutes -= 1f;
            AdvanceMinutes(1);
        }
    }

    public void AdvanceMinutes(int mins)
    {
        DateTime before = Now;
        Now = Now.AddMinutes(mins);
        if (before.Date != Now.Date)
            OnDayPassed?.Invoke();
    }

    public void SkipToNextMorning()
    {
        DateTime next = new DateTime(Now.Year, Now.Month, Now.Day, dayStartHour, 0, 0);
        if (Now.Hour >= dayStartHour) next = next.AddDays(1);
        if (next > Now)
        {
            DateTime d = Now;
            while (d.Date < next.Date)
            {
                d = d.AddDays(1);
                OnDayPassed?.Invoke();
            }
            Now = next;
        }
    }
}