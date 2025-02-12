using System;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject entryPortal;
    [SerializeField] private GameObject exitPortal;

    [Header("Settings")] [SerializeField] private float speedOnExit = 700f;

    private void TransferProjectile(Projectile projectile)
    {
        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        
        // Set the position of the projectile to be where the portal is
        projectile.gameObject.transform.SetParent(exitPortal.transform);
        projectile.gameObject.transform.localPosition = Vector3.zero;
        projectile.gameObject.transform.localRotation = Quaternion.Euler(0,0,0);
        
        // Direction
        Vector3 direction = projectile.transform.forward * 10;
        
        // Draw a red line for debugging
        Debug.DrawRay(projectile.transform.position, direction, Color.red, 10);

        // Sets the new direction of the projectile
        projectileRigidbody.linearVelocity = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        Projectile projectile = other.gameObject.GetComponentInParent<Projectile>();
        
        if (projectile == null)
        {
            Debug.Log("No projectile object found on collider!" + name);
        }
        
        TransferProjectile(projectile);
    }
}
