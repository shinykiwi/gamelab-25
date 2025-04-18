using System;
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

        [SerializeField]
        private float bounceForce = 0.001f;

        [SerializeField]
        private Transform modelTransform;

        private void Bounce()
        {
            modelTransform.DOPunchScale(bounceForce * Vector3.one, 0.3f);
        }

        private void OnCollisionEnter(Collision other)
        {
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
                bool isProjectileDirRightAngle = IsDirRightAngle(oldVelocity, newVelocity);
                if(isProjectileDirRightAngle)
                {
                    newVelocity = DoRightAngleCorrection(oldVelocity, newVelocity, wallNormal);

                    // Do Aim Assist for enemies only
                    newVelocity = AimAssistUtils.GetAutoAimVelocity(projectile.transform.position, newVelocity,
                        aimAssistDistanceMax, maxRightAngleDegreeDeviation, AimAssistUtils.ignoredMaskForLOS,
                        aimAssistOnEnemy, false, false);
                }
                else // Try to aim assist only if there projectile is not bouncing in a right angle
                {
                    newVelocity = AimAssistUtils.GetAutoAimVelocity(projectile.transform.position, newVelocity,
                        aimAssistDistanceMax, degreesAimAssist, AimAssistUtils.ignoredMaskForLOS,
                        aimAssistOnEnemy, aimAssistOnPlayer, aimAssistOnBouncyWall);
                }

                projectileRb.linearVelocity = newVelocity;

                // Play bounce effects
                SFX_Settings.PlayAudioClip(sfx.BouncyWall, transform.position, sfx.group);
                Bounce();

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
                Vector3 oldVelocity = other.relativeVelocity;
                Vector3 newVelocity = Vector3.Reflect(oldVelocity, wallNormal);
                newVelocity = newVelocity.normalized * Mathf.Max(newVelocity.magnitude, minSpeedEnemyOnExit);
                
                // If the enemy will bounce in a close to 90 degree angle, make sure it is a right angle
                bool isDirRightAngle = IsDirRightAngle(oldVelocity, newVelocity);
                if(isDirRightAngle)
                {
                    newVelocity = DoRightAngleCorrection(oldVelocity, newVelocity, wallNormal);
                }

                enemy.GetHitByBouncyWall(newVelocity);

                // Play effects
                SFX_Settings.PlayAudioClip(sfx.BouncyWall, transform.position, sfx.group);
                Bounce();
            }
        }

        Vector3 DoRightAngleCorrection(Vector3 oldVelocity, Vector3 newVelocity, Vector3 wallNormal)
        {
            float signOfAngle = Mathf.Sign(Vector3.Cross(wallNormal, newVelocity).y);
            return newVelocity.magnitude * (Quaternion.AngleAxis(signOfAngle * 45.0f, Vector3.up) * wallNormal.normalized);
        }

        bool IsDirRightAngle(Vector3 oldVelocity, Vector3 newVelocity)
        {
            return Mathf.Abs(90f - Mathf.Abs(Vector3.Angle(oldVelocity, newVelocity))) < maxRightAngleDegreeDeviation;
        }
    }
}
