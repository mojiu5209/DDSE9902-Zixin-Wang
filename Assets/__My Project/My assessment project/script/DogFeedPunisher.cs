using UnityEngine;

public class DogFeedPunisher : MonoBehaviour
{
    public GameObject dropPrefab;    // 砸家具的物体 Prefab
    public Transform dropPoint;      // 掉落位置（在家具上方）

    private bool fedToday = false;   // 今天有没有被吸附（喂过）
    private int daysSinceStart = 0;  // 从游戏开始已经过了多少天

    private void Start()
    {
        // 第一天默认算喂过，避免刚开局就惩罚
        fedToday = true;
    }

    private void OnEnable()
    {
        if (GameClock.I != null)
            GameClock.I.OnDayPassed += OnDayPassed;
    }

    private void OnDisable()
    {
        if (GameClock.I != null)
            GameClock.I.OnDayPassed -= OnDayPassed;
    }

    /// <summary>
    /// 有物品被 Magnet Snap 吸附时，在 onSnap 事件里调用
    /// </summary>
    public void OnItemSnapped()
    {
        fedToday = true;
        // Debug.Log("今天有东西被吸附，算喂过狗了");
    }

    private void OnDayPassed()
    {
        daysSinceStart++;

        // 第一天结束只开始计数，不惩罚
        if (daysSinceStart == 1)
        {
            fedToday = false;  // 从第二天开始正式判断
            return;
        }

        // 第二天开始，如果当天完全没喂，就砸一次
        if (!fedToday)
        {
            TriggerDrop();
        }

        // 新的一天重置
        fedToday = false;
    }

    private void TriggerDrop()
    {
        if (dropPrefab == null)
        {
            Debug.LogWarning("DogFeedPunisher: dropPrefab 没设置！");
            return;
        }

        Vector3 pos = dropPoint ? dropPoint.position : transform.position;
        Quaternion rot = dropPoint ? dropPoint.rotation : Quaternion.identity;

        Debug.Log("【DogFeedPunisher】实例化 damage，一天没喂狗触发惩罚");
        Instantiate(dropPrefab, pos, rot);
    }
}
