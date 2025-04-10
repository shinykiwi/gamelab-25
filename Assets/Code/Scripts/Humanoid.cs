using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Code.Scripts
{
    [RequireComponent(typeof(Collider))]
    
    public class Humanoid : MonoBehaviour
    {
        [SerializeField]
        Renderer characterRenderer;

        private Collider coll;
        protected Rigidbody rb;

        // Not sure if this will be needed but it's just an example of where we can put common vars like this
        private float health = 100f;
        public bool IsAlive => health > 0f;

        protected virtual void Start()
        {
            if(!characterRenderer)
            {
                Debug.LogError("No MeshRenderer found on this object.", this);
            }
            coll = gameObject.GetComponent<Collider>();
            rb = gameObject.GetComponent<Rigidbody>();
        }

        // For colliding with void
        protected virtual void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Void"))
            {
                Death();
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("Void"))
            {
                Death();
            }
        }

        public virtual void Death()
        {
            if (!IsAlive)
                return;

            health = 0f;

            // Disable collisions while dead
            foreach(var c in gameObject.GetComponentsInChildren<Collider>())
            {
                c.enabled = false;
            }
            foreach(var c in gameObject.GetComponents<Collider>())
            {
                c.enabled = false;
            }
            rb.useGravity = false;


            // Damage visual sequence
            Material material = characterRenderer.material;
            Color originalColor = material.color;
            float duration = 0.2f;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(material.DOColor(Color.red, duration));
            sequence.Append(material.DOColor(originalColor, duration));
            sequence.AppendCallback(ToggleVisibility);
            sequence.Play();
        }

        /// <summary>
        /// Either shows or hides self and children.
        /// </summary>
        public void ToggleVisibility()
        {
            if(!characterRenderer)
                return;

            bool visibility = !characterRenderer.enabled;
            characterRenderer.enabled = visibility;

            MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach(var r in renderers)
            {
                r.enabled = visibility;
            }

            ParticleSystem[] particles = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach(var p in particles)
            {
                if(visibility)
                    p.Play();
                else
                    p.Stop();
            }

            SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach(var sr in spriteRenderers)
            {
                sr.enabled = visibility;
            }
        }

        protected virtual void Respawn()
        {
            health = 100f;

            // Re-enable collisions
            foreach(var c in gameObject.GetComponentsInChildren<Collider>(true))
            {
                c.enabled = true;
            }
            foreach(var c in gameObject.GetComponents<Collider>())
            {
                c.enabled = true;
            }
            rb.useGravity = true;

            Material material = characterRenderer.material;

            Color originalColor = material.color;
            originalColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

            material.color = originalColor;

            material.DOFade(1f, 1f);
        }

        /// <summary>
        /// Spawns self at the specified position given by the transform.
        /// </summary>
        /// <param name="t"></param>
        private void Spawn(Transform t)
        {
            Spawn(t.position);
        }

        /// <summary>
        /// Spawns self at the specified position.
        /// </summary>
        /// <param name="position"></param>
        private void Spawn(Vector3 position)
        {
            gameObject.transform.position = position;
            ToggleVisibility();
            Respawn();
        }

        protected IEnumerator DelaySpawn(float delay, Transform location)
        {
            yield return new WaitForSeconds(delay);
            Spawn(location);
        }


    }
}
