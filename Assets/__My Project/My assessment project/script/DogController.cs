using UnityEngine;
using System.Collections;

public class DogController : MonoBehaviour
{
    [Header("Animator & Run-to-player")]
    public Animator anim;
    public Transform player;      // 让狗跑向的目标（可拖拽玩家或空物体）
    public float runSpeed = 2.0f;
    public float stopDistance = 0.6f;

    Coroutine _runCo;

    // ====== TrainingManager 调用的 6 个方法（名字要一模一样） ======

    public void TrySit()
    {
        if (anim) { anim.SetBool("Sit", true); }
    }

    public void TryStandUp()
    {
        if (anim)
        {
            anim.SetBool("Sit", false);
            anim.SetTrigger("StandUp");
        }
    }

    public void TryShake()
    {
        if (anim) anim.SetTrigger("Shake");
    }

    public void TrySpin()
    {
        if (anim) anim.SetTrigger("Spin");
    }

    public void RunToPlayer()
    {
        if (player == null) return;
        if (_runCo != null) StopCoroutine(_runCo);
        _runCo = StartCoroutine(RunTo(player.position));
    }

    public void PlayConfused()
    {
        if (anim) anim.SetTrigger("Confused");
    }

    // ====== 简单的跑向目标协程，占位实现 ======
    IEnumerator RunTo(Vector3 target)
    {
        if (anim) anim.SetFloat("Speed", 1f);
        while (Vector3.Distance(transform.position, target) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, runSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(target.x, transform.position.y, target.z)); // 水平朝向
            yield return null;
        }
        if (anim) anim.SetFloat("Speed", 0f);
        _runCo = null;
    }
}