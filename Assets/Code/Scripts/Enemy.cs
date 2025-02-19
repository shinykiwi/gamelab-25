using UnityEngine;

namespace Code.Scripts
{
    public class Enemy : Humanoid
    {
        ProjectileSpawner projectileSpawner;
        
        protected override void Start()
        {
            base.Start();
            projectileSpawner = GetComponent<ProjectileSpawner>();
            if(projectileSpawner == null)
            {
                Debug.LogError("No ProjectileSpawner found on this object");
            }
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                projectileSpawner.SpawnProjectile();
            }
        }
    }
}
