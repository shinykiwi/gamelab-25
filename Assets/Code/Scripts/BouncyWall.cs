using UnityEngine;

namespace Code.Scripts
{
    public class BouncyWall : MonoBehaviour
    {
        [Range(0.5f,12f)]
        [Tooltip("The minimal speed at which the projectile will rebound off the wall.")]
        [SerializeField] private float minSpeedProjectileOnExit = 2f;

        [SerializeField, Min(0.01f), Tooltip("The mininum speed an enemy will have after they collide with the bouncy wall.")]
        float minSpeedEnemyOnExit = 2.0f;

        private void OnCollisionEnter(Collision other)
        {
            // If it's a projectile that hits the wall
            if(other.gameObject.GetComponent<Projectile>() is { } projectile)
            {
                Rigidbody projectileRb = projectile.gameObject.GetComponent<Rigidbody>();
                Vector3 wallNormal = other.contacts[0].normal;
                Vector3 newSpeed = Vector3.Reflect(other.relativeVelocity, wallNormal);
                if(newSpeed.magnitude < minSpeedProjectileOnExit)
                {
                    newSpeed = newSpeed.normalized * minSpeedProjectileOnExit;
                }
                projectileRb.linearVelocity = newSpeed;

                Debug.DrawRay(other.transform.position, other.relativeVelocity, Color.green, 1f);
                Debug.DrawRay(transform.position, wallNormal, Color.red, 1f);
                Debug.DrawRay(other.transform.position, newSpeed, Color.blue, 1f);
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
