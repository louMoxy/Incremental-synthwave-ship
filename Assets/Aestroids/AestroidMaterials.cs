using UnityEngine;

public class AestroidMaterials : MonoBehaviour
{
    int ironAmount = 1;
    int coalAmount = 1;

    [SerializeField] GameObject ironPrefab;
    [SerializeField] GameObject coalPrefab;

    public void GenerateMaterials(Vector3 spawnPos)
    {
        spawnPos.z = 0f;
        // Add more eventually and make it more random
        for (int i = 0; i < ironAmount; i++)
        {
            Instantiate(ironPrefab, spawnPos, Quaternion.identity);
        }
        for (int i = 0; i < coalAmount; i++)
        {
            Instantiate(coalPrefab, spawnPos, Quaternion.identity);
        }
    }

    public void SetResources(AsteroidData data)
    {
       ironAmount = data.ironAmount;
       coalAmount = data.coalAmount;
    }
}
