using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    [SerializeField]
    MeshRenderer renderer;
    Color color;
    Material mat_glow;
    Material mat_albedo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //mat.EnableKeyword("_EMISSION");
        List<Material> mat = new List<Material>();
        renderer.GetMaterials(mat);
        mat[1].EnableKeyword("_EMISSION");
        mat_glow = mat[1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive(bool state)
    {
        if (state)
        {
            mat_glow.SetColor("_EmissionColor", color * 10);
        }

        else
        {
            mat_glow.SetColor("_EmissionColor", color * 3);
        }
    }

    public void SetZoneTypeColor(Material type_mat)
    {
        color = type_mat.GetColor("_EmissionColor");
        mat_glow.SetColor("_EmissionColor", color);

    }
}
