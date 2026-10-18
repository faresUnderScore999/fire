using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;   // Main Camera, child of the Player

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float pushStrength = 2f;

    [Header("Mouse Look")]
    [SerializeField] private float lookSensitivity = 0.1f;   // degrees per mouse pixel
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Input System")]
    [Tooltip("Drag your 'Move' action from your Input Actions asset here.")]
    [SerializeField] private InputActionReference moveActionReference;

    // Set to false by the end screen so the player stops reacting
    public bool ControlsEnabled { get; set; } = true;

    private CharacterController controller;
    private float pitch;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Start() => LockCursor();

    private void OnEnable()
    {
        if (moveActionReference != null) moveActionReference.action.Enable();
    }

    private void OnDisable()
    {
        if (moveActionReference != null) moveActionReference.action.Disable();
    }

    private void Update()
    {
        if (!ControlsEnabled) return;

        // After pressing Esc, a click gives the mouse back to the game
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) LockCursor();
            return;
        }

        Look();
        Move();
    }

    private void Look()
    {
        if (Mouse.current == null || cameraTransform == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();

        // Yaw turns the whole body, pitch tilts only the camera
        transform.Rotate(0f, delta.x * lookSensitivity, 0f);
        pitch = Mathf.Clamp(pitch - delta.y * lookSensitivity, minPitch, maxPitch);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void Move()
    {
        Vector2 input = moveActionReference != null
            ? moveActionReference.action.ReadValue<Vector2>()
            : Vector2.zero;

        // Relative to where the body faces (the body only yaws, so these are horizontal)
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        move = Vector3.ClampMagnitude(move, 1f);

        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        else verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * moveSpeed;
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Lets the player push the crates
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic || hit.moveDirection.y < -0.3f) return;

        body.linearVelocity = new Vector3(
            hit.moveDirection.x * pushStrength,
            body.linearVelocity.y,
            hit.moveDirection.z * pushStrength);
    }
}