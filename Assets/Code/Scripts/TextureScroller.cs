using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    Material mat;

    [SerializeField]
    Vector2 tilingSpeed = new Vector2(0.1f, 0.1f);

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }
    void Update()
    {
        mat.mainTextureOffset += tilingSpeed * Time.deltaTime;
    }
}
