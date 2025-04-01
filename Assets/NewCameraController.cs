using System;
using Code.Scripts;
using Unity.Cinemachine;
using Unity.VisualScripting;
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
        
        DontDestroyOnLoad(gameObject);
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
        target.Weight = 1;
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

    private void ClearTargets()
    {
        Debug.Log("Clearing targets");
        for (int i = 2; i < targetGroup.Targets.Count; i++)
        {
            targetGroup.Targets.RemoveAt(i);
        }
    }
    
    // Only focuses on the players
    public void Reset()
    {
        ClearTargets();
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
        if (!IsInTargetGroup(t)){}
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
        
        Debug.Log("Index is " + index);

        //Debug.Log("There are " + enemies.Length);
        if(index == -1)
        {
            return null;
            Debug.Log("An error occured in finding the enemy.");
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
