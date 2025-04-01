using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
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

        [SerializeField, Min(0.01f)]
        float maxRangeToSeePlayer = 15.0f;

        [SerializeField, Tooltip("Whether the enemy tries to detect the player")]
        bool isEnemyActive = true;

        [Header("Hit by projectiles")]
        [SerializeField]
        float distanceTravelledHitByProjectile = 1.0f;

        [SerializeField, Min(0.01f)]
        float durationTimeHitByProjectile = 0.25f;

        [SerializeField]
        Animator animator;

        public Enemy EnemyCollidedWithThisFrame { get; private set; }
        public Vector3 PrevVelocity { get; private set; } // Previous physics iteration velocity

        ProjectileSpawner projectileSpawner;
        RaycastHit[] raycastHits = new RaycastHit[1];

        Player[] players;
        Tween rotationTween;
        Coroutine velocityChangeCoroutine;
        Player targetPlayer;
        float curTimeSeeingTargetPlayer = 0.0f;
        float timeLeftHit = 0.0f;



        private EnemyAudio enemyAudio;

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
            enemyAudio = GetComponentInChildren<EnemyAudio>();
        }
        
        void Update()
        {
            EnemyCollidedWithThisFrame = null; // Reset every frame

            if(!isEnemyActive || !IsAlive)
                return;

            FaceNearestPlayer();

            if(curTimeSeeingTargetPlayer >= timeToSeePlayerToSpawnProjectile
                && IsFacingTargetPlayer())
            {
                projectileSpawner.SpawnProjectile();
                curTimeSeeingTargetPlayer -= timeToSeePlayerToSpawnProjectile;
            }
        }

        private void FixedUpdate()
        {
            PrevVelocity = rb.linearVelocity;
        }

        private void OnDestroy()
        {
            rotationTween.Kill();
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);

            if(collision.gameObject.GetComponent<Enemy>() is { } otherEnemy)
            {
                HitEnemy(collision, otherEnemy);
            }
        }

        public void SetEnemyActive(bool isActive)
        {
            isEnemyActive = isActive;
        }

        public void GetHitByProjectile(Vector3 direction)
        {
            PlayGettingHitAnimation();
            Vector3 velocity = direction * distanceTravelledHitByProjectile / durationTimeHitByProjectile;
            ChangeVelocityOnHit(velocity, durationTimeHitByProjectile);
        }

        public void GetHitByBouncyWall(Vector3 newVelocity)
        {
            // duration for the hit on a bouncy wall is the same as a projectile (could change)
            ChangeVelocityOnHit(newVelocity, durationTimeHitByProjectile);
        }

        public void GetHitByEnemy(Enemy otherEnemy, Vector3 newVelocity)
        {
            if(EnemyCollidedWithThisFrame == otherEnemy)
            {
                // We already did the collision with that enemy
                return;
            }
            EnemyCollidedWithThisFrame = otherEnemy;
            ChangeVelocityOnHit(newVelocity, durationTimeHitByProjectile);
        }

        private void HitEnemy(Collision collision, Enemy otherEnemy)
        {
            // This would get called once on each enemy, but we want for it to be called only once for the whole collision
            if(EnemyCollidedWithThisFrame == otherEnemy)
            {
                // We already did the collision
                return;
            }
            EnemyCollidedWithThisFrame = otherEnemy;

            // The enemy exchanges their previous velocities
            otherEnemy.GetHitByEnemy(this, PrevVelocity);
            ChangeVelocityOnHit(otherEnemy.PrevVelocity, durationTimeHitByProjectile);
        }

        protected override void Death()
        {
            if(!IsAlive)
                return;

            base.Death();
            NewCameraController.Instance.Reset();
            Level.Instance.EnemyHasBeenDefeated(this);
            Destroy(this.gameObject);
        }

        private void ChangeVelocityOnHit(Vector3 newVelocity, float duration)
        {
            if(velocityChangeCoroutine != null)
            {
                StopCoroutine(velocityChangeCoroutine);
            }
            velocityChangeCoroutine = StartCoroutine(DoChangeVelocity(newVelocity, duration));
        }

        // Changes the newVelocity of an enemy for a duration, and doesn't let them act
        private IEnumerator DoChangeVelocity(Vector3 newVelocity, float duration)
        {
            // TODO add animation
            // Reset time seeing player when hit
            curTimeSeeingTargetPlayer = 0.0f;
            rb.linearVelocity = newVelocity;
            rb.useGravity = false;
            timeLeftHit = duration;
            while(timeLeftHit > 0.0f)
            {
                timeLeftHit -= Time.deltaTime;
                yield return null;
            }
            timeLeftHit = 0.0f;
            curTimeSeeingTargetPlayer = 0.0f;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
        }

        void FaceNearestPlayer()
        {
            Player closestPlayer = GetClosestSeenPlayer();
            bool isPlayerInRange = closestPlayer != null && Vector3.Distance(transform.position, closestPlayer.transform.position) <= maxRangeToSeePlayer;

            if(isPlayerInRange)
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

            Physics.RaycastNonAlloc(rayTowardsPlayer, raycastHits, distance, ~AimAssistUtils.ignoredMaskForLOS.value, QueryTriggerInteraction.Ignore);
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

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxRangeToSeePlayer);
        }

        private void PlayGettingHitAnimation()
        {
            if(animator == null)
                return;

            animator.SetTrigger("is_hit");
        }
    }
}
