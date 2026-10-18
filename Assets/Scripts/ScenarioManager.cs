using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public enum EtatScenario { Attente, Alerte, Intervention, Reussite, Echec }

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance;

    [Header("Etat")]
    public EtatScenario etat = EtatScenario.Attente;

    [Header("Reglages")]
    public float tempsMax = 60f;
    public float dureeAlerte = 3f;
    public float dureeExtinction = 3f;

    [Header("A glisser depuis la Hierarchy")]
    public TextMeshProUGUI texteEtat;
    public TextMeshProUGUI texteChrono;
    public GameObject panneauAlerte;

    [Header("Obstacles")]
    public GameObject obstaclePrefab;
    public int nombreObstacles = 5;
    List<GameObject> obstacles = new List<GameObject>();

    GameObject feu;
    GameObject extincteur;
    Vector3 feuScaleInitial;
    float feuIntensite = 1f;

    float chrono;
    float minuteurAlerte;
    bool aExtincteur;
    bool dansZoneExtincteur;
    bool dansZoneFeu;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        feu = GameObject.FindWithTag("Feu");
        extincteur = GameObject.FindWithTag("Extincteur");
        feuScaleInitial = feu.transform.localScale;
        ChangerEtat(EtatScenario.Attente);
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        switch (etat)
        {
            case EtatScenario.Attente:
                if (kb.spaceKey.wasPressedThisFrame)
                    ChangerEtat(EtatScenario.Alerte);
                break;

            case EtatScenario.Alerte:
                panneauAlerte.transform.Rotate(0, 120f * Time.deltaTime, 0);
                minuteurAlerte -= Time.deltaTime;
                if (minuteurAlerte <= 0f)
                    ChangerEtat(EtatScenario.Intervention);
                break;

            case EtatScenario.Intervention:
                UpdateIntervention(kb);
                break;

            case EtatScenario.Reussite:
            case EtatScenario.Echec:
                if (kb.rKey.wasPressedThisFrame)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }
    }

    void UpdateIntervention(Keyboard kb)
    {
        chrono -= Time.deltaTime;
        texteChrono.text = "Temps : " + Mathf.Max(chrono, 0).ToString("F1") + " s";
        panneauAlerte.transform.Rotate(0, 120f * Time.deltaTime, 0);

        if (chrono <= 0f)
        {
            ChangerEtat(EtatScenario.Echec);
            return;
        }

        if (!aExtincteur && dansZoneExtincteur)
        {
            aExtincteur = true;
            Destroy(extincteur);
            texteEtat.text = "Allez vers le feu et maintenez E";
        }

        if (aExtincteur && dansZoneFeu && kb.eKey.isPressed)
        {
            feuIntensite -= Time.deltaTime / dureeExtinction;
            feu.transform.localScale = feuScaleInitial * Mathf.Max(feuIntensite, 0f);

            if (feuIntensite <= 0f)
            {
                Destroy(feu);
                ChangerEtat(EtatScenario.Reussite);
            }
        }
    }

    public void ZoneEntree(string id, bool dedans)
    {
        if (id == "Extincteur") dansZoneExtincteur = dedans;
        if (id == "ZoneFeu") dansZoneFeu = dedans;
    }

    void ChangerEtat(EtatScenario nouvelEtat)
    {
        etat = nouvelEtat;

        switch (etat)
        {
            case EtatScenario.Attente:
                panneauAlerte.SetActive(false);
                texteEtat.text = "Appuyez sur ESPACE pour demarrer";
                texteChrono.text = "";
                break;

            case EtatScenario.Alerte:
                panneauAlerte.SetActive(true);
                minuteurAlerte = dureeAlerte;
                texteEtat.text = "ALERTE INCENDIE !";
                break;

            case EtatScenario.Intervention:
                chrono = tempsMax;
                SpawnObstacles();
                texteEtat.text = "Prenez l'extincteur";
                break;

            case EtatScenario.Reussite:
                panneauAlerte.SetActive(false);
                NettoyerObstacles();
                texteEtat.text = "FEU MAITRISE ! Appuyez sur R pour recommencer";
                break;

            case EtatScenario.Echec:
                panneauAlerte.SetActive(false);
                NettoyerObstacles();
                texteEtat.text = "TEMPS ECOULE ! Appuyez sur R pour recommencer";
                break;
        }
    }

    void SpawnObstacles()
    {
        if (obstaclePrefab == null) return;
        for (int i = 0; i < nombreObstacles; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-8f, 8f), 5f, Random.Range(-6f, 4f));
            obstacles.Add(Instantiate(obstaclePrefab, pos, Quaternion.identity));
        }
    }

    void NettoyerObstacles()
    {
        foreach (GameObject o in obstacles)
            if (o != null) Destroy(o);
        obstacles.Clear();
    }
}