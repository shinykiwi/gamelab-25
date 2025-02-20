using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Parameters")]

    [SerializeField, Min(0)]
    float projectileSpawnVelocity = 1f;

    [SerializeField]
    Vector3 spawnVelocityDirection = Vector3.forward;

    [SerializeField, Min(0)]
    int maxNbProjectilesAlive = 3;

    [SerializeField, Min(0)]
    float projectileMaxLifetime = 60.0f;

    [Header("References")]
    [SerializeField]
    Projectile projectilePrefab;

    [SerializeField]
    Transform spawnPoint;


    List<Projectile> projectilesSpawned = new();
    
    public void SpawnProjectile()
    {
        // Only allow a certain number of projectiles to be alive at a time
        projectilesSpawned.RemoveAll(projectile => projectile == null);
        if(projectilesSpawned.Count >= maxNbProjectilesAlive)
        {
            return;
        }

        Projectile spawnedProjectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.Euler(transform.forward));
        Vector3 worldForce = projectileSpawnVelocity * (transform.rotation * spawnVelocityDirection);
        spawnedProjectile.Fire(worldForce);
        spawnedProjectile.Owner = gameObject;

        projectilesSpawned.Add(spawnedProjectile);
        StartCoroutine(DestroyProjectile(spawnedProjectile));
    }

    private IEnumerator DestroyProjectile(Projectile projectile)
    {
        yield return new WaitForSeconds(projectileMaxLifetime);
        if(projectile)
        {
            projectile.Kill();
        }
    }


    private void OnValidate()
    {
        spawnVelocityDirection = spawnVelocityDirection.normalized;
    }

    private void OnDrawGizmosSelected()
    {
        if(spawnPoint && projectilePrefab && 
            projectilePrefab.GetComponentInChildren<MeshFilter>() is MeshFilter meshFilter && meshFilter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawMesh(meshFilter.sharedMesh, spawnPoint.position);
            Gizmos.DrawRay(spawnPoint.position, transform.rotation * spawnVelocityDirection);
        }
    }
}
