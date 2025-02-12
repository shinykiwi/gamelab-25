using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    private Rigidbody rb;
    [SerializeField] private float force = 700f;

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
        rb.AddRelativeForce(new Vector3(0, 0, -force));
    }
}
