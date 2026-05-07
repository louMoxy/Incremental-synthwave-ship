using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileLifetime = 5f;
    [SerializeField] float fireRate = 0.2f;
    [SerializeField] float fireRateVariance = 0f;
    [SerializeField] float minFireRate = 0.1f;
    [SerializeField] float maxFireRate = 2f;

    public bool isFiring = true;
    Coroutine fireCouroutine;

    private void Update()
    {
        Fire();
    }

    private float GetRandomFireRate()
    {
        float random = Random.Range(fireRate * fireRateVariance, fireRate * fireRateVariance);
        return Mathf.Clamp(random, minFireRate, maxFireRate);
    }

    void Fire()
    {
        if (isFiring && fireCouroutine == null)
        {
            fireCouroutine = StartCoroutine(FireContinously());
        }
        else if (fireCouroutine != null && !isFiring)
        {
            {
                StopCoroutine(fireCouroutine);
                fireCouroutine= null;
            }
        }
    }

    IEnumerator FireContinously()
    {
        while (true) {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation, transform);

            Destroy(projectile, projectileLifetime);

            yield return new WaitForSeconds(GetRandomFireRate());
        }
    }
}
