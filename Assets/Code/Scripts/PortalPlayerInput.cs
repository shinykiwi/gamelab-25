using Code.Scripts;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PortalPlayerInput : MonoBehaviour
{

    public Transform portal; // portal (child of the player)
    public Transform player_indicator; //The UI indicator at the feet
    public Transform character; // Character (child of the player)
    public float orbitRadius = 2f; // How far the shield orbits from the character
    public float rotationSpeed = 5f; // Speed of rotation around the character
    public float positionSmoothness = 10f; // Speed of smooth movement for shield's position
    public float angle_treshold = 0.3f;

    public PlayerInput playerInput;

    [SerializeField, Tooltip("Size of the gaps in angle (Use dividers of 360 and 90 for good results:30,45")]
    private float portalAngleStepSize = 30;

    [SerializeField]
    private float portalAngleSpeed = 5f;

    [SerializeField]
    private float timeToLockTargetAngle = 0.15f;

    private InputAction LookAction;

    private float currentAngleDeg = 0; // Current angleDegree of the portal around the character
    private float targetAngleDeg = 0.0f; // Target angleDegree of the portal around the character
    private float timeKeptSameTargetAngle = 0.0f;

    private void Start()
    {
        LookAction = playerInput.actions.FindAction("Look");
    }

    void Update()
    {
        CalculatePortalAngle();
        RotatePortal();
        RotatePlayerIndicator();
    }

    private void RotatePlayerIndicator()
    {
        if (player_indicator != null)
        {
            Vector3 direction = portal.position - character.position;
            player_indicator.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void CalculatePortalAngle()
    {
        Vector2 lookInputValue = LookAction.ReadValue<Vector2>();
        // Get look direction for mouse controls
        if(playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            // Get the distance to the camera
            const float offset = 1.0f; // Offset to make the result more accurate
            Plane raycastPlane = new Plane(Vector3.up, new Vector3(0.0f, transform.position.y + offset, 0.0f));
            Ray rayToTarget = Camera.main.ScreenPointToRay(lookInputValue);
            raycastPlane.Raycast(rayToTarget, out float distToCamera);

            // Get a look-direction
            Vector3 lookTarget = Camera.main.ScreenToWorldPoint(new Vector3(lookInputValue.x, lookInputValue.y, distToCamera));
            Vector3 lookDir = lookTarget - character.position;
            lookInputValue = (new Vector2(lookDir.x, lookDir.z)).normalized;
        }

        if(lookInputValue.magnitude > 0.7f)
        {
            lookInputValue = lookInputValue.normalized;
            // Calculate the target angleDegree based on joystick input
            float newTargetAngleDeg = Mathf.Atan2(lookInputValue.y, lookInputValue.x) * Mathf.Rad2Deg;

            // Lock the angles to specific degree increments, if not using keyboard and mouse
            if(ArePortalAnglesStepped())
            {
                newTargetAngleDeg = QuantizeAngle(newTargetAngleDeg, portalAngleStepSize);
                if (newTargetAngleDeg == targetAngleDeg)
                {
                    timeKeptSameTargetAngle += Time.deltaTime;
                }
                else
                {
                    timeKeptSameTargetAngle = 0.0f;
                }
            }
            targetAngleDeg = newTargetAngleDeg;

            // Use Mathf.DeltaAngle to calculate the shortest path to the target angleDegree
            float angleDifference = Mathf.DeltaAngle(currentAngleDeg, targetAngleDeg);

            // Smoothly interpolate to the target angleDegree
            // Use LerpAngle to ensure smooth transition, combined with delta angleDegree for shortest path
            currentAngleDeg = Mathf.LerpAngle(currentAngleDeg, currentAngleDeg + angleDifference, portalAngleSpeed * Time.deltaTime);
        }
        else
        {
            // If the joystick is not being used, keep the portal in the nearest angleDegree
            if(ArePortalAnglesStepped() && timeKeptSameTargetAngle > 0.0f && timeKeptSameTargetAngle <= timeToLockTargetAngle)
            {
                // Lock the angles to specific degree increments, if not using keyboard and mouse
                targetAngleDeg = QuantizeAngle(currentAngleDeg, portalAngleStepSize);
            }
            timeKeptSameTargetAngle = 0.0f;
            float angleDifference = Mathf.DeltaAngle(currentAngleDeg, targetAngleDeg);
            currentAngleDeg = Mathf.LerpAngle(currentAngleDeg, currentAngleDeg + angleDifference, portalAngleSpeed * Time.deltaTime);
        }
    }

    private void RotatePortal()
    {
        // Calculate the new position based on the smooth angleDegree
        float x = orbitRadius * Mathf.Cos(currentAngleDeg * Mathf.Deg2Rad) + character.position.x;
        float z = orbitRadius * Mathf.Sin(currentAngleDeg * Mathf.Deg2Rad) + character.position.z;

        // Update the shield's position to stay on the orbit
        portal.position = new Vector3(x, character.position.y, z);

        // Calculate the current rotation that the shield should have based on the new position
        Vector3 direction = portal.position - character.position;
        portal.rotation = Quaternion.LookRotation(direction);
    }

    private float QuantizeAngle(float angleDegree, float stepSize)
    {
        return Mathf.Round(angleDegree / stepSize) * stepSize;
    }
    private bool ArePortalAnglesStepped()
    {
        return playerInput.currentControlScheme == "Gamepad" || playerInput.currentControlScheme == "Joystick";
    }
}
