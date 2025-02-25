using Code.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestOrbit : MonoBehaviour
{

    public Transform shield; // Shield (child of the character)
    public Transform character; // Character (parent of the shield)
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
        currentShieldPosition = shield.position;
    }

    void Update()
    {
        Vector2 lookInputValue = LookAction.ReadValue<Vector2>().normalized;
        if(lookInputValue.magnitude > 0.3f)
        {
            //float target_angle = Mathf.Atan2(lookInputValue.y, lookInputValue.x);

            //float x = orbitRadius * Mathf.Cos(target_angle) + character.position.x;
            //float z = orbitRadius * Mathf.Sin(target_angle) + character.position.z;

            //shield.position = new Vector3(x, character.position.y, z);
            //shield.rotation = Quaternion.LookRotation(new Vector3(lookInputValue.x, 0, lookInputValue.y));

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
            shield.position = new Vector3(x, character.position.y, z);

            // Step 5: Calculate the current rotation that the shield should have based on the new position
            Vector3 direction = shield.position - character.position;
            shield.rotation = Quaternion.LookRotation(direction);
        }
    }
}
