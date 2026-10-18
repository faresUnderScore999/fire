using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class FireController : MonoBehaviour
{
    public enum FireState { Burning, Controlled }

    [Header("Intensity")]
    [Range(0f, 1f)] public float intensity = 0.6f;
    [SerializeField] private float growthRate = 0.01f;        // per second while untouched
    [SerializeField] private float controlledThreshold = 0.05f;

    [Header("Visuals")]
    [SerializeField] private VisualEffect fireVfx;
    [SerializeField] private Transform visual;                // optional debug sphere, can stay empty
    [SerializeField] private Light fireLight;
    [SerializeField] private float maxVisualScale = 1.5f;
    [SerializeField] private float maxLightIntensity = 3f;
    [SerializeField] private float flickerAmount = 0.25f;
    [SerializeField] private float flickerSpeed = 8f;
    [SerializeField] private float cleanupDelay = 3f;         // lets the smoke fade out

    [Header("Navigation")]
    [SerializeField] private NavMeshObstacle obstacle;

    public FireState State { get; private set; } = FireState.Burning;
    public event Action<float> OnIntensityChanged;
    public event Action<FireController> OnControlled;

    private static readonly int IntensityId = Shader.PropertyToID("Intensity");
    private float flickerSeed;

    private void Awake() => flickerSeed = UnityEngine.Random.value * 100f;

    private void Start() => ApplyVisuals();

    private void Update()
    {
        if (State == FireState.Burning)
            SetIntensity(intensity + growthRate * Time.deltaTime);

        UpdateLight();
    }

    public void ApplyExtinguisher(float amount)
    {
        if (State != FireState.Burning) return;
        SetIntensity(intensity - amount);
    }

    private void SetIntensity(float value)
    {
        intensity = Mathf.Clamp01(value);
        ApplyVisuals();
        OnIntensityChanged?.Invoke(intensity);

        if (intensity <= controlledThreshold) Control();
    }

    private void ApplyVisuals()
    {
        if (fireVfx != null) fireVfx.SetFloat(IntensityId, intensity);

        if (visual != null)
        {
            float s = Mathf.Lerp(0.05f, maxVisualScale, intensity);
            visual.localScale = Vector3.one * s;
            visual.localPosition = new Vector3(0f, s * 0.5f, 0f);
        }
    }

    private void UpdateLight()
    {
        if (fireLight == null || !fireLight.enabled) return;

        float noise = Mathf.PerlinNoise(flickerSeed, Time.time * flickerSpeed);
        float flicker = 1f + (noise - 0.5f) * 2f * flickerAmount;
        fireLight.intensity = Mathf.Lerp(0f, maxLightIntensity, intensity) * flicker;
    }

    private void Control()
    {
        State = FireState.Controlled;
        if (obstacle != null) obstacle.enabled = false;     // the NPC can walk through again
        if (fireVfx != null) fireVfx.Stop();                // stop spawning; remaining smoke fades
        if (fireLight != null) fireLight.enabled = false;

        OnControlled?.Invoke(this);
        Destroy(gameObject, cleanupDelay);
    }
}