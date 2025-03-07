
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts
{
    public enum PlayerNum
    {
        Player1,
        Player2
    }
    public class Player : Humanoid
    {
        private PortalController portalController;
        private GameObject portalObject;
        [SerializeField] private PlayerNum playerNum;
        private Transform spawnPoint;
        private PlayerInput playerInput;

        protected override void Start()
        {
            base.Start();
            portalController = GetComponentInChildren<PortalController>();
            portalObject = portalController.gameObject;
            spawnPoint = playerNum == PlayerNum.Player1 ? Level.SpawnPointP1 : Level.SpawnPointP2;

            playerInput = GetComponent<PlayerInput>();
        }
        
        protected override void Death()
        {
            if(!IsAlive)
                return;

            base.Death();

            StartCoroutine(DelaySpawn(Level.RespawnDelay, spawnPoint));
            
            // Deactivates portal
            portalObject.SetActive(false);

            playerInput.DeactivateInput();
            
            // TODO: deactivate the other players' portal
            // See PortalController's exit portal ref
        }

        protected override void Respawn()
        {
            base.Respawn();
            
            // Activates portal
            portalObject.SetActive(true);

            playerInput.ActivateInput();

            // TODO: reactivate the other players' portal
        }
    
    }
}
