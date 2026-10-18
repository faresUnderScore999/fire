using UnityEngine;

public class TriggerRelay : MonoBehaviour
{
    public string id;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && ScenarioManager.Instance != null)
            ScenarioManager.Instance.ZoneEntree(id, true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && ScenarioManager.Instance != null)
            ScenarioManager.Instance.ZoneEntree(id, false);
    }
}