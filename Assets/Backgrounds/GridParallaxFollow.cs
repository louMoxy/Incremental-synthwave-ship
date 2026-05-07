using UnityEngine;

public class GridParallaxFollow : MonoBehaviour
{
    [SerializeField] Transform target;         // usually Main Camera
    [Range(0f, 1f)]
    [SerializeField] float followStrength = 0.2f; // 0 = fixed world, 1 = stuck to camera
    [SerializeField] float z = 0f;

    Vector3 startPos;

    void Awake()
    {
        startPos = transform.position;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Only follow a fraction of the camera movement
        Vector3 desired = startPos + target.position * followStrength;
        transform.position = new Vector3(desired.x, desired.y, z);
    }
}