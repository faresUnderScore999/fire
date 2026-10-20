using System.Collections.Specialized;
using UnityEngine;

public class NpcSaving : MonoBehaviour
{
    // Start is pricalled once before the first execution of Update after the MonoBehaviour is created
    public Transform targetDestination; // Drag an object here for the car to drive toward
    private UnityEngine.AI.NavMeshAgent agent;
    private bool isPlayerInside = false;
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

      
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
           
        }
        Debug.LogWarning("Player entered the trigger zone.");
    }

    // Update is called once per frame
    void Update()
    {
if (isPlayerInside && targetDestination != null)
        {
            agent.SetDestination(targetDestination.position);
        }


    }
}
