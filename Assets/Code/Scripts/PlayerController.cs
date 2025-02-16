using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    float moveSpeed = 1.0f;

    [SerializeField]
    float rotationSpeedDegrees = 720.0f;

    PlayerInput playerInput;
    Rigidbody rb;

    InputAction moveAction;
    InputAction lookAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInputValue = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInputValue.x, 0, moveInputValue.y);
        Move(move);

        Vector2 lookInputValue = lookAction.ReadValue<Vector2>();
        Vector3 look = new Vector3(lookInputValue.x, 0, lookInputValue.y);
        Look(look);
    }
    
    void Move(Vector3 move)
    {
        Vector3 displacement = moveSpeed * Time.deltaTime * move;
        rb.MovePosition(rb.position + displacement);
    }

    void Look(Vector3 look)
    {
        if(look.magnitude < 0.1f)
        {
            return;
        }
        Vector3 lookDir = look.normalized;
        Vector3 camera_dir = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
        Quaternion target_rotation = Quaternion.LookRotation(Quaternion.LookRotation(camera_dir.normalized) * lookDir);
        Quaternion curRotation = Quaternion.RotateTowards(rb.rotation, target_rotation, rotationSpeedDegrees * Time.deltaTime);
        rb.rotation = curRotation;
    }
}
