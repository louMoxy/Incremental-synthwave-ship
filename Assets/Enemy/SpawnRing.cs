using UnityEngine;

public class SpawnRing : MonoBehaviour
{
    [Header("Ring")]
    [SerializeField] float spawnRadius = 15f;

    [Header("Spawn Rates (seconds between spawns)")]
    [SerializeField] float[] spawnIntervals = { 0f, 20f, 10f, 5f };

    [Header("Spawn Counts per Band")]
    [SerializeField] int[] spawnCounts = { 0, 1, 1, 2 };

    [SerializeField] GameObject interceptorPrefab;

    ThreatManager threatManager;
    Transform player;
    float spawnTimer;


    void Start()
    {
        threatManager = FindFirstObjectByType<ThreatManager>();
        player = FindFirstObjectByType<Flying>().transform;
        Debug.DrawLine(player.position, GetSpawnPoint(), Color.red, 2f);
    }

    void Update()
    {
        if (threatManager.ThreatBand == 0) return;  // band 0 = no spawns

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnWave();
            spawnTimer = spawnIntervals[threatManager.ThreatBand];
        }
    }

    public void ResetSpawner()
    {
        spawnTimer = spawnIntervals.Length > 1 ? spawnIntervals[1] : 20f;
    }

    void SpawnWave()
    {
        int count = spawnCounts[threatManager.ThreatBand];

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPoint = GetSpawnPoint();
            Instantiate(interceptorPrefab, spawnPoint, Quaternion.identity);
        }
    }

    Vector2 GetSpawnPoint()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        return (Vector2)player.position + offset;
    }
}