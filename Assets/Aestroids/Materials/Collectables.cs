using UnityEngine;

public class Collectables : MonoBehaviour
{
    [SerializeField] MaterialType type;

    public MaterialType Type => type;
}
