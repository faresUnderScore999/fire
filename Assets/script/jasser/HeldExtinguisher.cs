using UnityEngine;
using UnityEngine.InputSystem;

public class HeldExtinguisher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private ExtinguisherSpray spray;

    [Header("Sway (lags behind mouse movement)")]
    [SerializeField] private float swayAmount = 0.0008f;   // meters per mouse pixel
    [SerializeField] private float maxSway = 0.04f;
    [SerializeField] private float smoothing = 10f;

    [Header("Walk bob")]
    [SerializeField] private float bobFrequency = 9f;
    [SerializeField] private float bobAmount = 0.012f;

    [Header("Spray feedback")]
    [SerializeField] private float recoil = 0.03f;
    [SerializeField] private float shake = 0.004f;

    private Vector3 restPosition;
    private Vector3 offset;
    private float bobTimer;

    private void Awake() => restPosition = transform.localPosition;

    private void Update()
    {
        // 1. Sway
        Vector2 mouse = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;
        Vector3 target = new Vector3(
            Mathf.Clamp(-mouse.x * swayAmount, -maxSway, maxSway),
            Mathf.Clamp(-mouse.y * swayAmount, -maxSway, maxSway),
            0f);

        // 2. Walk bob
        Vector3 planar = new Vector3(playerController.velocity.x, 0f, playerController.velocity.z);
        if (planar.magnitude > 0.1f && playerController.isGrounded)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            target += new Vector3(Mathf.Cos(bobTimer * 0.5f) * bobAmount, Mathf.Sin(bobTimer) * bobAmount, 0f);
        }
        else bobTimer = 0f;

        // 3. Recoil and shake while spraying
        if (spray != null && spray.IsSpraying)
            target += new Vector3(0f, 0f, -recoil) + Random.insideUnitSphere * shake;

        offset = Vector3.Lerp(offset, target, smoothing * Time.deltaTime);
        transform.localPosition = restPosition + offset;
    }
}