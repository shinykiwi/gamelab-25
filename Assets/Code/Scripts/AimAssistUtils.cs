using Code.Scripts;
using UnityEngine;

public class AimAssistUtils 
{
    public static readonly LayerMask ignoredMaskForLOS = LayerMask.GetMask("Ignore Raycast") 
                                                + LayerMask.GetMask("Projectile")
                                                + LayerMask.GetMask("Fence");


    private static LayerMask enemyMask = LayerMask.GetMask("Enemy");
    private static LayerMask playerMask = LayerMask.GetMask("Player");
    private static LayerMask bouncyWallMask = LayerMask.GetMask("BouncyWall");

    public static bool HasLineOfSightTo(Vector3 origin, Vector3 targetPosition, LayerMask targetLayer, LayerMask ignoredMasks)
    {
        float distance = Vector3.Distance(origin, targetPosition);
        Ray rayToTarget = new Ray(origin, (targetPosition - origin).normalized);
        Physics.Raycast(rayToTarget, out RaycastHit hitInfo, distance, ~(ignoredMasks), QueryTriggerInteraction.Ignore);
        return hitInfo.collider != null && 1 << hitInfo.collider.gameObject.layer == targetLayer;
    }

    public static Vector3 GetAutoAimVelocity(Vector3 projectilePosition, Vector3 velocity, 
        float aimAssistDistanceMax, float degreesAimAssist, LayerMask ignoredMasksForLOS,
        bool aimAtEnemy = true, bool aimAtPlayer = true, bool aimAtBouncyWall = true)
    {
        float bestAngle = float.MaxValue;
        Vector3 bestVelocity = velocity;
        Vector3 bestToTarget = int.MaxValue * Vector3.one;

        // Check if the projectile is towards an enemy
        if(aimAtEnemy)
        {
            foreach(Enemy enemy in Level.Instance.AllEnemies)
            {
                if(!enemy.IsAlive)
                    continue;

                Vector3 toTarget = enemy.transform.position - projectilePosition;
                if(toTarget.magnitude >= aimAssistDistanceMax)
                    continue;
                float angle = Vector3.Angle(velocity, toTarget);
                if(angle < degreesAimAssist / 2
                    && IsTargetBetter(angle, toTarget, bestAngle, bestToTarget, aimAssistDistanceMax)
                    && HasLineOfSightTo(projectilePosition, enemy.transform.position, enemyMask, ignoredMasksForLOS))
                {
                    bestVelocity = velocity.magnitude * toTarget.normalized;
                    bestAngle = angle;
                    bestToTarget = toTarget;
                }
            }
        }

        if(aimAtPlayer)
        {
            foreach(Player player in Level.Instance.players)
            {
                Vector3 toTarget = player.transform.position - projectilePosition;
                if(toTarget.magnitude >= aimAssistDistanceMax)
                    continue;

                float angle = Vector3.Angle(velocity, toTarget);
                if(angle < degreesAimAssist / 2
                    && IsTargetBetter(angle, toTarget, bestAngle, bestToTarget, aimAssistDistanceMax) 
                    && HasLineOfSightTo(projectilePosition, player.transform.position, playerMask, ignoredMasksForLOS))
                {
                    bestVelocity = velocity.magnitude * toTarget.normalized;
                    bestAngle = angle;
                    bestToTarget = toTarget;
                }
            }
        }

        if(aimAtBouncyWall)
        {
            // Check if the projectile is towards a bouncy wall
            foreach(BouncyWall bouncyWall in Level.Instance.AllBouncyWalls)
            {
                Vector3 toTarget = bouncyWall.transform.position - projectilePosition;
                if(toTarget.magnitude >= aimAssistDistanceMax)
                    continue;
                float angle = Vector3.Angle(velocity, toTarget);
                if(angle < degreesAimAssist / 2
                    && IsTargetBetter(angle, toTarget, bestAngle, bestToTarget, aimAssistDistanceMax)
                    && HasLineOfSightTo(projectilePosition, bouncyWall.transform.position, bouncyWallMask, ignoredMasksForLOS))
                {
                    bestVelocity = velocity.magnitude * toTarget.normalized;
                    bestAngle = angle;
                    bestToTarget = toTarget;
                }
            }
        }

        // No target found, check if a target is partly in the angle
        if(bestAngle == float.MaxValue)
        {
            // Check if a target is partly in the angle
            Vector3 lineOfSightLeftEdgeDir = Quaternion.AngleAxis(-degreesAimAssist / 2, Vector3.up) * velocity.normalized;
            Vector3 lineOfSightRightEdgeDir = Quaternion.AngleAxis(degreesAimAssist / 2, Vector3.up) * velocity.normalized;

            Vector3 lineOfSightLeftEdge = projectilePosition + lineOfSightLeftEdgeDir * aimAssistDistanceMax;
            Vector3 lineOfSightRightEdge = projectilePosition + lineOfSightRightEdgeDir * aimAssistDistanceMax;
            Debug.DrawRay(projectilePosition, lineOfSightLeftEdgeDir * aimAssistDistanceMax, Color.magenta, 2.0f);
            Debug.DrawRay(projectilePosition, lineOfSightRightEdgeDir * aimAssistDistanceMax, Color.blue, 2.0f);

            if(aimAtEnemy)
            {
                // Check if an enemy is present in that direction, then check if 
                if(HasLineOfSightTo(projectilePosition, lineOfSightLeftEdge, enemyMask, ignoredMasksForLOS))
                {
                    bestVelocity = velocity.magnitude * lineOfSightLeftEdgeDir;
                }
                else if(HasLineOfSightTo(projectilePosition, lineOfSightRightEdge, enemyMask, ignoredMasksForLOS))
                {
                    bestVelocity = velocity.magnitude * lineOfSightRightEdgeDir;
                }
            }
        }

        return bestVelocity;
    }

    private static bool IsTargetBetter(float targetAngle, Vector3 toTarget, float bestAngle, Vector3 bestToTarget, float maxDistance)
    {
        // Try to prioritize targets that are closer and have a smaller angle, might work
        return targetAngle * (toTarget.sqrMagnitude / (maxDistance * maxDistance)) < bestAngle * (bestToTarget.sqrMagnitude / (maxDistance * maxDistance));
    }
}
