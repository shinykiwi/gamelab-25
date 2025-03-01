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


    [SerializeField]
    Animator player_animator;

    [SerializeField]
    Transform character_transform;

    float rotation_speed = 1000f;

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
        if(move.magnitude > 0)
            Move(move);
    }
    
    void Move(Vector3 move)
    {
        float movement_amount = Mathf.Abs(move.x) + Mathf.Abs(move.z);
        player_animator.SetFloat("movement_amount", movement_amount);
        player_animator.SetBool("is_aiming", false);

        //Move the player transform
        Vector3 displacement = moveSpeed * Time.deltaTime * move;
        rb.MovePosition(rb.position + displacement);


        //Adjust the character rotation (child)
        Vector3 camera_rotation = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
        Quaternion target_rotation = Quaternion.LookRotation(Quaternion.LookRotation(camera_rotation) * move);
        character_transform.rotation =
           Quaternion.RotateTowards(character_transform.rotation, target_rotation, rotation_speed * Time.deltaTime);
    }
}
