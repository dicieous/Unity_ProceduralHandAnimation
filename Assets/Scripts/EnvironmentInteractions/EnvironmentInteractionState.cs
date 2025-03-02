using UnityEngine;

public abstract class
    EnvironmentInteractionState : BaseState<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    protected EnvironmentInteractionContext Context;
    private bool shouldReset = false;

    public EnvironmentInteractionState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironmentInteractionState state)
        : base(state)
    {
        this.Context = context;
    }

    protected bool CheckShouldReset()
    {
        if (shouldReset)
        {
            Context.LowestDistance = Mathf.Infinity;
            shouldReset = false;
            return true;
        }
        
        bool isPlayerStopped = Context.Rigidbody.linearVelocity == Vector3.zero;
        bool isMovingAway = CheckIsMovingAway();
        bool isBadAngle = CheckIsBadAngle();
        bool isPlayerJumping = Mathf.Round(Context.Rigidbody.linearVelocity.y) >= 1;

        if (isMovingAway || isPlayerStopped || isBadAngle || isPlayerJumping)
        {
            Context.LowestDistance = Mathf.Infinity;
            return true;
        }

        return false;
    }

    protected bool CheckIsBadAngle()
    {
        if (Context.CurrentIntersectingCollider == null) return false;

        Vector3 targetDirection = Context.ClosestPointOnColliderFromShoulder - Context.CurrentShoulderTransform.position;
        Vector3 shoulderDirection = Context.CurrentBodySide == EnvironmentInteractionContext.EBodySide.RIGHT
            ? Context.RootTransform.right
            : -Context.RootTransform.right;

        float dot = Vector3.Dot(shoulderDirection, targetDirection.normalized);
        bool isBadAngle = dot < 0;

        return isBadAngle;
    }
    
    protected bool CheckIsMovingAway()
    {
        float currentDisatanceToTarget =
            Vector3.Distance(Context.RootTransform.position, Context.ClosestPointOnColliderFromShoulder);

        bool isSearchingForNewInteraction = Context.CurrentIntersectingCollider == null;
        if (isSearchingForNewInteraction)
            return false;

        bool isGettingCloserToTarget = currentDisatanceToTarget <= Context.LowestDistance;
        if (isGettingCloserToTarget)
        {
            Context.LowestDistance = currentDisatanceToTarget;
            return false;
        }

        float movingAwayOffset = 0.005f;
        bool isMovingAwayFromTarget = currentDisatanceToTarget > Context.LowestDistance + movingAwayOffset;
        if (isMovingAwayFromTarget)
        {
            Context.LowestDistance = Mathf.Infinity;
            return true;
        }

        return false;
    }

    private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
    {
        return intersectingCollider.ClosestPoint(positionToCheck);
    }

    protected void StartIKTargetPositionTracking(Collider intersectingCollider)
    {
        if (intersectingCollider.gameObject.layer == LayerMask.NameToLayer("Interactable") &&
            Context.CurrentIntersectingCollider == null)
        {
            Context.CurrentIntersectingCollider = intersectingCollider;
            var closestPointFromRoot = GetClosestPointOnCollider(intersectingCollider, Context.RootTransform.position);
            Context.SetCurrentSide(closestPointFromRoot);

            SetIKTargetPosition();
        }
    }

    protected void UpdateIKTargetPosition(Collider intersectingCollider)
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider)
        {
            SetIKTargetPosition();
        }
    }

    protected void ResetIKTargetPositionTracking(Collider intersectingCollider)
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider)
        {
            Context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity;
            Context.CurrentIntersectingCollider = null;
            shouldReset = true;
        }
    }

    private void SetIKTargetPosition()
    {
        var shoulderPosition = Context.CurrentShoulderTransform.position;
        Context.ClosestPointOnColliderFromShoulder = GetClosestPointOnCollider(Context.CurrentIntersectingCollider,
            new Vector3(shoulderPosition.x, Context.CharacterShoulderHeight,
                shoulderPosition.z));

        Vector3 rayDirectionNormalized =
            Vector3.Normalize(shoulderPosition - Context.ClosestPointOnColliderFromShoulder);
        float offsetDistance = 0.05f;
        Vector3 offset = rayDirectionNormalized * offsetDistance;

        Vector3 offsetPosition = Context.ClosestPointOnColliderFromShoulder + offset;
        Context.CurrentIKTargetTransform.position =
            new Vector3(offsetPosition.x, Context.InteractionPointYOffset, offsetPosition.z);
    }
}