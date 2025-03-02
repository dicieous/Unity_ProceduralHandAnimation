using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class ModelController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float groundDrag = 5f;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private bool invertY = false;
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float cameraHeight = 2f;
    [SerializeField] private float cameraSmoothTime = 0.1f;

    // Component references
    private Rigidbody rb;
    private Animator animator;
    private Camera mainCamera;
    private Transform cameraTransform;

    // Movement variables
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    
    // Camera variables
    private float cameraYaw;
    private float cameraPitch = 10f; // Default pitch angle (looking down a bit)
    private Vector3 cameraVelocity;

    // Animation parameters
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    private void Awake()
    {
        // Get component references
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        if (mainCamera != null) cameraTransform = mainCamera.transform;

        // Create camera target if not assigned
        if (cameraTarget == null)
        {
            GameObject target = new GameObject("CameraTarget");
            cameraTarget = target.transform;
            cameraTarget.position = transform.position + Vector3.up * cameraHeight;
            cameraTarget.parent = transform;
        }

        // Set rigidbody constraints
        rb.freezeRotation = true;
        
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Get input
        GetInput();
        
        // Calculate movement direction
        CalculateMovement();
        
        // Handle animations
        UpdateAnimations();
        
        // Apply drag when on ground
        rb.linearDamping = groundDrag;
    }

    private void FixedUpdate()
    {
        // Apply movement force
        MovePlayer();
    }

    private void LateUpdate()
    {
        // Handle camera rotation and positioning
        UpdateCamera();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        // Mouse input for camera rotation
        cameraYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        cameraPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * (invertY ? -1 : 1);
        cameraPitch = Mathf.Clamp(cameraPitch, -30f, 60f); // Limit vertical rotation
    }

    private void CalculateMovement()
    {
        // Calculate movement direction relative to camera direction
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        
        // Project vectors onto the horizontal plane
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        // Combine input with camera direction
        moveDirection = cameraForward * verticalInput + cameraRight * horizontalInput;
        
        // Normalize for consistent speed in all directions
        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection.Normalize();
        }
    }

    private void MovePlayer()
    {
        // Apply force to move the character
        rb.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
        
        // Rotate character to face movement direction if moving
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateCamera()
    {
        // Calculate the desired camera position based on the character's position
        Vector3 targetPosition = cameraTarget.position;
        
        // Calculate rotation quaternion from Euler angles
        Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);
        
        // Position the camera behind the player at the specified distance
        Vector3 desiredCameraPosition = targetPosition - (rotation * Vector3.forward * cameraDistance);
        
        // Smoothly move the camera to the desired position
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, desiredCameraPosition, ref cameraVelocity, cameraSmoothTime);
        
        // Make the camera look at the target
        cameraTransform.LookAt(targetPosition);
    }

    private void UpdateAnimations()
    {
        // Set animation based on whether the player is moving
        bool isWalking = moveDirection.magnitude > 0.1f;
        animator.SetBool(IsWalking, isWalking);
    }
}