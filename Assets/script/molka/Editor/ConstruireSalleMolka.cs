// A placer dans un dossier nomme "Editor", par exemple : Assets/script/molka/Editor/
// Utilisation : ouvrir molkaScene (hors Play), puis menu  Molka > Construire la salle
//
// Ce script ne touche QUE a l'objet "ExerciceMolka" (qu'il cree) et a deux materiaux
// dans Assets/script/molka. Il ne modifie pas Player, EvaluationSysteme, les cameras,
// le Plane, ni rien qui appartient aux autres etudiants.

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConstruireSalleMolka
{
    // ----- Dossiers (adapte-les si besoin) -----
    const string DossierMateriaux  = "Assets/script/molka";
    const string DossierRoom       = "Assets/room";
    const string DossierExtincteur = "Assets/OldExtinguisher";

    // ----- Reglages faciles a modifier -----
    const float SolY = 0f;                 // hauteur du Plane
    const float RotationMeubles = 180f;    // si un meuble est de dos, essaie 0

    [MenuItem("Molka/Construire la salle")]
    public static void Construire()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Molka", "Arrete le mode Play avant de construire la salle.", "OK");
            return;
        }

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "molkaScene")
        {
            EditorUtility.DisplayDialog("Molka",
                "Ouvre d'abord molkaScene (scene active actuelle : " + scene.name + ").", "OK");
            return;
        }

        var ancien = GameObject.Find("ExerciceMolka");
        if (ancien != null)
        {
            if (!EditorUtility.DisplayDialog("Molka",
                    "ExerciceMolka existe deja. Le remplacer par une nouvelle version ?",
                    "Remplacer", "Annuler"))
                return;
            Undo.DestroyObjectImmediate(ancien);
        }

        AssurerDossier(DossierMateriaux);

        var racine = new GameObject("ExerciceMolka");
        Undo.RegisterCreatedObjectUndo(racine, "Construire salle Molka");
        var t = racine.transform;

        // ---------- Materiaux ----------
        Shader shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");
        Material matMur = MaterielAsset("MurMolka", shaderLit, new Color(0.82f, 0.84f, 0.88f));

        Shader shaderFeu = Shader.Find("Sprites/Default");
        Material matFeu = MaterielAsset("FeuMolka", shaderFeu, Color.white);

        // ---------- Murs (piece de 8 x 12 m autour du joueur) ----------
        Mur(t, "Mur_Gauche", new Vector3(-2.5f, SolY + 1.5f, 0f),  new Vector3(0.2f, 3f, 12f), matMur);
        Mur(t, "Mur_Droite", new Vector3(5.5f,  SolY + 1.5f, 0f),  new Vector3(0.2f, 3f, 12f), matMur);
        Mur(t, "Mur_Arriere", new Vector3(1.5f, SolY + 1.5f, -6f), new Vector3(8f, 3f, 0.2f),  matMur);
        Mur(t, "Mur_Fond",   new Vector3(1.5f,  SolY + 1.5f, 6f),  new Vector3(8f, 3f, 0.2f),  matMur);

        // ---------- Mobilier (taille auto : hauteur cible en metres) ----------
        var table = Placer(t, "CR_Table", DossierRoom, 1.5f, 4.5f, RotationMeubles, 0.75f, SolY);
        float dessusTable = table != null ? Bornes(table).max.y : SolY + 0.75f;

        Placer(t, "CR_Chair", DossierRoom, 1.5f, 3.5f, RotationMeubles, 1.0f, SolY);
        Placer(t, "CR_Monitor", DossierRoom, 1.1f, 4.5f, RotationMeubles, 0.4f, dessusTable);
        Placer(t, "CR_Com", DossierRoom, 1.9f, 4.5f, RotationMeubles, 0.6f, dessusTable);
        Placer(t, "CR_Stora", DossierRoom, -1f, 1f, 90f, 1.0f, SolY);
        Placer(t, "CR_Lock", DossierRoom, -1f, 4.5f, 90f, 1.8f, SolY);
        Placer(t, "CR_Tool", DossierRoom, 3f, 2f, 20f, 0f, SolY);
        Placer(t, "CR_Wall", DossierRoom, 1.5f, 5.8f, 180f, 1.2f, SolY + 0.6f);

        // ---------- Extincteur (contre le mur droit, ~0,9 m du sol) ----------
        Placer(t, "", DossierExtincteur, 5.3f, -1.5f, -90f, 0.5f, SolY + 0.9f);

        // ---------- Feu ----------
        CreerFeu(t, new Vector3(1.5f, dessusTable + 0.05f, 4.4f), matFeu);

        EditorSceneManager.MarkSceneDirty(scene);
        Selection.activeGameObject = racine;
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Molka",
            "Salle construite dans ExerciceMolka.\n\nVerifie la Console pour les avertissements, " +
            "puis sauvegarde avec Ctrl + S.", "OK");
    }

    // ================= Outils =================

    static GameObject Placer(Transform parent, string debutNom, string dossier,
                             float x, float z, float rotY, float hauteurCible, float yBas)
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { dossier });
        string chemin = guids
            .Select(AssetDatabase.GUIDToAssetPath)
            .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p).StartsWith(debutNom));

        if (chemin == null)
        {
            Debug.LogWarning("[Molka] Prefab introuvable dans " + dossier + " : \"" + debutNom + "\"");
            return null;
        }

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(chemin);
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Undo.RegisterCreatedObjectUndo(go, "Placer " + go.name);

        go.transform.SetParent(parent, true);
        go.transform.position = new Vector3(x, 0f, z);
        go.transform.rotation = Quaternion.Euler(0f, rotY, 0f);
        go.transform.localScale = Vector3.one;

        if (hauteurCible > 0f)
        {
            float h = Bornes(go).size.y;
            if (h > 0.0001f)
                go.transform.localScale = Vector3.one * (hauteurCible / h);
        }

        // Pose l'objet exactement a la hauteur voulue (compense le pivot)
        float decalage = yBas - Bornes(go).min.y;
        go.transform.position += new Vector3(0f, decalage, 0f);

        return go;
    }

    static Bounds Bornes(GameObject go)
    {
        var rendus = go.GetComponentsInChildren<Renderer>();
        if (rendus.Length == 0)
            return new Bounds(go.transform.position, Vector3.zero);

        Bounds b = rendus[0].bounds;
        foreach (var r in rendus) b.Encapsulate(r.bounds);
        return b;
    }

    static void Mur(Transform parent, string nom, Vector3 pos, Vector3 echelle, Material mat)
    {
        var m = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Undo.RegisterCreatedObjectUndo(m, "Creer " + nom);
        m.name = nom;
        m.transform.SetParent(parent, true);
        m.transform.position = pos;
        m.transform.localScale = echelle;
        m.GetComponent<Renderer>().sharedMaterial = mat;
    }

    static void CreerFeu(Transform parent, Vector3 position, Material matFeu)
    {
        var feu = new GameObject("Feu");
        Undo.RegisterCreatedObjectUndo(feu, "Creer Feu");
        feu.transform.SetParent(parent, true);
        feu.transform.position = position;
        feu.transform.rotation = Quaternion.Euler(-90f, 0f, 0f); // emission vers le haut

        var ps = feu.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.loop = true;
        main.startLifetime = 1f;
        main.startSpeed = 1f;
        main.startSize = 0.3f;
        main.startColor = new Color(1f, 0.55f, 0.1f, 1f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.1f;

        var emission = ps.emission;
        emission.rateOverTime = 30f;

        var forme = ps.shape;
        forme.shapeType = ParticleSystemShapeType.Cone;
        forme.angle = 8f;
        forme.radius = 0.15f;

        var couleur = ps.colorOverLifetime;
        couleur.enabled = true;
        var degrade = new Gradient();
        degrade.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(1f, 0.8f, 0.2f), 0f),
                new GradientColorKey(new Color(1f, 0.3f, 0f), 0.5f),
                new GradientColorKey(new Color(0.4f, 0f, 0f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.8f, 0.6f),
                new GradientAlphaKey(0f, 1f)
            });
        couleur.color = degrade;

        var taille = ps.sizeOverLifetime;
        taille.enabled = true;
        taille.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.2f));

        feu.GetComponent<ParticleSystemRenderer>().sharedMaterial = matFeu;

        var lumiere = new GameObject("LumiereFeu");
        Undo.RegisterCreatedObjectUndo(lumiere, "Creer LumiereFeu");
        lumiere.transform.SetParent(feu.transform, true);
        lumiere.transform.position = position + Vector3.up * 0.4f;
        var l = lumiere.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = new Color(1f, 0.5f, 0.15f);
        l.intensity = 3f;
        l.range = 5f;
    }

    static Material MaterielAsset(string nom, Shader shader, Color couleur)
    {
        string chemin = DossierMateriaux + "/" + nom + ".mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(chemin);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, chemin);
        }
        mat.color = couleur;
        EditorUtility.SetDirty(mat);
        return mat;
    }

    static void AssurerDossier(string chemin)
    {
        if (AssetDatabase.IsValidFolder(chemin)) return;
        string parent = Path.GetDirectoryName(chemin).Replace('\\', '/');
        string nom = Path.GetFileName(chemin);
        if (!AssetDatabase.IsValidFolder(parent))
            AssurerDossier(parent);
        AssetDatabase.CreateFolder(parent, nom);
    }
}
