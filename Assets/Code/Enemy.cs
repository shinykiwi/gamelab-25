using UnityEngine;

public class Enemy : MonoBehaviour
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
