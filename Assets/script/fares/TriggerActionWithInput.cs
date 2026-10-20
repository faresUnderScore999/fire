using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class TriggerActionWithInput : MonoBehaviour
{
    [Header("Objects to Modify")]
    [Tooltip("The object that will be destroyed.")]
    public GameObject objectToDestroy;


    public GameObject objectToDestroy2;

    [Tooltip("The object that will be activated/turned on.")]
    public GameObject objectToActivate;

    [Header("Input Settings")]
    [Tooltip("Check this if you are using the NEW Input System. Uncheck if using legacy Input.GetMouseButtonDown(0).")]
    public bool useNewInputSystem = true;

    private bool isPlayerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is the Player
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            Debug.Log("Player entered the trigger zone. Press Mouse Click to perform action.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player leaves the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            Debug.Log("Player left the trigger zone.");
        }
    }

    private void Update()
    {
        // Only listen for clicks if the player is currently inside the trigger
        if (isPlayerInside)
        {
            bool mouseClicked = false;

            // Check for mouse click based on input system choice
            if (useNewInputSystem)
            {
                // New Input System check (Left Mouse Button / Press)
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    mouseClicked = true;
                }
            }
            else
            {
                // Legacy Input System check
                if (Input.GetMouseButtonDown(0))
                {
                    mouseClicked = true;
                }
            }

            // If clicked, perform the destroy and activate actions
            if (mouseClicked)
            {
                PerformAction();
            }
        }
    }

    private void PerformAction()
    {
        // Destroy the target object if assigned
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
            Debug.Log(objectToDestroy.name + " was destroyed.");
        }
        if (objectToDestroy2 != null)
        {
            Destroy(objectToDestroy2);
            Debug.Log(objectToDestroy2.name + " was destroyed.");
        }
        // Activate the target object if assigned
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log(objectToActivate.name + " was activated.");
        }

        // Optional: Disable this script or reset state so it only happens once
        enabled = false;
    }
}