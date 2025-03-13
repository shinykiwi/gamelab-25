using UnityEngine;
using System.Collections.Generic;
using Code.Scripts;
using System;

public enum ZoneType
{
    A, B, C, D, E, F
}

[RequireComponent(typeof(BoxCollider))]
public class Zone : MonoBehaviour
{
    [SerializeField]
    public ZoneType type;

    bool is_active = false;

    [SerializeField]
    BoxCollider boxCollider;

    [SerializeField]
    List<Tile> tile_list;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        ZoneManager.Instance.RegisterZone(this);
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
