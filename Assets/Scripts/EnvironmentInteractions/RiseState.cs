using UnityEngine;

public class RiseState : EnvironmentInteractionState
{
    private float elapsedTime = 0.0f;
    private float lerpDuration = 5.0f;
    private float riseWeight = 1.0f;
    private Quaternion expectecHandRotation;
    private float maxDistance = 0.5f;
    private float rotationSpeed = 1000f;
    protected LayerMask interactibleLayerMask = LayerMask.GetMask("Interactable");
    private float TouchDistanceThreshold = 0.05f;
    private float touchTimeThreshold = 1f;
    
    public RiseState(EnvironmentInteractionContext context,
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
        
        CalculateExpectedHandRotation();
        
        Context.InteractionPointYOffset = Mathf.Lerp(Context.InteractionPointYOffset,
            Context.ClosestPointOnColliderFromShoulder.y, elapsedTime / lerpDuration);
        
        Context.CurrentIKConstraint.weight =
            Mathf.Lerp(Context.CurrentIKConstraint.weight, riseWeight, elapsedTime / lerpDuration);

        Context.CurrentMultiRotationConstraint.weight =
            Mathf.Lerp(Context.CurrentMultiRotationConstraint.weight, riseWeight, elapsedTime / lerpDuration);

        Context.CurrentIKTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIKTargetTransform.rotation,
            expectecHandRotation, rotationSpeed * Time.deltaTime);
    }

    public void CalculateExpectedHandRotation()
    {
        Vector3 startPos = Context.CurrentShoulderTransform.position;
        Vector3 endPos = Context.ClosestPointOnColliderFromShoulder;
        Vector3 direction = (endPos - startPos).normalized;

        RaycastHit hit;
        if (Physics.Raycast(startPos, direction, out hit, maxDistance, interactibleLayerMask))
        {
            Vector3 surfaceNormal = hit.normal;
            Vector3 targetForward = -surfaceNormal;
            expectecHandRotation = Quaternion.LookRotation(targetForward, Vector3.up);
        }
    }
    
    public  override void ExitState() {}

    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {

        if (CheckShouldReset()) return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        
        if (Vector3.Distance(Context.CurrentIKTargetTransform.position, Context.ClosestPointOnColliderFromShoulder) <
            TouchDistanceThreshold && elapsedTime >= touchTimeThreshold)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Touch;
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
