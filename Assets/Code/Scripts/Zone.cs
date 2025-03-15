using UnityEngine;
using System.Collections.Generic;
using Code.Scripts;
using System;

public enum ZoneType
{
    A, B, C, D, E, F, NONE
}

[RequireComponent(typeof(BoxCollider))]
public class Zone : MonoBehaviour
{
    [SerializeField]
    public ZoneType type;

    bool is_active = false;

    [SerializeField]
    BoxCollider boxCollider;

    List<Tile> tile_list = new List<Tile>();

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        ZoneManager.Instance.RegisterZone(this);

        Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center);
        Vector3 halfExtents = boxCollider.size * 0.5f;
        Quaternion rotation = boxCollider.transform.rotation;

        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, rotation);

        foreach (Collider obj in hitColliders)
        {
            var tile = obj.gameObject.GetComponentInParent<Tile>();
            if (tile != null)
            {
                tile_list.Add(tile);
                tile.SetZoneTypeColor(ZoneManager.Instance.ZoneColorSettings.GetZone(type));
            }
        }
    }

    private void OnDestroy()
    {
        ZoneManager.Instance.UnregisterZone(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<Player>();
        if (player == null) return;

        player.zone = type;

        ZoneManager.Instance.NotifyZoneChange();
    }

    public void SetActive(bool state)
    {
        Debug.Log(type + " setting state " + state);
        foreach (Tile tile in tile_list)
        {
            tile.SetActive(state);
        }

    }

    private void OnrawGizmos()
    {
        Gizmos.DrawCube(boxCollider.center, boxCollider.size);
    }
}
