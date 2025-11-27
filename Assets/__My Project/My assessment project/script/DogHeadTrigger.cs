using UnityEngine;

public class DogHeadTrigger : MonoBehaviour
{
    public Animator dogAnimator;
    public string triggerName = "Pet"; // Animator 里你设置的 Trigger 名字

    private void OnTriggerEnter(Collider other)
    {
        // 确认是玩家
        if (other.CompareTag("Player"))
        {
            dogAnimator.SetTrigger(triggerName);
        }
    }
}