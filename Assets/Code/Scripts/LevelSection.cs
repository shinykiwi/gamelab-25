using Code.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class LevelSection
{
    [Header("Parameters")]
    [Tooltip("The default place to spawn when starting the Instance and also when respawning.")]
    [SerializeField] public Transform spawnPointP1;
    [SerializeField] public Transform spawnPointP2;

    [SerializeField]
    public List<Enemy> enemiesToBeat;

    [SerializeField]
    public UnityEvent onSectionComplete;

    public bool isCompleted = false;
}
