using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GridFisheyeMotion : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cam;
    [SerializeField] Rigidbody2D shipRb;
    [SerializeField] float shipMaxSpeed = 5f;

    [Header("Planet Tilt (UV Offset)")]
    [SerializeField] float worldToOffset = 0.02f;
    [SerializeField] float offsetSmoothing = 10f;
    [SerializeField] float maxAbsOffset = 10f;

    [Header("Flying Feel (Texture Scroll Speed)")]
    [SerializeField] float maxScrollAtMaxSpeed = 0.5f;
    [SerializeField] float scrollSmoothing = 10f;
    [SerializeField] bool invertScroll = true;

    [Header("Curve Control")]
    [Range(0f, 1f)]
    [SerializeField] float turnDampStrength = 0.35f;
    [SerializeField] float turnDampMaxAngular = 180f;
    [SerializeField] float curveDampStrength = 0.6f;   // 0..1
    [SerializeField] float curveDampMax = 2.5f;

    [Header("Small Stability Tweaks")]
    [SerializeField] float scrollDeadzoneSpeed = 0.02f; // world speed where scroll = 0

    static readonly int OffsetX = Shader.PropertyToID("_OffsetUvX");
    static readonly int OffsetY = Shader.PropertyToID("_OffsetUvY");
    static readonly int ScrollX = Shader.PropertyToID("_TextureScrollXSpeed");
    static readonly int ScrollY = Shader.PropertyToID("_TextureScrollYSpeed");


    SpriteRenderer sr;
    Material mat;

    bool hasOffsetX, hasOffsetY, hasScrollX, hasScrollY;

    Vector3 lastCamPos;
    Vector2 offsetCurrent;
    Vector2 scrollCurrent;
    Vector2 dirSmoothed = Vector2.up;
    Vector2 lastVelDir = Vector2.up;

    float _nextLogTime;

    void LogScrollDebug(
        float speed, float speed01, float ang, float turn01, float damp,
        Vector2 v, Vector2 dirRaw, Vector2 dirSmoothed, Vector2 targetScroll, Vector2 scrollCurrent)
    {
        if (Time.time < _nextLogTime) return;
        _nextLogTime = Time.time + 0.25f;

        Debug.Log(
            $"speed {speed:0.00}/{shipMaxSpeed:0.00} (s01 {speed01:0.00}) | " +
            $"ang {ang:0.0} (t01 {turn01:0.00}) damp {damp:0.00} | " +
            $"v({v.x:0.00},{v.y:0.00}) dirRaw({dirRaw.x:0.00},{dirRaw.y:0.00}) dirS({dirSmoothed.x:0.00},{dirSmoothed.y:0.00}) | " +
            $"target({targetScroll.x:0.00},{targetScroll.y:0.00}) curr({scrollCurrent.x:0.00},{scrollCurrent.y:0.00})"
        );
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Material instance so runtime changes don't touch the asset
        sr.material = new Material(sr.material);
        mat = sr.material;

        hasOffsetX = mat.HasProperty(OffsetX);
        hasOffsetY = mat.HasProperty(OffsetY);
        hasScrollX = mat.HasProperty(ScrollX);
        hasScrollY = mat.HasProperty(ScrollY);

        if (!cam) cam = Camera.main ? Camera.main.transform : null;
        if (cam) lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        if (!mat || !cam || !shipRb) return;

        UpdateOffsetFromCamera();
        UpdateScrollFromShip();
    }

    // Frame-rate independent smoothing helper (exponential)
    float ExpSmoothing(float sharpness, float dt)
        => 1f - Mathf.Exp(-sharpness * dt);

    void UpdateOffsetFromCamera()
    {
        Vector3 camDelta = cam.position - lastCamPos;
        lastCamPos = cam.position;

        Vector2 targetOffset = offsetCurrent + new Vector2(-camDelta.x, -camDelta.y) * worldToOffset;

        // Clamp so it can’t drift forever
        targetOffset.x = Mathf.Clamp(targetOffset.x, -maxAbsOffset, maxAbsOffset);
        targetOffset.y = Mathf.Clamp(targetOffset.y, -maxAbsOffset, maxAbsOffset);

        float a = ExpSmoothing(offsetSmoothing, Time.deltaTime);
        offsetCurrent = Vector2.Lerp(offsetCurrent, targetOffset, a);

        if (hasOffsetX) mat.SetFloat(OffsetX, offsetCurrent.x);
        if (hasOffsetY) mat.SetFloat(OffsetY, offsetCurrent.y);
    }


    void UpdateScrollFromShip()
    {
        Vector2 v = shipRb.linearVelocity;
        float speed = v.magnitude;

        if (speed < 0.02f)
        {
            scrollCurrent = Vector2.Lerp(scrollCurrent, Vector2.zero, scrollSmoothing * Time.deltaTime);
            if (mat.HasProperty(ScrollX)) mat.SetFloat(ScrollX, scrollCurrent.x);
            if (mat.HasProperty(ScrollY)) mat.SetFloat(ScrollY, scrollCurrent.y);
            return;
        }

        // Direction (world)
        Vector2 dirRaw = v / speed;

        // Smooth direction so it doesn't snap
        dirSmoothed = Vector2.Lerp(dirSmoothed, dirRaw, 6f * Time.deltaTime);
        if (dirSmoothed.sqrMagnitude > 0.0001f) dirSmoothed.Normalize();

        // Soft speed saturation (prevents "always max scroll" when speed > max)
        float safeMax = Mathf.Max(0.01f, shipMaxSpeed);
        float s = speed / safeMax;
        float speed01 = (s / (1f + s)) * 2f; // saturating curve
        speed01 = Mathf.Clamp01(speed01);

        // Curvature damping (damp when velocity direction changes quickly)
        float dirChange = (dirRaw - lastVelDir).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastVelDir = dirRaw;

        float curve01 = Mathf.Clamp01(dirChange / curveDampMax);
        float curveDamp = Mathf.Lerp(1f, 1f - curveDampStrength, curve01);

        float sign = invertScroll ? -1f : 1f;
        Vector2 targetScroll = sign * dirSmoothed * (maxScrollAtMaxSpeed * speed01 * curveDamp);

        scrollCurrent = Vector2.Lerp(scrollCurrent, targetScroll, scrollSmoothing * Time.deltaTime);

        scrollCurrent.x = Mathf.Clamp(scrollCurrent.x, -1f, 1f);
        scrollCurrent.y = Mathf.Clamp(scrollCurrent.y, -1f, 1f);

        if (mat.HasProperty(ScrollX)) mat.SetFloat(ScrollX, scrollCurrent.x);
        if (mat.HasProperty(ScrollY)) mat.SetFloat(ScrollY, scrollCurrent.y);
    }
}