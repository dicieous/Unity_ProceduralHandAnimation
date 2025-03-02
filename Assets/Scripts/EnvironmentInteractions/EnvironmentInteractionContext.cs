using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Vector3 = UnityEngine.Vector3;

public class EnvironmentInteractionContext
{
    public enum EBodySide
    {
        RIGHT,
        LEFT,
    }
    
    public TwoBoneIKConstraint LeftIKConstraint => leftIKConstraint;
    public TwoBoneIKConstraint RightIKConstraint => rightIKConstraint;
    public MultiRotationConstraint LeftMultiRotationConstraint => leftMultiRotationConstraint;
    public MultiRotationConstraint RightMultiRotationConstraint => rightMultiRotationConstraint;
    public Rigidbody Rigidbody => rigidbody;
    public CapsuleCollider RootCollider => rootCollider;
    public Transform RootTransform => rootTransform;
    
    public Collider CurrentIntersectingCollider { get; set; }
    public TwoBoneIKConstraint CurrentIKConstraint { get; private set; }
    public MultiRotationConstraint CurrentMultiRotationConstraint { get; private set; }
    public Transform CurrentIKTargetTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }
    public EBodySide CurrentBodySide { get; private set; }
    public Vector3 ClosestPointOnColliderFromShoulder { get; set; } = Vector3.positiveInfinity;
    public float InteractionPointYOffset { get; set; } = 0.0f;
    public float ColliderCenterY { get; set; } = 0.0f;
    public float CharacterShoulderHeight { get; private set; }
    public Vector3 CurrentOriginalTargetPosition { get; private set; }
    public Quaternion OriginalTargetRotation { get; private set; }
    public float LowestDistance { get; set; } = Mathf.Infinity;
    
    
    private TwoBoneIKConstraint leftIKConstraint;
    private TwoBoneIKConstraint rightIKConstraint;
    private MultiRotationConstraint leftMultiRotationConstraint;
    private MultiRotationConstraint rightMultiRotationConstraint;
    private Rigidbody rigidbody;
    private CapsuleCollider rootCollider;
    private Transform rootTransform;
    private Vector3 leftOriginalTargetPosition;
    private Vector3 rightOriginalTargetPosition;
    
    public EnvironmentInteractionContext(TwoBoneIKConstraint leftIKConstraint, TwoBoneIKConstraint rightIKConstraint,
        MultiRotationConstraint leftMultiRotationConstraint, MultiRotationConstraint rightMultiRotationConstraint,
        Rigidbody rigidbody, CapsuleCollider capsuleCollider, Transform rootTransform)
    {
        this.leftIKConstraint = leftIKConstraint;
        this.rightIKConstraint = rightIKConstraint;
        this.rightMultiRotationConstraint = rightMultiRotationConstraint;
        this.leftMultiRotationConstraint = leftMultiRotationConstraint;
        this.rigidbody = rigidbody;
        this.rootCollider = capsuleCollider;
        this.rootTransform = rootTransform;
        leftOriginalTargetPosition = this.leftIKConstraint.data.target.transform.localPosition;
        rightOriginalTargetPosition = this.rightIKConstraint.data.target.transform.localPosition;
        OriginalTargetRotation = this.leftIKConstraint.data.target.rotation;

        CharacterShoulderHeight = leftIKConstraint.data.root.transform.position.y;
        SetCurrentSide(Vector3.positiveInfinity);
    }

    public void SetCurrentSide(Vector3 positionToCheck)
    {
        Vector3 leftShoulder = leftIKConstraint.data.root.transform.position;
        Vector3 rightShoulder = rightIKConstraint.data.root.transform.position;

        bool isLeftCloser = Vector3.Distance(positionToCheck, leftShoulder) <
                            Vector3.Distance(positionToCheck, rightShoulder);

        if (isLeftCloser)
        {
            Debug.Log("Left side is closer");
            CurrentBodySide = EBodySide.LEFT;
            CurrentIKConstraint = leftIKConstraint;
            CurrentMultiRotationConstraint = leftMultiRotationConstraint;
            CurrentOriginalTargetPosition = leftOriginalTargetPosition;
        }
        else
        {
            Debug.Log("Right side is closer");
            CurrentBodySide = EBodySide.RIGHT;
            CurrentIKConstraint = rightIKConstraint;
            CurrentMultiRotationConstraint = rightMultiRotationConstraint;
            CurrentOriginalTargetPosition = rightOriginalTargetPosition;
        }

        CurrentShoulderTransform = CurrentIKConstraint.data.root.transform;
        CurrentIKTargetTransform = CurrentIKConstraint.data.target.transform;
    }
}
