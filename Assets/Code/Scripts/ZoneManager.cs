using UnityEngine;
using System.Collections.Generic;
using Code.Scripts;
using System;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }

    [SerializeField]
    public ZoneColorSettings ZoneColorSettings;
    public ZoneType active_type;

    [SerializeField]
    bool is_split_zone;


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

    //Need a way to implement the split zone. Without changing all the code. And it's easy to setup


    public void NotifyZoneChange()
    {
        if (player1 == null || player2 == null) return;

        if (is_split_zone)
        {
            SplitZoneLogicUpdate();
        }
        else
        {
            DefaultZoneLogicUpdate();
        }
    }


    public bool ArePortalActive()
    {
        if (player1 == null || player2 == null)
            return false;

        if(player1.zone.type == ZoneType.NONE || player2.zone.type == ZoneType.NONE)
            return false;

        if(is_split_zone)
        {
            if (player1.zone.type == player2.zone.type && player1.zone != player2.zone)
                return true;
            else return false;
        }
        else
        {
            if (player1.zone.type == player2.zone.type)
                return true;
            else return false;
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


    //Only usefull for split zone logic
    private Zone player1_zone;
    private Zone player2_zone;
    private void SplitZoneLogicUpdate()
    {
        //Should only be called the first time the function is called 
        if(player1_zone == null || player2_zone == null)
        {
            player1_zone = player1.zone;
            player2_zone = player2.zone;
        }

        if (player1.zone == null || player2.zone == null)
            return;

        if (player1.zone.type == player2.zone.type && player1.zone != player2.zone)
        {
            active_type = player1.zone.type;

            DefaultCameraLogic();

            player1.zone.SetActive(true);
            player2.zone.SetActive(true);

            if (player1.zone != player1_zone)
            {
                player1_zone.SetActive(false);
                player1_zone = player1.zone;
            }

            if (player2.zone != player2_zone)
            {
                player2_zone.SetActive(false);
                player2_zone = player2.zone;
            }

        }
        else
        {
            player2_zone.SetActive(false);
            player1_zone.SetActive(false);
        }
    }

    private void DefaultZoneLogicUpdate()
    {
        if (player1.zone.type == player2.zone.type)
        {
            active_type = player1.zone.type;
            DefaultCameraLogic();

            foreach (Zone zone in zones)
            {
                if (zone.type == active_type)
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

    private void DefaultCameraLogic()
    {
        if (active_type != ZoneType.NONE)
        {
            CameraController cameraController = FindAnyObjectByType<CameraController>();
            cameraController.SetClosestEnemy();
        }
        else if (active_type == ZoneType.NONE)
        {
            CameraController cameraController = FindAnyObjectByType<CameraController>();
            cameraController.ClearClosestEnemy();
        }
    }
}
