using UnityEngine;

public class ApproachState : EnvironmentInteractionState
{

    private float elapsedTime = 0.0f;
    private float lerpDuration = 10.0f;
    private float approachDuration = 2.0f;
    private float desiredWeight = 0.5f;
    private float desiredRotationWeight = 0.75f;
    private float rotationSpeed = 500f;
    private float riseDistanceThreshold = 0.5f;
    
    public ApproachState(EnvironmentInteractionContext context,
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
        //Create a quaternion with Z-axis pointing downwards to the ground
        Quaternion expectedGroundRotation = Quaternion.LookRotation(-Vector3.up, Context.RootTransform.forward);
        elapsedTime += Time.deltaTime;

        Context.CurrentIKTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIKTargetTransform.rotation,
            expectedGroundRotation, rotationSpeed * Time.deltaTime);
        
        Context.CurrentIKConstraint.weight =
            Mathf.Lerp(Context.CurrentIKConstraint.weight, desiredWeight, elapsedTime / lerpDuration);
        
        Context.CurrentMultiRotationConstraint.weight =
            Mathf.Lerp(Context.CurrentMultiRotationConstraint.weight, desiredRotationWeight, elapsedTime / lerpDuration);
    }
    public  override void ExitState() {}

    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        bool isOverStateLifeDuration = elapsedTime >= approachDuration;

        if (isOverStateLifeDuration || CheckShouldReset())
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        }
        
        bool isWithinArmsReach = Vector3.Distance(Context.ClosestPointOnColliderFromShoulder,
            Context.CurrentShoulderTransform.position) < riseDistanceThreshold;

        bool isClosestPointOnColliderValid = Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;

        if (isWithinArmsReach && isClosestPointOnColliderValid)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Rise;
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
