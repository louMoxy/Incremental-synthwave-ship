using UnityEngine;

[CreateAssetMenu(fileName = "AsteroidData", menuName = "Game/Asteroid Data")]
public class AsteroidData : ScriptableObject
{
    [Header("Visuals")]
    public GameObject prefab;

    [Header("Stats")]
    public int health = 1;
    public float minSize = 0.5f;
    public float maxSize = 1.5f;
    public int damage = 1;

    [Header("Drops")]
    public int ironAmount = 0;
    public int coalAmount = 1;

    [Header("Spawn Weight")]
    [Tooltip("Higher = more common")]
    public float spawnWeight = 1f;
}
