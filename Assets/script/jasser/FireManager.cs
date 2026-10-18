using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FireManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private FireController firePrefab;

    [Header("UI")]
    [SerializeField] private Slider intensityBar;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button restartButton;

    [Header("Rules")]
    [SerializeField] private float timeLimit = 60f;
    [SerializeField] private float spreadCheckInterval = 8f;
    [SerializeField] private float spreadIntensity = 0.9f;
    [SerializeField] private int maxFires = 2;

    private readonly List<FireController> fires = new List<FireController>();
    private float startTime;
    private float nextSpreadCheck;
    private bool finished;

    private void Start()
    {
        startTime = Time.time;
        nextSpreadCheck = startTime + spreadCheckInterval;

        restartButton.gameObject.SetActive(false);
        restartButton.onClick.AddListener(Restart);

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("FireSpawn");
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No object tagged 'FireSpawn' in the scene.");
            return;
        }

        Spawn(spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position);
        stateText.text = "Feu en cours";
    }

    private void Update()
    {
        if (finished)
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) Restart();
            return;
        }

        float remaining = Mathf.Max(0f, timeLimit - (Time.time - startTime));
        timerText.text = $"Temps : {remaining:0} s";

        float strongest = 0f;
        foreach (FireController f in fires)
            if (f != null && f.intensity > strongest) strongest = f.intensity;
        intensityBar.value = strongest;

        if (remaining <= 0f) { Finish(false); return; }

        if (Time.time >= nextSpreadCheck)
        {
            nextSpreadCheck = Time.time + spreadCheckInterval;
            TrySpread();
        }
    }

    private void Spawn(Vector3 position)
    {
        FireController fire = Instantiate(firePrefab, position, Quaternion.identity);
        fire.OnControlled += HandleControlled;
        fires.Add(fire);
    }

    // If a fire is raging, a new one starts near a random flammable object
    private void TrySpread()
    {
        if (fires.Count == 0 || fires.Count >= maxFires) return;

        bool raging = false;
        foreach (FireController f in fires)
            if (f.intensity >= spreadIntensity) raging = true;
        if (!raging) return;

        GameObject[] flammables = GameObject.FindGameObjectsWithTag("Flammable");
        if (flammables.Length == 0) return;

        Transform target = flammables[Random.Range(0, flammables.Length)].transform;
        Vector3 pos = new Vector3(target.position.x, 0f, target.position.z);

        foreach (FireController f in fires)
            if (Vector3.Distance(f.transform.position, pos) < 1.5f) return;

        Spawn(pos);
    }

    private void HandleControlled(FireController fire)
    {
        fires.Remove(fire);
        if (!finished && fires.Count == 0) Finish(true);
    }

    private void Finish(bool success)
    {
        finished = true;
        float elapsed = Time.time - startTime;
        stateText.text = success ? $"Feu maîtrisé en {elapsed:0} s" : "Échec : feu non maîtrisé";
        if (success) intensityBar.value = 0f;

        restartButton.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FindAnyObjectByType<PlayerMove>().ControlsEnabled = false;
    }

    private void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}