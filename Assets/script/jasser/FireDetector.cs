using System;
using TMPro;
using UnityEngine;

public class FireDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private TMP_Text messageText;
    [Tooltip("Optional: a UI Image whose width shrinks/grows with the progress.")]
    [SerializeField] private RectTransform progressBar;

    [Header("Detection")]
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float viewAngle = 30f;        // half-angle of the view cone (degrees)
    [SerializeField] private float requiredTime = 1.5f;    // seconds of looking needed
    [SerializeField] private float decaySpeed = 1f;        // how fast progress drains when you look away
    [SerializeField] private float scanInterval = 0.25f;   // how often we refresh the list of fires

    [Header("Message")]
    [SerializeField] private string detectedMessage = "Danger détecté : incendie !";
    [SerializeField] private float messageDuration = 4f;

    public bool HasDetected { get; private set; }
    public float Progress => Mathf.Clamp01(timer / requiredTime);
    public event Action<FireController> FireDetected;

    private FireController[] fires = Array.Empty<FireController>();
    private float timer;
    private float nextScan;
    private float hideMessageAt;
    private int obstacleMask;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Walls and props block the view; the player, water, fire trigger and NPC don't
        obstacleMask = ~LayerMask.GetMask("Player", "Water", "Fire", "NPC", "Ignore Raycast");

        if (messageText != null) messageText.gameObject.SetActive(false);
        if (progressBar != null) progressBar.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateMessage();
        if (HasDetected) return;

        if (Time.time >= nextScan)
        {
            nextScan = Time.time + scanInterval;
            fires = FindObjectsByType<FireController>(FindObjectsSortMode.None);
        }

        FireController visible = FindVisibleFire();
        if (visible != null) timer += Time.deltaTime;
        else timer = Mathf.Max(0f, timer - decaySpeed * Time.deltaTime);

        UpdateProgressBar();

        if (visible != null && timer >= requiredTime) Detect(visible);
    }

    private FireController FindVisibleFire()
    {
        Vector3 eye = cameraTransform.position;

        foreach (FireController fire in fires)
        {
            if (fire == null || fire.State != FireController.FireState.Burning) continue;

            Vector3 target = fire.transform.position + Vector3.up * 0.5f;
            Vector3 toFire = target - eye;

            if (toFire.magnitude > maxDistance) continue;                                  // too far
            if (Vector3.Angle(cameraTransform.forward, toFire) > viewAngle) continue;      // outside the view cone
            if (Physics.Linecast(eye, target, obstacleMask, QueryTriggerInteraction.Ignore)) continue; // blocked

            return fire;
        }
        return null;
    }

    private void Detect(FireController fire)
    {
        HasDetected = true;
        if (progressBar != null) progressBar.gameObject.SetActive(false);

        if (messageText != null)
        {
            messageText.text = detectedMessage;
            messageText.gameObject.SetActive(true);
            hideMessageAt = Time.time + messageDuration;
        }

        FireDetected?.Invoke(fire);
    }

    private void UpdateProgressBar()
    {
        if (progressBar == null) return;

        bool show = timer > 0.01f;
        if (progressBar.gameObject.activeSelf != show) progressBar.gameObject.SetActive(show);
        progressBar.localScale = new Vector3(Progress, 1f, 1f);
    }

    private void UpdateMessage()
    {
        if (messageText != null && messageText.gameObject.activeSelf && Time.time >= hideMessageAt)
            messageText.gameObject.SetActive(false);
    }
}