using UnityEngine;

public class TouchState : EnvironmentInteractionState
{
    public float elapsedTime = 0.0f;
    public float resetThreshold = 1f;
    public TouchState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironmentInteractionState state)
        : base(context, state)
    {
        EnvironmentInteractionContext Context = context;
    }

    public override void EnterState()
    {
        elapsedTime = 0.0f;
    }

    public override void UpdateState()
    {
        elapsedTime += Time.deltaTime;
    }
    public  override void ExitState() {}

    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        if (elapsedTime > resetThreshold || CheckShouldReset())
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
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
