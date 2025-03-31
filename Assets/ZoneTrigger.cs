using System;
using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        NewCameraController.Instance.UpdateCamera(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        NewCameraController.Instance.Reset();
    }
}
