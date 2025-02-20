using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    ParticleSystem explodeVFX;

    public GameObject Owner { get; set; }

    private Rigidbody rb;

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
}
