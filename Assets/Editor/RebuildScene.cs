using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class RebuildScene
{
    [MenuItem("Outils/Reconstruire la scene")]
    static void Rebuild()
    {
        AddTag("Feu");
        AddTag("Extincteur");
        AddTag("Sortie");

        Material rouge = MakeMat("Rouge", Color.red);
        Material vert = MakeMat("Vert", Color.green);
        Material gris = MakeMat("Gris", Color.gray);

        // Decor
        Make(PrimitiveType.Plane, "Sol", new Vector3(0, 0, 0), new Vector3(3, 1, 3), gris);
        Make(PrimitiveType.Cube, "Mur_Fond", new Vector3(0, 1.5f, 15), new Vector3(30, 3, 0.5f), gris);
        Make(PrimitiveType.Cube, "Mur_Gauche", new Vector3(-15, 1.5f, 0), new Vector3(0.5f, 3, 30), gris);
        Make(PrimitiveType.Cube, "Mur_Droit", new Vector3(15, 1.5f, 0), new Vector3(0.5f, 3, 30), gris);

        // Feu, extincteur, sortie
        Make(PrimitiveType.Cube, "Feu", new Vector3(0, 0.5f, 8), new Vector3(2, 1, 2), rouge, "Feu");

        GameObject ext = Make(PrimitiveType.Cylinder, "Extincteur", new Vector3(3, 0.5f, -5), new Vector3(0.4f, 0.5f, 0.4f), gris, "Extincteur");
        CapsuleCollider cc = ext.GetComponent<CapsuleCollider>();
        cc.isTrigger = true;
        cc.radius = 1.5f;

        GameObject sortie = Make(PrimitiveType.Cube, "Sortie", new Vector3(-12, 1, -12), new Vector3(3, 2, 1), vert, "Sortie");
        sortie.GetComponent<BoxCollider>().isTrigger = true;

        // Joueur + camera
        GameObject joueur = Make(PrimitiveType.Capsule, "Joueur", new Vector3(0, 1, -10), Vector3.one, null, "Player");
        Object.DestroyImmediate(joueur.GetComponent<Collider>());
        joueur.AddComponent<CharacterController>();
        joueur.AddComponent<PlayerMovement>();

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.SetParent(joueur.transform);
            cam.transform.localPosition = new Vector3(0, 0.8f, 0);
            cam.transform.localRotation = Quaternion.identity;
        }

        // Zone du feu (trigger invisible)
        GameObject zone = new GameObject("ZoneFeu");
        zone.transform.position = new Vector3(0, 0.5f, 8);
        SphereCollider sc = zone.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 4;

        // Panneau d'alerte (desactive au depart)
        GameObject panneau = Make(PrimitiveType.Cube, "PanneauAlerte", new Vector3(0, 2.5f, -8), new Vector3(2, 1, 0.1f), rouge);
        panneau.SetActive(false);

        // Manager vide
        new GameObject("ScenarioManager");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Scene reconstruite et sauvegardee !");
    }

    static GameObject Make(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Material mat, string tag = null)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = scale;
        if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;
        if (tag != null) go.tag = tag;
        return go;
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

    static void AddTag(string tag)
    {
        SerializedObject tm = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tags = tm.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tm.ApplyModifiedProperties();
    }
}