using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Transform mainCameraTransform;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    private float verticalVelocity;

    [Header("Input System")]
    [Tooltip("Drag your 'Move' action from your Input Actions asset here.")]
    [SerializeField] private InputActionReference moveActionReference;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Get reference to the Main Camera
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Main Camera not found! Make sure your Cinemachine Virtual Camera is driving a Camera tagged 'MainCamera'.");
        }
    }

    private void OnEnable()
    {
        if (moveActionReference != null)
            moveActionReference.action.Enable();
    }

    private void OnDisable()
    {
        if (moveActionReference != null)
            moveActionReference.action.Disable();
    }

    private void Update()
    {
        // 1. Keep camera reference fresh across scenes
        if (mainCameraTransform == null && Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        // 1. Read Vector2 input (WASD / Left Stick)
        Vector2 inputVector = moveActionReference != null 
            ? moveActionReference.action.ReadValue<Vector2>() 
            : Vector2.zero;

        // 2. Get Camera forward and right vectors, zeroing out vertical Y tilt
        Vector3 camForward = mainCameraTransform.forward;
        Vector3 camRight = mainCameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // 3. Calculate camera-relative movement direction
        Vector3 moveDirection = (camForward * inputVector.y) + (camRight * inputVector.x);

        // 4. Smoothly rotate character to face the movement direction
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 5. Handle gravity
        if (controller.isGrounded && verticalVelocity < 0.0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;

        // 6. Move character
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
    private void Start()
{
    // Lock the cursor to the center of the screen and hide it
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}
}