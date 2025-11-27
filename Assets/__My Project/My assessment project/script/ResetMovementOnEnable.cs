using UnityEngine;
public class ResetMovementsOnEnable : MonoBehaviour
{
    int move = 1;
    void OnEnable()
    {
        var anim = GetComponent<Animator>();
        if (anim) anim.SetInteger("Movements Type", move); // idle at spawn
    }
    void OnDisable()
    {
        var anim = GetComponent<Animator>();
        if (anim) anim.SetInteger("Movements Type", 0);
    }
}