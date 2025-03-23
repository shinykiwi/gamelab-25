using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.Scripts
{
    public class Level : MonoBehaviour
    {
        public static Level Instance { get; private set; }

        [Header("Parameters")]
        [Tooltip("The amount of time a player has to wait before respawning.")] 
        [SerializeField] private float respawnDelay = 1f;

        [SerializeField, Tooltip("Sections are to be completed in order of this array.")]
        LevelSection[] levelSections;

        public static LevelSection CurrentSection => Instance?.currentSection;
        public static Transform SpawnPointP1 => CurrentSection?.spawnPointP1;
        public static Transform SpawnPointP2 => CurrentSection?.spawnPointP2;
        public static float RespawnDelay => Instance?.respawnDelay ?? 0f;

        public static List<Enemy> SectionEnemies => CurrentSection?.enemiesToBeat;
        public List<Enemy> AllEnemies { get; private set; }
        public List<BouncyWall> AllBouncyWalls { get; private set; }
        public List<Player> players { get; private set; }

        private int currentSectionIndex = 0;
        private LevelSection currentSection = null;
        
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

            // Require to beat all enemies in the level
            AllEnemies = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
            AllBouncyWalls = new List<BouncyWall>(FindObjectsByType<BouncyWall>(FindObjectsSortMode.None));
            players = new List<Player>(FindObjectsByType<Player>(FindObjectsSortMode.None));

            if(levelSections.Length > 0)
                currentSection = levelSections[currentSectionIndex];
            else
                Debug.LogWarning("No sections in the level.", this);
        }

        public void EnemyHasBeenDefeated(Enemy enemy)
        {
            AllEnemies.Remove(enemy);
            CurrentSection.enemiesToBeat.Remove(enemy);

            if(CurrentSection.enemiesToBeat.Count == 0)
            {
                SectionCompleted();
            }
        }

        public bool ArePlayersAlive()
        {
            foreach(var player in players)
            {
                if(!player.IsAlive)
                {
                    return false;
                }
            }
            return true;
        }


        public void SectionCompleted()
        {
            CurrentSection.isCompleted = true;
            CurrentSection.onSectionComplete.Invoke();

            // Go to next section if there is one
            if(currentSectionIndex < levelSections.Length - 1)
            {
                currentSectionIndex++;
                currentSection = levelSections[currentSectionIndex];

                // For now if there are no enemies in the next section, we consider it completed, to avoid softlocks
                if(currentSection.enemiesToBeat.Count == 0)
                {
                    SectionCompleted();
                }
            }
            else
            {
                LevelCompleted();
            }
        }

        void LevelCompleted()
        {
            // Reveals the level trigger
            FindFirstObjectByType<LevelTrigger>().Show();
        }
    }
}
