using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    [SerializeField]
    MeshRenderer renderer;
    Color color_glow;
    Color color_albedo;
    Material mat_glow;
    Material mat_albedo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //mat.EnableKeyword("_EMISSION");
        List<Material> mat = new List<Material>();
        renderer.GetMaterials(mat);
        mat[1].EnableKeyword("_EMISSION");
        mat_albedo = mat[0];
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
            mat_glow.SetColor("_EmissionColor", color_glow * 10);
            mat_albedo.color = color_albedo * 1f;
        }

        else
        {
            mat_glow.SetColor("_EmissionColor", color_glow * 1);
            mat_albedo.color = color_albedo * 0.1f;
        }
    }

    public void SetZoneTypeColor(Material type_mat)
    {
        color_glow = type_mat.GetColor("_EmissionColor");
        color_albedo = type_mat.color;
        mat_glow.SetColor("_EmissionColor", color_glow);
        mat_albedo.color = color_albedo;
    }
}
