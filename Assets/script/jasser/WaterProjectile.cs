using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float extinguishPower = 0.012f;
    [SerializeField] private float gravityScale = 0.3f;
    [SerializeField] private float pushMultiplier = 3f;

    private Rigidbody rb;
    private Vector3 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // we apply a reduced gravity ourselves
    }

    private void Start() => Destroy(gameObject, lifetime);

    private void FixedUpdate()
    {
        rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
        lastVelocity = rb.linearVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        FireController fire = other.GetComponentInParent<FireController>();
        if (fire == null) return;

        fire.ApplyExtinguisher(extinguishPower);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            collision.rigidbody.AddForceAtPosition(
                lastVelocity * rb.mass * pushMultiplier,
                collision.GetContact(0).point,
                ForceMode.Impulse);
        }
        Destroy(gameObject);
    }
}