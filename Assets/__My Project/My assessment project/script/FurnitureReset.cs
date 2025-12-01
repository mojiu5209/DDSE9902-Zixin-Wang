using UnityEngine;

public class FurnitureReset : MonoBehaviour
{
    private Vector3 initialPos;
    private Quaternion initialRot;
    private Rigidbody rb;

    void Awake()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    public void ResetFurniture()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;   // �ȹ�һ������
        }

        transform.position = initialPos;
        transform.rotation = initialRot;

        if (rb != null)
        {
            rb.isKinematic = false;  // ���������ٱ�����Ӱ�죬���Ա��� true
        }
    }
}
