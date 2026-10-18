using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FeuRealiste
{
    [MenuItem("Outils/Creer Feu Realiste")]
    static void Creer()
    {
        GameObject feu = GameObject.FindWithTag("Feu");
        if (feu == null) { Debug.LogError("Objet Feu introuvable !"); return; }
        if (feu.transform.Find("FeuParticules") != null)
        {
            Debug.LogWarning("Feu deja cree ! Ne clique qu'une seule fois.");
            return;
        }

        // Enlever les anciennes flammes (spheres) et cacher le cube rouge
        for (int i = feu.transform.childCount - 1; i >= 0; i--)
        {
            Transform c = feu.transform.GetChild(i);
            if (c.name.StartsWith("Flamme")) Object.DestroyImmediate(c.gameObject);
        }
        Renderer r = feu.GetComponent<Renderer>();
        if (r != null) r.enabled = false;

        Texture2D tex = CreerTexture();
        Material matFeu = MakeMat("FeuMat", tex, new Color(1f, 1f, 1f, 1f), true);
        Material matFumee = MakeMat("FumeeMat", tex, new Color(0.25f, 0.25f, 0.25f, 0.5f), false);

        // ----- FLAMMES -----
        GameObject flammes = new GameObject("FeuParticules");
        flammes.transform.SetParent(feu.transform, false);
        flammes.transform.localPosition = new Vector3(0, 0.2f, 0);
        flammes.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        ParticleSystem ps = flammes.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = true;
        main.playOnAwake = true;
        main.duration = 5f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.5f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.8f, 1.8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.7f, 1.3f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0, 6.28f);
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        main.maxParticles = 300;

        var em = ps.emission;
        em.rateOverTime = 60;

        var sh = ps.shape;
        sh.shapeType = ParticleSystemShapeType.Cone;
        sh.angle = 8f;
        sh.radius = 0.3f;

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 0.95f, 0.5f), 0f),
                new GradientColorKey(new Color(1f, 0.5f, 0.05f), 0.4f),
                new GradientColorKey(new Color(0.8f, 0.1f, 0.0f), 1f) },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.9f, 0.15f),
                new GradientAlphaKey(0.5f, 0.7f),
                new GradientAlphaKey(0f, 1f) });
        col.color = g;

        var size = ps.sizeOverLifetime;
        size.enabled = true;
        AnimationCurve cs = new AnimationCurve(new Keyframe(0, 0.6f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0.1f));
        size.size = new ParticleSystem.MinMaxCurve(1f, cs);

        var rot = ps.rotationOverLifetime;
        rot.enabled = true;
        rot.z = new ParticleSystem.MinMaxCurve(-0.6f, 0.6f);

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.4f;
        noise.frequency = 0.8f;

        ParticleSystemRenderer pr = flammes.GetComponent<ParticleSystemRenderer>();
        pr.sharedMaterial = matFeu;
        pr.renderMode = ParticleSystemRenderMode.Billboard;

        // ----- FUMEE -----
        GameObject fumee = new GameObject("Fumee");
        fumee.transform.SetParent(feu.transform, false);
        fumee.transform.localPosition = new Vector3(0, 0.8f, 0);
        fumee.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        ParticleSystem sm = fumee.AddComponent<ParticleSystem>();
        var mm = sm.main;
        mm.loop = true;
        mm.startLifetime = new ParticleSystem.MinMaxCurve(2f, 3.5f);
        mm.startSpeed = new ParticleSystem.MinMaxCurve(0.6f, 1.2f);
        mm.startSize = new ParticleSystem.MinMaxCurve(1.2f, 2.2f);
        mm.simulationSpace = ParticleSystemSimulationSpace.Local;
        mm.scalingMode = ParticleSystemScalingMode.Hierarchy;
        mm.maxParticles = 100;

        var me = sm.emission;
        me.rateOverTime = 10;
        var ms = sm.shape;
        ms.shapeType = ParticleSystemShapeType.Cone;
        ms.angle = 12f;
        ms.radius = 0.3f;

        var mc = sm.colorOverLifetime;
        mc.enabled = true;
        Gradient gs = new Gradient();
        gs.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.3f, 0.3f, 0.3f), 0f), new GradientColorKey(new Color(0.1f, 0.1f, 0.1f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.45f, 0.2f), new GradientAlphaKey(0f, 1f) });
        mc.color = gs;

        var msz = sm.sizeOverLifetime;
        msz.enabled = true;
        msz.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 1.6f)));

        fumee.GetComponent<ParticleSystemRenderer>().sharedMaterial = matFumee;

        // ----- LUMIERE ORANGE -----
        GameObject lum = new GameObject("LumiereFeu");
        lum.transform.SetParent(feu.transform, false);
        lum.transform.localPosition = new Vector3(0, 1.5f, 0);
        Light l = lum.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = new Color(1f, 0.55f, 0.15f);
        l.intensity = 6f;
        l.range = 10f;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Feu realiste cree et sauvegarde !");
    }

    // Image de flamme douce (cercle flou) generee automatiquement
    static Texture2D CreerTexture()
    {
        string path = "Assets/Textures/FeuSoft.png";
        if (!Directory.Exists("Assets/Textures")) Directory.CreateDirectory("Assets/Textures");

        int n = 128;
        Texture2D t = new Texture2D(n, n, TextureFormat.RGBA32, false);
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float dx = (x - n / 2f) / (n / 2f);
                float dy = (y - n / 2f) / (n / 2f);
                float d = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
                float a = d * d * (3f - 2f * d);
                t.SetPixel(x, y, new Color(1, 1, 1, a));
            }
        File.WriteAllBytes(path, t.EncodeToPNG());
        AssetDatabase.ImportAsset(path);

        TextureImporter imp = AssetImporter.GetAtPath(path) as TextureImporter;
        if (imp != null)
        {
            imp.alphaIsTransparency = true;
            imp.mipmapEnabled = false;
            imp.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    static Material MakeMat(string name, Texture2D tex, Color color, bool additif)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            Shader s = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (s == null) s = Shader.Find("Legacy Shaders/Particles/Additive");
            m = new Material(s);
            AssetDatabase.CreateAsset(m, path);
        }

        m.SetTexture("_BaseMap", tex);
        m.SetColor("_BaseColor", color);
        m.SetFloat("_Surface", 1);                 // Transparent
        m.SetFloat("_ZWrite", 0);
        m.SetOverrideTag("RenderType", "Transparent");
        m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        if (additif)
        {
            m.SetFloat("_Blend", 2);               // Additive
            m.SetFloat("_SrcBlend", 1);
            m.SetFloat("_DstBlend", 1);
            m.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }
        else
        {
            m.SetFloat("_Blend", 0);               // Alpha
            m.SetFloat("_SrcBlend", 5);
            m.SetFloat("_DstBlend", 10);
        }
        m.renderQueue = 3000;
        EditorUtility.SetDirty(m);
        return m;
    }
}