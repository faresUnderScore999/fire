using UnityEngine;
using UnityEngine.InputSystem;

// A placer dans Assets/script/molka/ (PAS dans le dossier Editor).
// A mettre sur un objet vide "LogiqueExercice", enfant de ExerciceMolka.
//
// Deroulement :
//   1. Le joueur s'approche de l'extincteur et appuie sur E pour le prendre.
//   2. Il s'approche du feu et maintient le clic gauche pour l'eteindre.
//   3. Quand le feu est eteint, EvaluationManager.FeuMaitrise() est appele.
public class ExerciceExtincteur : MonoBehaviour
{
    [Header("References (trouvees automatiquement si vides)")]
    public Transform joueur;
    public Transform extincteur;
    public ParticleSystem feu;
    public Light lumiereFeu;
    public EvaluationManager evaluation;

    [Header("Parametres")]
    public float distancePrise = 2.5f;        // distance pour prendre l'extincteur
    public float distanceExtinction = 3.5f;   // distance max pour eteindre le feu
    public float dureeExtinction = 3f;        // secondes de clic maintenu necessaires
    public Vector3 positionEnMain = new Vector3(0.35f, 1.0f, 0.45f); // dans le repere du joueur

    private float vie = 1f;
    private bool extincteurPris;
    private bool feuEteint;

    private float tauxEmissionInitial = 30f;
    private float intensiteInitiale = 3f;

    private Transform parentInitial;
    private Vector3 positionInitiale;
    private Quaternion rotationInitiale;

    void Start()
    {
        if (joueur == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) joueur = go.transform;
        }

        if (evaluation == null)
            evaluation = FindFirstObjectByType<EvaluationManager>();

        if (feu == null)
        {
            var go = GameObject.Find("Feu");
            if (go != null) feu = go.GetComponent<ParticleSystem>();
        }

        if (lumiereFeu == null && feu != null)
            lumiereFeu = feu.GetComponentInChildren<Light>();

        if (extincteur == null)
            extincteur = ChercherExtincteur();

        if (feu != null)
            tauxEmissionInitial = feu.emission.rateOverTime.constant;
        if (lumiereFeu != null)
            intensiteInitiale = lumiereFeu.intensity;

        if (extincteur != null)
        {
            parentInitial = extincteur.parent;
            positionInitiale = extincteur.position;
            rotationInitiale = extincteur.rotation;
        }
        else
        {
            Debug.LogWarning("[Molka] Extincteur introuvable : glisse-le dans le champ Extincteur du script.");
        }

        if (joueur == null) Debug.LogWarning("[Molka] Joueur introuvable (tag Player).");
        if (feu == null) Debug.LogWarning("[Molka] Objet Feu introuvable.");
        if (evaluation == null) Debug.LogWarning("[Molka] EvaluationManager introuvable.");

        Reinitialiser();
    }

    // Appelee au demarrage et par EvaluationManager.Recommencer()
    public void Reinitialiser()
    {
        vie = 1f;
        extincteurPris = false;
        feuEteint = false;

        if (extincteur != null)
        {
            extincteur.SetParent(parentInitial, true);
            extincteur.position = positionInitiale;
            extincteur.rotation = rotationInitiale;
        }

        if (feu != null)
        {
            var emission = feu.emission;
            emission.rateOverTime = tauxEmissionInitial;
            feu.Play();
        }

        if (lumiereFeu != null)
        {
            lumiereFeu.enabled = true;
            lumiereFeu.intensity = intensiteInitiale;
        }
    }

    void Update()
    {
        if (feuEteint || joueur == null || feu == null)
            return;

        var clavier = Keyboard.current;
        var souris = Mouse.current;

        if (!extincteurPris)
        {
            if (extincteur != null
                && Distance(joueur, extincteur) <= distancePrise
                && clavier != null
                && clavier.eKey.wasPressedThisFrame)
            {
                Prendre();
            }
            return;
        }

        bool tire = souris != null && souris.leftButton.isPressed;

        if (tire && Distance(joueur, feu.transform) <= distanceExtinction)
        {
            vie -= Time.deltaTime / dureeExtinction;
            AppliquerVie();

            if (vie <= 0f)
                Eteindre();
        }
    }

    private void Prendre()
    {
        extincteurPris = true;
        extincteur.SetParent(joueur, true);
        extincteur.localPosition = positionEnMain;
        extincteur.localRotation = Quaternion.identity;
    }

    private void AppliquerVie()
    {
        float v = Mathf.Clamp01(vie);

        var emission = feu.emission;
        emission.rateOverTime = tauxEmissionInitial * v;

        if (lumiereFeu != null)
            lumiereFeu.intensity = intensiteInitiale * v;
    }

    private void Eteindre()
    {
        feuEteint = true;
        feu.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (lumiereFeu != null)
            lumiereFeu.enabled = false;

        if (evaluation != null)
            evaluation.FeuMaitrise();
    }

    private static float Distance(Transform a, Transform b)
    {
        Vector3 d = a.position - b.position;
        d.y = 0f;
        return d.magnitude;
    }

    private Transform ChercherExtincteur()
    {
        var racine = GameObject.Find("ExerciceMolka");
        if (racine == null) return null;

        foreach (Transform enfant in racine.GetComponentsInChildren<Transform>())
        {
            if (enfant.name.ToLower().Contains("xting"))
                return enfant;
        }
        return null;
    }

    // Petits messages d'aide a l'ecran
    void OnGUI()
    {
        if (feuEteint || joueur == null || feu == null)
            return;

        string message = null;

        if (!extincteurPris)
        {
            if (extincteur != null && Distance(joueur, extincteur) <= distancePrise)
                message = "Appuyez sur E pour prendre l'extincteur";
        }
        else if (Distance(joueur, feu.transform) <= distanceExtinction)
        {
            message = "Maintenez le clic gauche pour eteindre le feu";
        }
        else
        {
            message = "Approchez-vous du feu";
        }

        if (message == null)
            return;

        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 26,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(0, Screen.height - 110, Screen.width, 50), message, style);
    }
}
