using UnityEngine;

public class Interceptor : MonoBehaviour
{
    enum State { Chase, Overshoot, Reposition }
    State currentState = State.Chase;

    [Header("Movement")]
    [SerializeField] float chaseForce = 15f;
    [SerializeField] float maxSpeed = 12f;
    [SerializeField] float overshootThreshold = 1.5f;
    [SerializeField] float overshootDuration = 0.6f;

    Rigidbody2D rb;
    Transform player;
    float overshootTimer;
    Vector2 repositionOffset;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<Flying>().transform;

        // Randomise so interceptors desync from each other
        chaseForce = Random.Range(chaseForce * 0.8f, chaseForce * 1.2f);
        maxSpeed = Random.Range(maxSpeed * 0.8f, maxSpeed * 1.2f);
        overshootDuration = Random.Range(0.4f, 1.0f);
        overshootThreshold = Random.Range(1.0f, 2.5f);
        // offset so they don't all try to reposition to the same spot
        repositionOffset = Random.insideUnitCircle * 2f;
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Chase: ChaseUpdate(); break;
            case State.Overshoot: OvershootUpdate(); break;
            case State.Reposition: RepositionUpdate(); break;
        }

        FaceVelocity();
    }

    void ChaseUpdate()
    {
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.AddForce(dir * chaseForce, ForceMode2D.Force);
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);

        if (Vector2.Distance(rb.position, player.position) < overshootThreshold)
            EnterOvershoot();
    }

    void EnterOvershoot()
    {
        currentState = State.Overshoot;
        overshootTimer = overshootDuration;
    }

    void OvershootUpdate()
    {
        overshootTimer -= Time.fixedDeltaTime;
        if (overshootTimer <= 0f)
        {
            repositionOffset = Random.insideUnitCircle * 2f; // new angle each pass
            currentState = State.Reposition;
        }
    }

    void RepositionUpdate()
    {
        Vector2 targetPosition = (Vector2)player.position + repositionOffset;
        Vector2 dir = (targetPosition - rb.position).normalized;
        rb.AddForce(dir * chaseForce, ForceMode2D.Force);
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);

        if (Vector2.Distance(rb.position, player.position) < overshootThreshold)
            EnterOvershoot();
    }

    void FaceVelocity()
    {
        if (rb.linearVelocity.sqrMagnitude < 0.1f) return;

        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg - 90f;
        rb.MoveRotation(Mathf.LerpAngle(rb.rotation, angle, 10f * Time.fixedDeltaTime));
    }
}