using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private Animator childAnimator;

    void Start()
    {
        // Get the animator from the child object
        childAnimator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object we collided with has the "Endpoint" tag
        if (other.CompareTag("Endpoint"))
        {
            Debug.LogWarning("Player reached the endpoint!");
            if (childAnimator != null)
            {
                // Set the boolean parameter "end" to true to trigger the transition
                childAnimator.SetBool("end", true);
            }
        }
    }
}