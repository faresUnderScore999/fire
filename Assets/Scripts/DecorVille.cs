using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class DecorVille
{
    static Material rouge, orange, blanc, noir, jaune, bleu, peau, herbe, route, vitre, beton, tronc, feuille;

    static void Mats()
    {
        rouge   = MakeMat("PC_Rouge",  new Color(0.8f, 0.05f, 0.05f));
        orange  = MakeMat("PC_Orange", new Color(1f, 0.45f, 0f));
        blanc   = MakeMat("PC_Blanc",  new Color(0.95f, 0.95f, 0.95f));
        noir    = MakeMat("PC_Noir",   new Color(0.08f, 0.08f, 0.08f));
        jaune   = MakeMat("PC_Jaune",  new Color(1f, 0.85f, 0f));
        bleu    = MakeMat("PC_Bleu",   new Color(0.1f, 0.3f, 0.9f));
        peau    = MakeMat("PC_Peau",   new Color(0.9f, 0.7f, 0.55f));
        herbe   = MakeMat("V_Herbe",   new Color(0.25f, 0.7f, 0.2f));
        route   = MakeMat("V_Route",   new Color(0.18f, 0.18f, 0.2f));
        vitre   = MakeMat("V_Vitre",   new Color(0.55f, 0.8f, 0.9f));
        beton   = MakeMat("V_Beton",   new Color(0.75f, 0.78f, 0.8f));
        tronc   = MakeMat("V_Tronc",   new Color(0.4f, 0.25f, 0.12f));
        feuille = MakeMat("V_Feuille", new Color(0.15f, 0.55f, 0.15f));
    }

    // ================= DECOR VILLE =================
    [MenuItem("Outils/Ajouter Decor Ville")]
    static void Ville()
    {
        if (GameObject.Find("DecorVille") != null)
        {
            Debug.LogWarning("Decor ville deja ajoute ! Ne clique qu'une seule fois.");
            return;
        }
        Mats();
        Transform p = new GameObject("DecorVille").transform;

        // Herbe tout autour (sous le sol)
        Box(p, PrimitiveType.Cube, "Herbe", new Vector3(0, -0.06f, 0), new Vector3(200, 0.1f, 200), herbe);

        // Route dans la salle (le long de X) + lignes blanches
        Box(p, PrimitiveType.Cube, "Route", new Vector3(0, 0.02f, -3), new Vector3(28, 0.02f, 4), route);
        for (int i = -6; i <= 6; i++)
            Box(p, PrimitiveType.Cube, "Ligne", new Vector3(i * 2f, 0.04f, -3), new Vector3(1f, 0.01f, 0.15f), blanc);

        // Passage pieton
        for (int i = 0; i < 6; i++)
            Box(p, PrimitiveType.Cube, "Zebra", new Vector3(6f, 0.04f, -4.5f + i * 0.6f), new Vector3(2.5f, 0.01f, 0.35f), blanc);

        // Immeubles derriere le mur du fond et sur les cotes
        Immeuble(p, -18, 24, 8, 14, 8);
        Immeuble(p, -6, 26, 8, 20, 8);
        Immeuble(p, 6, 24, 8, 16, 8);
        Immeuble(p, 18, 26, 8, 12, 8);

        // Arbres (dehors et dans les coins)
        Vector3[] arbres = {
            new Vector3(-20, 0, 5), new Vector3(-24, 0, -8), new Vector3(20, 0, 3),
            new Vector3(25, 0, -10), new Vector3(-22, 0, 16), new Vector3(23, 0, 17),
            new Vector3(-13, 0, 13), new Vector3(13, 0, -13), new Vector3(-30, 0, 0), new Vector3(30, 0, 8)
        };
        foreach (Vector3 a in arbres) Arbre(p, a);

        // Lampadaires
        Vector3[] lampes = { new Vector3(-13, 0, -6), new Vector3(13, 0, -6), new Vector3(-13, 0, 6), new Vector3(13, 0, 6) };
        foreach (Vector3 l in lampes)
        {
            Box(p, PrimitiveType.Cylinder, "Poteau", l + new Vector3(0, 2f, 0), new Vector3(0.12f, 2f, 0.12f), noir);
            Box(p, PrimitiveType.Sphere, "Lampe", l + new Vector3(0, 4.1f, 0), new Vector3(0.5f, 0.5f, 0.5f), jaune);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Decor ville ajoute et sauvegarde !");
    }

    static void Immeuble(Transform p, float x, float z, float w, float h, float d)
    {
        Box(p, PrimitiveType.Cube, "Immeuble", new Vector3(x, h / 2f, z), new Vector3(w, h, d), beton);
        Box(p, PrimitiveType.Cube, "Toit", new Vector3(x, h + 0.15f, z), new Vector3(w + 0.3f, 0.3f, d + 0.3f), noir);
        int cols = Mathf.FloorToInt(w / 2.2f);
        int rows = Mathf.FloorToInt((h - 1f) / 2.5f);
        for (int c = 0; c < cols; c++)
            for (int r = 0; r < rows; r++)
            {
                float wx = x - w / 2f + 1.1f + c * 2.2f;
                float wy = 2f + r * 2.5f;
                Box(p, PrimitiveType.Cube, "Fenetre", new Vector3(wx, wy, z - d / 2f - 0.03f), new Vector3(1.4f, 1.4f, 0.08f), vitre);
            }
    }

    static void Arbre(Transform p, Vector3 pos)
    {
        Box(p, PrimitiveType.Cylinder, "Tronc", pos + new Vector3(0, 1.2f, 0), new Vector3(0.4f, 1.2f, 0.4f), tronc);
        Box(p, PrimitiveType.Sphere, "Feuilles", pos + new Vector3(0, 3.0f, 0), new Vector3(2.4f, 2.2f, 2.4f), feuille);
        Box(p, PrimitiveType.Sphere, "Feuilles", pos + new Vector3(0.6f, 3.6f, 0.3f), new Vector3(1.6f, 1.5f, 1.6f), feuille);
    }

    // ================= SECOURISTE =================
    [MenuItem("Outils/Ameliorer Secouriste")]
    static void Secouriste()
    {
        GameObject pnj = GameObject.Find("PNJ");
        if (pnj == null) { Debug.LogError("Objet PNJ introuvable !"); return; }
        if (pnj.transform.Find("Torse") != null) { Debug.LogWarning("Secouriste deja ameliore !"); return; }
        Mats();

        // Enlever les anciennes pieces
        for (int i = pnj.transform.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(pnj.transform.GetChild(i).gameObject);

        // La capsule d'origine devient invisible (on garde son collider et le NavMesh Agent)
        pnj.GetComponent<Renderer>().enabled = false;
        Transform t = pnj.transform;

        // Jambes + bottes + bandes
        foreach (float x in new float[] { -0.15f, 0.15f })
        {
            Box(t, PrimitiveType.Capsule, "Jambe", new Vector3(x, -0.55f, 0), new Vector3(0.25f, 0.4f, 0.25f), orange, true);
            Box(t, PrimitiveType.Cylinder, "BandeJambe", new Vector3(x, -0.5f, 0), new Vector3(0.27f, 0.03f, 0.27f), jaune, true);
            Box(t, PrimitiveType.Cube, "Botte", new Vector3(x, -0.9f, 0.04f), new Vector3(0.26f, 0.2f, 0.36f), noir, true);
        }

        // Torse + bandes
        Box(t, PrimitiveType.Cube, "Torse", new Vector3(0, 0.05f, 0), new Vector3(0.62f, 0.7f, 0.35f), orange, true);
        Box(t, PrimitiveType.Cube, "BandeTorse1", new Vector3(0, 0.12f, 0), new Vector3(0.64f, 0.08f, 0.37f), jaune, true);
        Box(t, PrimitiveType.Cube, "BandeTorse2", new Vector3(0, -0.12f, 0), new Vector3(0.64f, 0.08f, 0.37f), jaune, true);

        // Bras + bandes + mains
        foreach (float x in new float[] { -0.42f, 0.42f })
        {
            Box(t, PrimitiveType.Capsule, "Bras", new Vector3(x, 0.05f, 0), new Vector3(0.18f, 0.32f, 0.18f), orange, true);
            Box(t, PrimitiveType.Cylinder, "BandeBras", new Vector3(x, 0.1f, 0), new Vector3(0.2f, 0.03f, 0.2f), jaune, true);
            Box(t, PrimitiveType.Sphere, "Main", new Vector3(x, -0.3f, 0), new Vector3(0.16f, 0.16f, 0.16f), peau, true);
        }

        // Tete + casque jaune + visiere
        Box(t, PrimitiveType.Sphere, "Tete", new Vector3(0, 0.65f, 0), new Vector3(0.38f, 0.38f, 0.38f), peau, true);
        Box(t, PrimitiveType.Sphere, "Casque", new Vector3(0, 0.76f, 0), new Vector3(0.46f, 0.3f, 0.46f), jaune, true);
        Box(t, PrimitiveType.Cube, "Visiere", new Vector3(0, 0.7f, 0.2f), new Vector3(0.4f, 0.03f, 0.2f), jaune, true);

        // Bouteille d'air dans le dos
        Box(t, PrimitiveType.Capsule, "Bouteille", new Vector3(0, 0.1f, -0.28f), new Vector3(0.2f, 0.28f, 0.2f), bleu, true);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Secouriste ameliore et sauvegarde !");
    }

    // ================= OUTILS =================
    // Cree une piece sans collider. local = true : position/taille relatives au parent
    static GameObject Box(Transform parent, PrimitiveType type, string name, Vector3 pos, Vector3 size, Material mat, bool local = false)
    {
        GameObject g = GameObject.CreatePrimitive(type);
        Object.DestroyImmediate(g.GetComponent<Collider>());
        g.name = name;
        g.transform.SetParent(parent, false);
        g.transform.localPosition = pos;
        g.transform.localScale = size;
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