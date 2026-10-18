using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AlarmSiren : MonoBehaviour
{
    [Tooltip("Optional: drop a real siren clip here to replace the generated one.")]
    [SerializeField] private AudioClip customClip;
    [SerializeField, Range(0f, 1f)] private float volume = 0.6f;
    [SerializeField, Range(0f, 1f)] private float spatialBlend = 0.6f;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.volume = volume;
        source.spatialBlend = spatialBlend;
        source.clip = customClip != null ? customClip : CreateSirenClip();
    }

    public void Play()
    {
        if (!source.isPlaying) source.Play();
    }

    public void Stop() => source.Stop();

    // Two alternating tones with short fades (no clicks when looping)
    private static AudioClip CreateSirenClip()
    {
        const int sampleRate = 44100;
        const float toneLength = 0.6f;
        int samplesPerTone = Mathf.RoundToInt(sampleRate * toneLength);
        float[] frequencies = { 700f, 950f };
        float[] data = new float[samplesPerTone * 2];

        for (int i = 0; i < data.Length; i++)
        {
            int tone = i / samplesPerTone;
            float t = (i % samplesPerTone) / (float)sampleRate;
            float fade = Mathf.Clamp01(Mathf.Min(t, toneLength - t) / 0.02f);
            data[i] = Mathf.Sin(2f * Mathf.PI * frequencies[tone] * t) * fade * 0.3f;
        }

        AudioClip clip = AudioClip.Create("ProceduralSiren", data.Length, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}