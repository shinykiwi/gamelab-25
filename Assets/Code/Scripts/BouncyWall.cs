using System;
using System.Security.Cryptography;
using DG.Tweening;
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

        [SerializeField, Tooltip("A rebounding projectile within this many degrees of a right angle, will have their rebound direction redirected to exactly 90 degrees.")]
        float maxRightAngleDegreeDeviation = 10.0f;

        [Header("Aim Assist Settings")]
        [SerializeField]
        bool aimAssistOnEnemy = true;

        [SerializeField]
        bool aimAssistOnBouncyWall = true;

        [SerializeField]
        bool aimAssistOnPlayer = true;

        [SerializeField, Min(0.01f)]
        float aimAssistDistanceMax = 50.0f;

        [SerializeField, Range(0f, 360.0f)]
        float degreesAimAssist = 30.0f;

        [SerializeField]
        SFX_Settings sfx;

        
        private float bounceForce = 0.001f;


        private void Bounce()
        {
            transform.DOPunchScale(bounceForce * Vector3.one, 0.3f);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Bounce();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            // Play bounce effect
            Bounce();
            
            // If it's a projectile that hits the wall
            if(other.gameObject.GetComponent<Projectile>() is { } projectile)
            {
                Rigidbody projectileRb = projectile.gameObject.GetComponent<Rigidbody>();
                Vector3 wallNormal = -other.contacts[0].normal;
                Vector3 oldVelocity = other.relativeVelocity;
                Vector3 newVelocity = Vector3.Reflect(oldVelocity, wallNormal);

                if(newVelocity.magnitude < minSpeedProjectileOnExit)
                {
                    newVelocity = newVelocity.normalized * minSpeedProjectileOnExit;
                }

                // If the projectile will bounce in a close to 90 degree angle, make sure it is a right angle
                bool isProjectileDirRightAngle = Mathf.Abs(90f - Mathf.Abs(Vector3.Angle(oldVelocity, newVelocity))) < maxRightAngleDegreeDeviation;

                if(isProjectileDirRightAngle)
                {
                    float signOfAngle = Mathf.Sign(Vector3.Cross(wallNormal, oldVelocity).y);
                    newVelocity = newVelocity.magnitude * (Quaternion.AngleAxis(signOfAngle * 45.0f, Vector3.up) * wallNormal.normalized);
                }
                else // Try to aim assist only if there projectile is not bouncing in a right angle
                {
                    newVelocity = AimAssistUtils.GetAutoAimVelocity(projectile.transform.position, newVelocity,
                        aimAssistDistanceMax, degreesAimAssist, AimAssistUtils.ignoredMaskForLOS,
                        aimAssistOnEnemy, aimAssistOnPlayer, aimAssistOnBouncyWall);
                }

                projectileRb.linearVelocity = newVelocity;
                SFX_Settings.PlayAudioClip(sfx.BouncyWall, transform.position, sfx.group);


                // Draw Debug Rays for the velocity
                Debug.DrawRay(projectile.transform.position, newVelocity, Color.red, 1);
                Quaternion rotateRight = Quaternion.Euler(0, degreesAimAssist / 2, 0);
                Debug.DrawRay(projectile.transform.position, aimAssistDistanceMax * (rotateRight * newVelocity.normalized), Color.green, 1);
                Quaternion rotateLeft = Quaternion.Euler(0, -degreesAimAssist / 2, 0);
                Debug.DrawRay(projectile.transform.position, aimAssistDistanceMax * (rotateLeft * newVelocity.normalized), Color.green, 1);

                Debug.DrawRay(other.transform.position, other.relativeVelocity, Color.green, 1f);
                Debug.DrawRay(other.transform.position, wallNormal.normalized * 3.0f, Color.red, 1f);
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
