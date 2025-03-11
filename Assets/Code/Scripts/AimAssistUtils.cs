using Code.Scripts;
using UnityEngine;

public class AimAssistUtils 
{
    private static RaycastHit[] hitInfo = new RaycastHit[1];
    private static LayerMask enemyMask = LayerMask.GetMask("Enemy");
    private static LayerMask playerMask = LayerMask.GetMask("Player");
    private static LayerMask bouncyWallMask = LayerMask.GetMask("BouncyWall");


    public static bool HasLineOfSightTo(Vector3 origin, Vector3 targetPosition, LayerMask targetLayer, LayerMask ignoredMasks)
    {
        float distance = Vector3.Distance(origin, targetPosition);
        Ray rayToTarget = new Ray(origin, (targetPosition - origin).normalized);
        Physics.RaycastNonAlloc(rayToTarget, hitInfo, distance, ~ignoredMasks, QueryTriggerInteraction.Ignore);
        return hitInfo[0].collider != null && 1 << hitInfo[0].collider.gameObject.layer == targetLayer;
    }

    public static Vector3 GetAutoAimVelocity(Vector3 projectilePosition, Vector3 velocity, 
        float aimAssistDistanceMax, float degreesAimAssist, LayerMask ignoredMasksForLOS)
    {
        float bestAngle = float.MaxValue;
        Vector3 bestVelocity = velocity;

        // Check if the projectile is towards an enemy
        foreach(Enemy enemy in Level.Instance.EnemiesToBeat)
        {
            if(!enemy.IsAlive)
                continue;

            Vector3 toTarget = enemy.transform.position - projectilePosition;
            if(toTarget.magnitude >= aimAssistDistanceMax)
                continue;
            float angle = Vector3.Angle(velocity, toTarget);
            if(angle < degreesAimAssist / 2
            && angle < bestAngle
                && HasLineOfSightTo(projectilePosition, enemy.transform.position, enemyMask, ignoredMasksForLOS))
            {
                bestVelocity = velocity.magnitude * toTarget.normalized;
            }
        }

        foreach(Player player in Level.Instance.Players)
        {
            Vector3 toTarget = player.transform.position - projectilePosition;
            if(toTarget.magnitude >= aimAssistDistanceMax)
                continue;

            float angle = Vector3.Angle(velocity, toTarget);
            if(angle < degreesAimAssist / 2
            && angle < bestAngle
                && HasLineOfSightTo(projectilePosition, player.transform.position, playerMask, ignoredMasksForLOS))
            {
                bestVelocity = velocity.magnitude * toTarget.normalized;
            }
        }

        // Check if the projectile is towards a bouncy wall
        foreach(BouncyWall bouncyWall in Level.Instance.BouncyWalls)
        {
            Vector3 toTarget = bouncyWall.transform.position - projectilePosition;
            if(toTarget.magnitude >= aimAssistDistanceMax)
                continue;
            float angle = Vector3.Angle(velocity, toTarget);
            if(angle < degreesAimAssist / 2
            && angle < bestAngle
                && HasLineOfSightTo(projectilePosition, bouncyWall.transform.position, bouncyWallMask, ignoredMasksForLOS))
            {
                bestVelocity = velocity.magnitude * toTarget.normalized;
            }
        }

        return bestVelocity;
    }
}
