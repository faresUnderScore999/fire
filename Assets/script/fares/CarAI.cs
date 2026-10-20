using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    public Transform targetDestination; // Drag an object here for the car to drive toward
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (targetDestination != null)
        {
            agent.SetDestination(targetDestination.position);
        }
    }

    void Update()
    {
        // Continuously update destination if the target moves
        if (targetDestination != null)
        {
            agent.SetDestination(targetDestination.position);
        }
    }
}