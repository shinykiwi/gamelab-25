using UnityEngine;

namespace Code.Scripts
{
    public class BouncyWall : MonoBehaviour
    {
        [Range(0.5f,12f)]
        [Tooltip("The minimal speed at which the projectile will rebound off the wall.")]
        [SerializeField] private float minSpeedOnExit = 7f;

        [Tooltip("The projectile's speed will be multiplied by this value when it hits the wall.")]
        [SerializeField] 
        private float speedModifier = 1f;

        private void OnCollisionEnter(Collision other)
        {
            // If it's a projectile that hits the wall
            if (other.gameObject.GetComponent<Projectile>() is {} projectile)
            {
                Rigidbody projectileRb = projectile.gameObject.GetComponent<Rigidbody>();
                Vector3 wallNormal = other.contacts[0].normal;
                Vector3 newSpeed = Vector3.Reflect(other.relativeVelocity, wallNormal);
                if(newSpeed.magnitude < minSpeedOnExit)
                {
                    newSpeed = newSpeed.normalized * minSpeedOnExit;
                }
                projectileRb.linearVelocity = speedModifier * newSpeed;

                Debug.DrawRay(other.transform.position, other.relativeVelocity, Color.green, 5f);
                Debug.DrawRay(transform.position, wallNormal, Color.red, 5f);
                Debug.DrawRay(other.transform.position, newSpeed, Color.blue, 5f);
            }
        }
    }
}
