using UnityEngine;
using System.Collections.Generic;
using Code.Scripts;
using System;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }

    [SerializeField]
    public ZoneColorSettings ZoneColorSettings;
    ZoneType active_type;


    List<Zone> zones = new List<Zone>();

    Player player1;
    Player player2;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void RegisterZone(Zone zone)
    {
        zones.Add(zone);
    }

    public void UnregisterZone(Zone zone)
    {
        zones.Remove(zone);
    }

    public void NotifyZoneChange()
    {
        if(player1 == null || player2 == null) return;

        if(player1.zone == player2.zone)
        {
            ZoneType active_zone = player1.zone;

            foreach (Zone zone in zones)
            {
                if(zone.type == active_zone)
                {
                    zone.SetActive(true);
                }
                else
                {
                    zone.SetActive(false);
                }
            }
        }
        else
        {
            foreach (Zone zone in zones)
            {
                zone.SetActive(false);
            }
        }
    }

    public void SetPlayer(Player player, PlayerNum playerNum)
    {
        switch (playerNum)
        {
            case PlayerNum.Player1:
                player1 = player;
                break;
            case PlayerNum.Player2:
                player2 = player;
                break;
        }
    }

}
