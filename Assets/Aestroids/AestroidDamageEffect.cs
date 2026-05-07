using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class AsteroidDamageEffect : MonoBehaviour
{
    [Header("References")]
    Transform player; 

    [Header("Clip Tuning")]
    [Tooltip("Degrees of clipping at 100% damage. Usually 360.")]
    [SerializeField] float maxClipDegrees = 360f;

    [Tooltip("Angle offset to match your shader's '0 degrees' direction.")]
    [SerializeField] float angleOffset = 0f;

    [Tooltip("If true, the wedge is centered on hit angle (left/right split).")]
    [SerializeField] bool symmetricWedge = true;

    Health health;
    Material material;

    static readonly int ClipAngle = Shader.PropertyToID("_RadialStartAngle");
    static readonly int ClipLeft = Shader.PropertyToID("_RadialClip");
    static readonly int ClipRight = Shader.PropertyToID("_RadialClip2");
    static readonly int ChromAberr = Shader.PropertyToID("_ChromAberrAmount");

    void Awake()
    {
        health = GetComponent<Health>();

        // If this is a SpriteRenderer in 2D, prefer SpriteRenderer.material
        // otherwise Renderer.material is fine.
        var r = GetComponent<Renderer>();
        material = r.material; // creates an instance for this renderer
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    /// <summary>
    /// Call this when the asteroid takes damage.
    /// Assumes hit direction comes from the player's position.
    /// </summary>
    public void OnDamaged()
    {
        if (!material || health == null) return;
        // --- 1) angle based on direction from asteroid to player ---
        if (player)
        {
            Vector2 dir = (Vector2)(player.position - transform.position);
            if (dir.sqrMagnitude > 0.0001f)
            {
                float raw = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                float angle = raw;
                angle = (angle + angleOffset) % 360f;
                if (angle < 0) angle += 360f;

                material.SetFloat(ClipAngle, angle);
            }
        }

        // --- 2) clip amount based on health lost ---
        float total = Mathf.Max(1f, health.GetTotalHealth());
        float cur = Mathf.Clamp(health.GetCurHealth(), 0f, total);

        float damage01 = 1f - (cur / total);                // 0 -> no damage, 1 -> dead
        float clipDeg = damage01 * maxClipDegrees;         // e.g. 0..360

        // Apply to shader:
        if (symmetricWedge)
        {
            // Wedge grows evenly on both sides of the angle.
            float half = clipDeg * 0.5f;
            material.SetFloat(ClipLeft, half);
            material.SetFloat(ClipRight, half);
        }
        else
        {
            // Wedge grows only in one direction.
            material.SetFloat(ClipLeft, 0f);
            material.SetFloat(ClipRight, clipDeg);
        }

        StartCoroutine(ChromaticPulse(0.5f));
    }

    IEnumerator ChromaticPulse(float duration)
    {
        if (!material) yield break;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;              // 0 → 1
            float pulse = Mathf.Sin(t * Mathf.PI);  // 0 → 1 → 0

            material.SetFloat(ChromAberr, pulse);

            yield return null;
        }

        material.SetFloat(ChromAberr, 0f); // ensure reset
    }
}