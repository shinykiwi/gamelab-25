using Code.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [Header("Dependencies")] [SerializeField]
    private GameObject exitPortal;

    [SerializeField]
    MeshRenderer portal_render;

    [SerializeField]
    MeshCollider portal_collider;

    [Header("Settings")] 
    
    [Range(0.5f, 10f)]
    [SerializeField] private float speedOnExit = 1f;

    [SerializeField]
    private bool isAimAssistEnabled = true;

    [SerializeField, Range(0f, 360.0f)]
    private float degreesAimAssist = 30.0f;

    [SerializeField, Min(0f)]
    private float aimAssistDistanceMax = 25.0f;

    [Header("Visuals")]
    [SerializeField, Min(0.001f)]
    private float portalClosedScale = 0.05f;

    [SerializeField, Min(0.001f)]
    private float portalOpenScale = 1.0f;

    [SerializeField, Min(0.001f)]
    private float portalScaleSpeed = 2.0f;

    Material portal_material;

    // Keep track of incoming objects to not re-teleport an object coming from the other portal
    private List<GameObject> incomingObjects = new List<GameObject>();

    private void Start()
    {
        // Find and assign the exit portal
        PortalController[] portals = FindObjectsByType<PortalController>(FindObjectsSortMode.None);
        foreach (var portal in portals)
        {
            if (!portal.gameObject.Equals(gameObject))
            {
                exitPortal = portal.gameObject;
            }
        }

        List<Material> temp = new List<Material> ();
        portal_render.GetMaterials(temp);

        portal_material = temp[0];
    }

    public void AddIncomingTeleportingObject(GameObject obj)
    {
        incomingObjects.Add(obj);
    }

    /// <summary>
    /// Is called when a projectile collides with a portal.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Projectile>() is { } projectile
            && !incomingObjects.Contains(other.gameObject))
        {
            if(Level.Instance.ArePlayersAlive() && ZoneManager.Instance.ArePortalActive())
            {
                TeleportProjectile(projectile);
            }
            else
            {
                projectile.Kill();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(incomingObjects.Contains(other.gameObject))
        {
            incomingObjects.Remove(other.gameObject);
            if(other.GetComponent<Projectile>() is {} projectile)
            {
                projectile.EndPortalTravel();
            }
        }
    }

    /// <summary>
    /// Teleports the projectile from one portal to the other.
    /// </summary>
    /// <param name="projectile"></param>
    private void TeleportProjectile(Projectile projectile)
    {
        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        
        projectile.BeginPortalTravel();
        
        // Set the projectilePosition of the projectile to be where the portal is
        projectile.transform.position = exitPortal.transform.position;
        float magnitude = projectileRigidbody.linearVelocity.magnitude;
        // The exit velocity is towards the normal of the exit portal
        Vector3 newVelocity = magnitude * exitPortal.transform.forward * speedOnExit;
        Debug.DrawLine(exitPortal.transform.position, exitPortal.transform.position + exitPortal.transform.forward * 10, Color.cyan, 5);
        
        // Draw Debug Rays for the velocity
        Debug.DrawRay(projectile.transform.position, newVelocity, Color.red, 1);
        Quaternion rotateRight = Quaternion.Euler(0, degreesAimAssist / 2, 0);
        Debug.DrawRay(projectile.transform.position, aimAssistDistanceMax * (rotateRight * newVelocity.normalized), Color.green, 1);
        Quaternion rotateLeft = Quaternion.Euler(0, -degreesAimAssist / 2, 0);
        Debug.DrawRay(projectile.transform.position, aimAssistDistanceMax * (rotateLeft * newVelocity.normalized), Color.green, 1);

        if(isAimAssistEnabled)
        {
            newVelocity = AimAssistUtils.GetAutoAimVelocity(projectile.transform.position, newVelocity, aimAssistDistanceMax, 
                degreesAimAssist, AimAssistUtils.ignoredMaskForLOS);
        }
        projectileRigidbody.linearVelocity = newVelocity;

        // De-parents the projectile so it can be free
        projectile.gameObject.transform.SetParent(null);
        // Sets the shieldOwner of the projectile to be this gameObject
        projectile.Owner = gameObject;

        PortalController portalController = exitPortal.GetComponent<PortalController>();
        portalController.AddIncomingTeleportingObject(projectile.gameObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 dir = transform.forward;
        Quaternion rotateRight = Quaternion.Euler(0, degreesAimAssist / 2, 0);
        Gizmos.DrawRay(transform.position, aimAssistDistanceMax * (rotateRight * dir));

        Quaternion rotateLeft = Quaternion.Euler(0, -degreesAimAssist / 2, 0);
        Debug.DrawRay(transform.position, aimAssistDistanceMax * (rotateLeft * dir));
    }

    private void Update()
    {
        if(ZoneManager.Instance.ArePortalActive())
        {
            portal_render.enabled = true;
            portal_collider.enabled = true;
            portal_material.SetColor("_Color", ZoneManager.Instance.ZoneColorSettings.GetZone(ZoneManager.Instance.active_type).color * 500);
            portal_material.SetColor("_EmisionColor", ZoneManager.Instance.ZoneColorSettings.GetZone(ZoneManager.Instance.active_type).color * 10);

            // Open portal slowly
            if(portal_render.gameObject.transform.localScale.x < portalOpenScale)
            {
                portal_render.gameObject.transform.localScale += 
                    portal_render.gameObject.transform.localScale.x * portalScaleSpeed * Time.deltaTime * Vector3.one;
                if(portal_render.gameObject.transform.localScale.x > portalOpenScale)
                {
                    portal_render.gameObject.transform.localScale = Vector3.one;
                }
            }
        }
        else
        {
            portal_collider.enabled = false;

            // Close portal slowly
            if(portal_render.gameObject.transform.localScale.x > portalClosedScale)
            {
                portal_render.gameObject.transform.localScale -=
                    portal_render.gameObject.transform.localScale.x * portalScaleSpeed * Time.deltaTime * Vector3.one;
                if(portal_render.gameObject.transform.localScale.x < portalClosedScale)
                {
                    portal_render.gameObject.transform.localScale = portalClosedScale * Vector3.one;
                    portal_render.enabled = false;
                }
            }
        }
    }
}
