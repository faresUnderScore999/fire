using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMove))]
public class InteractionController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private TMP_Text promptText;
    [Tooltip("Optional. If empty, the E key is used.")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private float range = 2.5f;

    private PlayerMove player;
    private int mask;

    private void Awake()
    {
        player = GetComponent<PlayerMove>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        mask = ~LayerMask.GetMask("Player", "Water", "Fire", "Ignore Raycast");
    }

    private void OnEnable()
    {
        if (interactAction != null) interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null) interactAction.action.Disable();
    }

    private void Update()
    {
        Interactable target = FindTarget();

        if (promptText != null)
        {
            promptText.gameObject.SetActive(target != null);
            if (target != null) promptText.text = target.Prompt;
        }

        if (target != null && InteractPressed()) target.Interact();
    }

    private Interactable FindTarget()
    {
        if (!player.ControlsEnabled || Cursor.lockState != CursorLockMode.Locked) return null;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit,
                range, mask, QueryTriggerInteraction.Ignore))
        {
            Interactable found = hit.collider.GetComponentInParent<Interactable>();
            if (found != null && found.CanInteract) return found;
        }
        return null;
    }

    private bool InteractPressed()
    {
        if (interactAction != null) return interactAction.action.WasPressedThisFrame();
        return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
    }
}