using System;
using Code.Scripts;
using Unity.Cinemachine;
using UnityEngine;

public class NewCameraController : MonoBehaviour
{
    [SerializeField] private GameObject targetGroupObject;
    private CinemachineTargetGroup targetGroup;

    private CinemachineCamera cineCamera;

    public static NewCameraController Instance { get; private set; }
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

    private void Start()
    {
        cineCamera = GetComponent<CinemachineCamera>();
        targetGroup = targetGroupObject.GetComponent<CinemachineTargetGroup>();
    }
    
    private void AddToTargetGroup(Transform t)
    {
        CinemachineTargetGroup.Target target = new CinemachineTargetGroup.Target();
        target.Object = t;
        target.Radius = 0;
        target.Weight = 0.75f; // Allowing some leeway seems to work in practice
        targetGroup.Targets.Add(target);
    }

    private bool IsInTargetGroup(Transform t)
    {
        Debug.Log(t);
        foreach (CinemachineTargetGroup.Target target in targetGroup.Targets)
        {
            if (target.Object.Equals(t))
            {
                return true;
            }
        }

        return false;
    }

    // Only focuses on the players
    public void ResetTargetsToPlayers()
    {
        const int nbPlayers = 2;
        if(targetGroup.Targets.Count > 2)
        {
            targetGroup.Targets.RemoveRange(nbPlayers, targetGroup.Targets.Count - nbPlayers);
        }
    }

    // Focuses on the enemy, players and the respective zones
    public void UpdateCamera(GameObject obj)
    {
        if (!IsInTargetGroup(obj.transform))
        {
            AddToTargetGroup(obj.transform);
            
            UpdateCameraEnemy();
        }
    }

    public void UpdateCameraEnemy()
    {
        Transform t = FindClosestEnemy();
        if (t != null && !IsInTargetGroup(t))
        {
            AddToTargetGroup(t);
        }
    }
    
    private Transform FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.InstanceID);

        float dToBeat = 0;
        int index = -1;
        for (int i = 0; i < enemies.Length; i++)
        {
            // If the enemy is not alive, then skip this one
            if (!enemies[i].IsAlive) continue;
            
            // The distance between the enemy and the players
            float distance = Vector3.Distance(enemies[i].transform.position, targetGroup.Transform.position);
            
            if (distance < dToBeat || dToBeat == 0)
            {
                dToBeat = distance;
                index = i;
            }
        }
        
        if(index == -1)
        {
            return null;
        }
        return enemies[index].transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            
        }
    }
}
