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
        List<Material> mat = new List<Material>();
        renderer.GetMaterials(mat);
        mat_albedo = new Material(mat[0]);
        mat_glow = new Material(mat[1]);
        mat_glow.EnableKeyword("_EMISSION");

        renderer.materials = new Material[] { mat_albedo, mat_glow };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive(bool state)
    {
        if (state)
        {
            Debug.Log("Tile active");
            mat_glow.SetColor("_EmissionColor", color_glow * ZoneManager.Instance.ZoneColorSettings.emission_force_on);
            mat_albedo.color = color_albedo * ZoneManager.Instance.ZoneColorSettings.albedo_force_on;
        }

        else
        {

            Debug.Log("Tile not active");

            mat_glow.SetColor("_EmissionColor", color_glow * ZoneManager.Instance.ZoneColorSettings.emission_force_off);
            mat_albedo.color = color_albedo * ZoneManager.Instance.ZoneColorSettings.albedo_force_off;
        }
    }

    public void SetZoneTypeColor(Material type_mat)
    {
        Color tmp = type_mat.GetColor("_EmissionColor");
        color_glow = new Color(tmp.r, tmp.g, tmp.b, tmp.a);

        tmp = type_mat.color;
        color_albedo = new Color(tmp.r, tmp.g, tmp.b, tmp.a);


        mat_glow.SetColor("_EmissionColor", color_glow);
        mat_albedo.color = color_albedo;
    }
}
