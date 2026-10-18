using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Input System Setup")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string sprintActionName = "Sprint";

    [Header("Components & Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private float dampTime = 0.1f;

    private InputAction moveAction;
    private InputAction sprintAction;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Find and assign actions from the Input Action Asset
        if (inputActions != null)
        {
            InputActionMap map = inputActions.FindActionMap(actionMapName);
            if (map != null)
            {
                moveAction = map.FindAction(moveActionName);
                sprintAction = map.FindAction(sprintActionName);
            }
            else
            {
                Debug.LogError($"[PlayerAnimationController] Action Map '{actionMapName}' not found in {inputActions.name}");
            }
        }
        else
        {
            Debug.LogError("[PlayerAnimationController] Please assign your Input Action Asset in the Inspector!");
        }
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        sprintAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        sprintAction?.Disable();
    }

    private void Update()
    {
        if (animator == null) return;

        // Read Movement and Sprint inputs
        Vector2 moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        bool isSprinting = sprintAction != null && sprintAction.IsPressed();

        // Calculate Target Blend Value: 0 = Idle, 0.5 = Run, 1.0 = Sprint
        float targetBlend = 0f;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            targetBlend = isSprinting ? 1.0f : 0.5f;
        }

        // Pass parameter smoothly to the Animator Blend Tree
        animator.SetFloat("Blend", targetBlend, dampTime, Time.deltaTime);
    }
}