using System;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class AlarmStation : MonoBehaviour
{
    [Header("Station visuals")]
    [SerializeField] private Transform lever;
    [SerializeField] private Vector3 pulledOffset = new Vector3(0f, -0.05f, 0f);
    [SerializeField] private Renderer indicator;
    [SerializeField] private Color indicatorColor = Color.red;

    [Header("Alarm")]
    [SerializeField] private AlarmSiren siren;
    [SerializeField] private AlarmLight[] alarmLights;

    public bool IsActive { get; private set; }
    public event Action AlarmTriggered;

    private Interactable interactable;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.OnInteracted.AddListener(Activate);
        if (siren == null) siren = GetComponent<AlarmSiren>();
    }

    private void OnDestroy()
    {
        if (interactable != null) interactable.OnInteracted.RemoveListener(Activate);
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;

        if (lever != null) lever.localPosition += pulledOffset;

        if (indicator != null)
        {
            Material m = indicator.material;
            m.EnableKeyword("_EMISSION");
            m.color = indicatorColor;
            m.SetColor("_EmissionColor", indicatorColor * 3f);
        }

        if (siren != null) siren.Play();
        foreach (AlarmLight alarmLight in alarmLights)
            if (alarmLight != null) alarmLight.StartFlashing();

        AlarmTriggered?.Invoke();
    }

    // Can be called later by the scenario (e.g. when the fire is out)
    public void Silence()
    {
        if (siren != null) siren.Stop();
        foreach (AlarmLight alarmLight in alarmLights)
            if (alarmLight != null) alarmLight.StopFlashing();
    }
}