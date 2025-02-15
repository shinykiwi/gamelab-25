using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Tooltip("The default place to spawn when starting the level and also when respawning.")]
    [SerializeField] private Transform spawnPoint;
    
    [Tooltip("The amount of time a player has to wait before respawning.")]
    [SerializeField] private float respawnDelay = 5f;

    public Transform GetSpawnPoint()
    {
        return spawnPoint;
    }
}
