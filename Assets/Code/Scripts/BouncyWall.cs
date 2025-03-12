using UnityEngine;

namespace Code.Scripts
{
    public class BouncyWall : MonoBehaviour
    {
        [Header("Settings")]
        [Range(0.5f,12f)]
        [Tooltip("The minimal speed at which the projectile will rebound off the wall.")]
        [SerializeField] private float minSpeedProjectileOnExit = 2f;

        [SerializeField, Min(0.01f), Tooltip("The mininum speed an enemy will have after they collide with the bouncy wall.")]
        float minSpeedEnemyOnExit = 2.0f;

        [SerializeField, Min(0.01f)]
        float aimAssistDistanceMax = 50.0f;

        [SerializeField, Range(0f, 360.0f)]
        float degreesAimAssist = 30.0f;

        [Header("Dependencies")]
        [SerializeField]
        LayerMask ignoredMasksForLOS;

        private void OnCollisionEnter(Collision other)
        {
            // If it's a projectile that hits the wall
            if(other.gameObject.GetComponent<Projectile>() is { } projectile)
            {
                Rigidbody projectileRb = projectile.gameObject.GetComponent<Rigidbody>();
                Vector3 wallNormal = other.contacts[0].normal;
                Vector3 newVelocity = Vector3.Reflect(other.relativeVelocity, wallNormal);
                if(newVelocity.magnitude < minSpeedProjectileOnExit)
                {
                    newVelocity = newVelocity.normalized * minSpeedProjectileOnExit;
                }
                Vector3 aimAssistedVelocity = AimAssistUtils.GetAutoAimVelocity(projectile.transform.position, newVelocity, 
                    aimAssistDistanceMax, degreesAimAssist, AimAssistUtils.ignoredMaskForLOS);

                projectileRb.linearVelocity = aimAssistedVelocity;

                // Draw Debug Rays for the velocity
                Debug.DrawRay(projectile.transform.position, newVelocity, Color.red, 1);
                Quaternion rotateRight = Quaternion.Euler(0, degreesAimAssist / 2, 0);
                Debug.DrawRay(projectile.transform.position, aimAssistDistanceMax * (rotateRight * newVelocity.normalized), Color.green, 1);
                Quaternion rotateLeft = Quaternion.Euler(0, -degreesAimAssist / 2, 0);
                Debug.DrawRay(projectile.transform.position, aimAssistDistanceMax * (rotateLeft * newVelocity.normalized), Color.green, 1);

                Debug.DrawRay(other.transform.position, other.relativeVelocity, Color.green, 1f);
                Debug.DrawRay(transform.position, wallNormal, Color.red, 1f);
                Debug.DrawRay(other.transform.position, newVelocity, Color.blue, 1f);
            }
            else if(other.gameObject.GetComponent<Enemy>() is { } enemy)
            {
                Vector3 wallNormal = -other.contacts[0].normal;
                Vector3 velocity = wallNormal * Mathf.Max(other.relativeVelocity.magnitude, minSpeedEnemyOnExit);
                enemy.GetHitByBouncyWall(velocity);
            }
        }
    }
}
