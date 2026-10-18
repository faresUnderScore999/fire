using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class npc : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform target; // Drag your Player or destination transform here in the Inspector

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }
}