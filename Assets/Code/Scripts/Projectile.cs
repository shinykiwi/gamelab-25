using System;
using System.Collections;
using UnityEngine;

namespace Code.Scripts
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        ParticleSystem explodeVFX;

        public GameObject Owner { get; set; }

        private Rigidbody rb;

        private bool queueDelete = true;

        private void OnEnable()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.Log("Cannot find rigidbody");
            }
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

        public void Kill()
        {
            if(explodeVFX)
            {
                GameObject vfx = Instantiate(explodeVFX, transform.position, Quaternion.identity).gameObject;
                Destroy(vfx, 15.0f);
            }
            Destroy(gameObject);
        }

        private void OnCollisionExit(Collision other)
        {
            queueDelete = true;
            StartCoroutine(KillAfterSeconds());

        }

        private void OnCollisionEnter(Collision other)
        {
            // If projectile hits another projectile or enemy it explodes
            if (other.gameObject.GetComponent<Projectile>())
            {
                Kill();
            }
            else
            {
                queueDelete = false;
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
                playerController.GetHitByProjectile(direction);
                Kill();
            }
            else if(other.gameObject.GetComponent<Enemy>() is { } enemy)
            {
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

        private IEnumerator KillAfterSeconds(float seconds = 4f)
        {
            yield return new WaitForSeconds(seconds);
            
            // If it's still set to delete
            if (queueDelete)
            {
                Kill();
            }
        }
    }
}
