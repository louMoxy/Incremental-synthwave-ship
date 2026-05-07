using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public AsteroidData[] asteroidTypes;
    public int targetAsteroidCount = 30;
    public float spawnRadius = 25f;      // how far out from player to spawn
    public float despawnRadius = 35f;    // how far before they get culled
    public float minSpawnDistance = 10f; // never spawn too close

    [Header("Spawn Rate")]
    public float spawnInterval = 0.3f; // seconds between each asteroid
    private float _spawnTimer;

    [Header("References")]
    public Transform player;

    private List<GameObject> _activeAsteroids = new List<GameObject>();
    private float _totalWeight;

    void Start()
    {
        foreach (var type in asteroidTypes)
            _totalWeight += type.spawnWeight;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        CullDistantAsteroids();

        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0f && _activeAsteroids.Count < targetAsteroidCount)
        {
            SpawnAsteroid();
            _spawnTimer = spawnInterval;
        }
    }

    void SpawnAsteroid()
    {
        AsteroidData data = GetWeightedRandom();
        if (data == null || data.prefab == null) return;

        Vector2 spawnPos = GetSpawnPosition();
        GameObject asteroid = Instantiate(data.prefab, spawnPos, 
            Quaternion.Euler(0, 0, Random.Range(0f, 360f)));

        // Set size
        float size = Random.Range(data.minSize, data.maxSize);
        asteroid.transform.localScale = Vector3.one * size;
        PassData(asteroid, data);
        _activeAsteroids.Add(asteroid);
    }

    void PassData(GameObject asteroid, AsteroidData data)
    {
        asteroid.GetComponent<Health>().SetTotalHealth(data.health, true);
        asteroid.GetComponent<DamageDealer>().SetDamage(data.damage);
        asteroid.GetComponent<AestroidMaterials>().SetResources(data);
    }

    void CullDistantAsteroids()
    {
        for (int i = _activeAsteroids.Count - 1; i >= 0; i--)
        {
            if (_activeAsteroids[i] == null)
            {
                _activeAsteroids.RemoveAt(i);
                continue;
            }

            float dist = Vector2.Distance(
                _activeAsteroids[i].transform.position, player.position);
            if (dist > despawnRadius)
            {
                Destroy(_activeAsteroids[i]);
                _activeAsteroids.RemoveAt(i);
            }
        }
    }

    Vector2 GetSpawnPosition()
    {
        // Spawn in a ring around the player - not too close, not too far
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float dist = Random.Range(minSpawnDistance, spawnRadius);
            Vector2 pos = (Vector2)player.position + dir * dist;

            if (Vector2.Distance(pos, player.position) >= minSpawnDistance)
                return pos;
        }

        return (Vector2)player.position + Random.insideUnitCircle.normalized * spawnRadius;
    }

    AsteroidData GetWeightedRandom()
    {
        float roll = Random.Range(0f, _totalWeight);
        float cumulative = 0f;

        foreach (var type in asteroidTypes)
        {
            cumulative += type.spawnWeight;
            if (roll <= cumulative) return type;
        }

        return asteroidTypes[0];
    }
}