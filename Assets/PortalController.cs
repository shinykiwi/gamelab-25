using System;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject entryPortal;
    [SerializeField] private GameObject exitPortal;

    [Header("Settings")] 
    
    //[Range(1, 10)]
    private float speedOnExit = 1f;

    
    /// <summary>
    /// Teleports the projectile from one portal to the other.
    /// </summary>
    /// <param name="projectile"></param>
    private void TeleportProjectile(Projectile projectile)
    {
        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        
        // Set the position of the projectile to be where the portal is
        projectile.gameObject.transform.SetParent(exitPortal.transform);
        projectile.gameObject.transform.localPosition = Vector3.zero;
        projectile.gameObject.transform.localRotation = Quaternion.Euler(0,0,0);

        // Get the magnitude so that it can maintain the same speed
        float magnitude = projectileRigidbody.linearVelocity.magnitude;
        Debug.Log(magnitude);
        
        // Direction
        // Gets the normal * the speed for exiting the portal * the initial speed of the projectile
        Vector3 direction = magnitude * projectile.transform.forward * speedOnExit;
        
        // Draw a red line for debugging
        Debug.DrawRay(projectile.transform.position, direction, Color.red, 10);

        // Sets the new direction of the projectile
        projectileRigidbody.linearVelocity = direction;
    }

    /// <summary>
    /// Is called when a projectile collides with a portal.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Projectile projectile = other.gameObject.GetComponentInParent<Projectile>();
        
        if (projectile == null)
        {
            Debug.Log("No projectile object found on collider!" + name);
        }
        
        TeleportProjectile(projectile);
    }
}
