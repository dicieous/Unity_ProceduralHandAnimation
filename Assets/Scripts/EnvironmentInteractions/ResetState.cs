using System;
using UnityEngine;

public class ResetState : EnvironmentInteractionState
{
    private float elapsedTime = 0.0f;
    private float resetDuration = 2.0f;
    private float lerpDuration = 5.0f;
    
    private float rotationSpeed = 500f;
    public ResetState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironmentInteractionState state)
        : base(context, state)
    {
        EnvironmentInteractionContext Context = context;
    }

    public override void EnterState()
    {
        elapsedTime = 0.0f;
        Context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity;
        Context.CurrentIntersectingCollider = null;
    }

    public override void UpdateState()
    {
        elapsedTime += Time.deltaTime;
        
        Context.InteractionPointYOffset = Mathf.Lerp(Context.InteractionPointYOffset,
            Context.ColliderCenterY, elapsedTime / lerpDuration);
        
        Context.CurrentIKConstraint.weight =
            Mathf.Lerp(Context.CurrentIKConstraint.weight, 0, elapsedTime / lerpDuration);
        
        Context.CurrentMultiRotationConstraint.weight =
            Mathf.Lerp(Context.CurrentMultiRotationConstraint.weight, 0, elapsedTime / lerpDuration);

        Context.CurrentIKTargetTransform.localPosition = Vector3.Lerp(Context.CurrentIKTargetTransform.localPosition,
            Context.CurrentOriginalTargetPosition, elapsedTime / lerpDuration);

        Context.CurrentIKTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIKTargetTransform.rotation,
            Context.OriginalTargetRotation, rotationSpeed * Time.deltaTime);
    }
    public  override void ExitState() {}

    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        bool isMoving = Context.Rigidbody.linearVelocity != Vector3.zero;
        if (elapsedTime >= resetDuration && isMoving)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Search;
        }
        
        return State;
    }
    public  override void OnTriggerEnter(Collider other) {}
    public  override void OnTriggerStay(Collider other) {}
    public  override void OnTriggerExit(Collider other) {}
}
