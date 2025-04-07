using System;
using System.Collections;
using UnityEngine;

namespace Code.Scripts
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        ParticleSystem explodeVFX;

        [SerializeField] private float killAfterSeconds = 6f;

        public GameObject Owner { get; set; }

        private Rigidbody rb;

        private bool queueDelete = true;
        private Vector3 worldSpawnPos;
        private float maxRangeFromSpawnPos = float.MaxValue;


        [SerializeField]
        SFX_Settings sfx;

        private void Awake()
        {

        }

        private void Update()
        {
            if(Vector3.Distance(worldSpawnPos, transform.position) >= maxRangeFromSpawnPos)
            {
                Kill();
            }
        }

        private void OnEnable()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.Log("Cannot find rigidbody", this);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            queueDelete = true;
            StartCoroutine(KillAfterSeconds());

        }

        private void OnCollisionEnter(Collision other)
        {
            // If projectile hits anything other than a bouncy wall, it explodes
            if(other.gameObject.GetComponent<BouncyWall>() != null)
            {
                queueDelete = false;
            }
            else
            {
                Kill();
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            // A trigger coll is used for bumping, because we want precise control over the collision resolution
            // instead of the physics system handling it
            Vector3 direction = rb.linearVelocity;
            direction = (new Vector3(direction.x, 0.0f, direction.z)).normalized;
            if(other.gameObject.GetComponent<PlayerController>() is { } playerController)
            {
                SFX_Settings.PlayAudioClip(sfx.ProjectileHitPlayer, transform.position, sfx.group);

                playerController.GetHitByProjectile(direction);
                Kill();
            }
            else if(other.gameObject.GetComponent<Enemy>() is { } enemy)
            {
                SFX_Settings.PlayAudioClip(sfx.ProjectileHitEnemy, transform.position, sfx.group);
                enemy.GetHitByProjectile(direction);
                Kill();
            }

            queueDelete = false;
        }

        private void OnTriggerExit(Collider other)
        {
            queueDelete = true;
            StartCoroutine(KillAfterSeconds());
        }


        public void Fire(Vector3 forceWorldSpace)
        {
            rb.AddForce(forceWorldSpace, ForceMode.Impulse);
        }

        public void BeginPortalTravel()
        {
            rb.excludeLayers = LayerMask.GetMask("Player");
        }

        public void EndPortalTravel()
        {
            rb.excludeLayers = new LayerMask();
        }

        public void SetMaxRange(float maxRangeFromSpawn, Vector3 spawnWorldPos)
        {
            this.maxRangeFromSpawnPos = maxRangeFromSpawn;
            this.worldSpawnPos = spawnWorldPos;
        }

        public void Kill()
        {
            if(explodeVFX)
            {
                GameObject vfx = Instantiate(explodeVFX, transform.position, Quaternion.identity).gameObject;
                Destroy(vfx, 15.0f);
            }
            Destroy(gameObject);
        }

        private IEnumerator KillAfterSeconds()
        {
            yield return new WaitForSeconds(killAfterSeconds);
            
            // If it's still set to delete
            if (queueDelete)
            {
                Kill();
            }
        }
    }
}
