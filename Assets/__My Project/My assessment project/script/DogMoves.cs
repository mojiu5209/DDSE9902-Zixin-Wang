using UnityEngine;

public class DogMoves : MonoBehaviour
{
    [Header("Target to control")]
    public string movementString = "Movements Type";
    [SerializeField] private Animator model;   // model
    [SerializeField] private int DogMoveType = 1;
    //[SerializeField] private Transform anchor;

    public void Play()   // shows up in Button OnClick()
    {



        Debug.Log("SDFSDF£»" + DogMoveType);

        // Then start movements
        model.SetInteger(movementString, DogMoveType);
        
    }

    public void Stop()   // optional "stop" button
    {

        model.SetInteger(movementString, 0);
    }
}
