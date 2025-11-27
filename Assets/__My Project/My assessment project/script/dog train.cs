using System.Collections;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    public class UIToast : MonoBehaviour
    {
        public static void Show(string msg, float sec = 1.5f) { Debug.Log(msg); }
    }
    public static class SaveSystem
    {
        public static void SaveAll() { Debug.Log("SaveAll called"); }
    }
    public static TrainingManager I;
    public DogController dog; // 你的 Animator 总控（上一条回复里给过）
    public float energyCostPerTrick = 0.2f;
    public float reactionBaseDelay = 0.5f; // 反应基础时延（会随等级/亲密下降）
    public void TrySit() { }
    public void TryStandUp() { }
    public void TryShake() { }
    public void TrySpin() { }
    public void RunToPlayer() { }
    public void PlayConfused() { }

    void Awake()
    {
        if (I == null) I = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        GameClock.I.OnDayPassed += OnNewDay;
    }

    void OnDestroy() 
    { 
        if (GameClock.I) 
            GameClock.I.OnDayPassed -= OnNewDay; 
    }

    void OnNewDay()
    {
        DogStats.I.DailyDecay();
        SaveSystem.SaveAll();
    }

    // === 成功率：Sigmoid 将“等级/亲密/服从/疲劳/随机”映射到 0-1 ===
    float SuccessProbability(DogTrick trick)
    {
        var stats = DogStats.I;
        var tp = stats.tricks[trick];
        int L = tp.level;
        float bond = stats.bond;
        float obed = stats.obedience;
        float energy = stats.energy;

        // 可调权重（直觉：等级与亲密最关键）
        float z = 0f;
        z += 0.9f * L;           // 等级每级 +0.9
        z += 2.0f * bond;        // 亲密 0-1 → 0~2
        z += 1.5f * obed;        // 服从 0-1 → 0~1.5
        z -= 1.0f * (1f - energy); // 精力低拖后腿
        z += UnityEngine.Random.Range(-0.5f, 0.5f);
        bool ok = UnityEngine.Random.value <= SuccessProbability(trick);

        // Sigmoid：P=1/(1+e^{-z})
        float p = 1f / (1f + Mathf.Exp(-z));
        // 上限/下限，避免“必成/必败”
        return Mathf.Clamp(p, 0.15f, 0.98f);
    }

    float ReactionDelay(DogTrick trick)
    {
        var tp = DogStats.I.tricks[trick];
        float L = tp.level;
        float bond = DogStats.I.bond;
        // 等级/亲密越高，反应越快
        float d = reactionBaseDelay - 0.06f * L - 0.2f * bond;
        return Mathf.Clamp(d, 0.1f, 0.6f);
    }

    public void Train(DogTrick trick)
    {
        // 精力检查
        if (DogStats.I.energy < energyCostPerTrick)
        {
            UIToast.Show("狗狗太累了，先休息一下~");
            return;
        }

        StartCoroutine(TrainRoutine(trick));
    }

    IEnumerator TrainRoutine(DogTrick trick)
    {
        float delay = ReactionDelay(trick);
        yield return new WaitForSeconds(delay);

        bool ok = Random.value <= SuccessProbability(trick);
        if (ok)
        {
            DoAnim(trick); // 成功 → 播对应动画
            GainXP(trick, 1f); // 每次成功给 1xp（可因难度/完美度再加权）
            DogStats.I.energy = Mathf.Clamp01(DogStats.I.energy - energyCostPerTrick);
            UIToast.Show($"成功！{trick} +1 XP");
        }
        else
        {
            // 失败反馈：困惑/嗅闻短动画（可做一个“Fail”状态）
            dog.PlayConfused();
            DogStats.I.energy = Mathf.Clamp01(DogStats.I.energy - energyCostPerTrick * 0.5f);
            UIToast.Show("这次没成功，再来一次！");
        }

        DogStats.I.tricks[trick].lastTrained = GameClock.I.Now;
        SaveSystem.SaveAll();
    }

    void DoAnim(DogTrick trick)
    {
        switch (trick)
        {
            case DogTrick.Sit: dog.TrySit(); break;
            case DogTrick.Stand: dog.TryStandUp(); break;
            case DogTrick.Shake: dog.TryShake(); break;
            case DogTrick.Spin: dog.TrySpin(); break;
            case DogTrick.RunToPlayer: dog.RunToPlayer(); break;
        }
    }

    void GainXP(DogTrick trick, float amount)
    {
        var tp = DogStats.I.tricks[trick];
        tp.xp += amount;

        // 把“越练越熟”的感觉做出来：一次训练多次成功也能叠 xp
        int need = DogStats.I.XpNeedForNextLevel(tp.level);
        if (tp.xp >= need)
        {
            tp.level += 1;
            tp.xp -= need;
            UIToast.Show($"{trick} 升到 Lv.{tp.level}！");
            // 升级时略增服从/亲密，正向循环
            DogStats.I.obedience = Mathf.Clamp01(DogStats.I.obedience + 0.02f);
            DogStats.I.bond = Mathf.Clamp01(DogStats.I.bond + 0.03f);
        }
    }
}