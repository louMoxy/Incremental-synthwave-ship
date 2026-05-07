using UnityEngine;

public class GridFollowCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float z = 10f;

    void LateUpdate()
    {
        if (!target) return;
        var p = target.position;
        transform.position = new Vector3(p.x, p.y, z);
    }
}