using Code.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PortalPlayerInput : MonoBehaviour
{

    public Transform portal; // portal (child of the player)
    public Transform character; // Character (child of the player)
    public float orbitRadius = 2f; // How far the shield orbits from the character
    public float rotationSpeed = 5f; // Speed of rotation around the character
    public float positionSmoothness = 10f; // Speed of smooth movement for shield's position
    public float angle_treshold = 0.3f;

    public PlayerInput playerInput;

    [SerializeField, Tooltip("Size of the gaps in angle (Use dividers of 360 and 90 for good results:30,45")]
    private int portalAngleStepSize = 30;

    private InputAction LookAction;

    private float currentAngle = 0; // Current angle of the shield around the character
    private float targetAngle = 0.0f; // Target angle of the shield around the character
    private Vector3 currentShieldPosition; // To store the current shield position

    private void Start()
    {
        LookAction = playerInput.actions.FindAction("Look");
        currentShieldPosition = portal.position;
    }

    void Update()
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

        // Step1: Update the target angle based on joystick input
        lookInputValue = lookInputValue.normalized;
        if(lookInputValue.magnitude > 0.3f)
        {
            // Calculate the target angle based on joystick input
            targetAngle = Mathf.Atan2(lookInputValue.y, lookInputValue.x);

            // Lock the angles to specific degree increments, if not using keyboard and mouse
            if(playerInput.currentControlScheme != "Keyboard&Mouse")
            {
                targetAngle = Mathf.Round(targetAngle * Mathf.Rad2Deg / portalAngleStepSize) * portalAngleStepSize;
                targetAngle *= Mathf.Deg2Rad;
            }
        }
        // Step2: Set the portal's target angle to the target angle
        // Use Mathf.DeltaAngle to calculate the shortest path to the target angle
        float angleDifference = Mathf.DeltaAngle(currentAngle * Mathf.Rad2Deg, targetAngle * Mathf.Rad2Deg);

        // Smoothly interpolate to the target angle
        // Use LerpAngle to ensure smooth transition, combined with delta angle for shortest path
        currentAngle = Mathf.LerpAngle(currentAngle, currentAngle + Mathf.Deg2Rad * angleDifference, Time.deltaTime * 5f); // 5f controls the speed

        // Calculate the new position based on the smooth angle
        float x = orbitRadius * Mathf.Cos(currentAngle) + character.position.x;
        float z = orbitRadius * Mathf.Sin(currentAngle) + character.position.z;

        // Update the shield's position to stay on the orbit
        portal.position = new Vector3(x, character.position.y, z);

        // Calculate the current rotation that the shield should have based on the new position
        Vector3 direction = portal.position - character.position;
        portal.rotation = Quaternion.LookRotation(direction);
    }
}
