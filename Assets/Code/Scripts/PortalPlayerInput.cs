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

    private InputAction LookAction;

    private float currentAngle = 0; // Current angle of the shield around the character
    private Vector3 currentShieldPosition; // To store the current shield position

    private void Start()
    {
        LookAction = playerInput.actions.FindAction("Look");
        currentShieldPosition = portal.position;
    }

    void Update()
    {
        Vector2 lookInputValue = LookAction.ReadValue<Vector2>();
        Vector3 look = new Vector3(lookInputValue.x, 0, lookInputValue.y);
        // Get look direction for mouse controls
        if(playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            float distToCamera = (Camera.main.transform.position - transform.position).magnitude;
            Vector3 lookTarget = Camera.main.ScreenToWorldPoint(new Vector3(lookInputValue.x, lookInputValue.y, distToCamera));
            Vector3 lookDir = lookTarget - transform.position;
            look = new Vector3(lookDir.x, 0.0f, lookDir.z);
            lookInputValue = (new Vector2(lookDir.x, lookDir.z)).normalized;
        }

        Debug.DrawRay(transform.position, lookInputValue);
        lookInputValue = lookInputValue.normalized;
        if(lookInputValue.magnitude > 0.3f)
        {
            // Step 1: Calculate the target angle based on joystick input
            float target_angle = Mathf.Atan2(lookInputValue.y, lookInputValue.x);

            // Step 2: Use Mathf.DeltaAngle to calculate the shortest path to the target angle
            float angleDifference = Mathf.DeltaAngle(currentAngle * Mathf.Rad2Deg, target_angle * Mathf.Rad2Deg);

            // Step 3: Smoothly interpolate to the target angle
            // Use LerpAngle to ensure smooth transition, combined with delta angle for shortest path
            currentAngle = Mathf.LerpAngle(currentAngle, currentAngle + Mathf.Deg2Rad * angleDifference, Time.deltaTime * 5f); // 5f controls the speed

            // Step 4: Calculate the new position based on the smooth angle
            float x = orbitRadius * Mathf.Cos(currentAngle) + character.position.x;
            float z = orbitRadius * Mathf.Sin(currentAngle) + character.position.z;

            // Update the shield's position to stay on the orbit
            portal.position = new Vector3(x, character.position.y, z);

            // Step 5: Calculate the current rotation that the shield should have based on the new position
            Vector3 direction = portal.position - character.position;
            portal.rotation = Quaternion.LookRotation(direction);
        }
    }
}
