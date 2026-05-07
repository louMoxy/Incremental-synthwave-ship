using UnityEngine;

public class CollectablePull : MonoBehaviour
{
    [Header("Pull Area")]
    [SerializeField] float radius = 3f;
    [SerializeField] LayerMask collectableLayers;

    [Header("Pull Motion")]
    [SerializeField] float pullSpeed = 6f;          // units/sec
    [SerializeField] float collectDistance = 0.25f; // destroy distance

    void FixedUpdate()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, radius, collectableLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (!col) continue;

            var rb = col.attachedRigidbody;
            if (!rb) continue;

            Vector2 toPlayer = (Vector2)transform.position - rb.position;
            float dist = toPlayer.magnitude;

            if (dist <= collectDistance)
            {
                // Add materials BEFORE destroy
                var c = col.GetComponent<Collectables>();
                if (c && MaterialManager.Instance != null)
                    MaterialManager.Instance.Add(c.Type, 1);

                Destroy(col.gameObject);
                continue;
            }

            // Disable drift while pulling (prevents force-fighting)
            var drift = col.GetComponent<RandomDrift>();
            if (drift) drift.enabled = false;

            // Kill rotation so it doesn't go nuts visually
            rb.angularVelocity = 0f;

            // Optional: kill existing velocity so it doesn’t slingshot
            rb.linearVelocity = Vector2.zero;

            // Move smoothly toward player
            Vector2 step = toPlayer.normalized * (pullSpeed * Time.fixedDeltaTime);
            if (step.magnitude > dist) step = toPlayer; // don't overshoot

            rb.MovePosition(rb.position + step);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectDistance);
    }
}