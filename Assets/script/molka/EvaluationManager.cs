
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class EvaluationManager : MonoBehaviour
{
    [Header("Indicateurs")]
    public TMP_Text chronometreText;
    public TMP_Text scoreText;
    public TMP_Text etatText;

    [Header("Ecran de resultat")]
    public GameObject panneauResultat;
    public TMP_Text resultatTitre;
    public TMP_Text resultatDetails;

    [Header("Parametres")]
    public float tempsLimite = 120f;

    private float temps = 0f;
    private int score = 0;
    private bool termine = false;

    void Start()
    {
        InitialiserExercice();
    }

    void Update()
    {
        if (termine)
            return;

        temps += Time.deltaTime;

        int secondes = Mathf.FloorToInt(temps);

        if (chronometreText != null)
        {
            chronometreText.text = "Temps : " +
                (secondes / 60).ToString("00") + ":" +
                (secondes % 60).ToString("00");
        }

        if (temps >= tempsLimite)
        {
            FeuNonMaitrise();
            return;
        }

       
    }

    public void FeuMaitrise()
    {
        if (termine)
            return;

        termine = true;

        score = Mathf.Clamp(
            Mathf.RoundToInt(100f * (1f - temps / tempsLimite)),
            10,
            100
        );

        if (scoreText != null)
            scoreText.text = "Score : " + score;

        if (etatText != null)
            etatText.text = "Etat : Feu maitrise !";

        AfficherResultat(
            "REUSSITE",
            "Feu eteint en " + Mathf.FloorToInt(temps) +
            " s\nScore : " + score
        );
    }

    public void FeuNonMaitrise()
    {
        if (termine)
            return;

        termine = true;
        score = 0;

        if (scoreText != null)
            scoreText.text = "Score : 0";

        if (etatText != null)
            etatText.text = "Etat : Echec";

        AfficherResultat(
            "ECHEC",
            "Le feu n'a pas ete maitrise.\nScore : 0"
        );
    }

    private void AfficherResultat(string titre, string details)
    {
        if (resultatTitre != null)
            resultatTitre.text = titre;

        if (resultatDetails != null)
            resultatDetails.text = details;

        if (panneauResultat != null)
            panneauResultat.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Recommencer()
    {
        Debug.Log("BOUTON RESULTAT CLIQUE !");

        if (panneauResultat != null)
        {
            panneauResultat.SetActive(false);

            Debug.Log(
                "PanneauResultat desactive : " +
                !panneauResultat.activeSelf
            );
        }
        else
        {
            Debug.LogError(
                "PanneauResultat n'est pas assigne !"
            );
        }

        InitialiserExercice();
        var exercice = FindFirstObjectByType<ExerciceExtincteur>();
if (exercice != null)
    exercice.Reinitialiser();
    }

    private void InitialiserExercice()
    {
        temps = 0f;
        score = 0;
        termine = false;

        if (chronometreText != null)
            chronometreText.text = "Temps : 00:00";

        if (scoreText != null)
            scoreText.text = "Score : 0";

        if (etatText != null)
            etatText.text = "Etat : En cours";

        if (resultatTitre != null)
            resultatTitre.text = "";

        if (resultatDetails != null)
            resultatDetails.text = "";

        if (panneauResultat != null)
            panneauResultat.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Exercice reinitialise.");
    }
}
