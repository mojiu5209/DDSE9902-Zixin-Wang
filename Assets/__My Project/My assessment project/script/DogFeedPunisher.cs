using UnityEngine;

public class DogFeedPunisher : MonoBehaviour
{
    public GameObject dropPrefab;    // 砸家具的物体 Prefab
    public Transform dropPoint;      // 掉落位置（在家具上方）

    // 这一“天”内是否喂过狗
    private bool fedToday = false;

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
    /// 喂食成功时调用（比如在磁吸 onSnap 或 FoodBowlTrigger 里调用）
    /// </summary>
    public void OnItemSnapped()
    {
        fedToday = true;
        Debug.Log("【DogFeedPunisher】记录：今天已经喂过狗了");
    }

    /// <summary>
    /// 每跨一天由 GameClock 调用
    /// </summary>
    private void OnDayPassed()
    {
        Debug.Log($"【DogFeedPunisher】新的一天到来，上一天 fedToday = {fedToday}");

        // 如果上一天完全没喂过狗，就触发惩罚
        if (!fedToday)
        {
            TriggerDrop();
        }

        // 进入新的一天，重新开始计数
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

        Debug.Log("【DogFeedPunisher】实例化 damage —— 上一天没喂狗，触发惩罚");
        Instantiate(dropPrefab, pos, rot);
    }
}
