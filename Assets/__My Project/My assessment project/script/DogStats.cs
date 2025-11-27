using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class TrickProgress
{
    public int level = 0;          // 技巧等级
    public float xp = 0f;          // 当前等级经验
    public DateTime lastTrained;   // 上次训练日期（用于衰减）
}

public enum DogTrick { Sit, Stand, Shake, Spin, RunToPlayer }

public class DogStats : MonoBehaviour
{
    public static DogStats I;

    [Header("核心属性(0-1)")]
    [Range(0, 1)] public float obedience = 0.5f; // 服从/理解指令
    [Range(0, 1)] public float bond = 0.4f;      // 亲密/信任
    [Range(0, 1)] public float energy = 1.0f;    // 精力（随时间恢复；训练消耗）

    [Header("成长曲线参数")]
    public AnimationCurve xpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 等级经验曲线
    public int xpPerLevelBase = 5; // 升级所需经验： xpNeed = xpPerLevelBase * (1 + level)^1.2

    [Header("每个技巧的进度")]
    public Dictionary<DogTrick, TrickProgress> tricks = new();

    void Awake()
    {
        if (I == null) I = this; else Destroy(gameObject);
        foreach (DogTrick t in System.Enum.GetValues(typeof(DogTrick)))
            tricks[t] = new TrickProgress() { level = 0, xp = 0, lastTrained = GameClock.I.Now };
        DontDestroyOnLoad(gameObject);
    }

    public int XpNeedForNextLevel(int currentLevel)
    {
        float need = xpPerLevelBase * Mathf.Pow(1f + currentLevel, 1.2f);
        return Mathf.CeilToInt(need);
    }

    // 每日衰减（没训练就稍微“生疏”）
    public void DailyDecay()
    {
        foreach (var kv in tricks)
        {
            var tp = kv.Value;
            int days = Mathf.Max(0, (int)(GameClock.I.Now.Date - tp.lastTrained.Date).TotalDays);
            if (days > 0 && tp.level > 0)
            {
                // 轻度衰减：每缺训一天 -0.25xp；隔很多天也不会直接掉等级
                tp.xp = Mathf.Max(0, tp.xp - 0.25f * days);

                // 如果长期不练且 xp=0，偶尔掉 1 级（给可见的“退步”）
                if (days >= 5 && tp.xp <= 0 && tp.level > 0)
                    tp.level -= 1;
            }
        }

        // 亲密与服从随规律训练微增
        bond = Mathf.Clamp01(bond + 0.01f);
        obedience = Mathf.Clamp01(obedience + 0.005f);

        // 精力每日重置/上限回满一些
        energy = Mathf.Clamp01(energy + 0.5f);
    }
}