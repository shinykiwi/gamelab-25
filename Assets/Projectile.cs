using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    private Rigidbody rb;
    private float force = 7;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.Log("Cannot find rigidbody");
        }
    }

    public void Fire()
    {
        rb.AddRelativeForce(new Vector3(0, 0, (-100 * force)));
    }

    public float GetForce()
    {
        return force;
    }
}
