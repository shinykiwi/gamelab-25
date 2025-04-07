using System.Collections.Generic;
using Code.Scripts;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class NewCameraController : MonoBehaviour
{
    [SerializeField] private GameObject targetGroupObject;

    [SerializeField]
    private float tweenTimeAddToTarget = 2.0f;

    [SerializeField]
    private float weightsOfTargets = 0.75f; // Allowing some leeway seems to work in practice

    private CinemachineTargetGroup targetGroup;

    private CinemachineCamera cineCamera;

    private List<Tween> removingTargetTweens = new List<Tween>();

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
        // If removing targets, just complete them to avoid issues adding/removing the same targets
        while(removingTargetTweens.Count > 0)
        {
            Tween tween = removingTargetTweens[0];
            tween.Complete();

            // Avoid infinite loop, by removing the tween from the list if it didn't
            if(removingTargetTweens.Count > 0 && removingTargetTweens[0] == tween)
                removingTargetTweens.RemoveAt(0);
        }

        CinemachineTargetGroup.Target target = new CinemachineTargetGroup.Target();
        target.Object = t;
        target.Radius = 0;
        target.Weight = 0.0f; 
        targetGroup.Targets.Add(target);

        DOTween.To(() => target.Weight, x => target.Weight = x, weightsOfTargets, tweenTimeAddToTarget).SetEase(Ease.OutCubic);
    }

    // Only focuses on the players
    public void ResetTargetsToPlayers()
    {
        const int nbPlayers = 2;

        if(targetGroup.Targets.Count <= nbPlayers)
            return;

        var targetsToRemove = targetGroup.Targets.GetRange(nbPlayers, targetGroup.Targets.Count - nbPlayers);
        foreach(var target in targetsToRemove)
        {
            Tween tween = DOTween.To(() => target.Weight, x => target.Weight = x, 0.0f, tweenTimeAddToTarget).SetEase(Ease.OutQuad);
            removingTargetTweens.Add(tween);
            tween.OnComplete(() =>
            {
                targetGroup.Targets.Remove(target);
                removingTargetTweens.Remove(tween);
            });
        }
    }

    public void AddZoneToCameraTargets(Transform[] targets)
    {
        foreach(var target in targets)
        {
            AddToTargetGroup(target);
        }
        UpdateCameraEnemy();
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

    private bool IsInTargetGroup(Transform t)
    {
        foreach(CinemachineTargetGroup.Target target in targetGroup.Targets)
        {
            if(target.Object.Equals(t))
            {
                return true;
            }
        }
        return false;
    }
}
