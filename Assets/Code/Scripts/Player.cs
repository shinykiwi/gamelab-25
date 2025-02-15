using Unity.VisualScripting;
using UnityEngine;

namespace Code.Scripts
{
    public class Player : Humanoid
    {

        private PortalController portalController;
        private GameObject portalObject;

        protected override void Start()
        {
            base.Start();
            portalController = GetComponentInChildren<PortalController>();
            portalObject = portalController.gameObject;
        }
        
        protected override void Death()
        {
            base.Death();
            
            // Deactivates portal
            portalObject.SetActive(false);
            
            // TODO: deactivate the other players' portal
            // See PortalController's exit portal ref
        }

        protected override void Respawn()
        {
            base.Respawn();
            
            // Activates portal
            portalObject.SetActive(true);
            
            // TODO: reactivate the other players' portal
        }
    
    }
}
