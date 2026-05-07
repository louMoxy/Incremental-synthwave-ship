using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] Vector2 scrollSpeed;

    Vector2 scrollOffset;
    Material material;

    private void Start()
    {
        material = GetComponent<Renderer>().material;

    }

    private void Update()
    {
        scrollOffset += scrollSpeed * Time.deltaTime;
        material.mainTextureOffset = scrollOffset;
    }

}
