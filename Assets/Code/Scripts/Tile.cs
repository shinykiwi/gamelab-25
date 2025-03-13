using UnityEngine;

public class Tile : MonoBehaviour
{
    public Material mat_active;
    public Material mat_not_active;


    public MeshRenderer mesh;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive(bool state)
    {
        if(state)
        {
            mesh.material = mat_active;
        }

        else
        {
            mesh.material = mat_not_active;
        }
    }
}
