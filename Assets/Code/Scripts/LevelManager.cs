using UnityEngine;

namespace Code.Scripts
{
    public class Level : MonoBehaviour
    {
        public static Level Instance { get; private set; }

        [Tooltip("The default place to spawn when starting the Instance and also when respawning.")] 
        [SerializeField] private Transform spawnPoint;

        [Tooltip("The amount of time a player has to wait before respawning.")] 
        [SerializeField] private float respawnDelay = 5f;
        
        public static Transform SpawnPoint => Instance?.spawnPoint;

        public static float RespawnDelay => Instance?.respawnDelay ?? 0f;
        
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
    }
}
