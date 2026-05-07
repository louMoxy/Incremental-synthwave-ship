using UnityEngine;

public class GridScrollFromVelocity : MonoBehaviour
{
    [SerializeField] Rigidbody2D shipRb;
    [SerializeField] Material gridMat;

    [SerializeField] float shipMaxSpeed = 20f;
    [SerializeField] float maxScrollSpeed = 0.6f;
    [SerializeField] float speedDeadzone = 0.05f;

    [Header("Smoothing")]
    [SerializeField] float accel = 8f;   // how fast scroll ramps up
    [SerializeField] float decel = 3f;   // how fast scroll ramps down (smaller = gentler)

    static readonly int ScrollX = Shader.PropertyToID("_TextureScrollXSpeed");
    static readonly int ScrollY = Shader.PropertyToID("_TextureScrollYSpeed");

    Vector2 scrollCurrent;

    void Update()
    {
        if (!shipRb || !gridMat) return;

        Vector2 v = shipRb.linearVelocity;
        float speed = v.magnitude;

        Vector2 target = Vector2.zero;

        if (speed >= speedDeadzone)
        {
            Vector2 dir = v / speed;
            float t = Mathf.Clamp01(speed / shipMaxSpeed);
            float s = t * maxScrollSpeed;

            target = -dir * s; // flip if needed
        }

        // Gentle decay avoids “reverse” illusion during slowdown
        float sharpness = (target.magnitude > scrollCurrent.magnitude) ? accel : decel;
        float a = 1f - Mathf.Exp(-sharpness * Time.deltaTime);

        scrollCurrent = Vector2.Lerp(scrollCurrent, target, a);

        gridMat.SetFloat(ScrollX, scrollCurrent.x);
        gridMat.SetFloat(ScrollY, scrollCurrent.y);
    }
}