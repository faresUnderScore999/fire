using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class HabillagePC
{
    [MenuItem("Outils/Habiller Protection Civile")]
    static void Habiller()
    {
        if (GameObject.Find("CamionPC") != null)
        {
            Debug.LogWarning("Deja habille ! Ne clique qu'une seule fois.");
            return;
        }

        Material rouge  = MakeMat("PC_Rouge",  new Color(0.8f, 0.05f, 0.05f));
        Material orange = MakeMat("PC_Orange", new Color(1f, 0.45f, 0f));
        Material blanc  = MakeMat("PC_Blanc",  new Color(0.95f, 0.95f, 0.95f));
        Material noir   = MakeMat("PC_Noir",   new Color(0.08f, 0.08f, 0.08f));
        Material jaune  = MakeMat("PC_Jaune",  new Color(1f, 0.85f, 0f));
        Material bleu   = MakeMat("PC_Bleu",   new Color(0.1f, 0.3f, 0.9f));
        Material peau   = MakeMat("PC_Peau",   new Color(0.9f, 0.7f, 0.55f));

        // ---------- EXTINCTEUR ----------
        GameObject ext = GameObject.Find("Extincteur");
        if (ext != null)
        {
            ext.GetComponent<Renderer>().sharedMaterial = rouge;
            Part(ext.transform, PrimitiveType.Cylinder, "Capuchon", new Vector3(0, 0.55f, 0), new Vector3(0.16f, 0.05f, 0.16f), noir);
            Part(ext.transform, PrimitiveType.Cube,     "Poignee",  new Vector3(0, 0.63f, 0), new Vector3(0.3f, 0.05f, 0.06f), noir);
            Part(ext.transform, PrimitiveType.Cylinder, "Tuyau",    new Vector3(0.21f, 0.2f, 0), new Vector3(0.04f, 0.2f, 0.04f), noir);
            Part(ext.transform, PrimitiveType.Cube,     "Buse",     new Vector3(0.21f, 0f, 0), new Vector3(0.1f, 0.08f, 0.1f), noir);
            Part(ext.transform, PrimitiveType.Cube,     "Etiquette",new Vector3(0, 0.1f, 0.2f), new Vector3(0.26f, 0.22f, 0.02f), blanc);
            Part(ext.transform, PrimitiveType.Sphere,   "Manometre",new Vector3(0, 0.55f, 0.1f), new Vector3(0.1f, 0.1f, 0.1f), blanc);
            Part(ext.transform, PrimitiveType.Cylinder, "Base",     new Vector3(0, -0.3f, 0), new Vector3(0.41f, 0.02f, 0.41f), noir);
        }

        // ---------- PNJ SECOURISTE ----------
        GameObject pnj = GameObject.Find("PNJ");
        if (pnj != null)
        {
            pnj.GetComponent<Renderer>().sharedMaterial = orange;
            Part(pnj.transform, PrimitiveType.Sphere,   "Tete",    new Vector3(0, 0.72f, 0), new Vector3(0.5f, 0.5f, 0.5f), peau);
            Part(pnj.transform, PrimitiveType.Sphere,   "Casque",  new Vector3(0, 0.86f, 0), new Vector3(0.58f, 0.36f, 0.58f), blanc);
            Part(pnj.transform, PrimitiveType.Cylinder, "Visiere", new Vector3(0, 0.78f, 0), new Vector3(0.64f, 0.02f, 0.64f), blanc);
            Part(pnj.transform, PrimitiveType.Cylinder, "Bande1",  new Vector3(0, 0.1f, 0),  new Vector3(1.02f, 0.04f, 1.02f), jaune);
            Part(pnj.transform, PrimitiveType.Cylinder, "Bande2",  new Vector3(0, -0.2f, 0), new Vector3(1.02f, 0.04f, 1.02f), jaune);
            Part(pnj.transform, PrimitiveType.Capsule,  "BrasG",   new Vector3(-0.58f, 0.1f, 0), new Vector3(0.22f, 0.3f, 0.22f), orange);
            Part(pnj.transform, PrimitiveType.Capsule,  "BrasD",   new Vector3(0.58f, 0.1f, 0),  new Vector3(0.22f, 0.3f, 0.22f), orange);
            Part(pnj.transform, PrimitiveType.Cube,     "Sac",     new Vector3(0, 0.1f, -0.5f),  new Vector3(0.5f, 0.6f, 0.25f), bleu);
        }

        // ---------- FEU (flammes) ----------
        GameObject feu = GameObject.Find("Feu");
        if (feu != null)
        {
            Part(feu.transform, PrimitiveType.Sphere, "Flamme1", new Vector3(0, 0.6f, 0),        new Vector3(1.0f, 1.2f, 1.0f), orange);
            Part(feu.transform, PrimitiveType.Sphere, "Flamme2", new Vector3(0.5f, 0.5f, 0.3f),  new Vector3(0.6f, 0.8f, 0.6f), jaune);
            Part(feu.transform, PrimitiveType.Sphere, "Flamme3", new Vector3(-0.5f, 0.5f, -0.3f),new Vector3(0.6f, 0.8f, 0.6f), jaune);
        }

        // ---------- DECOR ----------
        GameObject decor = new GameObject("Decor");

        // Affiche premiers secours (mur gauche)
        Part(decor.transform, PrimitiveType.Cube, "AfficheFond",  new Vector3(-14.7f, 2f, 0), new Vector3(0.1f, 2f, 2f), blanc);
        Part(decor.transform, PrimitiveType.Cube, "CroixV",       new Vector3(-14.68f, 2f, 0), new Vector3(0.12f, 1.2f, 0.4f), rouge);
        Part(decor.transform, PrimitiveType.Cube, "CroixH",       new Vector3(-14.68f, 2f, 0), new Vector3(0.12f, 0.4f, 1.2f), rouge);

        // Panneau Protection Civile (mur du fond)
        Part(decor.transform, PrimitiveType.Cube, "PanneauPC",    new Vector3(0, 2.2f, 14.7f), new Vector3(4f, 1.2f, 0.1f), orange);
        Part(decor.transform, PrimitiveType.Cube, "PanneauBande", new Vector3(0, 2.2f, 14.64f), new Vector3(3.6f, 0.2f, 0.1f), blanc);

        // Camion Protection Civile
        GameObject camion = new GameObject("CamionPC");
        camion.transform.position = new Vector3(11, 0, 11);
        Part(camion.transform, PrimitiveType.Cube, "Chassis",  new Vector3(0, 0.9f, 0),     new Vector3(2.8f, 1.2f, 6f), rouge);
        Part(camion.transform, PrimitiveType.Cube, "Bande",    new Vector3(0, 0.9f, 0),     new Vector3(2.82f, 0.2f, 6.02f), jaune);
        Part(camion.transform, PrimitiveType.Cube, "Cabine",   new Vector3(0, 1.95f, 2f),   new Vector3(2.8f, 0.9f, 1.8f), blanc);
        Part(camion.transform, PrimitiveType.Cube, "Gyrophare",new Vector3(0, 2.5f, 2f),    new Vector3(1.5f, 0.15f, 0.3f), bleu);
        float[] xs = { -1.5f, 1.5f };
        float[] zs = { -2f, 2f };
        foreach (float x in xs)
            foreach (float z in zs)
            {
                GameObject r = Part(camion.transform, PrimitiveType.Cylinder, "Roue", new Vector3(x, 0.4f, z), new Vector3(0.8f, 0.15f, 0.8f), noir);
                r.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }

        // ---------- PREFAB OBSTACLE (caisse de secours) ----------
        string path = "Assets/Prefabs/Obstacle.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            if (root.transform.childCount == 0)
            {
                root.GetComponent<Renderer>().sharedMaterial = orange;
                Part(root.transform, PrimitiveType.Cube, "Bande", Vector3.zero, new Vector3(1.02f, 0.25f, 1.02f), jaune);
                Part(root.transform, PrimitiveType.Cube, "Croix", new Vector3(0, 0, 0.5f), new Vector3(0.5f, 0.15f, 0.02f), blanc);
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            PrefabUtility.UnloadPrefabContents(root);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Scene habillee Protection Civile et sauvegardee !");
    }

    // Ajoute une piece enfant (sans collider) avec une taille reelle en metres
    static GameObject Part(Transform parent, PrimitiveType type, string name, Vector3 offset, Vector3 size, Material mat)
    {
        GameObject g = GameObject.CreatePrimitive(type);
        Object.DestroyImmediate(g.GetComponent<Collider>());
        g.name = name;
        g.transform.SetParent(parent, false);
        Vector3 s = parent.lossyScale;
        g.transform.localPosition = new Vector3(offset.x / s.x, offset.y / s.y, offset.z / s.z);
        g.transform.localScale = new Vector3(size.x / s.x, size.y / s.y, size.z / s.z);
        g.GetComponent<Renderer>().sharedMaterial = mat;
        return g;
    }

    static Material MakeMat(string name, Color color)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(m, path);
        }
        m.SetColor("_BaseColor", color);
        EditorUtility.SetDirty(m);
        return m;
    }
}