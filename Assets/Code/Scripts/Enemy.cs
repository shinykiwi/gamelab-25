using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts
{
    public class Enemy : Humanoid
    {
        [Header("Enemy Settings")]
        [SerializeField]
        float rotationSpeedDegrees = 90.0f;

        [SerializeField, Min(0.01f), Tooltip("Time the enemy has to have a Line of Sight on a player before spawning a projectile")]
        float timeToSeePlayerToSpawnProjectile = 2.0f;

        [Header("Hit by projectiles")]
        [SerializeField]
        float distanceTravelledHitByProjectile = 1.0f;

        [SerializeField, Min(0.01f)]
        float durationTimeHitByProjectile = 0.25f;

        [Header("Setup")]
        [SerializeField]
        LayerMask ignoredMasksForPlayerLOS;

        ProjectileSpawner projectileSpawner;
        Rigidbody rb;
        RaycastHit[] raycastHits = new RaycastHit[1];

        Player[] players;
        Tween rotationTween;
        Player targetPlayer;
        float curTimeSeeingTargetPlayer = 0.0f;

        protected override void Start()
        {
            base.Start();
            projectileSpawner = GetComponent<ProjectileSpawner>();
            if(projectileSpawner == null)
            {
                Debug.LogError("No ProjectileSpawner Component on this object.", this);
            }
            rb = GetComponent<Rigidbody>();
            if(projectileSpawner == null)
            {
                Debug.LogError("No Rigidbody Component on this object.", this);
            }

            players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        }
        
        void Update()
        {
            FaceNearestPlayer();

            if(!IsAlive)
                return;

            if(curTimeSeeingTargetPlayer >= timeToSeePlayerToSpawnProjectile
                && IsFacingTargetPlayer())
            {
                projectileSpawner.SpawnProjectile();
                curTimeSeeingTargetPlayer -= timeToSeePlayerToSpawnProjectile;
            }
        }

        private void OnDestroy()
        {
            rotationTween.Kill();
        }

        protected override void Death()
        {
            if(!IsAlive)
                return;

            base.Death();
            Level.Instance.EnemyHasBeenDefeated(this);
        }

        public void GetHitByProjectile(Vector3 direction)
        {
            //rb.DOMove(rb.position + direction * distanceTravelledHitByProjectile, durationTimeHitByProjectile)
            //    .SetEase(Ease.OutSine);
            StartCoroutine(DoProjectileHit(direction, distanceTravelledHitByProjectile, durationTimeHitByProjectile));
        }

        private IEnumerator DoProjectileHit(Vector3 direction, float distance, float duration)
        {
            // TODO add animation

            // Reset time seeing player when hit
            curTimeSeeingTargetPlayer = 0.0f;

            rb.linearVelocity = distance / duration * direction;
            yield return new WaitForSeconds(duration);

            curTimeSeeingTargetPlayer = 0.0f;
            rb.linearVelocity = Vector3.zero;
        }

        void FaceNearestPlayer()
        {
            Player closestPlayer = GetClosestSeenPlayer();

            if(closestPlayer != null)
            {
                // Rotate towards player
                Vector3 lookDir = closestPlayer.transform.position - transform.position;
                lookDir = new Vector3(lookDir.x, 0, lookDir.z).normalized;
                Vector3 angles = Quaternion.LookRotation(lookDir).eulerAngles;
                float timeRotation = Mathf.Abs(Vector3.Angle(transform.forward, lookDir)) / rotationSpeedDegrees;

                rotationTween.Kill();
                rotationTween = rb.DORotate(angles, timeRotation);

                // Increment time seeing a player
                targetPlayer = closestPlayer;
                curTimeSeeingTargetPlayer += Time.deltaTime;
            }
            else
            {
                targetPlayer = null;
                curTimeSeeingTargetPlayer = 0.0f;
            }
        }

        private Player GetClosestSeenPlayer()
        {
            Player closestPlayer = null;
            float closestDistance = float.MaxValue;
            foreach(Player player in players)
            {
                Humanoid humanoid = player.GetComponent<Humanoid>();
                if(humanoid == null || !humanoid.IsAlive)
                {
                    continue;
                }

                if(HasLineOfSightToPlayer(player, out float distanceToPlayer) && distanceToPlayer < closestDistance)
                {
                    closestPlayer = player;
                    closestDistance = distanceToPlayer;
                }
            }

            return closestPlayer;
        }

        private bool HasLineOfSightToPlayer(Player player, out float distanceToPlayer)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            Ray rayTowardsPlayer = new Ray(transform.position, (player.transform.position - transform.position).normalized);

            int noProjectileMask = int.MaxValue - ignoredMasksForPlayerLOS;
            Physics.RaycastNonAlloc(rayTowardsPlayer, raycastHits, distance, noProjectileMask, QueryTriggerInteraction.Ignore);
            distanceToPlayer = raycastHits[0].distance;
            return raycastHits[0].collider != null && raycastHits[0].collider.gameObject.layer == LayerMask.NameToLayer("Player");
        }

        private bool IsFacingTargetPlayer()
        {
            if(targetPlayer == null)
            {
                return false;
            }
            Vector3 lookDir = targetPlayer.transform.position - transform.position;
            lookDir = new Vector3(lookDir.x, 0, lookDir.z).normalized;
            return Vector3.Dot(new Vector3(transform.forward.x, 0.0f, transform.forward.z), lookDir) > 0.995f;
        }

        private void OnDrawGizmosSelected()
        {
            if(targetPlayer != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, targetPlayer.transform.position);
            }
        }
    }
}
