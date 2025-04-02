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
                if(type != ZoneType.NONE)
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

        player.zone = null;
    }

    private void OnTriggerStay(Collider other)
    {

        var player = other.gameObject.GetComponent<Player>();
        if (player == null) return;

        if(type == ZoneType.NONE && player.zone == null) 
        {
            player.zone = this;
            ZoneManager.Instance.NotifyZoneChange();
        }

        else if (player.zone != this)
        {
            player.zone = this;
            ZoneManager.Instance.NotifyZoneChange();

        }

    }
    
    public void SetActive(bool state)
    {
        //Debug.Log(type + " setting state " + state);
        if(type != ZoneType.NONE)
            foreach (Tile tile in tile_list)
            {
                tile.SetActive(state, ZoneManager.Instance.ArePortalActive());
            }
    }

    private void OnDrawGizmos()
    {
        Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center);
        switch (type)
        {
            case ZoneType.A:
                Gizmos.color = Color.yellow;
                break;
            case ZoneType.B:
                Gizmos.color = Color.red;
                break;
            case ZoneType.C:
                Gizmos.color = Color.green;
                break;
            case ZoneType.D:
                Gizmos.color = Color.blue;
                break;
            case ZoneType.E:
                Gizmos.color = Color.cyan;
                break;
            case ZoneType.F:
                Gizmos.color = Color.white;
                break;
            case ZoneType.NONE:
                Gizmos.color = Color.gray;
                break;
        }
        Gizmos.DrawWireCube(center, boxCollider.size );
    }
}
