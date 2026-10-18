using UnityEngine;
using UnityEngine.InputSystem;

public class ExtinguisherFollowSimple : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Drag your Player object here in the Inspector

    [Header("Input")]
    public InputActionReference interactAction; // Assign Player/Interact here

    [Header("Movement Settings")]
    public float followSpeed = 5.0f;
    public float followDistance = 1.5f;
    public float interactionRadius = 3.0f;

    private bool isFollowing = false;
    private ExtinguisherFloat floatScript;

    private void Awake()
    {
        floatScript = GetComponent<ExtinguisherFloat>();
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
            interactAction.action.Disable();
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If close enough, pick up or drop
        if (distanceToPlayer <= interactionRadius || isFollowing)
        {
            isFollowing = !isFollowing;

            // Update floating state immediately when toggled
            if (floatScript != null)
            {
                floatScript.SetHeld(isFollowing);
            }
        }
    }

    private void Update()
    {
        if (isFollowing && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // Move towards the player until reaching followDistance
            if (distance > followDistance)
            {
                Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
                
                // Face towards the player
                transform.LookAt(targetPosition);
            }
        }
    }
}