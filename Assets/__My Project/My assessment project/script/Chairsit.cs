using UnityEngine;

public class ChairSitButton : MonoBehaviour
{
    public Transform sitPoint;          // 椅子前面/上面的坐姿点

    public Transform playerRoot;        // EZPZ_Player Type A 拖进来

    private bool isSitting = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // 按钮按下时调用这个函数
    public void OnButtonPressed()
    {
        if (playerRoot == null || sitPoint == null)
        {
            Debug.LogWarning("ChairSitButton: playerRoot 或 sitPoint 没有设置！");
            return;
        }

        if (!isSitting)
            SitDown();
        else
            StandUp();
    }

    void SitDown()
    {
        originalPosition = playerRoot.position;
        originalRotation = playerRoot.rotation;

        playerRoot.position = sitPoint.position;
        playerRoot.rotation = sitPoint.rotation;

        // 如果想再稍微降低一点视角，比如 0.15 米：
        // playerRoot.position += Vector3.down * 0.15f;

        isSitting = true;
    }

    void StandUp()
    {
        playerRoot.position = originalPosition;
        playerRoot.rotation = originalRotation;

        isSitting = false;
    }
}
