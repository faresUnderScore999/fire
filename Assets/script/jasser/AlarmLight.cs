using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    [SerializeField] private Light lightSource;
    [SerializeField] private Renderer lamp;
    [SerializeField] private float flashesPerSecond = 1.5f;
    [SerializeField] private float maxIntensity = 4f;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private Material lampMaterial;
    private bool flashing;

    private void Awake()
    {
        if (lightSource != null) lightSource.enabled = false;
        if (lamp != null)
        {
            lampMaterial = lamp.material;
            lampMaterial.EnableKeyword("_EMISSION");
        }
    }

    public void StartFlashing() => flashing = true;

    public void StopFlashing()
    {
        flashing = false;
        SetLevel(0f);
    }

    private void Update()
    {
        if (!flashing) return;
        SetLevel(Mathf.PingPong(Time.time * flashesPerSecond * 2f, 1f));   // smooth 0..1 pulse
    }

    private void SetLevel(float level)
    {
        if (lightSource != null)
        {
            lightSource.enabled = level > 0.01f;
            lightSource.intensity = level * maxIntensity;
        }
        if (lampMaterial != null)
            lampMaterial.SetColor(EmissionColor, Color.red * (level * 3f));
    }
}