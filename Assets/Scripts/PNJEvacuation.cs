using UnityEngine;
using UnityEngine.AI;

public class PNJEvacuation : MonoBehaviour
{
    NavMeshAgent agent;
    Transform sortie;
    bool parti;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject s = GameObject.FindWithTag("Sortie");
        if (s != null) sortie = s.transform;
    }

    void Update()
    {
        if (ScenarioManager.Instance == null || sortie == null) return;

        if (!parti && ScenarioManager.Instance.etat == EtatScenario.Alerte)
        {
            parti = true;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(sortie.position, out hit, 5f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }

        if (parti && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            Debug.Log("PNJ evacue !");
            Destroy(gameObject);
        }
    }
}