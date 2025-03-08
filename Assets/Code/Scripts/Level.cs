using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.Scripts
{
    public class Level : MonoBehaviour
    {
        public static Level Instance { get; private set; }

        [Header("Parameters")]
        [Tooltip("The default place to spawn when starting the Instance and also when respawning.")] 
        [SerializeField] private Transform spawnPointP1;
        [SerializeField] private Transform spawnPointP2;

        [Tooltip("The amount of time a player has to wait before respawning.")] 
        [SerializeField] private float respawnDelay = 5f;

        [Header("References")]
        [SerializeField]
        private Canvas endLevelCanvas;

        public static Transform SpawnPointP1 => Instance?.spawnPointP1;
        public static Transform SpawnPointP2 => Instance?.spawnPointP2;
        public static float RespawnDelay => Instance?.respawnDelay ?? 0f;

        public List<Enemy> EnemiesToBeat { get; private set; }

        public List<Player> Players { get; private set; }
        
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
            EnemiesToBeat = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
            Players = new List<Player>(FindObjectsByType<Player>(FindObjectsSortMode.None));
        }

        public void EnemyHasBeenDefeated(Enemy enemy)
        {
            EnemiesToBeat.Remove(enemy);

            if(EnemiesToBeat.Count == 0)
            {
                LevelCompleted();
            }
        }

        public bool ArePlayersAlive()
        {
            foreach(var player in Players)
            {
                if(!player.IsAlive)
                {
                    return false;
                }
            }
            return true;
        }

        void LevelCompleted()
        {
            // Reveals the level trigger
            FindFirstObjectByType<LevelTrigger>().Show();
            
            //endLevelCanvas.gameObject.SetActive(true);
        }
    }
}
