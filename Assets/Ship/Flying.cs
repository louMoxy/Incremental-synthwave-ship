using UnityEngine;
using UnityEngine.InputSystem;

public class Flying : MonoBehaviour
{
    [Header("Thrust")]
    [SerializeField] float thrust = 30f;
    [SerializeField] float maxSpeed = 20f;

    [Header("Mouse Steering")]
    [Tooltip("How fast the ship rotates to face the mouse (degrees/sec). High = snappy, low = sluggish.")]
    [SerializeField] float rotationSpeed = 720f;

    [Header("Flight Feel")]
    [SerializeField] float coastDamping = 1.0f;
    [SerializeField] float brakeDamping = 3.0f;
    [SerializeField] float brakeStrength = 2.0f;

    [Header("VFX")]
    [SerializeField] Transform thrustVFXRoot;
    [SerializeField] float maxEmissionRate = 60f;
    [SerializeField] float emissionLerp = 12f;

    FuelManager fuelManager;
    ParticleSystem[] thrustSystems;
    float emissionCurrent;

    Camera mainCam;
    bool thrusting;
    bool braking;

    Rigidbody2D rb;
    float baseDamping;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        fuelManager = FindFirstObjectByType<FuelManager>();
        baseDamping = rb.linearDamping;
        mainCam = Camera.main;

        if (thrustVFXRoot)
            thrustSystems = thrustVFXRoot.GetComponentsInChildren<ParticleSystem>();
    }

    void FixedUpdate()
    {
        // --- Rotate toward mouse ---
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (mouseWorld - transform.position).normalized;

        // Atan2 gives the angle of the direction vector; -90 corrects for Unity's
        // "up" being the ship's forward direction rather than world right
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newAngle);

        // --- Thrust / brake ---
        float throttle = Mouse.current.leftButton.isPressed ? 1f : 0f;
        float brake = Mouse.current.rightButton.isPressed ? 1f : 0f;

        bool canThrust = fuelManager ? fuelManager.Consume(throttle, Time.fixedDeltaTime) : true;

        if (throttle > 0f && canThrust)
            rb.AddForce((Vector2)transform.up * (throttle * thrust), ForceMode2D.Force);

        if (brake > 0f)
            rb.AddForce(-rb.linearVelocity * (brakeStrength * brake), ForceMode2D.Force);

        // Damping
        float targetDamping =
            brake > 0f ? (baseDamping + brakeDamping) :
            throttle > 0f ? baseDamping :
                            (baseDamping + coastDamping);

        rb.linearDamping = Mathf.Lerp(rb.linearDamping, targetDamping, 10f * Time.fixedDeltaTime);

        // Speed cap
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);

        UpdateThrustVFX(throttle, canThrust);
    }

    void UpdateThrustVFX(float throttle, bool canThrust)
    {
        if (thrustSystems == null || thrustSystems.Length == 0) return;

        float target = (canThrust ? throttle : 0f) * maxEmissionRate;
        emissionCurrent = Mathf.Lerp(emissionCurrent, target, emissionLerp * Time.fixedDeltaTime);

        for (int i = 0; i < thrustSystems.Length; i++)
        {
            var ps = thrustSystems[i];
            var emission = ps.emission;
            emission.rateOverTime = emissionCurrent;

            if (target <= 0.01f && emissionCurrent <= 0.1f)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            else if (!ps.isPlaying)
                ps.Play();
        }
    }
}