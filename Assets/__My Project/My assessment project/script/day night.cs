// DayNightCycle.cs
using System;
using UnityEngine;

[ExecuteAlways]
public class DayNightCycle : MonoBehaviour
{
    [Header("References")]
    public Light sun;                      // 场景里的 Directional Light（作为太阳）
    public Material skyboxMat;             // 可选：你的天空盒材质（Procedural/其他）

    [Header("Day Params")]
    [Range(0, 24)] public float sunriseHour = 6f;
    [Range(0, 24)] public float sunsetHour = 19f;
    public Vector3 sunAxis = new Vector3(1, 0, 0); // 太阳绕哪个轴旋转（默认绕X）

    [Header("Lighting Curves (t = 0..1 当天进度)")]
    public Gradient sunColor;              // 太阳光颜色（随时间）
    public AnimationCurve sunIntensity =   // 太阳强度（随时间）
        AnimationCurve.EaseInOut(0f, 0f, 0.25f, 1f); // 你可以在Inspector里调
    public Gradient ambientColor;          // 环境光颜色
    public AnimationCurve skyboxExposure = // 天空盒曝光
        AnimationCurve.EaseInOut(0f, 0.3f, 1f, 1.2f);
    public Gradient fogColor;              // 雾颜色（可选）
    public AnimationCurve fogDensity =     // 雾密度（可选）
        AnimationCurve.EaseInOut(0f, 0.005f, 1f, 0.01f);

    void Reset()
    {
        // 默认渐变（可在Inspector里再调）
        sunColor = new Gradient
        {
            colorKeys = new[]{
                new GradientColorKey(new Color(0.9f,0.4f,0.2f), 0f),   // 清晨偏橙
                new GradientColorKey(Color.white, 0.3f),              // 白天
                new GradientColorKey(Color.white, 0.7f),
                new GradientColorKey(new Color(0.9f,0.4f,0.2f), 1f),   // 黄昏
            }
        };
        ambientColor = new Gradient
        {
            colorKeys = new[]{
                new GradientColorKey(new Color(0.05f,0.08f,0.1f), 0f), // 夜
                new GradientColorKey(new Color(0.5f,0.6f,0.7f), 0.5f), // 白天
                new GradientColorKey(new Color(0.05f,0.08f,0.1f), 1f), // 夜
            }
        };
        fogColor = ambientColor;
    }

    void Update()
    {
        float t = GetDayT();                     // 0..1
        DriveSun(t);
        DriveEnvironment(t);
    }

    float GetDayT()
    {
        // 从 GameClock 取当天分钟 → 0..1
        DateTime now = (GameClock.I != null) ? GameClock.I.Now : DateTime.Now;
        float minutes = (float)now.TimeOfDay.TotalMinutes; // 0..1440
        return minutes / 1440f;
    }

    void DriveSun(float t)
    {
        if (!sun) return;

        // 太阳角度：一天 360 度（正午最高），你也可以加偏移修正
        float angle = t * 360f - 90f; // 让 t=0.25(6点) 接近地平线
        sun.transform.rotation = Quaternion.AngleAxis(angle, sunAxis);

        // 强度 & 颜色
        sun.color = sunColor.Evaluate(t);

        // 白天/夜晚强度（基于日出日落时段做个包络）
        float dayFactor = DaylightFactor(t);                     // 0..1
        float baseIntensity = sunIntensity.Evaluate(t);          // 曲线权重
        sun.intensity = Mathf.Max(0f, baseIntensity) * dayFactor;
        sun.enabled = sun.intensity > 0.02f;                   // 夜里关灯
    }

    float DaylightFactor(float t)
    {
        float sr = sunriseHour / 24f;
        float ss = sunsetHour / 24f;

        // 早晚做个平滑过渡（30 分钟的缓冲）
        float fade = 30f / 1440f;

        float up = Mathf.InverseLerp(sr - fade, sr + fade, t); // 0->1
        float down = 1f - Mathf.InverseLerp(ss - fade, ss + fade, t);
        // 白天取1，夜晚取0，日出/日落在 0..1 之间平滑
        return Mathf.Clamp01(Mathf.Min(up, down) * 2f); // 简单合成
    }

    void DriveEnvironment(float t)
    {
        // 环境光
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor.Evaluate(t);

        // 天空盒曝光（如果有 Procedural 或自定义天空盒）
        if (skyboxMat && skyboxMat.HasProperty("_Exposure"))
        {
            skyboxMat.SetFloat("_Exposure", skyboxExposure.Evaluate(t));
            RenderSettings.skybox = skyboxMat;
            DynamicGI.UpdateEnvironment(); // 让反射探针/环境更新
        }

        // 雾
        if (RenderSettings.fog)
        {
            RenderSettings.fogColor = fogColor.Evaluate(t);
            RenderSettings.fogDensity = fogDensity.Evaluate(t);
        }
    }
}
