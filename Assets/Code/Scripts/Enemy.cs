using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Scripts
{
    public class Enemy : Humanoid
    {
        [SerializeField]
        float rotationSpeedDegrees = 90.0f;

        ProjectileSpawner projectileSpawner;
        Rigidbody rb;

        List<Player> players;
        Tween rotationTween;

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

            players = FindObjectsByType<Player>(FindObjectsSortMode.None).ToList();
        }
        
        void Update()
        {
            FaceNearestPlayer();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                projectileSpawner.SpawnProjectile();
            }
        }

        protected override void Death()
        {
            if(!IsAlive)
                return;

            base.Death();
            Level.Instance.EnemyHasBeenDefeated(this);
        }

        void FaceNearestPlayer()
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

                float distance = Vector3.Distance(transform.position, player.transform.position);
                if(distance < closestDistance)
                {
                    closestPlayer = player;
                    closestDistance = distance;
                }
            }

            if(closestPlayer != null)
            {
                Vector3 lookDir = closestPlayer.transform.position - transform.position;
                lookDir = new Vector3(lookDir.x, 0, lookDir.z).normalized;
                Vector3 angles = Quaternion.LookRotation(lookDir).eulerAngles;
                float timeRotation = Mathf.Abs(Vector3.Angle(transform.forward, lookDir)) / rotationSpeedDegrees;

                rotationTween.Kill();
                rotationTween = rb.DORotate(angles, timeRotation);

                Debug.DrawLine(transform.position, closestPlayer.transform.position, Color.red);
            }
        }
    }
}
