using System;
using UnityEngine;

public class BouncyWall : MonoBehaviour
{
    [Range(0.5f,12f)]
    [Tooltip("The speed at which the projectile will rebound off the wall.")]
    [SerializeField] private float speedOnExit = 7f;
    private void OnCollisionEnter(Collision other)
    {
        // If it's a projectile that hits the wall
        if (other.gameObject.GetComponent<Projectile>() is {} projectile)
        {
            Rigidbody projectileRb = projectile.gameObject.GetComponent<Rigidbody>();
            Vector3 wallNormal = transform.forward;

            Vector3 direction = -wallNormal * 100f *speedOnExit;
            
            Debug.DrawRay(projectile.transform.position, direction * 10, Color.blue, 10);
            
            projectileRb.AddRelativeForce(direction);
            
        }
    }
}
