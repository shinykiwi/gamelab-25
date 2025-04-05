using Code.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    Collider zoneCollider;

    // Corners of the zone trigger to used as targets for the camera
    // to try and keep the section of the level in view
    GameObject[] cornersOfZone = new GameObject[4];

    List<Player> playersInZone = new List<Player>();

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
        if(zoneCollider == null)
        {
            Debug.LogError("No collider found on this object.", this);
        }

        CreateCornersOfZoneForTargetCamera();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Player>() is { } player && !playersInZone.Contains(player))
        {
            playersInZone.Add(player);
        }

        if(playersInZone.Count == Level.Instance.players.Count)
        {
            AddZoneToCameraTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Player>() is { } player && playersInZone.Contains(player))
        {
            playersInZone.Remove(player);
        }

        if(playersInZone.Count == 0)
        {
            NewCameraController.Instance.ResetTargetsToPlayers();
        }
    }

    void AddZoneToCameraTarget()
    {
        foreach(var corner in cornersOfZone)
        {
            NewCameraController.Instance.UpdateCamera(corner);
        }
    }

    void CreateCornersOfZoneForTargetCamera()
    {
        for(int i = 0; i < cornersOfZone.Length; i++)
        {
            Vector3 cornerPosition = new Vector3(
                (i % 2 == 0 ? -1 : 1) * zoneCollider.bounds.extents.x,
                0.0f,
                (i < 2 ? -1 : 1) * zoneCollider.bounds.extents.z
            );
            GameObject corner = new GameObject(name + "_corner_" + i);
            corner.transform.parent = transform;
            corner.transform.position = cornerPosition + zoneCollider.bounds.center;
            cornersOfZone[i] = corner;
        }
    }
}
