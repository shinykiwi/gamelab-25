using Code.Scripts;
using System;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    [SerializeField]
    private bool projectilesOnly = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Projectile>() is { } projectile)
        {
            projectile.Kill();
        }
        else if(!projectilesOnly && other.gameObject.GetComponent<Humanoid>() is { } humanoid)
        {
            humanoid.Death();
        }
    }
}
