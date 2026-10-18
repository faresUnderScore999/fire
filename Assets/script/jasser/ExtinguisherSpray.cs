using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMove))]
public class ExtinguisherSpray : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody waterPrefab;
    [SerializeField] private Transform nozzle;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private ParticleSystem sprayVfx;   // optional mist on the nozzle

    [Header("Input")]
    [SerializeField] private InputActionReference sprayActionReference;

    [Header("Jet")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float shotsPerSecond = 20f;
    [SerializeField] private float spread = 0.25f;
    [SerializeField] private float maxAimDistance = 25f;

    public bool IsSpraying { get; private set; }

    private PlayerMove player;
    private int aimMask;
    private float nextShot;

    private void Awake()
    {
        player = GetComponent<PlayerMove>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // The aim ray ignores the player, the water and the fire trigger
        aimMask = ~LayerMask.GetMask("Player", "Water", "Fire", "Ignore Raycast");
    }

    private void OnEnable()
    {
        if (sprayActionReference != null) sprayActionReference.action.Enable();
    }

    private void OnDisable()
    {
        if (sprayActionReference != null) sprayActionReference.action.Disable();
    }

    private void Update()
    {
        IsSpraying = player.ControlsEnabled
                     && Cursor.lockState == CursorLockMode.Locked
                     && sprayActionReference != null
                     && sprayActionReference.action.IsPressed();

        if (!IsSpraying)
        {
            if (sprayVfx != null && sprayVfx.isPlaying)
                sprayVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            return;
        }

        Vector3 dir = GetAimDirection();

        if (sprayVfx != null)
        {
            sprayVfx.transform.rotation = Quaternion.LookRotation(dir);
            if (!sprayVfx.isPlaying) sprayVfx.Play();
        }

        if (Time.time < nextShot) return;
        nextShot = Time.time + 1f / shotsPerSecond;

        Rigidbody ball = Instantiate(waterPrefab, nozzle.position, Quaternion.LookRotation(dir));
        ball.linearVelocity = dir * speed + Random.insideUnitSphere * spread;
    }

    // The jet leaves the nozzle and converges on what the crosshair points at
    private Vector3 GetAimDirection()
    {
        Vector3 aimPoint = cameraTransform.position + cameraTransform.forward * maxAimDistance;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit,
                maxAimDistance, aimMask, QueryTriggerInteraction.Ignore))
        {
            aimPoint = hit.point;
        }

        Vector3 dir = (aimPoint - nozzle.position).normalized;

        // Very close targets (a wall in your face) could flip the direction backward
        if (Vector3.Dot(dir, cameraTransform.forward) < 0.5f) dir = cameraTransform.forward;
        return dir;
    }
}