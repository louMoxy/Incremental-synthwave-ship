using UnityEngine;

public class GridWrapPhaseFromVelocity : MonoBehaviour
{
    [SerializeField] Rigidbody2D shipRb;
    [SerializeField] Material gridMat;

    [Tooltip("How many UV units per 1 world unit. Tune this.")]
    [SerializeField] float worldToUv = 0.02f;

    [Tooltip("When speed is tiny, stop updating phase (prevents jitter).")]
    [SerializeField] float speedDeadzone = 0.05f;

    [Tooltip("Clamp phase so numbers don’t grow forever.")]
    [SerializeField] float wrapClamp = 1000f;

    static readonly int OffsetX = Shader.PropertyToID("_OffsetUvX");
    static readonly int OffsetY = Shader.PropertyToID("_OffsetUvY");

    Vector2 phase;

    void Awake()
    {
        // Start from whatever the material is currently set to (optional)
        if (gridMat)
        {
            if (gridMat.HasProperty(OffsetX)) phase.x = gridMat.GetFloat(OffsetX);
            if (gridMat.HasProperty(OffsetY)) phase.y = gridMat.GetFloat(OffsetY);
        }
    }

    private void Start()
    {
        shipRb = FindFirstObjectByType<Flying>().GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!shipRb || !gridMat) return;

        Vector2 v = shipRb.linearVelocity;
        float speed = v.magnitude;

        if (speed < speedDeadzone)
        {
            // IMPORTANT: do nothing. Keep phase. No snap-back.
            return;
        }

        phase += (v * worldToUv) * Time.deltaTime;

        // Keep values bounded (prevents floating drift)
        phase.x = Mathf.Repeat(phase.x + wrapClamp, wrapClamp * 2f) - wrapClamp;
        phase.y = Mathf.Repeat(phase.y + wrapClamp, wrapClamp * 2f) - wrapClamp;

        if (gridMat.HasProperty(OffsetX)) gridMat.SetFloat(OffsetX, phase.x);
        if (gridMat.HasProperty(OffsetY)) gridMat.SetFloat(OffsetY, phase.y);
    }
}