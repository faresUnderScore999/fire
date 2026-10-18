using UnityEngine;
using UnityEngine.InputSystem; // Added for the new Input System
using TMPro;

public class ExtinguisherInfo : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject infoPanel;
    public TMP_Text infoText;
    [TextArea(3, 10)]
    public string extinguisherDetails = "Water Extinguisher: Best for Class A fires (wood, paper, trash). Do not use on electrical or grease fires!";

    [Header("Input System Settings")]
    [Tooltip("Drag the 'Interact' InputActionReference here or configure it in the Inspector")]
    public InputActionReference interactAction;

    private bool playerInRange = false;

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteractPerformed;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteractPerformed;
            interactAction.action.Disable();
        }
    }

    private void Start()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (playerInRange)
        {
            ToggleInfoPanel();
        }
    }

    private void ToggleInfoPanel()
    {
        if (infoPanel != null)
        {
            bool isActive = infoPanel.activeSelf;
            infoPanel.SetActive(!isActive);

            if (!isActive && infoText != null)
            {
                infoText.text = extinguisherDetails;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (infoPanel != null)
            {
                infoPanel.SetActive(false);
            }
        }
    }
}