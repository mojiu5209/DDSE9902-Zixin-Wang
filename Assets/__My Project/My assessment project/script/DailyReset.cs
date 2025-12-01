using UnityEngine;

public class DailyReset : MonoBehaviour
{
    public Transform respawnPoint;      // ��������Ź�����Ǹ�λ��

    private Movable movable;
    private Rigidbody rb;

    private void Awake()
    {
        movable = GetComponent<Movable>();
        rb = GetComponent<Rigidbody>();
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

    // ÿ��һ�����һ��
    private void OnDayPassed()
    {
        ResetNow();
    }

    // Ҳ�����ڱ�ĵط��ֶ�����
    public void ResetNow()
    {
        // 1. ������ڱ�������ס�����ô�������
        if (movable != null && movable.myMagnetSnapper != null)
        {
            movable.myMagnetSnapper.SoftReleaseSubject();
        }

        // 2. ȡ�����ӹ�ϵ����ֹ������ SnappingPoint ���棩
        transform.SetParent(null);

        // 3. ���ø���״̬
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 4. ���͵�ָ��������
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }

        Debug.Log($"[DailyReset] {name} reset to {transform.position}");
    }
}
