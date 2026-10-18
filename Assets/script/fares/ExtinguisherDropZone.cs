using UnityEngine;
using TMPro;

public class ExtinguisherDropZone : MonoBehaviour
{
    [Header("Box Settings")]
    [Tooltip("The correct extinguisher type required for this specific box.")]
    public ExtinguisherType requiredType;

    [Header("Target Extinguisher")]
    [Tooltip("Drag your specific extinguisher object here.")]
    public Extinguisher targetExtinguisher;

    [Header("UI Reference")]
    [Tooltip("The Canvas Text component passed as a parameter.")]
    public TextMeshProUGUI feedbackText;

    [Header("Messages")]
    public string correctMessage = "Correct Extinguisher!";
    public string wrongMessage = "Wrong Extinguisher, try again!";

    [Header("Detection Settings")]
    [Tooltip("Distance threshold to consider the extinguisher 'dropped' inside the box.")]
    public float detectionRadius = 5.0f;

    private void Update()
    {
        if (targetExtinguisher == null)
        {
            return;
        }

        // Check the distance between this box and the extinguisher
        float distance = Vector3.Distance(transform.position, targetExtinguisher.transform.position);

        if (distance <= detectionRadius)
        {
            // Inside the zone: show the message
            if (targetExtinguisher.extinguisherType == requiredType)
            {
                if (feedbackText != null)
                {
                    feedbackText.text = correctMessage;
                    feedbackText.color = Color.green;
                }
            }
            else
            {
                if (feedbackText != null)
                {
                    feedbackText.text = wrongMessage;
                    feedbackText.color = Color.red;
                }
            }
        }
        else
        {
            // Outside the zone: clear the text
            if (feedbackText != null)
            {
                feedbackText.text = "";
            }
        }
    }
}