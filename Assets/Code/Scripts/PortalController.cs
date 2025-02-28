using Code.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [Header("Dependencies")] [SerializeField]
    private GameObject exitPortal;

    [Header("Settings")] 
    
    [Range(0.5f, 10f)]
    [SerializeField] private float speedOnExit = 1f;

    [SerializeField]
    private bool isAimAssistEnabled = true;

    [SerializeField, Range(0f, 360.0f)]
    private float degreesAimAssist = 30.0f;

    [SerializeField, Min(0f)]
    private float aimAssistDistanceMax = 25.0f;

    [SerializeField]
    private LayerMask ignoredMasksForLOS;

    private RaycastHit[] raycastHits = new RaycastHit[1];

    /// <summary>
    /// Is called when a projectile collides with a portal.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Projectile projectile = other.gameObject.GetComponent<Projectile>();

        if(projectile == null)
        {
            Debug.Log("No projectile object found on collider!" + name);
        }
        else
        {
            TeleportProjectile(projectile);
        }
    }

    /// <summary>
    /// Teleports the projectile from one portal to the other.
    /// </summary>
    /// <param name="projectile"></param>
    private void TeleportProjectile(Projectile projectile)
    {
        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        
        projectile.AllowPortalTravel();
        
        // Set the position of the projectile to be where the portal is
        projectile.transform.position = exitPortal.transform.position;
        float magnitude = projectileRigidbody.linearVelocity.magnitude;
        // The exit velocity is towards the normal of the exit portal
        Vector3 newVelocity = magnitude * exitPortal.transform.forward * speedOnExit;
        Debug.DrawLine(exitPortal.transform.position, exitPortal.transform.position + exitPortal.transform.forward * 10, Color.cyan, 5);
        
        // Draw Debug Rays for the velocity
        Debug.DrawRay(projectile.transform.position, newVelocity, Color.red, 1);
        Quaternion rotateRight = Quaternion.Euler(0, degreesAimAssist / 2, 0);
        Debug.DrawRay(projectile.transform.position, aimAssistDistanceMax * (rotateRight * newVelocity.normalized), Color.green, 1);
        Quaternion rotateLeft = Quaternion.Euler(0, -degreesAimAssist / 2, 0);
        Debug.DrawRay(projectile.transform.position, aimAssistDistanceMax * (rotateLeft * newVelocity.normalized), Color.green, 1);

        projectileRigidbody.linearVelocity = GetAutoAimVelocity(projectile.transform.position, newVelocity);
        // De-parents the projectile so it can be free
        projectile.gameObject.transform.SetParent(null);
        // Sets the shieldOwner of the projectile to be this gameObject
        projectile.Owner = gameObject;

        StartCoroutine(ProjectileExits(projectile));
    }

    IEnumerator ProjectileExits(Projectile projectile)
    {
        yield return new WaitForSeconds(1f);

        if(projectile != null)
        {
            projectile.DisablePortalTravel();
        }
    }

    Vector3 GetAutoAimVelocity(Vector3 position, Vector3 velocity)
    {
        if(!isAimAssistEnabled)
            return velocity;

        float bestAngle = float.MaxValue;
        Vector3 bestVelocity = velocity;
        foreach(Enemy enemy in Level.Instance.EnemiesToBeat)
        {
            if(!enemy.IsAlive)
                continue;

            Vector3 enemyPosition = enemy.transform.position;
            Vector3 toEnemy = enemyPosition - position;

            if(toEnemy.magnitude >= aimAssistDistanceMax)
                continue;

            float angle = Vector3.Angle(velocity, toEnemy);
            if(angle < degreesAimAssist / 2 && angle < bestAngle && HasLineOfSightToEnemy(enemy))
            {
                bestVelocity = velocity.magnitude * toEnemy.normalized;
            }
        }

        return bestVelocity;
    }

    private bool HasLineOfSightToEnemy(Enemy enemy)
    {
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        Ray rayTowardsPlayer = new Ray(transform.position, (enemy.transform.position - transform.position).normalized);

        int noProjectileMask = int.MaxValue - ignoredMasksForLOS;
        Physics.RaycastNonAlloc(rayTowardsPlayer, raycastHits, distance, noProjectileMask, QueryTriggerInteraction.Ignore);
        return raycastHits[0].collider != null && raycastHits[0].collider.gameObject.layer == LayerMask.NameToLayer("Enemy");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 dir = transform.forward;
        Quaternion rotateRight = Quaternion.Euler(0, degreesAimAssist / 2, 0);
        Gizmos.DrawRay(transform.position, aimAssistDistanceMax * (rotateRight * dir));

        Quaternion rotateLeft = Quaternion.Euler(0, -degreesAimAssist / 2, 0);
        Debug.DrawRay(transform.position, aimAssistDistanceMax * (rotateLeft * dir));
    }
}
