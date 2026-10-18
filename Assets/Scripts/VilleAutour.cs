using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class VilleAutour
{
    static Material herbe, route, trottoir, ligne, noir, blanc, toit, vitre, porte, tronc, feuille, jaune, rouge, bleu, carrelage;
    static Material[] murs;

    [MenuItem("Outils/Creer Ville Autour")]
    static void Creer()
    {
        if (GameObject.Find("VilleAutour") != null)
        {
            Debug.LogWarning("Ville deja creee ! Ne clique qu'une seule fois.");
            return;
        }

        // Supprimer l'ancien decor ville s'il existe
        GameObject ancien = GameObject.Find("DecorVille");
        if (ancien != null) Object.DestroyImmediate(ancien);

        herbe = Mat("W_Herbe", new Color(0.3f, 0.75f, 0.25f));
        route = Mat("W_Route", new Color(0.2f, 0.2f, 0.22f));
        trottoir = Mat("W_Trottoir", new Color(0.72f, 0.72f, 0.7f));
        ligne = Mat("W_Ligne", new Color(0.95f, 0.95f, 0.95f));
        noir = Mat("W_Noir", new Color(0.08f, 0.08f, 0.08f));
        blanc = Mat("W_Blanc", new Color(0.95f, 0.95f, 0.95f));
        toit = Mat("W_Toit", new Color(0.7f, 0.2f, 0.15f));
        vitre = Mat("W_Vitre", new Color(0.55f, 0.8f, 0.92f));
        porte = Mat("W_Porte", new Color(0.4f, 0.25f, 0.12f));
        tronc = Mat("W_Tronc", new Color(0.4f, 0.26f, 0.13f));
        feuille = Mat("W_Feuille", new Color(0.15f, 0.55f, 0.18f));
        jaune = Mat("W_Jaune", new Color(1f, 0.85f, 0f));
        rouge = Mat("W_Rouge", new Color(0.8f, 0.08f, 0.08f));
        bleu = Mat("W_Bleu", new Color(0.15f, 0.35f, 0.85f));
        carrelage = Mat("W_Carrelage", new Color(0.35f, 0.37f, 0.4f));
        murs = new Material[] {
            Mat("W_Mur1", new Color(0.95f, 0.85f, 0.65f)),
            Mat("W_Mur2", new Color(0.85f, 0.55f, 0.45f)),
            Mat("W_Mur3", new Color(0.65f, 0.8f, 0.9f)),
            Mat("W_Mur4", new Color(0.9f, 0.9f, 0.75f)),
            Mat("W_Mur5", new Color(0.7f, 0.85f, 0.65f))
        };

        Transform p = new GameObject("VilleAutour").transform;

        // ---------- HERBE ----------
        B(p, PrimitiveType.Cube, "Herbe", new Vector3(0, -0.06f, 0), new Vector3(300, 0.1f, 300), herbe);

        // ---------- TROTTOIR INTERIEUR (contre les murs) ----------
        Anneau(p, 16.5f, 3f, 36f, 0.03f, trottoir, "TrottoirInt");
        // ---------- ROUTE ----------
        Anneau(p, 21f, 6f, 48f, 0.04f, route, "Route");
        // ---------- TROTTOIR EXTERIEUR ----------
        Anneau(p, 26f, 4f, 56f, 0.03f, trottoir, "TrottoirExt");

        // Lignes blanches au milieu de la route
        for (float i = -22f; i <= 22f; i += 3f)
        {
            B(p, PrimitiveType.Cube, "Ligne", new Vector3(i, 0.07f, 21f), new Vector3(1.4f, 0.01f, 0.2f), ligne);
            B(p, PrimitiveType.Cube, "Ligne", new Vector3(i, 0.07f, -21f), new Vector3(1.4f, 0.01f, 0.2f), ligne);
            B(p, PrimitiveType.Cube, "Ligne", new Vector3(21f, 0.07f, i), new Vector3(0.2f, 0.01f, 1.4f), ligne);
            B(p, PrimitiveType.Cube, "Ligne", new Vector3(-21f, 0.07f, i), new Vector3(0.2f, 0.01f, 1.4f), ligne);
        }

        // Passages pietons
        for (int k = 0; k < 6; k++)
        {
            B(p, PrimitiveType.Cube, "Zebra", new Vector3(-8f + k * 0.9f, 0.07f, 21f), new Vector3(0.5f, 0.01f, 5f), ligne);
            B(p, PrimitiveType.Cube, "Zebra", new Vector3(21f, 0.07f, 8f - k * 0.9f), new Vector3(5f, 0.01f, 0.5f), ligne);
        }

        // ---------- MAISONS ----------
        int n = 0;
        for (float x = -30f; x <= 30f; x += 10f)
        {
            Maison(p, new Vector3(x, 0, 32f), 0f, n++);
            Maison(p, new Vector3(x, 0, -32f), 180f, n++);
        }
        for (float z = -20f; z <= 20f; z += 10f)
        {
            Maison(p, new Vector3(32f, 0, z), 90f, n++);
            Maison(p, new Vector3(-32f, 0, z), -90f, n++);
        }

        // ---------- IMMEUBLES AU FOND ----------
        Immeuble(p, new Vector3(-24f, 0, 48f), 10, 22, 10, 0);
        Immeuble(p, new Vector3(-8f, 0, 50f), 10, 30, 10, 2);
        Immeuble(p, new Vector3(10f, 0, 48f), 10, 25, 10, 1);
        Immeuble(p, new Vector3(26f, 0, 50f), 10, 18, 10, 3);
        Immeuble(p, new Vector3(-48f, 0, 20f), 10, 20, 10, 2);
        Immeuble(p, new Vector3(48f, 0, -15f), 10, 24, 10, 0);

        // ---------- ARBRES ----------
        for (float i = -24f; i <= 24f; i += 8f)
        {
            Arbre(p, new Vector3(i, 0, 25f));
            Arbre(p, new Vector3(i, 0, -25f));
            Arbre(p, new Vector3(25f, 0, i));
            Arbre(p, new Vector3(-25f, 0, i));
        }

        // ---------- LAMPADAIRES ----------
        foreach (float i in new float[] { -12f, 0f, 12f })
        {
            Lampe(p, new Vector3(i, 0, 17f));
            Lampe(p, new Vector3(i, 0, -17f));
            Lampe(p, new Vector3(17f, 0, i));
            Lampe(p, new Vector3(-17f, 0, i));
        }

        // ---------- VOITURES ----------
        Voiture(p, new Vector3(-12f, 0.04f, 22.5f), 90f, rouge);
        Voiture(p, new Vector3(8f, 0.04f, 19.5f), -90f, bleu);
        Voiture(p, new Vector3(22.5f, 0.04f, -6f), 0f, jaune);
        Voiture(p, new Vector3(-19.5f, 0.04f, 10f), 180f, blanc);
        Voiture(p, new Vector3(10f, 0.04f, -22.5f), 90f, bleu);

        // ---------- AMELIORER LA SALLE ----------
        Transform s = new GameObject("DecorSalle").transform;

        // Lignes de carrelage
        for (float i = -12f; i <= 12f; i += 3f)
        {
            B(s, PrimitiveType.Cube, "LigneX", new Vector3(i, 0.012f, 0), new Vector3(0.06f, 0.005f, 30f), carrelage);
            B(s, PrimitiveType.Cube, "LigneZ", new Vector3(0, 0.012f, i), new Vector3(30f, 0.005f, 0.06f), carrelage);
        }
        // Plinthes noires
        B(s, PrimitiveType.Cube, "PlintheFond", new Vector3(0, 0.125f, 14.68f), new Vector3(29.5f, 0.25f, 0.1f), noir);
        B(s, PrimitiveType.Cube, "PlintheG", new Vector3(-14.68f, 0.125f, 0), new Vector3(0.1f, 0.25f, 29.5f), noir);
        B(s, PrimitiveType.Cube, "PlintheD", new Vector3(14.68f, 0.125f, 0), new Vector3(0.1f, 0.25f, 29.5f), noir);
        // Bande jaune de securite le long du mur du fond
        B(s, PrimitiveType.Cube, "BandeSecurite", new Vector3(0, 0.02f, 13.5f), new Vector3(29f, 0.01f, 0.3f), jaune);
        // Banc
        B(s, PrimitiveType.Cube, "BancAssise", new Vector3(-9f, 0.45f, 13.5f), new Vector3(3f, 0.1f, 0.6f), porte);
        B(s, PrimitiveType.Cube, "BancPied1", new Vector3(-10.2f, 0.2f, 13.5f), new Vector3(0.1f, 0.4f, 0.5f), noir);
        B(s, PrimitiveType.Cube, "BancPied2", new Vector3(-7.8f, 0.2f, 13.5f), new Vector3(0.1f, 0.4f, 0.5f), noir);
        // Boite de premiers secours
        B(s, PrimitiveType.Cube, "BoiteSecours", new Vector3(8f, 1.5f, 14.65f), new Vector3(0.8f, 0.6f, 0.1f), blanc);
        B(s, PrimitiveType.Cube, "CroixV", new Vector3(8f, 1.5f, 14.58f), new Vector3(0.1f, 0.4f, 0.04f), rouge);
        B(s, PrimitiveType.Cube, "CroixH", new Vector3(8f, 1.5f, 14.58f), new Vector3(0.4f, 0.1f, 0.04f), rouge);

        // Mur avant (ferme la salle, avec collider)
        GameObject avant = GameObject.CreatePrimitive(PrimitiveType.Cube);
        avant.name = "Mur_Avant";
        avant.transform.position = new Vector3(0, 1.5f, -15f);
        avant.transform.localScale = new Vector3(30f, 3f, 0.5f);
        Material brique = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/MurBrique.mat");
        avant.GetComponent<Renderer>().sharedMaterial = brique != null ? brique : carrelage;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Ville et salle ameliorees et sauvegardees !");
    }

    // ---------- Constructions ----------
    static void Anneau(Transform p, float centre, float largeur, float longueur, float y, Material m, string nom)
    {
        B(p, PrimitiveType.Cube, nom, new Vector3(0, y, centre), new Vector3(longueur, 0.04f, largeur), m);
        B(p, PrimitiveType.Cube, nom, new Vector3(0, y, -centre), new Vector3(longueur, 0.04f, largeur), m);
        B(p, PrimitiveType.Cube, nom, new Vector3(centre, y, 0), new Vector3(largeur, 0.04f, longueur), m);
        B(p, PrimitiveType.Cube, nom, new Vector3(-centre, y, 0), new Vector3(largeur, 0.04f, longueur), m);
    }

    static void Maison(Transform parent, Vector3 pos, float rotY, int i)
    {
        GameObject g = new GameObject("Maison");
        g.transform.SetParent(parent, false);
        g.transform.position = pos;
        g.transform.rotation = Quaternion.Euler(0, rotY, 0);
        Transform t = g.transform;

        float w = 7f, h = 4f + (i % 3) * 0.8f, d = 6f;
        Material mur = murs[i % murs.Length];

        B(t, PrimitiveType.Cube, "Corps", new Vector3(0, h / 2f, 0), new Vector3(w, h, d), mur);

        // Toit a deux pentes (cretes le long de X)
        float a = 35f;
        float L = (d / 2f) / Mathf.Cos(a * Mathf.Deg2Rad) + 0.4f;
        float yToit = h + (d / 4f) * Mathf.Tan(a * Mathf.Deg2Rad);
        GameObject t1 = B(t, PrimitiveType.Cube, "Toit", new Vector3(0, yToit, d / 4f), new Vector3(w + 0.6f, 0.2f, L), toit);
        t1.transform.localRotation = Quaternion.Euler(a, 0, 0);
        GameObject t2 = B(t, PrimitiveType.Cube, "Toit", new Vector3(0, yToit, -d / 4f), new Vector3(w + 0.6f, 0.2f, L), toit);
        t2.transform.localRotation = Quaternion.Euler(-a, 0, 0);

        // Facade (cote -Z, face a la salle) : porte + fenetres
        B(t, PrimitiveType.Cube, "Porte", new Vector3(0, 1.1f, -d / 2f - 0.03f), new Vector3(1.1f, 2.2f, 0.1f), porte);
        B(t, PrimitiveType.Sphere, "Poignee", new Vector3(0.35f, 1.1f, -d / 2f - 0.1f), new Vector3(0.12f, 0.12f, 0.12f), jaune);
        foreach (float x in new float[] { -2.2f, 2.2f })
        {
            B(t, PrimitiveType.Cube, "Fenetre", new Vector3(x, 2.2f, -d / 2f - 0.03f), new Vector3(1.3f, 1.2f, 0.1f), vitre);
            B(t, PrimitiveType.Cube, "Volet", new Vector3(x - 0.85f, 2.2f, -d / 2f - 0.04f), new Vector3(0.3f, 1.2f, 0.08f), blanc);
            B(t, PrimitiveType.Cube, "Volet", new Vector3(x + 0.85f, 2.2f, -d / 2f - 0.04f), new Vector3(0.3f, 1.2f, 0.08f), blanc);
        }
        // Cheminee
        B(t, PrimitiveType.Cube, "Cheminee", new Vector3(2f, h + 1.4f, 0.8f), new Vector3(0.7f, 1.4f, 0.7f), noir);
    }

    static void Immeuble(Transform parent, Vector3 pos, float w, float h, float d, int c)
    {
        GameObject g = new GameObject("Immeuble");
        g.transform.SetParent(parent, false);
        g.transform.position = pos;
        Transform t = g.transform;
        B(t, PrimitiveType.Cube, "Corps", new Vector3(0, h / 2f, 0), new Vector3(w, h, d), murs[c % murs.Length]);
        B(t, PrimitiveType.Cube, "Toit", new Vector3(0, h + 0.15f, 0), new Vector3(w + 0.4f, 0.3f, d + 0.4f), noir);
        for (int col = 0; col < 3; col++)
            for (int row = 0; row < (int)((h - 3f) / 3f); row++)
                B(t, PrimitiveType.Cube, "Fenetre", new Vector3(-3f + col * 3f, 3.5f + row * 3f, -d / 2f - 0.03f), new Vector3(1.6f, 1.6f, 0.1f), vitre);
        B(t, PrimitiveType.Cube, "Entree", new Vector3(0, 1.2f, -d / 2f - 0.03f), new Vector3(2f, 2.4f, 0.1f), porte);
    }

    static void Arbre(Transform p, Vector3 pos)
    {
        B(p, PrimitiveType.Cylinder, "Tronc", pos + new Vector3(0, 1f, 0), new Vector3(0.35f, 1f, 0.35f), tronc);
        B(p, PrimitiveType.Sphere, "Feuilles", pos + new Vector3(0, 2.8f, 0), new Vector3(2.2f, 2f, 2.2f), feuille);
        B(p, PrimitiveType.Sphere, "Feuilles", pos + new Vector3(0.5f, 3.4f, 0.3f), new Vector3(1.5f, 1.4f, 1.5f), feuille);
    }

    static void Lampe(Transform p, Vector3 pos)
    {
        B(p, PrimitiveType.Cylinder, "Poteau", pos + new Vector3(0, 2f, 0), new Vector3(0.12f, 2f, 0.12f), noir);
        B(p, PrimitiveType.Sphere, "Lampe", pos + new Vector3(0, 4.1f, 0), new Vector3(0.5f, 0.5f, 0.5f), jaune);
    }

    static void Voiture(Transform parent, Vector3 pos, float rotY, Material couleur)
    {
        GameObject g = new GameObject("Voiture");
        g.transform.SetParent(parent, false);
        g.transform.position = pos;
        g.transform.rotation = Quaternion.Euler(0, rotY, 0);
        Transform t = g.transform;
        B(t, PrimitiveType.Cube, "Caisse", new Vector3(0, 0.55f, 0), new Vector3(1.8f, 0.6f, 4f), couleur);
        B(t, PrimitiveType.Cube, "Cabine", new Vector3(0, 1.1f, -0.2f), new Vector3(1.6f, 0.5f, 2f), vitre);
        foreach (float x in new float[] { -0.95f, 0.95f })
            foreach (float z in new float[] { -1.2f, 1.2f })
            {
                GameObject r = B(t, PrimitiveType.Cylinder, "Roue", new Vector3(x, 0.35f, z), new Vector3(0.7f, 0.15f, 0.7f), noir);
                r.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
    }

    // ---------- Outils ----------
    static GameObject B(Transform parent, PrimitiveType type, string nom, Vector3 pos, Vector3 size, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(type);
        Object.DestroyImmediate(g.GetComponent<Collider>());
        g.name = nom;
        g.transform.SetParent(parent, false);
        g.transform.localPosition = pos;
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