using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public IState currentState;

    public void ChangeState(IState state)
    {
        if (currentState != null)
            currentState.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void StateUpdate()
    {
        if (currentState != null) currentState.Execute();
    }
}
