using System.Collections;
using UnityEngine;

public class AutoHideText : MonoBehaviour
{
    [Tooltip("显示多久后自动隐藏（秒）")]
    public float lifeTime = 10f;

    private Coroutine hideRoutine;

    // 物体被启用/SetActive(true) 时调用
    private void OnEnable()
    {
        // 重新启用时重开计时
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
}
