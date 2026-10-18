using UnityEngine;

public class ExtinguisherFloat : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;

    [Header("Hover Settings")]
    public float hoverAmplitude = 0.25f;
    public float hoverFrequency = 2f;

    private Vector3 startPosition;
    [HideInInspector] public bool isHeld = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Stop floating and rotating when the player is holding/following it
        if (isHeld) return;

        // Rotation around the vertical axis (Y)
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Vertical up-and-down motion using a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    public void SetHeld(bool held)
    {
        isHeld = held;
        if (!isHeld)
        {
            // Reset the baseline floating position to wherever it was dropped
            startPosition = transform.position;
        }
    }
}