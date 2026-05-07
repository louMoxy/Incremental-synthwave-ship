using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RandomDrift : MonoBehaviour
{
    [Header("Linear Drift")]
    [SerializeField] float minStartSpeed = 0.2f;
    [SerializeField] float maxStartSpeed = 0.8f;

    [Tooltip("How much the direction slowly changes over time.")]
    [SerializeField] float directionChangeSpeed = 0.2f;

    [Header("Rotation")]
    [SerializeField] float minSpin = 5f;
    [SerializeField] float maxSpin = 20f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
    }
     
    void Start()
    {
        // Random constant starting velocity
        Vector2 dir = Random.insideUnitCircle.normalized;
        float speed = Random.Range(minStartSpeed, maxStartSpeed);
        rb.linearVelocity = dir * speed;

        // Random constant spin
        rb.angularVelocity = Random.Range(minSpin, maxSpin) * (Random.value > 0.5f ? 1 : -1);
    }

    void FixedUpdate()
    {
        // VERY subtle smooth drift direction change
        Vector2 v = rb.linearVelocity;
        float speed = v.magnitude;

        if (speed > 0.01f)
        {
            // rotate velocity slightly over time
            float angle = directionChangeSpeed * Time.fixedDeltaTime * 50f;
            rb.linearVelocity = Quaternion.Euler(0, 0, angle) * v;
        }
    }
}