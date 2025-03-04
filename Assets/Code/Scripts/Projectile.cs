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

        public void AllowPortalTravel()
        {
            rb.excludeLayers = LayerMask.GetMask("Player");
        }

        public void DisablePortalTravel()
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
            // If projectile hits another projectile it explodes
            if (other.collider.gameObject.GetComponent<Projectile>())
            {
                Kill();
            }
            // If projectile hits a player
            else if(other.collider.gameObject.GetComponent<PlayerController>() is { } playerController
                && other.contactCount > 0)
            {
                Vector3 direction = -other.contacts[0].normal;
                playerController.GetHitByProjectile(direction);

                Kill();
            }
            else
            {
                queueDelete = false;
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
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
