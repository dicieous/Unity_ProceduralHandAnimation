using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

public class EnvironmentInteractionStateMachine : StateManager<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    public enum EEnvironmentInteractionState
    {
        Search,
        Approach,
        Rise,
        Touch,
        Reset,
    }

    private EnvironmentInteractionContext context;
    
    [SerializeField] private TwoBoneIKConstraint leftIKConstraint;
    [SerializeField] private TwoBoneIKConstraint rightIKConstraint;
    [Space(10)]
    [SerializeField] private MultiRotationConstraint leftMultiRotationConstraint;
    [SerializeField] private MultiRotationConstraint rightMultiRotationConstraint;
    [Space(10)] 
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private CapsuleCollider rootCollider;

    private void Awake()
    {
        ValidateConstraints();

        context = new EnvironmentInteractionContext(leftIKConstraint, rightIKConstraint, leftMultiRotationConstraint,
            rightMultiRotationConstraint, rigidbody, rootCollider, transform.root);
        
        InitializeStates();
        ConstructEnvironmentDetectionCollider();
    }

    private void ValidateConstraints()
    {
        Assert.IsNotNull(leftIKConstraint);
        Assert.IsNotNull(rightIKConstraint);
        Assert.IsNotNull(leftMultiRotationConstraint);
        Assert.IsNotNull(rightMultiRotationConstraint);
        Assert.IsNotNull(rigidbody);
        Assert.IsNotNull(rootCollider);
    }

    private void InitializeStates()
    {
        StatesDict.Add(EEnvironmentInteractionState.Approach, new ApproachState(context, EEnvironmentInteractionState.Approach));
        StatesDict.Add(EEnvironmentInteractionState.Rise, new RiseState(context, EEnvironmentInteractionState.Rise));
        StatesDict.Add(EEnvironmentInteractionState.Search, new SearchState(context, EEnvironmentInteractionState.Search));
        StatesDict.Add(EEnvironmentInteractionState.Touch, new TouchState(context, EEnvironmentInteractionState.Touch));
        StatesDict.Add(EEnvironmentInteractionState.Reset, new ResetState(context, EEnvironmentInteractionState.Reset));

        CurrentState = StatesDict[EEnvironmentInteractionState.Reset];
    }

    private void ConstructEnvironmentDetectionCollider()
    {
        float wingSpan = rootCollider.height;

        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(wingSpan, wingSpan, wingSpan);

        var center = rootCollider.center;
        boxCollider.center = new Vector3(center.x, center.y * (0.25f * wingSpan),
            center.z + (0.5f * wingSpan));
        boxCollider.isTrigger = true;

        context.ColliderCenterY = center.y;
    }
    
}
