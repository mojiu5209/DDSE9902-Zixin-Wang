using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DogLiftTeleport : MonoBehaviour
{
    public NavMeshAgent dogAgent;      // 拖 indoor dog 身上的 NavMeshAgent 进来

    public Transform topPoint;         // 二楼狗狗出现的位置

    public float delay = 3f;           // 默认 3 秒

    private Coroutine tpRoutine;

    // 狗进入这块区域时开始计时
    private void OnTriggerEnter(Collider other)
    {
        if (dogAgent == null || topPoint == null) return;

        // 只认这一只狗：碰到的东西必须是狗的子物体
        if (!other.transform.IsChildOf(dogAgent.transform)) return;

        // 已经在计时就先停掉
        if (tpRoutine != null)
            StopCoroutine(tpRoutine);

        tpRoutine = StartCoroutine(TeleportAfterDelay());
        // Debug.Log("狗进入传送区，开始计时...");
    }

    // 狗离开区域就取消传送
    private void OnTriggerExit(Collider other)
    {
        if (dogAgent == null) return;
        if (!other.transform.IsChildOf(dogAgent.transform)) return;

        if (tpRoutine != null)
        {
            StopCoroutine(tpRoutine);
            tpRoutine = null;
            // Debug.Log("狗离开传送区，取消传送");
        }
    }

    private IEnumerator TeleportAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        // 3 秒后还在区域内就传送
        dogAgent.Warp(topPoint.position);

        tpRoutine = null;
        // Debug.Log("狗已传送到二楼");
    }
}