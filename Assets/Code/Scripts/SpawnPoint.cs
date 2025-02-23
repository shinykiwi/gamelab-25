using System;
using UnityEngine;


public class SpawnPoint : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponentInChildren<MeshRenderer>().gameObject.SetActive(false);
    }
}
