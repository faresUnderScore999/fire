using UnityEngine;
using TMPro; // Use this if you are using TextMeshPro. If using standard UI Text, delete this line and change TextMeshProUGUI to Text.

public class TriggerMessageUI : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject uiCanvasObject; // Drag your Canvas or the specific UI panel here
     public TextMeshProUGUI messageText; // Optional: If you want to change the text dynamically

    [Header("Message")]
    [TextArea]
    public string messageToShow = "Welcome to the area!";

    private void Start()
    {
        // Make sure the UI is hidden at the start of the game
        if (uiCanvasObject != null)
        {
            uiCanvasObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            if (uiCanvasObject != null)
            {
                uiCanvasObject.SetActive(true);
                
                // If you want to set the text dynamically via script:
                 messageText.text = messageToShow;
            }
        }
        Debug.LogWarning("Player entered the trigger zone.");
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide the UI when the player leaves the trigger zone
        if (other.CompareTag("Player"))
        {
            if (uiCanvasObject != null)
            {
                uiCanvasObject.SetActive(false);
            }
        }
    }
}