using UnityEngine;

public class SearchState : EnvironmentInteractionState
{
    public float approachDistanceThreshold = 2.0f;

    public SearchState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironmentInteractionState state)
        : base(context, state)
    {
        EnvironmentInteractionContext Context = context;
    }

    public override void EnterState()
    {
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
    }

    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        if (CheckShouldReset()) return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        
        bool isCloseToTarget = Vector3.Distance(Context.ClosestPointOnColliderFromShoulder,
            Context.RootTransform.position) < approachDistanceThreshold;

        bool isClosestPointOnColliderValid = Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;
        if ((isClosestPointOnColliderValid && isCloseToTarget))
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Approach;
        }

        return State;
    }

    public override void OnTriggerEnter(Collider other)
    {
        StartIKTargetPositionTracking(other);
    }
    
    public override void OnTriggerStay(Collider other)
    {
        UpdateIKTargetPosition(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        ResetIKTargetPositionTracking(other);
    }
}