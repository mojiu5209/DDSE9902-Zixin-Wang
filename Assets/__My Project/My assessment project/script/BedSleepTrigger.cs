using System.Collections;
using UnityEngine;

public class BedSleepTrigger : MonoBehaviour
{
    public string playerTag = "Player";   // 记得给玩家物体设这个 Tag
    public bool onlyOnce = true;          // 只睡一次？

    public CanvasGroup fadeCanvas;        // 全屏黑幕的 CanvasGroup
    public float fadeDuration = 1f;       // 黑 / 亮 渐变时间
    public float blackHoldTime = 0.5f;    // 全黑停留多久

    private bool playerInside = false;
    private bool hasSlept = false;
    private bool isSleeping = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = false;
    }

    /// <summary>
    /// “睡觉按钮”按下时调用这个函数
    /// </summary>
    public void OnSleepButtonPressed()
    {
        // 人必须在床的触发区里，且没睡过 / 没在睡觉中
        if (!playerInside) return;
        if (isSleeping) return;

        StartCoroutine(SleepSequence());
    }

    private IEnumerator SleepSequence()
    {
        isSleeping = true;

        // 渐黑
        if (fadeCanvas != null)
            yield return StartCoroutine(Fade(0f, 1f));

        // 跳到第二天早上 8 点
        if (GameClock.I != null)
        {
            GameClock.I.SkipToNextMorning8AM();
        }
        else
        {
            Debug.LogWarning("BedSleepTrigger: 没找到 GameClock.I！");
        }

        // 保持一小会儿全黑
        yield return new WaitForSeconds(blackHoldTime);

        // 渐亮
        if (fadeCanvas != null)
            yield return StartCoroutine(Fade(1f, 0f));

        hasSlept = true;
        isSleeping = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeCanvas == null) yield break;

        float t = 0f;
        fadeCanvas.blocksRaycasts = true;   // 防止点到后面的按钮

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / fadeDuration);
            fadeCanvas.alpha = Mathf.Lerp(from, to, lerp);
            yield return null;
        }

        fadeCanvas.alpha = to;

        // 完全透明时就不挡点击
        if (Mathf.Approximately(to, 0f))
            fadeCanvas.blocksRaycasts = false;
    }
}
