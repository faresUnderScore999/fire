using System.IO;
using UnityEditor;
using UnityEngine;

public static class TextureGenerator
{
    [MenuItem("Tools/Generate Fire Textures")]
    public static void Generate()
    {
        const int size = 256;
        var flame = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var soft = new Texture2D(size, size, TextureFormat.RGBA32, false);

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float u = x / (float)(size - 1) * 2f - 1f;   // -1..1
                float v = y / (float)(size - 1);             // 0 bottom .. 1 top

                // Flame: wide at the base, narrowing to a wobbly tip
                float halfWidth = Mathf.Lerp(0.7f, 0f, Mathf.Pow(v, 0.8f));
                float wobble = (Mathf.PerlinNoise(v * 4f, 3.7f) - 0.5f) * 0.25f * v;
                float d = Mathf.Abs(u - wobble) / Mathf.Max(halfWidth, 0.0001f);
                float f = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(1f - d));
                f *= Mathf.SmoothStep(0f, 0.08f, v) * (1f - Mathf.SmoothStep(0.85f, 1f, v));
                flame.SetPixel(x, y, new Color(f, f, f, f));

                // Soft round blob for smoke and embers
                float r = Mathf.Sqrt(u * u + (v * 2f - 1f) * (v * 2f - 1f));
                float s = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(1f - r));
                soft.SetPixel(x, y, new Color(s, s, s, s));
            }

        Save(flame, "Assets/FlameShape.png");
        Save(soft, "Assets/SoftCircle.png");
        AssetDatabase.Refresh();
    }

    private static void Save(Texture2D tex, string path)
    {
        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
    }
}