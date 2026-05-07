using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int totalHealth = 50;
    [SerializeField] bool isPlayer = false;
    [SerializeField] int health = 50;

    PrefabEvents prefabEvents;

    private void Awake()
    {
        prefabEvents = GetComponent<PrefabEvents>();
        totalHealth = health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer damageDealer = collision.GetComponent<DamageDealer>();
        if(damageDealer)
        {
            TakeDamage(damageDealer.GetDamage());
            damageDealer.Hit();
        }
    }

    public void SetTotalHealth(int value, bool resetHealth)
    {
        totalHealth = value;
        if (resetHealth)
        {
            health = value;
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        prefabEvents?.TakeDamage();

        if (health <= 0) {

            Die();
        }
    }

    void Die()
    {
        prefabEvents?.OnDeath(transform.position);
        
        Destroy(gameObject);
    }

    public int GetCurHealth()
    {
        return health;
    }

    public int GetTotalHealth()
    {
        return totalHealth;
    }
}
