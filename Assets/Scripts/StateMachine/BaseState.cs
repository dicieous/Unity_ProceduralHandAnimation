using UnityEngine;
using System;
using Unity.VisualScripting;

public abstract class BaseState<StateType> where StateType : Enum
{
    public StateType State { get; private set; }
    
    public BaseState(StateType key)
    {
        State = key;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract StateType GetNextState();
    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerStay(Collider other);
    public abstract void OnTriggerExit(Collider other);
    
}
