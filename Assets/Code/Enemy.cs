using System;
using UnityEngine;

public class Enemy : Humanoid
{
    
    [SerializeField] private GameObject projectilePrefab;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        Projectile projectile = Instantiate(projectilePrefab).GetComponent<Projectile>();
        projectile.Fire();
    }
}
