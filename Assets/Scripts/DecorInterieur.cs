using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class DecorInterieur
{
    static Material rouge, rougeSombre, noir, blanc, jaune, bois, gris, vert, bleu, orange, verre, feuille;

    [MenuItem("Outils/Decorer Interieur Salle")]
    static void Creer()
    {
        if (GameObject.Find("DecorInterieur") != null)
        {
            Debug.LogWarning("Interieur deja decore ! Ne clique qu'une seule fois.");
            return;
        }

        rouge = Mat("I_Rouge", new Color(0.8f, 0.08f, 0.08f));
        rougeSombre = Mat("I_Tapis", new Color(0.55f, 0.1f, 0.12f));
        noir = Mat("I_Noir", new Color(0.08f, 0.08f, 0.08f));
        blanc = Mat("I_Blanc", new Color(0.95f, 0.95f, 0.95f));
        jaune = Mat("I_Jaune", new Color(1f, 0.85f, 0f));
        bois = Mat("I_Bois", new Color(0.55f, 0.36f, 0.18f));
        gris = Mat("I_Gris", new Color(0.55f, 0.58f, 0.6f));
        vert = Mat("I_Vert", new Color(0.1f, 0.65f, 0.25f));
        bleu = Mat("I_Bleu", new Color(0.15f, 0.4f, 0.9f));
        orange = Mat("I_Orange", new Color(1f, 0.45f, 0f));
        verre = Mat("I_Verre", new Color(0.6f, 0.85f, 0.95f));
        feuille = Mat("I_Feuille", new Color(0.15f, 0.55f, 0.18f));

        Transform root = new GameObject("DecorInterieur").transform;

        // ---------- TAPIS rouge du depart vers le feu ----------
        B(root, PrimitiveType.Cube, "Tapis", new Vector3(0, 0.025f, -3f), new Vector3(3f, 0.01f, 16f), rougeSombre);
        B(root, PrimitiveType.Cube, "TapisBordG", new Vector3(-1.6f, 0.03f, -3f), new Vector3(0.1f, 0.01f, 16f), jaune);
        B(root, PrimitiveType.Cube, "TapisBordD", new Vector3(1.6f, 0.03f, -3f), new Vector3(0.1f, 0.01f, 16f), jaune);

        // ---------- ZONE DE DANGER autour du feu (jaune/noir) ----------
        for (int i = 0; i < 24; i++)
        {
            float a = i * Mathf.PI * 2f / 24f;
            Vector3 pos = new Vector3(Mathf.Cos(a) * 4f, 0.04f, 8f + Mathf.Sin(a) * 4f);
            B(root, PrimitiveType.Cube, "Danger", pos, new Vector3(1.1f, 0.01f, 0.4f), i % 2 == 0 ? jaune : noir, new Vector3(0, -a * Mathf.Rad2Deg, 0));
        }

        // ---------- CONES DE SECURITE ----------
        Cone(root, new Vector3(-5.5f, 0, 6.5f));
        Cone(root, new Vector3(5.5f, 0, 6.5f));
        Cone(root, new Vector3(0f, 0, 3.2f));

        // ---------- FLECHES VERS LA SORTIE ----------
        for (float x = -3f; x >= -9f; x -= 1.5f)
            B(root, PrimitiveType.Cube, "FlecheTrait", new Vector3(x, 0.05f, -12.5f), new Vector3(0.9f, 0.01f, 0.25f), vert);
        B(root, PrimitiveType.Cube, "FlecheA", new Vector3(-10f, 0.05f, -12.2f), new Vector3(0.7f, 0.01f, 0.25f), vert, new Vector3(0, 45, 0));
        B(root, PrimitiveType.Cube, "FlecheB", new Vector3(-10f, 0.05f, -12.8f), new Vector3(0.7f, 0.01f, 0.25f), vert, new Vector3(0, -45, 0));

        // ---------- MOBILIER ----------
        Bureau(root, new Vector3(-13.3f, 0, -6f), -90f);
        Bureau(root, new Vector3(-13.3f, 0, 6f), -90f);
        Etagere(root, new Vector3(14.4f, 0, -12.5f), 90f);
        Etagere(root, new Vector3(14.4f, 0, -10f), 90f);
        Etagere(root, new Vector3(14.4f, 0, -2f), 90f);
        Armoire(root, new Vector3(14.55f, 0, -6f), 90f);
        Fontaine(root, new Vector3(14f, 0, 1.5f));

        // Tonneaux
        Tonneau(root, new Vector3(13.6f, 0, 5f), rouge);
        Tonneau(root, new Vector3(13.6f, 0, 6.2f), bleu);
        Tonneau(root, new Vector3(12.5f, 0, 5.6f), rouge);

        // Plantes dans les coins
        Plante(root, new Vector3(13.7f, 0, -13.7f));
        Plante(root, new Vector3(-13.7f, 0, 13.7f));
        Plante(root, new Vector3(-13.7f, 0, 0f));

        // ---------- FENETRES (mur gauche) ----------
        foreach (float z in new float[] { -9f, -3f, 3f, 9f })
        {
            B(root, PrimitiveType.Cube, "CadreFenetre", new Vector3(-14.7f, 2f, z), new Vector3(0.08f, 1.4f, 1.8f), blanc);
            B(root, PrimitiveType.Cube, "Vitre", new Vector3(-14.66f, 2f, z), new Vector3(0.06f, 1.2f, 1.6f), verre);
            B(root, PrimitiveType.Cube, "BarreH", new Vector3(-14.62f, 2f, z), new Vector3(0.05f, 0.06f, 1.6f), blanc);
            B(root, PrimitiveType.Cube, "BarreV", new Vector3(-14.62f, 2f, z), new Vector3(0.05f, 1.2f, 0.06f), blanc);
        }

        // ---------- MUR DU FOND : horloge + tableau d'affichage ----------
        B(root, PrimitiveType.Cylinder, "Horloge", new Vector3(-5f, 2.6f, 14.68f), new Vector3(0.8f, 0.04f, 0.8f), blanc, new Vector3(90, 0, 0));
        B(root, PrimitiveType.Cube, "AiguilleH", new Vector3(-5f, 2.68f, 14.64f), new Vector3(0.04f, 0.28f, 0.02f), noir);
        B(root, PrimitiveType.Cube, "AiguilleM", new Vector3(-4.9f, 2.6f, 14.64f), new Vector3(0.24f, 0.04f, 0.02f), noir);

        B(root, PrimitiveType.Cube, "Tableau", new Vector3(4.5f, 1.8f, 14.68f), new Vector3(1.8f, 1f, 0.06f), bois);
        B(root, PrimitiveType.Cube, "Papier1", new Vector3(4f, 1.9f, 14.64f), new Vector3(0.4f, 0.5f, 0.02f), blanc);
        B(root, PrimitiveType.Cube, "Papier2", new Vector3(4.6f, 1.75f, 14.64f), new Vector3(0.4f, 0.5f, 0.02f), jaune);
        B(root, PrimitiveType.Cube, "Papier3", new Vector3(5.1f, 1.9f, 14.64f), new Vector3(0.35f, 0.45f, 0.02f), orange);

        // ---------- MUR AVANT : plan d'evacuation + point de rassemblement ----------
        B(root, PrimitiveType.Cube, "PlanEvac", new Vector3(-5f, 1.8f, -14.72f), new Vector3(1.4f, 1f, 0.05f), blanc);
        B(root, PrimitiveType.Cube, "PlanVert", new Vector3(-5f, 1.8f, -14.69f), new Vector3(1.1f, 0.7f, 0.02f), vert);
        B(root, PrimitiveType.Sphere, "PlanPoint", new Vector3(-4.6f, 1.7f, -14.67f), new Vector3(0.12f, 0.12f, 0.02f), rouge);

        B(root, PrimitiveType.Cube, "Rassemblement", new Vector3(6f, 2.2f, -14.72f), new Vector3(1.2f, 1.2f, 0.05f), vert);
        B(root, PrimitiveType.Sphere, "RassTete", new Vector3(6f, 2.55f, -14.68f), new Vector3(0.22f, 0.22f, 0.03f), blanc);
        B(root, PrimitiveType.Cube, "RassCorps", new Vector3(6f, 2.1f, -14.68f), new Vector3(0.3f, 0.5f, 0.03f), blanc);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Interieur de la salle decore et sauvegarde !");
    }

    // ---------- Meubles ----------
    static Transform Groupe(Transform parent, string nom, Vector3 pos, float rotY)
    {
        GameObject g = new GameObject(nom);
        g.transform.SetParent(parent, false);
        g.transform.position = pos;
        g.transform.rotation = Quaternion.Euler(0, rotY, 0);
        return g.transform;
    }

    static void Bureau(Transform parent, Vector3 pos, float rotY)
    {
        Transform t = Groupe(parent, "Bureau", pos, rotY);
        B(t, PrimitiveType.Cube, "Plateau", new Vector3(0, 0.75f, 0), new Vector3(1.6f, 0.06f, 0.8f), bois);
        foreach (float x in new float[] { -0.72f, 0.72f })
            foreach (float z in new float[] { -0.32f, 0.32f })
                B(t, PrimitiveType.Cube, "Pied", new Vector3(x, 0.375f, z), new Vector3(0.06f, 0.75f, 0.06f), noir);
        B(t, PrimitiveType.Cube, "SocleEcran", new Vector3(0, 0.8f, 0.2f), new Vector3(0.2f, 0.04f, 0.2f), noir);
        B(t, PrimitiveType.Cube, "Ecran", new Vector3(0, 1.05f, 0.2f), new Vector3(0.6f, 0.38f, 0.04f), noir);
        B(t, PrimitiveType.Cube, "Affichage", new Vector3(0, 1.05f, 0.17f), new Vector3(0.54f, 0.32f, 0.02f), bleu);
        B(t, PrimitiveType.Cube, "Clavier", new Vector3(0, 0.79f, -0.1f), new Vector3(0.4f, 0.02f, 0.14f), gris);
        B(t, PrimitiveType.Cube, "Assise", new Vector3(0.1f, 0.45f, -0.85f), new Vector3(0.5f, 0.06f, 0.5f), noir);
        B(t, PrimitiveType.Cube, "Dossier", new Vector3(0.1f, 0.75f, -1.08f), new Vector3(0.5f, 0.6f, 0.06f), noir);
        B(t, PrimitiveType.Cylinder, "PiedChaise", new Vector3(0.1f, 0.22f, -0.85f), new Vector3(0.08f, 0.22f, 0.08f), gris);
    }

    static void Etagere(Transform parent, Vector3 pos, float rotY)
    {
        Transform t = Groupe(parent, "Etagere", pos, rotY);
        B(t, PrimitiveType.Cube, "Fond", new Vector3(0, 1f, 0.22f), new Vector3(1.8f, 2f, 0.04f), gris);
        B(t, PrimitiveType.Cube, "CoteG", new Vector3(-0.9f, 1f, 0f), new Vector3(0.04f, 2f, 0.5f), gris);
        B(t, PrimitiveType.Cube, "CoteD", new Vector3(0.9f, 1f, 0f), new Vector3(0.04f, 2f, 0.5f), gris);
        foreach (float y in new float[] { 0.1f, 0.7f, 1.3f, 1.95f })
            B(t, PrimitiveType.Cube, "Planche", new Vector3(0, y, 0), new Vector3(1.8f, 0.04f, 0.5f), gris);
        Material[] cols = { orange, jaune, bleu, rouge, blanc };
        int k = 0;
        foreach (float y in new float[] { 0.32f, 0.92f, 1.52f })
        {
            B(t, PrimitiveType.Cube, "Boite", new Vector3(-0.55f, y, 0), new Vector3(0.5f, 0.35f, 0.38f), cols[k++ % 5]);
            B(t, PrimitiveType.Cube, "Boite", new Vector3(0.05f, y - 0.03f, 0), new Vector3(0.45f, 0.28f, 0.38f), cols[k++ % 5]);
            B(t, PrimitiveType.Cube, "Boite", new Vector3(0.6f, y, 0), new Vector3(0.4f, 0.36f, 0.38f), cols[k++ % 5]);
        }
    }

    static void Armoire(Transform parent, Vector3 pos, float rotY)
    {
        Transform t = Groupe(parent, "ArmoireIncendie", pos, rotY);
        B(t, PrimitiveType.Cube, "Corps", new Vector3(0, 1.3f, -0.1f), new Vector3(0.9f, 1.1f, 0.3f), rouge);
        B(t, PrimitiveType.Cube, "Porte", new Vector3(0, 1.3f, -0.27f), new Vector3(0.75f, 0.9f, 0.03f), verre);
        B(t, PrimitiveType.Cylinder, "Derouleur", new Vector3(0, 1.3f, -0.2f), new Vector3(0.5f, 0.04f, 0.5f), noir, new Vector3(90, 0, 0));
        B(t, PrimitiveType.Cube, "Poignee", new Vector3(0.3f, 1.3f, -0.3f), new Vector3(0.05f, 0.2f, 0.03f), noir);
    }

    static void Fontaine(Transform parent, Vector3 pos)
    {
        Transform t = Groupe(parent, "Fontaine", pos, 0);
        B(t, PrimitiveType.Cube, "Corps", new Vector3(0, 0.45f, 0), new Vector3(0.4f, 0.9f, 0.4f), blanc);
        B(t, PrimitiveType.Cylinder, "Bonbonne", new Vector3(0, 1.1f, 0), new Vector3(0.3f, 0.2f, 0.3f), bleu);
    }

    static void Tonneau(Transform parent, Vector3 pos, Material m)
    {
        Transform t = Groupe(parent, "Tonneau", pos, 0);
        B(t, PrimitiveType.Cylinder, "Fut", new Vector3(0, 0.5f, 0), new Vector3(0.6f, 0.5f, 0.6f), m);
        B(t, PrimitiveType.Cylinder, "Bande", new Vector3(0, 0.5f, 0), new Vector3(0.62f, 0.04f, 0.62f), noir);
        B(t, PrimitiveType.Cylinder, "Couvercle", new Vector3(0, 1.01f, 0), new Vector3(0.5f, 0.02f, 0.5f), gris);
    }

    static void Plante(Transform parent, Vector3 pos)
    {
        Transform t = Groupe(parent, "Plante", pos, 0);
        B(t, PrimitiveType.Cylinder, "Pot", new Vector3(0, 0.25f, 0), new Vector3(0.5f, 0.25f, 0.5f), orange);
        B(t, PrimitiveType.Sphere, "Feuilles", new Vector3(0, 0.85f, 0), new Vector3(0.75f, 0.75f, 0.75f), feuille);
        B(t, PrimitiveType.Sphere, "Feuilles", new Vector3(0.2f, 1.2f, 0.1f), new Vector3(0.5f, 0.5f, 0.5f), feuille);
    }

    static void Cone(Transform parent, Vector3 pos)
    {
        Transform t = Groupe(parent, "Cone", pos, 0);
        B(t, PrimitiveType.Cube, "Base", new Vector3(0, 0.02f, 0), new Vector3(0.45f, 0.04f, 0.45f), orange);
        B(t, PrimitiveType.Cylinder, "Bas", new Vector3(0, 0.2f, 0), new Vector3(0.32f, 0.16f, 0.32f), orange);
        B(t, PrimitiveType.Cylinder, "Bande", new Vector3(0, 0.3f, 0), new Vector3(0.26f, 0.03f, 0.26f), blanc);
        B(t, PrimitiveType.Cylinder, "Haut", new Vector3(0, 0.45f, 0), new Vector3(0.18f, 0.12f, 0.18f), orange);
    }

    // ---------- Outils ----------
    static GameObject B(Transform parent, PrimitiveType type, string nom, Vector3 pos, Vector3 size, Material m, Vector3 euler = default(Vector3))
    {
        GameObject g = GameObject.CreatePrimitive(type);
        Object.DestroyImmediate(g.GetComponent<Collider>());
        g.name = nom;
        g.transform.SetParent(parent, false);
        g.transform.localPosition = pos;
        g.transform.localRotation = Quaternion.Euler(euler);
        g.transform.localScale = size;
        g.GetComponent<Renderer>().sharedMaterial = m;
        return g;
    }

    static Material Mat(string name, Color color)
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