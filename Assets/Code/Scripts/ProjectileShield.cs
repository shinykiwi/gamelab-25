using UnityEngine;

public class ProjectileShield : MonoBehaviour
{
    [Tooltip("Projectiles will pass through this shield if they are owned by the shieldOwner.")]
    [SerializeField]
    private GameObject shieldOwner;

    void OnTriggerEnter(Collider other)
    {
        Projectile projectile = other.GetComponentInParent<Projectile>();
        if(projectile && projectile.Owner != shieldOwner)
        {
            projectile.Kill();
        }
    }
}
