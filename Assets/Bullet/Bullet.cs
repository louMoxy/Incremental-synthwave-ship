using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] public float speed = 10f;
    [SerializeField] bool autoAimPlayer = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if(autoAimPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        rb.linearVelocity = transform.up * speed;
    }
}
