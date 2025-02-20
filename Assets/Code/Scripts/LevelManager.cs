using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts
{
    public class Level : MonoBehaviour
    {
        public static Level Instance { get; private set; }

        [Header("Parameters")]
        [Tooltip("The default place to spawn when starting the Instance and also when respawning.")] 
        [SerializeField] private Transform spawnPoint;

        [Tooltip("The amount of time a player has to wait before respawning.")] 
        [SerializeField] private float respawnDelay = 5f;

        [Header("References")]
        [SerializeField]
        private Canvas endLevelCanvas;

        public static Transform SpawnPoint => Instance?.spawnPoint;

        public static float RespawnDelay => Instance?.respawnDelay ?? 0f;

        private List<Enemy> enemiesToBeat;
        
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

            // Require to beat all enenmies in the level
            enemiesToBeat = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
        }

        public void EnemyHasBeenDefeated(Enemy enemy)
        {
            enemiesToBeat.Remove(enemy);

            if(enemiesToBeat.Count == 0)
            {
                LevelCompleted();
            }
        }

        void LevelCompleted()
        {
            endLevelCanvas.gameObject.SetActive(true);
        }
    }
}
