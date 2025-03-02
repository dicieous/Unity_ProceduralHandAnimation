using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<StateType> : MonoBehaviour where StateType : Enum
{
   protected Dictionary<StateType, BaseState<StateType>> StatesDict = new Dictionary<StateType, BaseState<StateType>>();

   protected BaseState<StateType> CurrentState;

   private bool isTransitioning = false;
   private void Start()
   {
      CurrentState.EnterState();
   }

   private void Update()
   {
      StateType nextState = CurrentState.GetNextState();
      if (!isTransitioning && nextState.Equals(CurrentState.State))
      {
         CurrentState.UpdateState();
      }
      else if(!isTransitioning)
      {
         TransitionState(nextState);
      }
   }

   private void TransitionState(StateType state)
   {
      isTransitioning = true;
      CurrentState.ExitState();
      CurrentState = StatesDict[state];
      CurrentState.EnterState();
      isTransitioning = false;
   }

   private void OnTriggerEnter(Collider other)
   {
      CurrentState.OnTriggerEnter(other);
   }
   
   private void OnTriggerStay(Collider other)
   {
      CurrentState.OnTriggerStay(other);
   }
   
   private void OnTriggerExit(Collider other)
   {
      CurrentState.OnTriggerExit(other);
   }
}
