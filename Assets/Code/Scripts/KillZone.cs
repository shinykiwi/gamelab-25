using Code.Scripts;
using System;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.GetComponent<Humanoid>() is { } humanoid)
        {
            humanoid.Death();
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
