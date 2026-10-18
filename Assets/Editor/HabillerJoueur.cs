using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class HabillerJoueur
{
    [MenuItem("Outils/Habiller Joueur Pompier")]
    static void Habiller()
    {
        GameObject joueur = GameObject.FindWithTag("Player");
        if (joueur == null) { Debug.LogError("Joueur introuvable !"); return; }

        Animator anim = joueur.GetComponentInChildren<Animator>();
        if (anim == null || !anim.isHuman) { Debug.LogError("Le robot doit etre en Humanoid (Rig) !"); return; }

        Transform tete = anim.GetBoneTransform(HumanBodyBones.Head);
        if (tete != null && tete.Find("CasquePompier") != null) { Debug.LogWarning("Deja habille !"); return; }

        Material jaune = MakeMat("PC_Jaune", new Color(1f, 0.85f, 0f));

        if (tete != null)
        {
            Piece(tete, PrimitiveType.Sphere, "CasquePompier", new Vector3(0, 0.08f, 0.02f), new Vector3(0.3f, 0.24f, 0.32f), jaune);
            Piece(tete, PrimitiveType.Cube, "VisierePompier", new Vector3(0, 0.07f, 0.17f), new Vector3(0.26f, 0.02f, 0.14f), jaune);
        }

        Bande(anim, HumanBodyBones.Spine, 0.32f, jaune);
        Bande(anim, HumanBodyBones.Chest, 0.34f, jaune);
        Bande(anim, HumanBodyBones.LeftUpperArm, 0.14f, jaune);
        Bande(anim, HumanBodyBones.RightUpperArm, 0.14f, jaune);
        Bande(anim, HumanBodyBones.LeftLowerArm, 0.12f, jaune);
        Bande(anim, HumanBodyBones.RightLowerArm, 0.12f, jaune);
        Bande(anim, HumanBodyBones.LeftUpperLeg, 0.17f, jaune);
        Bande(anim, HumanBodyBones.RightUpperLeg, 0.17f, jaune);
        Bande(anim, HumanBodyBones.LeftLowerLeg, 0.14f, jaune);
        Bande(anim, HumanBodyBones.RightLowerLeg, 0.14f, jaune);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Joueur habille en pompier !");
    }

    static void Bande(Animator anim, HumanBodyBones os, float diametre, Material mat)
    {
        Transform b = anim.GetBoneTransform(os);
        if (b == null) return;
        Vector3 milieu = b.position;
        if (b.childCount > 0) milieu = (b.position + b.GetChild(0).position) / 2f;

        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Object.DestroyImmediate(g.GetComponent<Collider>());
        g.name = "Bande_" + os;
        g.transform.SetParent(b, true);
        g.transform.position = milieu;
        g.transform.rotation = Quaternion.identity;
        g.transform.localScale = new Vector3(diametre, 0.025f, diametre);
        g.GetComponent<Renderer>().sharedMaterial = mat;
    }

    static GameObject Piece(Transform parent, PrimitiveType type, string nom, Vector3 pos, Vector3 taille, Material mat)
    {
        GameObject g = GameObject.CreatePrimitive(type);
        Object.DestroyImmediate(g.GetComponent<Collider>());
        g.name = nom;
        g.transform.SetParent(parent, false);
        g.transform.localPosition = pos;
        g.transform.localScale = taille;
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