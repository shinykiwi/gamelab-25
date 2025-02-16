using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 1.0f;

    PlayerInput playerInput;
    Rigidbody rb;

    InputAction moveAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        moveAction = playerInput.actions.FindAction("Move");

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Move(moveValue);
    }

    // Inputs
    // ////////////////////////////////////////////////
    void Move(Vector2 move)
    {
        Vector3 displacement = moveSpeed * Time.deltaTime * new Vector3(move.x, 0, move.y);
        rb.MovePosition(rb.position + displacement);
    }
}
