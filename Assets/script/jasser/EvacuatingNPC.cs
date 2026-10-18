using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EvacuatingNPC : MonoBehaviour
{
    [SerializeField] private Transform exit;
    [SerializeField] private AlarmStation alarm;
    [SerializeField] private float reactionDelay = 1.5f;
    [SerializeField] private float arriveDistance = 1f;
    [SerializeField] private float retryInterval = 0.5f;

    private NavMeshAgent agent;
    private float retryTimer;
    private bool evacuating;
    private bool evacuated;

    private void Awake() => agent = GetComponent<NavMeshAgent>();

    private void Start()
    {
        if (alarm == null) alarm = FindAnyObjectByType<AlarmStation>();

        if (alarm != null) alarm.AlarmTriggered += OnAlarm;
        else StartEvacuation();   // no alarm in the scene: leave right away
    }

    private void OnDestroy()
    {
        if (alarm != null) alarm.AlarmTriggered -= OnAlarm;
    }

    private void OnAlarm() => Invoke(nameof(StartEvacuation), reactionDelay);

    private void StartEvacuation()
    {
        evacuating = true;
        agent.SetDestination(exit.position);
    }

    private void Update()
    {
        if (!evacuating || evacuated || agent.pathPending) return;

        Vector3 delta = exit.position - transform.position;
        delta.y = 0f;
        if (delta.magnitude <= arriveDistance)
        {
            evacuated = true;
            agent.ResetPath();
            Debug.Log("NPC evacuated");
            return;
        }

        // Path blocked by the fire? Keep retrying until it opens
        if (agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            retryTimer -= Time.deltaTime;
            if (retryTimer <= 0f)
            {
                agent.SetDestination(exit.position);
                retryTimer = retryInterval;
            }
        }
    }
}